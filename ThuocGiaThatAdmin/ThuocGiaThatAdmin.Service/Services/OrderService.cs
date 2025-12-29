using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuocGiaThat.Infrastucture;
using ThuocGiaThatAdmin.Common;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Contract.Enums;
using ThuocGiaThatAdmin.Contract.Models;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Service.Services
{
    /// <summary>
    /// Service for Order business logic
    /// </summary>
    public class OrderService
    {
        private readonly TrueMecContext _context;
        private readonly VNPayService _vnPayService;

        public OrderService(TrueMecContext context, VNPayService vnPayService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _vnPayService = vnPayService;
        }

        /// <summary>
        /// Customer places order from Ecommerce site
        /// </summary>
        public async Task<OrderResponseDto> CustomerPlaceOrderAsync(CustomerPlaceOrderDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.OrderItems == null || !dto.OrderItems.Any())
                throw new ArgumentException("Order must have at least one item", nameof(dto.OrderItems));

            // Check if customer already exists by email
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == dto.CustomerEmail);

            int customerId;
            if (existingCustomer != null)
            {
                // Use existing customer
                customerId = existingCustomer.Id;
            }
            else
            {
                // Create new customer
                var newCustomer = new Customer
                {
                    FullName = dto.CustomerFullName,
                    Email = dto.CustomerEmail,
                    PhoneNumber = dto.CustomerPhone,
                    PasswordHash = string.Empty, // Will be set when customer registers
                    CreatedDate = DateTime.UtcNow
                };

                _context.Customers.Add(newCustomer);
                await _context.SaveChangesAsync();
                customerId = newCustomer.Id;
            }

            // Create order with customer defaults
            var order = await CreateOrderInternalAsync(
                customerId: customerId,
                orderItems: dto.OrderItems,
                paymentMethod: dto.PaymentMethod,
                shippingFee: dto.ShippingFee,
                discountAmount: dto.DiscountAmount,
                shippingName: dto.ShippingName,
                shippingAddress: dto.ShippingAddress,
                shippingPhone: dto.ShippingPhone,
                wardId: dto.WardId,
                provinceId: dto.ProvinceId,
                note: dto.Note,
                orderStatus: "Pending", // Always Pending for customer orders
                paymentStatus: "Pending" // Always Pending for customer orders
            );

            return order;
        }

        /// <summary>
        /// Admin creates order from Admin panel
        /// </summary>
        public async Task<OrderResponseDto> AdminCreateOrderAsync(AdminCreateOrderDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.OrderItems == null || !dto.OrderItems.Any())
                throw new ArgumentException("Order must have at least one item", nameof(dto.OrderItems));

            // Handle customer - use existing or create new
            int? customerId = dto.CustomerId;
            if (!customerId.HasValue && !string.IsNullOrWhiteSpace(dto.CustomerFullName))
            {
                // Create new customer
                var newCustomer = new Customer
                {
                    FullName = dto.CustomerFullName,
                    Email = dto.CustomerEmail ?? string.Empty,
                    PhoneNumber = dto.CustomerPhone,
                    PasswordHash = string.Empty,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Customers.Add(newCustomer);
                await _context.SaveChangesAsync();
                customerId = newCustomer.Id;
            }

            // Admin can set custom status or use defaults
            var orderStatus = !string.IsNullOrWhiteSpace(dto.OrderStatus) ? dto.OrderStatus : "Pending";
            var paymentStatus = !string.IsNullOrWhiteSpace(dto.PaymentStatus) ? dto.PaymentStatus : "Pending";

            // Create order
            var order = await CreateOrderInternalAsync(
                customerId: customerId,
                orderItems: dto.OrderItems,
                paymentMethod: dto.PaymentMethod,
                shippingFee: dto.ShippingFee,
                discountAmount: dto.DiscountAmount,
                shippingName: dto.ShippingName,
                shippingAddress: dto.ShippingAddress,
                shippingPhone: dto.ShippingPhone,
                wardId: dto.WardId,
                provinceId: dto.ProvinceId,
                note: dto.Note,
                orderStatus: orderStatus,
                paymentStatus: paymentStatus
            );

            return order;
        }

        /// <summary>
        /// Internal method to create order with all parameters
        /// </summary>
        private async Task<OrderResponseDto> CreateOrderInternalAsync(
            int? customerId,
            List<OrderItemDto> orderItems,
            string paymentMethod,
            decimal shippingFee,
            decimal discountAmount,
            string shippingName,
            string shippingAddress,
            string shippingPhone,
            int? wardId,
            int? provinceId,
            string? note,
            string orderStatus,
            string paymentStatus)
        {
            // Validate product variants exist and get their prices
            var variantIds = orderItems.Select(i => i.ProductVariantId).Distinct().ToList();
            var variants = await _context.ProductVariants
                .Include(v => v.Product)
                .Where(v => variantIds.Contains(v.Id))
                .ToListAsync();

            if (variants.Count != variantIds.Count)
            {
                var missingIds = variantIds.Except(variants.Select(v => v.Id));
                throw new ArgumentException($"Product variants not found: {string.Join(", ", missingIds)}");
            }

            // Validate location codes if provided
            if (wardId.HasValue)
            {
                var wardExists = await _context.Wards.AnyAsync(w => w.Id == wardId.Value);
                if (!wardExists)
                    throw new ArgumentException($"Ward with ID {wardId.Value} not found");
            }

            if (provinceId.HasValue)
            {
                var provinceExists = await _context.Provinces.AnyAsync(p => p.Id == provinceId.Value);
                if (!provinceExists)
                    throw new ArgumentException($"Province with ID {provinceId.Value} not found");
            }

            // Create order
            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                CustomerId = customerId,
                OrderStatus = orderStatus,
                PaymentStatus = paymentStatus,
                PaymentMethod = paymentMethod,
                ShippingFee = shippingFee,
                DiscountAmount = discountAmount,
                ShippingName = shippingName,
                ShippingAddress = shippingAddress,
                ShippingPhone = shippingPhone,
                Note = note,
                CreatedDate = DateTime.UtcNow
            };

            // Create order items with auto-calculated prices
            decimal subTotal = 0;
            foreach (var itemDto in orderItems)
            {
                var variant = variants.First(v => v.Id == itemDto.ProductVariantId);

                // Use the current price from the product variant
                var unitPrice = variant.Price;
                var totalLineAmount = itemDto.Quantity * unitPrice;
                subTotal += totalLineAmount;

                var orderItem = new OrderItem
                {
                    ProductVariantId = itemDto.ProductVariantId,
                    ProductId = variant.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = unitPrice, // Auto-calculated from variant
                    TotalLineAmount = totalLineAmount
                };

                order.OrderItems.Add(orderItem);
            }

            // Calculate totals
            order.SubTotal = subTotal;
            order.TotalAmount = subTotal + order.ShippingFee - order.DiscountAmount;

            // Save to database
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Load the created order with related data
            var createdOrder = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
                .ThenInclude(v => v.Product)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            // Load location data if available
            Ward? ward = null;
            Province? province = null;
            Country? country = null;

            if (wardId.HasValue)
                ward = await _context.Wards.FindAsync(wardId.Value);

            if (provinceId.HasValue)
                province = await _context.Provinces.FindAsync(provinceId.Value);

            // Map to response DTO
            return MapToResponseDto(createdOrder!, ward, province, country);
        }

        /// <summary>
        /// Get order by ID
        /// </summary>
        public async Task<OrderResponseDto?> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
                .ThenInclude(v => v.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            return order == null ? null : MapToResponseDto(order, null, null, null);
        }

        /// <summary>
        /// Get orders with pagination and search
        /// </summary>
        public async Task<(List<OrderListDto> Orders, int TotalCount)> GetOrdersAsync(
            int pageNumber,
            int pageSize,
            string? searchText,
            int? customerId = null)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));

            if (pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Page size must be between 1 and 100", nameof(pageSize));

            var query = _context.Orders
                .Include(o => o.Customer)
                .AsQueryable();

            // Filter by customer if provided
            if (customerId.HasValue)
            {
                query = query.Where(o => o.CustomerId == customerId.Value);
            }

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim().ToLower();
                query = query.Where(o =>
                    o.OrderNumber.ToLower().Contains(searchText) ||
                    (o.Customer != null && o.Customer.PhoneNumber.ToLower().Contains(searchText)) ||
                    (o.Customer != null && o.Customer.Email != null && o.Customer.Email.ToLower().Contains(searchText))
                );
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination and ordering (newest first)
            var orders = await query
                .OrderByDescending(o => o.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Map to OrderListDto
            var orderListDtos = orders.Select(o => new OrderListDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerName = o.Customer?.FullName ?? "Guest",
                CustomerPhone = o.Customer?.PhoneNumber ?? o.ShippingPhone,
                CustomerEmail = o.Customer?.Email,
                CreatedDate = o.CreatedDate,
                OrderStatus = o.OrderStatus,
                PaymentStatus = o.PaymentStatus,
                IsFulfilled = o.IsFulfilled.GetValueOrDefault(),
                TotalAmount = o.TotalAmount
            }).ToList();

            return (orderListDtos, totalCount);
        }

        /// <summary>
        /// Update order status
        /// </summary>
        public async Task<OrderResponseDto> UpdateOrderStatusAsync(int orderId, string newStatusString)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
                .ThenInclude(v => v.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new ArgumentException($"Order with ID {orderId} not found");

            // Parse current and new status
            var currentStatus = OrderStatusExtensions.ParseStatus(order.OrderStatus);
            var newStatus = OrderStatusExtensions.ParseStatus(newStatusString);

            // Validate status transition
            if (!currentStatus.CanTransitionTo(newStatus))
            {
                var validStatuses = string.Join(", ", currentStatus.GetValidNextStatuses().Select(s => s.ToString()));
                throw new InvalidOperationException(
                    $"Cannot transition from {currentStatus} to {newStatus}. Valid transitions are: {validStatuses}");
            }

            // Update status
            order.OrderStatus = newStatus.ToStatusString();
            await _context.SaveChangesAsync();

            // Return updated order
            return MapToResponseDto(order, null, null, null);
        }

        /// <summary>
        /// Update order delivery status
        /// </summary>
        public async Task<OrderResponseDto> UpdateDeliveryStatusAsync(int orderId, UpdateOrderDeliveryStatusDto dto)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
                .ThenInclude(v => v.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new ArgumentException($"Order with ID {orderId} not found");

            // Parse current and new delivery status
            var currentDeliveryStatus = OrderDeliveryStatusExtensions.ParseStatus(order.DeliveryStatus);
            var newDeliveryStatus = OrderDeliveryStatusExtensions.ParseStatus(dto.NewDeliveryStatus);

            // Validate delivery status transition
            if (!currentDeliveryStatus.CanTransitionTo(newDeliveryStatus))
            {
                var validStatuses = string.Join(", ",
                    currentDeliveryStatus.GetValidNextStatuses().Select(s => s.ToString()));
                throw new InvalidOperationException(
                    $"Cannot transition from {currentDeliveryStatus} to {newDeliveryStatus}. Valid transitions are: {validStatuses}");
            }

            // Update delivery status
            order.DeliveryStatus = newDeliveryStatus.ToStatusString();

            // Update optional fields if provided
            if (!string.IsNullOrWhiteSpace(dto.ShippingCarrier))
                order.ShippingCarrier = dto.ShippingCarrier;

            if (!string.IsNullOrWhiteSpace(dto.TrackingNumber))
                order.TrackingNumber = dto.TrackingNumber;

            if (dto.ShippedDate.HasValue)
                order.ShippedDate = dto.ShippedDate.Value;

            if (dto.DeliveredDate.HasValue)
                order.DeliveredDate = dto.DeliveredDate.Value;

            if (!string.IsNullOrWhiteSpace(dto.DeliveryNotes))
                order.DeliveryNotes = dto.DeliveryNotes;

            // Auto-set timestamps for certain status transitions
            if (newDeliveryStatus == OrderDeliveryStatus.InTransit && !order.ShippedDate.HasValue)
                order.ShippedDate = DateTime.UtcNow;

            if (newDeliveryStatus == OrderDeliveryStatus.Delivered && !order.DeliveredDate.HasValue)
                order.DeliveredDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Return updated order
            return MapToResponseDto(order, null, null, null);
        }

        /// <summary>
        /// Generate unique order number
        /// </summary>
        private string GenerateOrderNumber()
        {
            // Format: ORD-YYYYMMDD-XXXXXX (e.g., ORD-20231126-000001)
            var date = DateTime.UtcNow.ToString("yyyyMMdd");
            var random = new Random().Next(1, 999999).ToString("D6");
            return $"ORD-{date}-{random}";
        }

        /// <summary>
        /// Map Order entity to OrderResponseDto
        /// </summary>
        private OrderResponseDto MapToResponseDto(Order order, Ward? ward, Province? province, Country? country)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer?.FullName,
                CustomerEmail = order.Customer?.Email,
                CustomerPhone = order.Customer?.PhoneNumber,
                CreatedDate = order.CreatedDate,
                OrderStatus = order.OrderStatus,
                PaymentStatus = order.PaymentStatus,
                PaymentMethod = order.PaymentMethod,
                SubTotal = order.SubTotal,
                ShippingFee = order.ShippingFee,
                DiscountAmount = order.DiscountAmount,
                TotalAmount = order.TotalAmount,
                ShippingName = order.ShippingName,
                ShippingAddress = order.ShippingAddress,
                ShippingPhone = order.ShippingPhone,
                WardId = ward?.Id,
                WardName = ward?.Name,
                ProvinceId = province?.Id,
                ProvinceName = province?.Name,
                CountryId = country?.Id,
                CountryName = country?.CommonName,
                DeliveryStatus = order.DeliveryStatus,
                ShippingCarrier = order.ShippingCarrier,
                TrackingNumber = order.TrackingNumber,
                ShippedDate = order.ShippedDate,
                DeliveredDate = order.DeliveredDate,
                EstimatedDeliveryDate = order.EstimatedDeliveryDate,
                DeliveryMethod = order.DeliveryMethod,
                DeliveryNotes = order.DeliveryNotes,
                Note = order.Note,
                OrderItems = order.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    Id = oi.Id,
                    ProductVariantId = oi.ProductVariantId,
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductVariant?.Product?.Name ?? "Unknown",
                    VariantSKU = oi.ProductVariant?.SKU ?? "Unknown",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    TotalLineAmount = oi.TotalLineAmount
                }).ToList()
            };
        }

        public async Task<dynamic> CreateOrderAsync(CheckoutOrderDto orderDto)
        {
            var order = new Order
            {
                OrderNumber = NumberGenerator.GenerateOrderNumber(),
                CustomerId = orderDto.CustomerId,
                Note = orderDto.Note,
                OrderStatus = OrderStatus.Pending.ToStatusString(),
                PaymentStatus = OrderPaymentStatus.Pending.ToString(),
                PaymentMethod = orderDto.PaymentMethod,
                DiscountAmount = orderDto.DiscountAmount,
                ShippingFee = orderDto.ShippingFee,
                TotalAmount = orderDto.Total,
                ShippingName = orderDto.ShippingName,
                ShippingAddress = orderDto.ShippingAddress,
                WardId = orderDto.WardId,
                ProvinceId = orderDto.ProvinceId,
                OrderItems = orderDto.Items
                    .Select(x => new OrderItem
                    {
                        ProductId = x.ProductId, ProductVariantId = x.ProductVariantId, Quantity = x.Quantity,
                        UnitPrice = x.Price, TotalLineAmount = x.Quantity * x.Price
                    })
                    .ToList(),
                SubTotal = orderDto.SubTotal,
                ShippingPhone = orderDto.ShippingPhone,
                ExportInvoice = orderDto.ExportInvoice,
                CustomerInvoiceInfoId = orderDto.InvoiceId,
            };

            // Create snapshots for all order items (using navigation property, not ID)
            foreach (var orderItem in order.OrderItems)
            {
                CreateOrderItemSnapshotForNewItem(orderItem);
            }

            if (order.PaymentMethod == PaymentMethod.Cash.ToString())
            {
                _context.Orders.Add(order);
                // Save everything in one transaction
                await _context.SaveChangesAsync();

                return new { orderId = order.Id };
            }
            else
            {
                var transaction = new PaymentTransaction
                {
                    OrderId = order.Id,
                    TransactionCode = Guid.NewGuid().ToString(),
                    Amount = order.TotalAmount,
                    PaymentStatus = (int)PaymentTransactionStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                order.PaymentTransactions = new List<PaymentTransaction> { transaction };

                _context.Orders.Add(order);

                // Save everything in one transaction
                await _context.SaveChangesAsync();

                var paymentInfo = new PaymentInformationModel
                {
                    OrderId = transaction.TransactionCode, // D�ng TransactionCode l�m vnp_TxnRef
                    Amount = order.TotalAmount,
                    CreatedDate = DateTime.Now,
                    BankCode = order.PaymentMethod
                };

                var paymentUrl = _vnPayService.CreatePaymentUrl(paymentInfo, orderDto.IpAddress);

                return new { orderId = order.Id, paymentUrl };
            }
        }

        /// <summary>
        /// Create snapshot of product information for a new order item (before save)
        /// Uses navigation property instead of OrderItemId
        /// </summary>
        private void CreateOrderItemSnapshotForNewItem(OrderItem orderItem)
        {
            var variant = _context.ProductVariants
                .Include(v => v.Product)
                .ThenInclude(p => p.Category)
                .Include(v => v.Product.Brand)
                .FirstOrDefault(v => v.Id == orderItem.ProductVariantId);

            if (variant == null) return;

            var product = variant.Product;

            // Get variant attributes as JSON string
            var variantAttributes = _context.VariantOptionValues
                .Where(vov => vov.ProductVariantId == variant.Id)
                .Include(vov => vov.ProductOptionValue)
                .ThenInclude(pov => pov.ProductOption)
                .Select(vov => new { vov.ProductOptionValue.ProductOption.Name, vov.ProductOptionValue.Value })
                .ToList();

            var variantAttributesJson = variantAttributes.Any()
                ? System.Text.Json.JsonSerializer.Serialize(variantAttributes)
                : null;

            var snapshot = new OrderItemSnapshot
            {
                OrderItem = orderItem, // Use navigation property instead of OrderItemId
                ProductId = product.Id,
                ProductVariantId = variant.Id,
                ProductName = product.Name,
                SKU = variant.SKU,
                Barcode = variant.Barcode,
                ShortDescription = product.ShortDescription,
                FullDescription = product.FullDescription,
                VariantAttributes = variantAttributesJson,
                Price = variant.Price,
                OriginalPrice = variant.OriginalPrice,
                ThumbnailUrl = product.ThumbnailUrl,
                VariantImageUrl = variant.ImageUrl,
                CategoryName = product.Category?.Name,
                BrandName = product.Brand?.Name,
                Ingredients = product.Ingredients,
                UsageInstructions = product.UsageInstructions,
                Contraindications = product.Contraindications,
                StorageInstructions = product.StorageInstructions,
                RegistrationNumber = product.RegistrationNumber,
                DrugEfficacy = product.DrugEfficacy,
                DosageInstructions = product.DosageInstructions,
                IsPrescriptionDrug = product.IsPrescriptionDrug
            };

            _context.OrderItemSnapshots.Add(snapshot);
        }

        public async Task<OrderSummaryDto?> GetOrderSummary(int id)
        {
            return await _context.Orders.Where(x => x.Id == id).Select(x => new OrderSummaryDto
                { Id = x.Id, Total = x.TotalAmount, OrderNumber = x.OrderNumber }).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<dynamic>> GetListOrders(int customerId)
        {
            return await _context.Orders.Where(x => x.CustomerId == customerId).OrderByDescending(x => x.CreatedDate)
                .Select(x => new
                {
                    Id = x.Id,
                    OrderNumber = x.OrderNumber,
                    OrderStatus = Enum.Parse<OrderStatus>(x.OrderStatus),
                    OrderStatusDescription = Enum.Parse<OrderStatus>(x.OrderStatus).GetDescription(),
                    NumberOfProducts = x.OrderItems.Count,
                    TotalOfItems = x.OrderItems.Sum(i => i.Quantity),
                    CreatedDate = x.CreatedDate
                }).ToListAsync();
        }

        public async Task<dynamic?> GetOrderDetailAsync(int id)
        {
            try
            {
                var orderDetail = await _context.Orders.Where(x => x.Id == id).Select(x => new
                {
                    Id = x.Id,
                    OrderNumber = x.OrderNumber,
                    OrderStatus = Enum.Parse<OrderStatus>(x.OrderStatus),
                    OrderStatusDescription = Enum.Parse<OrderStatus>(x.OrderStatus).GetDescription(),
                    NumberOfProducts = x.OrderItems.Count,
                    TotalOfItems = x.OrderItems.Sum(i => i.Quantity),
                    CreatedDate = x.CreatedDate,
                    Receiver = new
                    {
                        Name = x.ShippingName,
                        Address = x.ShippingAddress,
                        Phone = x.ShippingPhone
                    },
                    PaymentMethod = x.PaymentMethod,
                    DeliveryDate = x.CreatedDate.AddDays(7),
                    DeliveryMethod = x.DeliveryMethod,
                    x.EstimatedDeliveryDate,
                    Note = x.Note,
                    Items = x.OrderItems.Select(y => new
                    {
                        Name = y.ProductVariant.Product.Name,
                        ImageUrl = y.ProductVariant.ImageUrl,
                        BrandName = y.ProductVariant.Product.Brand.Name,
                        Quantity = y.Quantity,
                        Price = y.UnitPrice,
                        TotalAmount = y.UnitPrice * y.Quantity
                    }),
                    Total = x.TotalAmount,
                    SubTotal = x.SubTotal,
                    ShippingFee = x.ShippingFee,
                    UtilityFee = 0,
                }).FirstOrDefaultAsync();

                return orderDetail;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> GetTotalItemsInCart(int customerId)
        {
            return await _context.ShoppingCarts.Where(x => x.CustomerId == customerId).Select(x => x.TotalItems).FirstOrDefaultAsync();
        }

        public async Task<dynamic> GetEstimatedDeliveryTime(int provinceId, int wardId)
        {
            var province = await _context.Provinces.FindAsync(provinceId);
            if (province == null) 
                 throw new ArgumentException("Province not found");

            int days = 4; // Default to max (North/Remote)

            // Normalize name for checking
            string provinceName = province.Name?.ToLower() ?? "";
            
            // Special case for Ho Chi Minh City
            if (provinceName.Contains("hồ chí minh") || province.Code == "79")
            {
                days = 1;
            }
            else if (province.Region == "N") // Mien Nam
            {
                days = 2;
            }
            else if (province.Region == "T") // Mien Trung
            {
                days = 3;
            }
            else if (province.Region == "B") // Mien Bac
            {
                days = 4;
            }

            return new
            {
                NumberOfDays = days,
                EstimatedDate = DateTime.Now.AddDays(days)
            };
        }
    }
}
