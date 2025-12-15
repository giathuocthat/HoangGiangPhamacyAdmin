-- ============================================================
-- Purchase Order System - Seed Data
-- ============================================================
-- This script creates sample data for testing the Purchase Order system
-- Run this after the migration has been applied

USE [ThuocGiaThat]
GO

-- ============================================================
-- 1. Suppliers
-- ============================================================
SET IDENTITY_INSERT [dbo].[Suppliers] ON
GO

INSERT INTO [dbo].[Suppliers] 
    ([Id], [Code], [Name], [Email], [Phone], [Address], [WardId], [ProvinceId], 
     [TaxCode], [BankAccount], [BankName], [PaymentTerms], [CreditLimit], 
     [IsActive], [Rating], [Notes], [CreatedDate], [UpdatedDate])
VALUES
    (1, 'SUP001', N'Công ty TNHH Dược phẩm ABC', 'contact@abcpharma.com', '0281234567', 
     N'123 Nguyễn Văn Linh, Quận 7', NULL, NULL, '0123456789', '1234567890', 
     N'Vietcombank', 30, 1000000000, 1, 5, N'Nhà cung cấp uy tín', GETDATE(), NULL),
    
    (2, 'SUP002', N'Công ty CP Dược Việt', 'info@duocviet.vn', '0287654321', 
     N'456 Lê Văn Việt, Quận 9', NULL, NULL, '9876543210', '0987654321', 
     N'Techcombank', 45, 500000000, 1, 4, N'Chuyên thuốc kháng sinh', GETDATE(), NULL),
    
    (3, 'SUP003', N'Công ty TNHH Thiết bị Y tế XYZ', 'sales@xyzmedical.com', '0283456789', 
     N'789 Võ Văn Ngân, Thủ Đức', NULL, NULL, '5555666677', '5566778899', 
     N'ACB', 60, 2000000000, 1, 5, N'Thiết bị y tế cao cấp', GETDATE(), NULL),
    
    (4, 'SUP004', N'Công ty CP Dược phẩm Hà Nội', 'hanoi@pharma.vn', '0241234567', 
     N'100 Trần Duy Hưng, Cầu Giấy, Hà Nội', NULL, NULL, '1111222233', '1122334455', 
     N'BIDV', 30, 800000000, 1, 4, NULL, GETDATE(), NULL),
    
    (5, 'SUP005', N'Công ty TNHH Dược liệu Thiên Nhiên', 'contact@naturalmeds.vn', '0287777888', 
     N'250 Lý Thường Kiệt, Quận 10', NULL, NULL, '9999888877', '9988776655', 
     N'Sacombank', 15, 300000000, 1, 3, N'Chuyên dược liệu', GETDATE(), NULL);

SET IDENTITY_INSERT [dbo].[Suppliers] OFF
GO

-- ============================================================
-- 2. Supplier Contacts
-- ============================================================
SET IDENTITY_INSERT [dbo].[SupplierContacts] ON
GO

INSERT INTO [dbo].[SupplierContacts]
    ([Id], [SupplierId], [FullName], [Position], [Department], [Email], [Phone], 
     [Mobile], [ContactType], [IsActive], [IsPrimary], [Notes], [CreatedDate], [UpdatedDate])
