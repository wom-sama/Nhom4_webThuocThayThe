using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nhom4WebThuocThayThe.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDatabaseForeignKeyConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserSearchHistories_CategoryId",
                table: "UserSearchHistories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Drugs_CategoryId",
                table: "Drugs",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Drugs_DosageFormId",
                table: "Drugs",
                column: "DosageFormId");

            migrationBuilder.CreateIndex(
                name: "IX_Drugs_ManufacturerId",
                table: "Drugs",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Drugs_UnitId",
                table: "Drugs",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drugs_Categories_CategoryId",
                table: "Drugs",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Drugs_DosageForms_DosageFormId",
                table: "Drugs",
                column: "DosageFormId",
                principalTable: "DosageForms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Drugs_Manufacturers_ManufacturerId",
                table: "Drugs",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Drugs_Units_UnitId",
                table: "Drugs",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedDrugs_RegisteredUserAccounts_UserEmail",
                table: "SavedDrugs",
                column: "UserEmail",
                principalTable: "RegisteredUserAccounts",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSearchHistories_Categories_CategoryId",
                table: "UserSearchHistories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSearchHistories_RegisteredUserAccounts_UserEmail",
                table: "UserSearchHistories",
                column: "UserEmail",
                principalTable: "RegisteredUserAccounts",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drugs_Categories_CategoryId",
                table: "Drugs");

            migrationBuilder.DropForeignKey(
                name: "FK_Drugs_DosageForms_DosageFormId",
                table: "Drugs");

            migrationBuilder.DropForeignKey(
                name: "FK_Drugs_Manufacturers_ManufacturerId",
                table: "Drugs");

            migrationBuilder.DropForeignKey(
                name: "FK_Drugs_Units_UnitId",
                table: "Drugs");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedDrugs_RegisteredUserAccounts_UserEmail",
                table: "SavedDrugs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSearchHistories_Categories_CategoryId",
                table: "UserSearchHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSearchHistories_RegisteredUserAccounts_UserEmail",
                table: "UserSearchHistories");

            migrationBuilder.DropIndex(
                name: "IX_UserSearchHistories_CategoryId",
                table: "UserSearchHistories");

            migrationBuilder.DropIndex(
                name: "IX_Drugs_CategoryId",
                table: "Drugs");

            migrationBuilder.DropIndex(
                name: "IX_Drugs_DosageFormId",
                table: "Drugs");

            migrationBuilder.DropIndex(
                name: "IX_Drugs_ManufacturerId",
                table: "Drugs");

            migrationBuilder.DropIndex(
                name: "IX_Drugs_UnitId",
                table: "Drugs");
        }
    }
}
