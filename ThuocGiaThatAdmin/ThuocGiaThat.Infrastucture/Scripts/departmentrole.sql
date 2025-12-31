GO
SET IDENTITY_INSERT [dbo].[Departments] ON
INSERT INTO [dbo].[Departments]([Id],[Name],[Code],[IsActive], [CreatedDate]) VALUES (1, N'Ban Giám Đốc', 'BOD', 1, GETDATE());
INSERT INTO [dbo].[Departments]([Id],[Name],[Code],[IsActive], [CreatedDate]) VALUES (2, N'Marketing', 'MAR', 1, GETDATE());
INSERT INTO [dbo].[Departments]([Id],[Name],[Code],[IsActive], [CreatedDate]) VALUES (3, N'Kinh Doanh', 'SAL', 1, GETDATE());
INSERT INTO [dbo].[Departments]([Id],[Name],[Code],[IsActive], [CreatedDate]) VALUES (4, N'Thua Mua', 'PUR', 1, GETDATE());
INSERT INTO [dbo].[Departments]([Id],[Name],[Code],[IsActive], [CreatedDate]) VALUES (5, N'Kế Toán', 'ACC', 1, GETDATE());
INSERT INTO [dbo].[Departments]([Id],[Name],[Code],[IsActive], [CreatedDate]) VALUES (6, N'Nghiên Cứu SP', 'RDP', 1, GETDATE());
INSERT INTO [dbo].[Departments]([Id],[Name],[Code],[IsActive], [CreatedDate]) VALUES (7, N'Công Nghệ', 'ITE', 1, GETDATE());
INSERT INTO [dbo].[Departments]([Id],[Name],[Code],[IsActive], [CreatedDate]) VALUES (8, N'Nhân Sự', 'HRD', 1, GETDATE());
INSERT INTO [dbo].[Departments]([Id],[Name],[Code],[IsActive], [CreatedDate]) VALUES (9, N'Pháp Lý', 'LOC', 1, GETDATE());
INSERT INTO [dbo].[Departments]([Id],[Name],[Code],[IsActive], [CreatedDate]) VALUES (10, N'Thầu', 'TED', 1, GETDATE());
INSERT INTO [dbo].[Departments]([Id],[Name],[Code],[IsActive], [CreatedDate]) VALUES (11, N'Thị Trường Quốc Tế', 'IND', 1, GETDATE());
INSERT INTO [dbo].[Departments]([Id],[Name],[Code],[IsActive], [CreatedDate]) VALUES (12, N'Kho','INV' ,1, GETDATE());
INSERT INTO [dbo].[Departments]([Id],[Name],[Code],[IsActive], [CreatedDate]) VALUES (14, N'Giao Nhận', 'LOG', 1, GETDATE());
SET IDENTITY_INSERT [dbo].[Departments] OFF
GO