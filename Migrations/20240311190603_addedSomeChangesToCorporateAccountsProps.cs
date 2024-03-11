using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayBridgeAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedSomeChangesToCorporateAccountsProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorparateBankAccounts");

            migrationBuilder.RenameColumn(
                name: "TypesOfActivity",
                table: "CorporateAccountHolders",
                newName: "Status");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CorporateAccountHolders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CorporateBankAccounts",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AccountOwnerId = table.Column<int>(type: "int", nullable: false),
                    ManagerId = table.Column<int>(type: "int", nullable: false),
                    BankId = table.Column<int>(type: "int", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporateBankAccounts", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_CorporateBankAccounts_Banks_BankId",
                        column: x => x.BankId,
                        principalTable: "Banks",
                        principalColumn: "BankId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CorporateBankAccounts_CorporateAccountHolders_AccountOwnerId",
                        column: x => x.AccountOwnerId,
                        principalTable: "CorporateAccountHolders",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CorporateBankAccounts_Managers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Managers",
                        principalColumn: "ManagerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorporateBankAccounts_AccountOwnerId",
                table: "CorporateBankAccounts",
                column: "AccountOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateBankAccounts_BankId",
                table: "CorporateBankAccounts",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateBankAccounts_ManagerId",
                table: "CorporateBankAccounts",
                column: "ManagerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorporateBankAccounts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CorporateAccountHolders");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "CorporateAccountHolders",
                newName: "TypesOfActivity");

            migrationBuilder.CreateTable(
                name: "CorparateBankAccounts",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankId = table.Column<int>(type: "int", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrencyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorparateBankAccounts", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_CorparateBankAccounts_Banks_BankId",
                        column: x => x.BankId,
                        principalTable: "Banks",
                        principalColumn: "BankId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorparateBankAccounts_BankId",
                table: "CorparateBankAccounts",
                column: "BankId");
        }
    }
}
