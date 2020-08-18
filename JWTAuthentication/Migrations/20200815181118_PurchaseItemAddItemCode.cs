using Microsoft.EntityFrameworkCore.Migrations;

namespace JWTAuthentication.Migrations
{
    public partial class PurchaseItemAddItemCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemOwner",
                table: "PurchaseItems");

            migrationBuilder.AddColumn<string>(
                name: "ItemCode",
                table: "PurchaseItems",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ItemOwnerId",
                table: "PurchaseItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_ItemOwnerId",
                table: "PurchaseItems",
                column: "ItemOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_UserAccounts_ItemOwnerId",
                table: "PurchaseItems",
                column: "ItemOwnerId",
                principalTable: "UserAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_UserAccounts_ItemOwnerId",
                table: "PurchaseItems");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseItems_ItemOwnerId",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "ItemCode",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "ItemOwnerId",
                table: "PurchaseItems");

            migrationBuilder.AddColumn<string>(
                name: "ItemOwner",
                table: "PurchaseItems",
                type: "text",
                nullable: true);
        }
    }
}
