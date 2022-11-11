using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog_MVC.Data.Migrations
{
    public partial class _002_creatorId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_AspNetUsers_BlogUserId",
                table: "BlogPosts");

            migrationBuilder.RenameColumn(
                name: "BlogUserId",
                table: "BlogPosts",
                newName: "CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPosts_BlogUserId",
                table: "BlogPosts",
                newName: "IX_BlogPosts_CreatorId");

            migrationBuilder.AlterColumn<string>(
                name: "Body",
                table: "Comments",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1500)",
                oldMaxLength: 1500,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_AspNetUsers_CreatorId",
                table: "BlogPosts",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_AspNetUsers_CreatorId",
                table: "BlogPosts");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "BlogPosts",
                newName: "BlogUserId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPosts_CreatorId",
                table: "BlogPosts",
                newName: "IX_BlogPosts_BlogUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Body",
                table: "Comments",
                type: "character varying(1500)",
                maxLength: 1500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_AspNetUsers_BlogUserId",
                table: "BlogPosts",
                column: "BlogUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
