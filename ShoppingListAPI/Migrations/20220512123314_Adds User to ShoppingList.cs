using Microsoft.EntityFrameworkCore.Migrations;

namespace ShoppingListAPI.Migrations
{
    public partial class AddsUsertoShoppingList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "ShoppingLists",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ShoppingLists",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingLists_UserId",
                table: "ShoppingLists",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingLists_AspNetUsers_UserId",
                table: "ShoppingLists",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingLists_AspNetUsers_UserId",
                table: "ShoppingLists");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingLists_UserId",
                table: "ShoppingLists");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ShoppingLists");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "ShoppingLists",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32,
                oldNullable: true);
        }
    }
}
