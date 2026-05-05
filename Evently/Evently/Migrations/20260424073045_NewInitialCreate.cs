using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evently.Migrations
{
    /// <inheritdoc />
    public partial class NewInitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Users_UserID",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "RoleName",
                table: "Roles");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Attendances",
                newName: "CheckerId");

            migrationBuilder.RenameIndex(
                name: "IX_Attendances_UserID",
                table: "Attendances",
                newName: "IX_Attendances_CheckerId");

            migrationBuilder.AddColumn<int>(
                name: "role",
                table: "Roles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Users_CheckerId",
                table: "Attendances",
                column: "CheckerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Users_CheckerId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "role",
                table: "Roles");

            migrationBuilder.RenameColumn(
                name: "CheckerId",
                table: "Attendances",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Attendances_CheckerId",
                table: "Attendances",
                newName: "IX_Attendances_UserID");

            migrationBuilder.AddColumn<string>(
                name: "RoleName",
                table: "Roles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Users_UserID",
                table: "Attendances",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
