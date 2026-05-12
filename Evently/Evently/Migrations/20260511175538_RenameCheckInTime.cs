using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evently.Migrations
{
    /// <inheritdoc />
    public partial class RenameCheckInTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_UserId",
                table: "Events");

            // RENAME CHECKINTIME
            migrationBuilder.RenameColumn(
                name: "checkInTime",
                table: "Attendances",
                newName: "CheckInTime");

            migrationBuilder.AlterColumn<string>(
                name: "Venue",
                table: "Events",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Events",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "StartTime",
                table: "Events",
                type: "interval",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "EventName",
                table: "Events",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EventDate",
                table: "Events",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "EndTime",
                table: "Events",
                type: "interval",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_EventId",
                table: "Attendances",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Events_EventId",
                table: "Attendances",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_UserId",
                table: "Events",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Events_EventId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_UserId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_EventId",
                table: "Attendances");

            // RENAME BACK CHECKINTIME
            migrationBuilder.RenameColumn(
                name: "CheckInTime",
                table: "Attendances",
                newName: "checkInTime");

            migrationBuilder.AlterColumn<string>(
                name: "Venue",
                table: "Events",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "StartTime",
                table: "Events",
                type: "time without time zone",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "interval");

            migrationBuilder.AlterColumn<string>(
                name: "EventName",
                table: "Events",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EventDate",
                table: "Events",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "EndTime",
                table: "Events",
                type: "time without time zone",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "interval");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_UserId",
                table: "Events",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}