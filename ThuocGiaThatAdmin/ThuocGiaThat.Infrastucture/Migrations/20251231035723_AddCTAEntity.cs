using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThuocGiaThat.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class AddCTAEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CTAs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ButtonText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Style = table.Column<int>(type: "int", nullable: false),
                    TargetUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OpenInNewTab = table.Column<bool>(type: "bit", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MobileImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BackgroundColor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TextColor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    ClickCount = table.Column<int>(type: "int", nullable: false),
                    CampaignId = table.Column<int>(type: "int", nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CTAs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CTAs_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CTAs_CampaignId",
                table: "CTAs",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CTAs_Code",
                table: "CTAs",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_CTAs_DisplayOrder",
                table: "CTAs",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_CTAs_IsActive",
                table: "CTAs",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CTAs_Position",
                table: "CTAs",
                column: "Position");

            migrationBuilder.CreateIndex(
                name: "IX_CTAs_Type",
                table: "CTAs",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_CTAs_ValidFrom_ValidTo",
                table: "CTAs",
                columns: new[] { "ValidFrom", "ValidTo" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CTAs");
        }
    }
}
