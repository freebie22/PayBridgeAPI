using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayBridgeAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedUserToUserTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserToUserTransactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<int>(type: "int", nullable: false),
                    ReceiverId = table.Column<int>(type: "int", nullable: false),
                    SenderBankCardId = table.Column<int>(type: "int", nullable: false),
                    ReceiverBankCardId = table.Column<int>(type: "int", nullable: false),
                    StripePaymentIntentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfTransaction = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToUserTransactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_UserToUserTransactions_BankCards_ReceiverBankCardId",
                        column: x => x.ReceiverBankCardId,
                        principalTable: "BankCards",
                        principalColumn: "BankCardId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserToUserTransactions_BankCards_SenderBankCardId",
                        column: x => x.SenderBankCardId,
                        principalTable: "BankCards",
                        principalColumn: "BankCardId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserToUserTransactions_PersonalBankAccounts_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "PersonalBankAccounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserToUserTransactions_PersonalBankAccounts_SenderId",
                        column: x => x.SenderId,
                        principalTable: "PersonalBankAccounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserToUserTransactions_ReceiverBankCardId",
                table: "UserToUserTransactions",
                column: "ReceiverBankCardId");

            migrationBuilder.CreateIndex(
                name: "IX_UserToUserTransactions_ReceiverId",
                table: "UserToUserTransactions",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_UserToUserTransactions_SenderBankCardId",
                table: "UserToUserTransactions",
                column: "SenderBankCardId");

            migrationBuilder.CreateIndex(
                name: "IX_UserToUserTransactions_SenderId",
                table: "UserToUserTransactions",
                column: "SenderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserToUserTransactions");
        }
    }
}