VALUES
    -- ABC Pharma contacts
    (1, 1, N'Nguyễn Văn A', N'Giám đốc kinh doanh', N'Kinh doanh', 'nva@abcpharma.com', 
     '0281234567', '0901234567', 1, 1, 1, NULL, GETDATE(), NULL),
    (2, 1, N'Trần Thị B', N'Trưởng phòng kế toán', N'Kế toán', 'ttb@abcpharma.com', 
     '0281234568', '0901234568', 2, 1, 0, NULL, GETDATE(), NULL),
    
    -- Dược Việt contacts
    (3, 2, N'Lê Văn C', N'Phó giám đốc', N'Điều hành', 'lvc@duocviet.vn', 
     '0287654321', '0907654321', 1, 1, 1, NULL, GETDATE(), NULL),
    (4, 2, N'Phạm Thị D', N'Nhân viên kinh doanh', N'Kinh doanh', 'ptd@duocviet.vn', 
     '0287654322', '0907654322', 3, 1, 0, NULL, GETDATE(), NULL),
    
    -- XYZ Medical contacts
    (5, 3, N'Hoàng Văn E', N'Giám đốc', N'Điều hành', 'hve@xyzmedical.com', 
     '0283456789', '0903456789', 1, 1, 1, NULL, GETDATE(), NULL),
    
    -- Hà Nội Pharma contacts
    (6, 4, N'Vũ Thị F', N'Trưởng phòng kinh doanh', N'Kinh doanh', 'vtf@pharma.vn', 
     '0241234567', '0981234567', 1, 1, 1, NULL, GETDATE(), NULL),
    
    -- Thiên Nhiên contacts
    (7, 5, N'Đặng Văn G', N'Chủ tịch', N'Điều hành', 'dvg@naturalmeds.vn', 
     '0287777888', '0907777888', 1, 1, 1, NULL, GETDATE(), NULL);

SET IDENTITY_INSERT [dbo].[SupplierContacts] OFF
GO

-- ============================================================
-- 3. Purchase Orders
-- ============================================================
-- Note: You need to have valid WarehouseId and CreatedByUserId
-- Replace these values with actual IDs from your database

DECLARE @WarehouseId INT = 1; -- Replace with actual warehouse ID
DECLARE @UserId INT = 1; -- Replace with actual user ID

SET IDENTITY_INSERT [dbo].[PurchaseOrders] ON
GO

INSERT INTO [dbo].[PurchaseOrders]
    ([Id], [OrderNumber], [SupplierId], [SupplierContactId], [WarehouseId], [Status], 
     [SubTotal], [TaxAmount], [DiscountAmount], [ShippingFee], [TotalAmount], 
     [OrderDate], [ExpectedDeliveryDate], [CompletedDate], [CreatedByUserId], 
     [ApprovedByUserId], [ApprovedDate], [Notes], [Terms], [CreatedDate], [UpdatedDate])
VALUES
    (1, 'PO-20241215-0001', 1, 1, @WarehouseId, 2, -- Status: Approved
     50000000, 5000000, 0, 500000, 55500000, 
     GETDATE(), DATEADD(DAY, 7, GETDATE()), NULL, @UserId, 
     @UserId, GETDATE(), N'Đơn hàng thuốc kháng sinh', N'Thanh toán trong 30 ngày', 
     GETDATE(), NULL),
    
    (2, 'PO-20241215-0002', 2, 3, @WarehouseId, 3, -- Status: PartiallyReceived
     30000000, 3000000, 500000, 300000, 32800000, 
     DATEADD(DAY, -5, GETDATE()), DATEADD(DAY, 2, GETDATE()), NULL, @UserId, 
     @UserId, DATEADD(DAY, -4, GETDATE()), N'Đơn hàng vitamin', N'Thanh toán trong 45 ngày', 
     DATEADD(DAY, -5, GETDATE()), NULL),
    
    (3, 'PO-20241215-0003', 3, 5, @WarehouseId, 0, -- Status: Draft
     100000000, 10000000, 2000000, 1000000, 109000000, 
     GETDATE(), DATEADD(DAY, 14, GETDATE()), NULL, @UserId, 
     NULL, NULL, N'Đơn hàng thiết bị y tế', N'Thanh toán trong 60 ngày', 
     GETDATE(), NULL),
    
    (4, 'PO-20241215-0004', 4, 6, @WarehouseId, 5, -- Status: Completed
     20000000, 2000000, 0, 200000, 22200000, 
     DATEADD(DAY, -10, GETDATE()), DATEADD(DAY, -3, GETDATE()), DATEADD(DAY, -1, GETDATE()), 
     @UserId, @UserId, DATEADD(DAY, -9, GETDATE()), N'Đơn hàng thuốc cảm cúm', 
     N'Thanh toán trong 30 ngày', DATEADD(DAY, -10, GETDATE()), NULL);

