using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServerSubscriptionManager.Migrations
{
    /// <inheritdoc />
    public partial class InvoiceMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscriptionPeriodUser");

            migrationBuilder.AddColumn<int>(
                name: "UserCount",
                table: "SubscriptionPeriods",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<long>(type: "INTEGER", nullable: false),
                    SubscriptionPeriodId = table.Column<long>(type: "INTEGER", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_SubscriptionPeriods_SubscriptionPeriodId",
                        column: x => x.SubscriptionPeriodId,
                        principalTable: "SubscriptionPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invoices_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_SubscriptionPeriodId",
                table: "Invoices",
                column: "SubscriptionPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_UserId",
                table: "Invoices",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropColumn(
                name: "UserCount",
                table: "SubscriptionPeriods");

            migrationBuilder.CreateTable(
                name: "SubscriptionPeriodUser",
                columns: table => new
                {
                    SubscriptionsId = table.Column<long>(type: "INTEGER", nullable: false),
                    UsersId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPeriodUser", x => new { x.SubscriptionsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_SubscriptionPeriodUser_SubscriptionPeriods_SubscriptionsId",
                        column: x => x.SubscriptionsId,
                        principalTable: "SubscriptionPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriptionPeriodUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPeriodUser_UsersId",
                table: "SubscriptionPeriodUser",
                column: "UsersId");
        }
    }
}
