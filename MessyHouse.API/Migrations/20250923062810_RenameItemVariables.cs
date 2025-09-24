using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessyHouse.API.Migrations
{
    /// <inheritdoc />
    public partial class RenameItemVariables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Items",
                newName: "ImageUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Items",
                newName: "Image");
        }
    }
}