SET IDENTITY_INSERT [dbo].[PurchaseOrders] OFF
GO

-- ============================================================
-- 4. Purchase Order Items
-- ============================================================
-- Note: You need to have valid ProductVariantId
-- Replace these values with actual IDs from your database

DECLARE @ProductVariantId1 INT = 1; -- Replace with actual product variant ID
DECLARE @ProductVariantId2 INT = 2;
DECLARE @ProductVariantId3 INT = 3;

SET IDENTITY_INSERT [dbo].[PurchaseOrderItems] ON
GO

INSERT INTO [dbo].[PurchaseOrderItems]
    ([Id], [PurchaseOrderId], [ProductVariantId], [OrderedQuantity], [ReceivedQuantity], 
     [UnitPrice], [TaxRate], [DiscountAmount], [TotalAmount], [ProductName], [SKU], 
     [VariantOptions], [Notes], [CreatedDate], [UpdatedDate])
VALUES
    -- Items for PO-20241215-0001
    (1, 1, @ProductVariantId1, 1000, 0, 50000, 10, 0, 55000000, 
     N'Amoxicillin 500mg', 'AMX-500', N'Hộp 100 viên', NULL, GETDATE(), NULL),
    
    -- Items for PO-20241215-0002 (Partially received)
    (2, 2, @ProductVariantId2, 500, 300, 60000, 10, 500000, 32800000, 
     N'Vitamin C 1000mg', 'VTC-1000', N'Hộp 50 viên', NULL, DATEADD(DAY, -5, GETDATE()), NULL),
    
    -- Items for PO-20241215-0003
    (3, 3, @ProductVariantId3, 50, 0, 2000000, 10, 2000000, 109000000, 
     N'Máy đo huyết áp điện tử', 'BP-MONITOR-01', N'Loại cao cấp', NULL, GETDATE(), NULL),
    
    -- Items for PO-20241215-0004 (Completed)
    (4, 4, @ProductVariantId1, 400, 400, 50000, 10, 0, 22000000, 
     N'Paracetamol 500mg', 'PARA-500', N'Hộp 100 viên', NULL, DATEADD(DAY, -10, GETDATE()), NULL);

SET IDENTITY_INSERT [dbo].[PurchaseOrderItems] OFF
GO

-- ============================================================
-- 5. Purchase Order History
-- ============================================================
SET IDENTITY_INSERT [dbo].[PurchaseOrderHistories] ON
GO

INSERT INTO [dbo].[PurchaseOrderHistories]
    ([Id], [PurchaseOrderId], [FromStatus], [ToStatus], [Action], [ChangedByUserId], 
     [ChangedDate], [ChangeDetails], [Reason], [Notes])
VALUES
    (1, 1, NULL, 'Draft', 'Created', @UserId, GETDATE(), NULL, NULL, N'Tạo đơn hàng mới'),
    (2, 1, 'Draft', 'Approved', 'Approved', @UserId, GETDATE(), NULL, NULL, N'Đã phê duyệt'),
    
    (3, 2, NULL, 'Draft', 'Created', @UserId, DATEADD(DAY, -5, GETDATE()), NULL, NULL, N'Tạo đơn hàng mới'),
    (4, 2, 'Draft', 'Approved', 'Approved', @UserId, DATEADD(DAY, -4, GETDATE()), NULL, NULL, N'Đã phê duyệt'),
    (5, 2, 'Approved', 'PartiallyReceived', 'Updated', @UserId, DATEADD(DAY, -2, GETDATE()), NULL, NULL, N'Đã nhận một phần hàng'),
    
    (6, 3, NULL, 'Draft', 'Created', @UserId, GETDATE(), NULL, NULL, N'Tạo đơn hàng mới'),
    
    (7, 4, NULL, 'Draft', 'Created', @UserId, DATEADD(DAY, -10, GETDATE()), NULL, NULL, N'Tạo đơn hàng mới'),
    (8, 4, 'Draft', 'Approved', 'Approved', @UserId, DATEADD(DAY, -9, GETDATE()), NULL, NULL, N'Đã phê duyệt'),
    (9, 4, 'Approved', 'Completed', 'Updated', @UserId, DATEADD(DAY, -1, GETDATE()), NULL, NULL, N'Đã hoàn thành');

