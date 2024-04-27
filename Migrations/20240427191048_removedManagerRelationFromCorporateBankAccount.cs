using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayBridgeAPI.Migrations
{
    /// <inheritdoc />
    public partial class removedManagerRelationFromCorporateBankAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CorporateBankAccounts_Managers_ManagerId",
                table: "CorporateBankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_CorporateBankAccounts_ManagerId",
                table: "CorporateBankAccounts");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "CorporateBankAccounts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "CorporateBankAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CorporateBankAccounts_ManagerId",
                table: "CorporateBankAccounts",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CorporateBankAccounts_Managers_ManagerId",
                table: "CorporateBankAccounts",
                column: "ManagerId",
                principalTable: "Managers",
                principalColumn: "ManagerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
