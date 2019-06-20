using Microsoft.EntityFrameworkCore.Migrations;

namespace Contacts.Migrations
{
    public partial class AddAvatartoContact : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "Contacts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Contacts");
        }
    }
}
