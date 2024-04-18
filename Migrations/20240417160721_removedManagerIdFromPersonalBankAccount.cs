using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayBridgeAPI.Migrations
{
    /// <inheritdoc />
    public partial class removedManagerIdFromPersonalBankAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonalBankAccounts_Managers_ManagerId",
                table: "PersonalBankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_PersonalBankAccounts_ManagerId",
                table: "PersonalBankAccounts");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "PersonalBankAccounts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "PersonalBankAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PersonalBankAccounts_ManagerId",
                table: "PersonalBankAccounts",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalBankAccounts_Managers_ManagerId",
                table: "PersonalBankAccounts",
                column: "ManagerId",
                principalTable: "Managers",
                principalColumn: "ManagerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
