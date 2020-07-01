using Microsoft.EntityFrameworkCore.Migrations;

namespace JWTAuthentication.Migrations
{
    public partial class NewMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProfilePageDatas");

            migrationBuilder.AddColumn<long>(
                name: "GameId",
                table: "UserAccounts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "Games",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserDatas",
                columns: table => new
                {
                    UserId = table.Column<long>(nullable: false),
                    MemberSince = table.Column<string>(nullable: true),
                    HoursPlayed = table.Column<string>(nullable: true),
                    Subscription = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDatas", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserDatas_UserAccounts_UserId",
                        column: x => x.UserId,
                        principalTable: "UserAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_GameId",
                table: "UserAccounts",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccounts_Games_GameId",
                table: "UserAccounts",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAccounts_Games_GameId",
                table: "UserAccounts");

            migrationBuilder.DropTable(
                name: "UserDatas");

            migrationBuilder.DropIndex(
                name: "IX_UserAccounts_GameId",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "Games");

            migrationBuilder.CreateTable(
                name: "UserProfilePageDatas",
                columns: table => new
                {
                    UserDataId = table.Column<long>(type: "bigint", nullable: false),
                    HoursPlayed = table.Column<string>(type: "text", nullable: true),
                    MemberSince = table.Column<string>(type: "text", nullable: true),
                    Subscription = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfilePageDatas", x => x.UserDataId);
                    table.ForeignKey(
                        name: "FK_UserProfilePageDatas_UserAccounts_UserDataId",
                        column: x => x.UserDataId,
                        principalTable: "UserAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
