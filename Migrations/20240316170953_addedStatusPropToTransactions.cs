using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayBridgeAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedStatusPropToTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "UserToUserTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserToUserTransactions");
        }
    }
}
