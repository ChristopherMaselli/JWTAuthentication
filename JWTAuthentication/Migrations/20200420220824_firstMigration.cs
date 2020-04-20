using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace JWTAuthentication.Migrations
{
    public partial class firstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    token = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.token);
                });

            migrationBuilder.CreateTable(
                name: "UserAccounts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserName = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    GameId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameName = table.Column<string>(nullable: true),
                    OwnerId = table.Column<long>(nullable: false),
                    DateCreated = table.Column<int>(nullable: false),
                    NextGameDateTime = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameId);
                    table.ForeignKey(
                        name: "FK_Games_UserAccounts_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "UserAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProfilePageDatas",
                columns: table => new
                {
                    UserDataId = table.Column<long>(nullable: false),
                    MemberSince = table.Column<string>(nullable: true),
                    HoursPlayed = table.Column<string>(nullable: true),
                    Subscription = table.Column<string>(nullable: true)
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

            migrationBuilder.CreateTable(
                name: "PlayerToGames",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameId = table.Column<long>(nullable: false),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerToGames", x => x.id);
                    table.ForeignKey(
                        name: "FK_PlayerToGames_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerToGames_UserAccounts_UserId",
                        column: x => x.UserId,
                        principalTable: "UserAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_OwnerId",
                table: "Games",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerToGames_GameId",
                table: "PlayerToGames",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerToGames_UserId",
                table: "PlayerToGames",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerToGames");

            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropTable(
                name: "UserProfilePageDatas");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "UserAccounts");
        }
    }
}
