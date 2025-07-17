using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITech.Migrations
{
    public partial class AtualizarServicos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPreferido",
                table: "Servicos");

           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPreferido",
                table: "Servicos",
                type: "bit",
                nullable: false,
                defaultValue: false);

           
        }
    }
}
