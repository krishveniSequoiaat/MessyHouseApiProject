using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessyHouseApiProject.Migrations
{
    /// <inheritdoc />
    public partial class RenameItemVariables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Label",
                table: "Items",
                newName: "Tag");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Items",
                newName: "Image");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Tag",
                table: "Items",
                newName: "Label");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Items",
                newName: "ImageUrl");
        }
    }
}
