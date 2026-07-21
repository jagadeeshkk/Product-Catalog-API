using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductCatalog.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class fourthversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceUnit",
                table: "ProductDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PriceUnit",
                table: "ProductDetails",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
