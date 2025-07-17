using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITech.Migrations
{
    public partial class popularTecnicos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Tecnicos(IsJuridica, DocIdentificacao, TecnicoNome, Email,Telefone, Endereco)" +
                "Values(1, '12.345.678/0001-90','Empresa1','empresa1@gmail.com','(33) 9 1234-5678'," +
                "'Rua dos pinheiros, 01, Bairro dos Carvalhos, Belo Horizonte - MG')");

            migrationBuilder.Sql("INSERT INTO Tecnicos(IsJuridica, DocIdentificacao, TecnicoNome, Email,Telefone, Endereco)" +
                "Values(0, '123.456.789-10','Pessoa1','pessoa1@gmail.com','(33) 9 9876-5432'," +
                "'Rua das Oliveiras, 02, Bairro dos Carvalhos, Belo Horizonte - MG')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql("DELETE FROM Tecnicos");

        }
    }
}
