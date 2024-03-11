using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayBridgeAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedSomeChangesToPersonalBankAccountModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonalBankAccounts_AspNetUsers_ManagerId",
                table: "PersonalBankAccounts");

            migrationBuilder.DropColumn(
                name: "EnglishName",
                table: "CorporateAccountHolders");

            migrationBuilder.AlterColumn<int>(
                name: "ManagerId",
                table: "PersonalBankAccounts",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalBankAccounts_Managers_ManagerId",
                table: "PersonalBankAccounts",
                column: "ManagerId",
                principalTable: "Managers",
                principalColumn: "ManagerId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonalBankAccounts_Managers_ManagerId",
                table: "PersonalBankAccounts");

            migrationBuilder.AlterColumn<string>(
                name: "ManagerId",
                table: "PersonalBankAccounts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "EnglishName",
                table: "CorporateAccountHolders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalBankAccounts_AspNetUsers_ManagerId",
                table: "PersonalBankAccounts",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
