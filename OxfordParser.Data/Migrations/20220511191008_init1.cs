using Microsoft.EntityFrameworkCore.Migrations;

namespace OxfordParser.Data.Migrations
{
    public partial class init1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordUsages_WordCategories_WordCategoryId",
                table: "WordUsages");

            migrationBuilder.AlterColumn<int>(
                name: "WordCategoryId",
                table: "WordUsages",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_WordUsages_WordCategories_WordCategoryId",
                table: "WordUsages",
                column: "WordCategoryId",
                principalTable: "WordCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordUsages_WordCategories_WordCategoryId",
                table: "WordUsages");

            migrationBuilder.AlterColumn<int>(
                name: "WordCategoryId",
                table: "WordUsages",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WordUsages_WordCategories_WordCategoryId",
                table: "WordUsages",
                column: "WordCategoryId",
                principalTable: "WordCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
