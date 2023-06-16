using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TareaMVC.Migrations
{
    /// <inheritdoc />
    public partial class migration01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Tareas",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_IdentityUserId",
                table: "Tareas",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tareas_AspNetUsers_IdentityUserId",
                table: "Tareas",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tareas_AspNetUsers_IdentityUserId",
                table: "Tareas");

            migrationBuilder.DropIndex(
                name: "IX_Tareas_IdentityUserId",
                table: "Tareas");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Tareas");
        }
    }
}
