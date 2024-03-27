using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayBridgeAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedCompanyBankAssetsToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyBankAssets",
                columns: table => new
                {
                    AssetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetUniqueNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IBAN_Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorporateAccountId = table.Column<int>(type: "int", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyBankAssets", x => x.AssetId);
                    table.ForeignKey(
                        name: "FK_CompanyBankAssets_CorporateBankAccounts_CorporateAccountId",
                        column: x => x.CorporateAccountId,
                        principalTable: "CorporateBankAccounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyBankAssets_CorporateAccountId",
                table: "CompanyBankAssets",
                column: "CorporateAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyBankAssets");
        }
    }
}
