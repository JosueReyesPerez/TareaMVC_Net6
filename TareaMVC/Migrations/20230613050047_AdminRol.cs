using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TareaMVC.Migrations
{
    /// <inheritdoc />
    public partial class AdminRol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS(Select Id from AspNetRoles where Id = '077c758c-1072-4328-9e41-40e1a7577e5b')
                BEGIN
	                insert into AspNetRoles(id, name, NormalizedName) values ('077c758c-1072-4328-9e41-40e1a7577e5b', 'admin', 'ADMIN');
                END"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"DELETE AspNetRoles WHERE Id = '077c758c-1072-4328-9e41-40e1a7577e5b'"
                );
        }
    }
}