SET IDENTITY_INSERT [dbo].[PurchaseOrderHistories] OFF
GO

-- ============================================================
-- 6. Goods Receipts
-- ============================================================
SET IDENTITY_INSERT [dbo].[GoodsReceipts] ON
GO

INSERT INTO [dbo].[GoodsReceipts]
    ([Id], [ReceiptNumber], [PurchaseOrderId], [WarehouseId], [Status], [ScheduledDate], 
     [ReceivedDate], [CompletedDate], [ReceivedByUserId], [InspectedByUserId], 
     [ShippingCarrier], [TrackingNumber], [VehicleNumber], [Notes], [RejectionReason], 
     [CreatedDate], [UpdatedDate])
VALUES
    -- Partial receipt for PO-20241215-0002
    (1, 'GR-20241215-0001', 2, @WarehouseId, 3, -- Status: Completed
     DATEADD(DAY, -3, GETDATE()), DATEADD(DAY, -2, GETDATE()), DATEADD(DAY, -1, GETDATE()), 
     @UserId, @UserId, N'Viettel Post', 'VTP123456789', '29A-12345', 
     N'Nhận hàng đợt 1', NULL, DATEADD(DAY, -3, GETDATE()), NULL),
    
    -- Full receipt for PO-20241215-0004
    (2, 'GR-20241215-0002', 4, @WarehouseId, 3, -- Status: Completed
     DATEADD(DAY, -2, GETDATE()), DATEADD(DAY, -1, GETDATE()), DATEADD(DAY, -1, GETDATE()), 
     @UserId, @UserId, N'Giao Hàng Nhanh', 'GHN987654321', '51B-67890', 
     N'Nhận hàng đầy đủ', NULL, DATEADD(DAY, -2, GETDATE()), NULL);

SET IDENTITY_INSERT [dbo].[GoodsReceipts] OFF
GO

-- ============================================================
-- 7. Goods Receipt Items
-- ============================================================
SET IDENTITY_INSERT [dbo].[GoodsReceiptItems] ON
GO

INSERT INTO [dbo].[GoodsReceiptItems]
    ([Id], [GoodsReceiptId], [PurchaseOrderItemId], [OrderedQuantity], [ReceivedQuantity], 
     [AcceptedQuantity], [QualityStatus], [RejectedQuantity], [BatchNumber], 
     [ManufactureDate], [ExpiryDate], [LocationCode], [ShelfNumber], [Notes], 
     [InspectionNotes], [CreatedDate], [UpdatedDate])
VALUES
    -- Partial receipt for Vitamin C
    (1, 1, 2, 300, 300, 300, 1, 0, -- QualityStatus: Good
     'BATCH-VTC-2024-12', DATEADD(MONTH, -2, GETDATE()), DATEADD(YEAR, 2, GETDATE()), 
     'A-01', 'S-05', NULL, N'Hàng đạt chất lượng tốt', DATEADD(DAY, -2, GETDATE()), NULL),
    
    -- Full receipt for Paracetamol
    (2, 2, 4, 400, 400, 395, 1, 5, -- QualityStatus: Good, 5 rejected
     'BATCH-PARA-2024-12', DATEADD(MONTH, -1, GETDATE()), DATEADD(YEAR, 3, GETDATE()), 
     'B-02', 'S-10', NULL, N'5 viên bị vỡ bao bì', DATEADD(DAY, -1, GETDATE()), NULL);

SET IDENTITY_INSERT [dbo].[GoodsReceiptItems] OFF
GO

PRINT 'Seed data for Purchase Order system has been inserted successfully!'
GO
