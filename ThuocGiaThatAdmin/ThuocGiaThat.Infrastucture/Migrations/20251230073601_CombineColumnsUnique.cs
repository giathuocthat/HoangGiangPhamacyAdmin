using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThuocGiaThat.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class CombineColumnsUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FavouriteProducts_CustomerId_ProductVariantId",
                table: "FavouriteProducts");

            migrationBuilder.CreateIndex(
                name: "IX_FavouriteProducts_CustomerId_ProductVariantId_Type",
                table: "FavouriteProducts",
                columns: new[] { "CustomerId", "ProductVariantId", "Type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FavouriteProducts_CustomerId_ProductVariantId_Type",
                table: "FavouriteProducts");

            migrationBuilder.CreateIndex(
                name: "IX_FavouriteProducts_CustomerId_ProductVariantId",
                table: "FavouriteProducts",
                columns: new[] { "CustomerId", "ProductVariantId" },
                unique: true);
        }
    }
}
