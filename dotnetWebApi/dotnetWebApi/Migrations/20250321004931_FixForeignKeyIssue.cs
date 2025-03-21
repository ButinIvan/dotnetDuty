using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnetWebApi.Migrations
{
    /// <inheritdoc />
    public partial class FixForeignKeyIssue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Reviewers",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviewers_OwnerId",
                table: "Reviewers",
                newName: "IX_Reviewers_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviewers_Users_UserId",
                table: "Reviewers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviewers_Users_UserId",
                table: "Reviewers");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Reviewers",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviewers_UserId",
                table: "Reviewers",
                newName: "IX_Reviewers_OwnerId");
            
            migrationBuilder.AddForeignKey(
                name: "FK_Reviewers_Users_OwnerId",
                table: "Reviewers",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
