using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayBridgeAPI.Migrations
{
    /// <inheritdoc />
    public partial class changedResponsiblePersonInCorporateAccountHolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "ResponsiblePersonId",
                table: "CorporateAccountHolders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CorporateAccountHolders_ResponsiblePersonId",
                table: "CorporateAccountHolders",
                column: "ResponsiblePersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_CorporateAccountHolders_ResponsiblePeople_ResponsiblePersonId",
                table: "CorporateAccountHolders",
                column: "ResponsiblePersonId",
                principalTable: "ResponsiblePeople",
                principalColumn: "ResponsiblePersonId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CorporateAccountHolders_ResponsiblePeople_ResponsiblePersonId",
                table: "CorporateAccountHolders");

            migrationBuilder.DropIndex(
                name: "IX_CorporateAccountHolders_ResponsiblePersonId",
                table: "CorporateAccountHolders");

            migrationBuilder.DropColumn(
                name: "ResponsiblePersonId",
                table: "CorporateAccountHolders");

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
    }
}
