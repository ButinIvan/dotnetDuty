using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnetWebApi.Migrations
{
    /// <inheritdoc />
    public partial class NewEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Users_OwnerId",
                table: "Documents");

            migrationBuilder.DropTable(
                name: "DocumentUser");

            migrationBuilder.RenameColumn(
                name: "LastEdited",
                table: "Documents",
                newName: "LastModified");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Documents",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Reviewers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviewers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviewers_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviewers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReviewerId = table.Column<Guid>(type: "uuid", nullable: false),
                    S3Path = table.Column<string>(type: "text", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DocumentId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewerId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Documents_DocumentId1",
                        column: x => x.DocumentId1,
                        principalTable: "Documents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_Reviewers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Reviewers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Reviewers_ReviewerId1",
                        column: x => x.ReviewerId1,
                        principalTable: "Reviewers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_UserId",
                table: "Documents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_DocumentId",
                table: "Comments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_DocumentId1",
                table: "Comments",
                column: "DocumentId1");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ReviewerId",
                table: "Comments",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ReviewerId1",
                table: "Comments",
                column: "ReviewerId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviewers_DocumentId",
                table: "Reviewers",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviewers_UserId",
                table: "Reviewers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Users_OwnerId",
                table: "Documents",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Users_UserId",
                table: "Documents",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Users_OwnerId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Users_UserId",
                table: "Documents");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Reviewers");

            migrationBuilder.DropIndex(
                name: "IX_Documents_UserId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "LastModified",
                table: "Documents",
                newName: "LastEdited");

            migrationBuilder.CreateTable(
                name: "DocumentUser",
                columns: table => new
                {
                    ReviewDocumentsId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReviewersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentUser", x => new { x.ReviewDocumentsId, x.ReviewersId });
                    table.ForeignKey(
                        name: "FK_DocumentUser_Documents_ReviewDocumentsId",
                        column: x => x.ReviewDocumentsId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentUser_Users_ReviewersId",
                        column: x => x.ReviewersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentUser_ReviewersId",
                table: "DocumentUser",
                column: "ReviewersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Users_OwnerId",
                table: "Documents",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
