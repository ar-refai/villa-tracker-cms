using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VillaManager.Data.Migrations
{
    public partial class MoreRelationsToTheTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Villas",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Villas",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Villas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Villas_CreatorId",
                table: "Villas",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Villas_AspNetUsers_CreatorId",
                table: "Villas",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Villas_AspNetUsers_CreatorId",
                table: "Villas");

            migrationBuilder.DropIndex(
                name: "IX_Villas_CreatorId",
                table: "Villas");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Villas");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Villas");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Villas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
