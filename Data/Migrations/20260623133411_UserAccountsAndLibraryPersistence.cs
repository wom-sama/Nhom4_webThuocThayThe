using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nhom4WebThuocThayThe.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserAccountsAndLibraryPersistence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegisteredUserAccounts",
                columns: table => new
                {
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordSalt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisteredUserAccounts", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "SavedDrugs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserEmail = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DrugId = table.Column<int>(type: "int", nullable: false),
                    SavedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedDrugs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedDrugs_Drugs_DrugId",
                        column: x => x.DrugId,
                        principalTable: "Drugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSearchHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserEmail = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Keyword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultCount = table.Column<int>(type: "int", nullable: false),
                    SearchedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSearchHistories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredUserAccounts_Role",
                table: "RegisteredUserAccounts",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_SavedDrugs_DrugId",
                table: "SavedDrugs",
                column: "DrugId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedDrugs_UserEmail_DrugId",
                table: "SavedDrugs",
                columns: new[] { "UserEmail", "DrugId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSearchHistories_UserEmail_SearchedAt",
                table: "UserSearchHistories",
                columns: new[] { "UserEmail", "SearchedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegisteredUserAccounts");

            migrationBuilder.DropTable(
                name: "SavedDrugs");

            migrationBuilder.DropTable(
                name: "UserSearchHistories");
        }
    }
}
