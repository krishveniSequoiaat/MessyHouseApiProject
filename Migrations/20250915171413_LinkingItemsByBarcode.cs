using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessyHouseApiProject.Migrations
{
    /// <inheritdoc />
    public partial class LinkingItemsByBarcode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StorageBoxId",
                table: "Items");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_StorageBoxes_Barcode",
                table: "StorageBoxes",
                column: "Barcode");

            migrationBuilder.CreateIndex(
                name: "IX_StorageBoxes_Barcode",
                table: "StorageBoxes",
                column: "Barcode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_Barcode",
                table: "Items",
                column: "Barcode");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_StorageBoxes_Barcode",
                table: "Items",
                column: "Barcode",
                principalTable: "StorageBoxes",
                principalColumn: "Barcode",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_StorageBoxes_Barcode",
                table: "Items");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_StorageBoxes_Barcode",
                table: "StorageBoxes");

            migrationBuilder.DropIndex(
                name: "IX_StorageBoxes_Barcode",
                table: "StorageBoxes");

            migrationBuilder.DropIndex(
                name: "IX_Items_Barcode",
                table: "Items");

            migrationBuilder.AddColumn<int>(
                name: "StorageBoxId",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
