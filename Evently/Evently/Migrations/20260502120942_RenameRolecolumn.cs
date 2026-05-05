using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evently.Migrations
{
    /// <inheritdoc />
    public partial class RenameRolecolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "role",
                table: "Roles",
                newName: "Role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Roles",
                newName: "role");
        }
    }
}
