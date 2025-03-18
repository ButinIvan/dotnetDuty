using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnetWebApi.Migrations
{
    /// <inheritdoc />
    public partial class ReturnedMissingEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviewers_Users_UserId",
                table: "Reviewers");

            migrationBuilder.DropIndex(
                name: "IX_Reviewers_UserId",
                table: "Reviewers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Reviewers");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewers",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_Reviewers_OwnerId",
                table: "Reviewers",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviewers_Users_OwnerId",
                table: "Reviewers",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviewers_Users_OwnerId",
                table: "Reviewers");

            migrationBuilder.DropIndex(
                name: "IX_Reviewers_OwnerId",
                table: "Reviewers");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewers",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Reviewers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Reviewers_UserId",
                table: "Reviewers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviewers_Users_UserId",
                table: "Reviewers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
