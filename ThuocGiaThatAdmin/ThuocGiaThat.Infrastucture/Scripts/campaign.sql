
SET IDENTITY_INSERT [dbo].[Campaigns] ON
INSERT INTO [dbo].[Campaigns](Id,[CampaignCode],[CampaignName],[Description],[StartDate],[EndDate],[Budget],[IsActive],[CreatedDate],[UpdatedDate]) VALUES(1,'CODE','HGSG Sale','Get up to 50% off on selected items',GETDATE(),GETDATE() + 7,20000000,1,GETDATE(),GETDATE())
SET IDENTITY_INSERT [dbo].[Campaigns] OFF
GO