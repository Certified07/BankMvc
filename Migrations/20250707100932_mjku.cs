using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMvc.Migrations
{
    /// <inheritdoc />
    public partial class mjku : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Transactions");

            migrationBuilder.AddColumn<string>(
                name: "TransactionType",
                table: "Transactions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "Transactions");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
