using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServerSubscriptionManager.Migrations
{
    /// <inheritdoc />
    public partial class UserOption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoInvoice",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoInvoice",
                table: "Users");
        }
    }
}
