using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestifyServer.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_ADMIN",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    AccessLevel = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_ADMIN", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_CATEGORY",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_CATEGORY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_CATEGORY_T_CATEGORY_ParentId",
                        column: x => x.ParentId,
                        principalTable: "T_CATEGORY",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "T_WAITER",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_WAITER", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Table",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Table", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_PRODUCT",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PRODUCT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_PRODUCT_T_CATEGORY_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "T_CATEGORY",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "T_INVOICE",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Payment = table.Column<int>(type: "integer", nullable: false),
                    TableId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tip = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    WaiterId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_INVOICE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_INVOICE_T_WAITER_WaiterId",
                        column: x => x.WaiterId,
                        principalTable: "T_WAITER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_INVOICE_Table_TableId",
                        column: x => x.TableId,
                        principalTable: "Table",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "T_ORDER",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_ORDER", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_ORDER_T_INVOICE_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "T_INVOICE",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_ORDER_T_PRODUCT_ProductId",
                        column: x => x.ProductId,
                        principalTable: "T_PRODUCT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_ADMIN_Username",
                table: "T_ADMIN",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_CATEGORY_ParentId",
                table: "T_CATEGORY",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_T_INVOICE_TableId",
                table: "T_INVOICE",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_T_INVOICE_WaiterId",
                table: "T_INVOICE",
                column: "WaiterId");

            migrationBuilder.CreateIndex(
                name: "IX_T_ORDER_InvoiceId",
                table: "T_ORDER",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_T_ORDER_ProductId",
                table: "T_ORDER",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_T_PRODUCT_CategoryId",
                table: "T_PRODUCT",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_T_WAITER_Username",
                table: "T_WAITER",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_ADMIN");

            migrationBuilder.DropTable(
                name: "T_ORDER");

            migrationBuilder.DropTable(
                name: "T_INVOICE");

            migrationBuilder.DropTable(
                name: "T_PRODUCT");

            migrationBuilder.DropTable(
                name: "T_WAITER");

            migrationBuilder.DropTable(
                name: "Table");

            migrationBuilder.DropTable(
                name: "T_CATEGORY");
        }
    }
}
