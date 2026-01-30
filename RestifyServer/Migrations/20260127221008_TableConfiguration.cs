using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestifyServer.Migrations
{
    /// <inheritdoc />
    public partial class TableConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_INVOICE_Table_TableId",
                table: "T_INVOICE");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Table",
                table: "Table");

            migrationBuilder.RenameTable(
                name: "Table",
                newName: "T_TABLE");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_TABLE",
                table: "T_TABLE",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_T_TABLE_Number",
                table: "T_TABLE",
                column: "Number",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_T_INVOICE_T_TABLE_TableId",
                table: "T_INVOICE",
                column: "TableId",
                principalTable: "T_TABLE",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_INVOICE_T_TABLE_TableId",
                table: "T_INVOICE");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_TABLE",
                table: "T_TABLE");

            migrationBuilder.DropIndex(
                name: "IX_T_TABLE_Number",
                table: "T_TABLE");

            migrationBuilder.RenameTable(
                name: "T_TABLE",
                newName: "Table");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Table",
                table: "Table",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_T_INVOICE_Table_TableId",
                table: "T_INVOICE",
                column: "TableId",
                principalTable: "Table",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
