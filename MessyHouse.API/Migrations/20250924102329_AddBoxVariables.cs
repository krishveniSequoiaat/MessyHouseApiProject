using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessyHouse.API.Migrations
{
    /// <inheritdoc />
    public partial class AddBoxVariables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "StorageBoxes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "StorageBoxes");
        }
    }
}
