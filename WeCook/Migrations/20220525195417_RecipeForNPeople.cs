using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeCook.Migrations
{
    public partial class RecipeForNPeople : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PeopleFor",
                table: "Recipes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PeopleFor",
                table: "Recipes");
        }
    }
}
