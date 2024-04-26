using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayBridgeAPI.Migrations
{
    /// <inheritdoc />
    public partial class removedRelationInCorporateAccountHolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CorporateAccountHolders_Managers_ManagerId",
                table: "CorporateAccountHolders");

            migrationBuilder.DropIndex(
                name: "IX_CorporateAccountHolders_ManagerId",
                table: "CorporateAccountHolders");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "CorporateAccountHolders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "CorporateAccountHolders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CorporateAccountHolders_ManagerId",
                table: "CorporateAccountHolders",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CorporateAccountHolders_Managers_ManagerId",
                table: "CorporateAccountHolders",
                column: "ManagerId",
                principalTable: "Managers",
                principalColumn: "ManagerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
