using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayBridgeAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedRelationInCorporateAccountHolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "CorporateAccountHolders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CorporateAccountHolders_UserId",
                table: "CorporateAccountHolders",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CorporateAccountHolders_AspNetUsers_UserId",
                table: "CorporateAccountHolders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CorporateAccountHolders_AspNetUsers_UserId",
                table: "CorporateAccountHolders");

            migrationBuilder.DropIndex(
                name: "IX_CorporateAccountHolders_UserId",
                table: "CorporateAccountHolders");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CorporateAccountHolders");
        }
    }
}
