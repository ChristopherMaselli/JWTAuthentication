using Microsoft.EntityFrameworkCore.Migrations;

namespace JWTAuthentication.Migrations
{
    public partial class PurchaseItemUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemAmount",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "PurchaseItems");

            migrationBuilder.AddColumn<int>(
                name: "ItemCost",
                table: "PurchaseItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ItemOwner",
                table: "PurchaseItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PriceId",
                table: "PurchaseItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemCost",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "ItemOwner",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "PriceId",
                table: "PurchaseItems");

            migrationBuilder.AddColumn<int>(
                name: "ItemAmount",
                table: "PurchaseItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ItemId",
                table: "PurchaseItems",
                type: "text",
                nullable: true);
        }
    }
}
