using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayBridgeAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedNewTransactionTablesToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyToCompanyTransactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanySenderId = table.Column<int>(type: "int", nullable: false),
                    CompanyReceiverId = table.Column<int>(type: "int", nullable: false),
                    ReceiverBankAssetId = table.Column<int>(type: "int", nullable: false),
                    SenderBankAssetId = table.Column<int>(type: "int", nullable: false),
                    StripePaymentIntentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfTransaction = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyToCompanyTransactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_CompanyToCompanyTransactions_CompanyBankAssets_ReceiverBankAssetId",
                        column: x => x.ReceiverBankAssetId,
                        principalTable: "CompanyBankAssets",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyToCompanyTransactions_CompanyBankAssets_SenderBankAssetId",
                        column: x => x.SenderBankAssetId,
                        principalTable: "CompanyBankAssets",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyToCompanyTransactions_CorporateBankAccounts_CompanyReceiverId",
                        column: x => x.CompanyReceiverId,
                        principalTable: "CorporateBankAccounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyToCompanyTransactions_CorporateBankAccounts_CompanySenderId",
                        column: x => x.CompanySenderId,
                        principalTable: "CorporateBankAccounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyToUserTransactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanySenderId = table.Column<int>(type: "int", nullable: false),
                    ReceiverId = table.Column<int>(type: "int", nullable: false),
                    ReceiverBankCardId = table.Column<int>(type: "int", nullable: false),
                    SenderBankAssetId = table.Column<int>(type: "int", nullable: false),
                    StripePaymentIntentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfTransaction = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyToUserTransactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_CompanyToUserTransactions_BankCards_ReceiverBankCardId",
                        column: x => x.ReceiverBankCardId,
                        principalTable: "BankCards",
                        principalColumn: "BankCardId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyToUserTransactions_CompanyBankAssets_SenderBankAssetId",
                        column: x => x.SenderBankAssetId,
                        principalTable: "CompanyBankAssets",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyToUserTransactions_CorporateBankAccounts_CompanySenderId",
                        column: x => x.CompanySenderId,
                        principalTable: "CorporateBankAccounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyToUserTransactions_PersonalBankAccounts_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "PersonalBankAccounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserToCompanyTransactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<int>(type: "int", nullable: false),
                    CompanyReceiverId = table.Column<int>(type: "int", nullable: false),
                    SenderBankCardId = table.Column<int>(type: "int", nullable: false),
                    ReceiverBankAssetId = table.Column<int>(type: "int", nullable: false),
                    StripePaymentIntentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfTransaction = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToCompanyTransactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_UserToCompanyTransactions_BankCards_SenderBankCardId",
                        column: x => x.SenderBankCardId,
                        principalTable: "BankCards",
                        principalColumn: "BankCardId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserToCompanyTransactions_CompanyBankAssets_ReceiverBankAssetId",
                        column: x => x.ReceiverBankAssetId,
                        principalTable: "CompanyBankAssets",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserToCompanyTransactions_CorporateBankAccounts_CompanyReceiverId",
                        column: x => x.CompanyReceiverId,
                        principalTable: "CorporateBankAccounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserToCompanyTransactions_PersonalBankAccounts_SenderId",
                        column: x => x.SenderId,
                        principalTable: "PersonalBankAccounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyToCompanyTransactions_CompanyReceiverId",
                table: "CompanyToCompanyTransactions",
                column: "CompanyReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyToCompanyTransactions_CompanySenderId",
                table: "CompanyToCompanyTransactions",
                column: "CompanySenderId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyToCompanyTransactions_ReceiverBankAssetId",
                table: "CompanyToCompanyTransactions",
                column: "ReceiverBankAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyToCompanyTransactions_SenderBankAssetId",
                table: "CompanyToCompanyTransactions",
                column: "SenderBankAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyToUserTransactions_CompanySenderId",
                table: "CompanyToUserTransactions",
                column: "CompanySenderId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyToUserTransactions_ReceiverBankCardId",
                table: "CompanyToUserTransactions",
                column: "ReceiverBankCardId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyToUserTransactions_ReceiverId",
                table: "CompanyToUserTransactions",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyToUserTransactions_SenderBankAssetId",
                table: "CompanyToUserTransactions",
                column: "SenderBankAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_UserToCompanyTransactions_CompanyReceiverId",
                table: "UserToCompanyTransactions",
                column: "CompanyReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_UserToCompanyTransactions_ReceiverBankAssetId",
                table: "UserToCompanyTransactions",
                column: "ReceiverBankAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_UserToCompanyTransactions_SenderBankCardId",
                table: "UserToCompanyTransactions",
                column: "SenderBankCardId");

            migrationBuilder.CreateIndex(
                name: "IX_UserToCompanyTransactions_SenderId",
                table: "UserToCompanyTransactions",
                column: "SenderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyToCompanyTransactions");

            migrationBuilder.DropTable(
                name: "CompanyToUserTransactions");

            migrationBuilder.DropTable(
                name: "UserToCompanyTransactions");
        }
    }
}
