using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace JWTAuthentication.Migrations
{
    public partial class Update02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDatas_UserAccounts_UserAccountId",
                table: "UserDatas");

            migrationBuilder.DropIndex(
                name: "IX_UserDatas_UserAccountId",
                table: "UserDatas");

            migrationBuilder.DropColumn(
                name: "UserAccountId",
                table: "UserDatas");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "UserDatas",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "UserDatas",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDatas_UserAccounts_UserId",
                table: "UserDatas",
                column: "UserId",
                principalTable: "UserAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDatas_UserAccounts_UserId",
                table: "UserDatas");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "UserDatas");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "UserDatas",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<long>(
                name: "UserAccountId",
                table: "UserDatas",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDatas_UserAccountId",
                table: "UserDatas",
                column: "UserAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDatas_UserAccounts_UserAccountId",
                table: "UserDatas",
                column: "UserAccountId",
                principalTable: "UserAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
