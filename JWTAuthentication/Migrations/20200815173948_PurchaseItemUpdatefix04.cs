using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace JWTAuthentication.Migrations
{
    public partial class PurchaseItemUpdatefix04 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SomethingDifferents",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PriceId = table.Column<string>(nullable: true),
                    ItemName = table.Column<string>(nullable: true),
                    ItemCost = table.Column<int>(nullable: false),
                    ItemDescription = table.Column<string>(nullable: true),
                    ItemOwner = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SomethingDifferents", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SomethingDifferents");
        }
    }
}
