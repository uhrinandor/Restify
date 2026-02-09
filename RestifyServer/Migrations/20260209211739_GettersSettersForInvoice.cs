using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestifyServer.Migrations
{
    /// <inheritdoc />
    public partial class GettersSettersForInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAt",
                table: "T_INVOICE",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: "T_INVOICE");
        }
    }
}
