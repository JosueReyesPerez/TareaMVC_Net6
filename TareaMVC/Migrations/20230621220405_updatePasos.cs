using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TareaMVC.Migrations
{
    /// <inheritdoc />
    public partial class updatePasos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Realizado",
                table: "Pasos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Realizado",
                table: "Pasos");
        }
    }
}
