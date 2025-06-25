using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllowedPartnerV2.Migrations
{
    /// <inheritdoc />
    public partial class InitWithOneToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Partners",
                columns: table => new
                {
                    partnerkey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    partnerrefno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    partnerpassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    totalamount = table.Column<long>(type: "bigint", nullable: false),
                    timestamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sig = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partners", x => x.partnerkey);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    partneritemref = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    qty = table.Column<int>(type: "int", nullable: false),
                    unitprice = table.Column<long>(type: "bigint", nullable: false),
                    partnerkey = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.partneritemref);
                    table.ForeignKey(
                        name: "FK_Items_Partners_partnerkey",
                        column: x => x.partnerkey,
                        principalTable: "Partners",
                        principalColumn: "partnerkey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_partnerkey",
                table: "Items",
                column: "partnerkey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Partners");
        }
    }
}
