using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayBridgeAPI.Migrations
{
    /// <inheritdoc />
    public partial class reworkedBankModelProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Banks",
                newName: "WebsiteURL");

            migrationBuilder.RenameColumn(
                name: "CEOMiddleName",
                table: "Banks",
                newName: "TaxIdentificationNumber");

            migrationBuilder.RenameColumn(
                name: "CEOLastName",
                table: "Banks",
                newName: "ShortBankName");

            migrationBuilder.RenameColumn(
                name: "CEOFirstName",
                table: "Banks",
                newName: "SettlementAccount");

            migrationBuilder.AddColumn<string>(
                name: "BankIdentificationCode",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullBankName",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IBAN",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalBankLicense",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalStateRegistryNumber",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankIdentificationCode",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "FullBankName",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "IBAN",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "NationalBankLicense",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "NationalStateRegistryNumber",
                table: "Banks");

            migrationBuilder.RenameColumn(
                name: "WebsiteURL",
                table: "Banks",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "TaxIdentificationNumber",
                table: "Banks",
                newName: "CEOMiddleName");

            migrationBuilder.RenameColumn(
                name: "ShortBankName",
                table: "Banks",
                newName: "CEOLastName");

            migrationBuilder.RenameColumn(
                name: "SettlementAccount",
                table: "Banks",
                newName: "CEOFirstName");
        }
    }
}
