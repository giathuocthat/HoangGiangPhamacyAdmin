using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuocGiaThat.Infrastucture;
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

        public OrderService(TrueMecContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
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
                countryId: dto.CountryId,
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
                countryId: dto.CountryId,
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
            int? countryId,
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

            if (countryId.HasValue)
            {
                var countryExists = await _context.Countries.AnyAsync(c => c.Id == countryId.Value);
                if (!countryExists)
                    throw new ArgumentException($"Country with ID {countryId.Value} not found");
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
            
            if (countryId.HasValue)
                country = await _context.Countries.FindAsync(countryId.Value);

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
        /// Get all orders
        /// </summary>
        public async Task<List<OrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                    .ThenInclude(v => v.Product)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();

            return orders.Select(o => MapToResponseDto(o, null, null, null)).ToList();
        }

        /// <summary>
        /// Get orders with pagination and search
        /// </summary>
        public async Task<(List<OrderListDto> Orders, int TotalCount)> GetOrdersAsync(
            int pageNumber,
            int pageSize,
            string? searchText)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));

            if (pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Page size must be between 1 and 100", nameof(pageSize));

            var query = _context.Orders
                .Include(o => o.Customer)
                .AsQueryable();

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
    }
}
