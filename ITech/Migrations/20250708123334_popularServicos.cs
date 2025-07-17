using ITech.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;
using System.Drawing;

#nullable disable

namespace ITech.Migrations
{
    public partial class popularServicos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Servicos(DescricaoCurta, DescricaoDetalhada, Valor, IsPreferido, EmDisposicao, CategoriaId, TecnicoId)" +
"VALUES" +
"('Formatação e otimização completa do sistema', 'Formatação completa, instalação de drivers e otimização de desempenho do sistema operacional.', 150.00, 1, 1, 1, 1)," +

"('Limpeza física e interna de notebook', 'Serviço de limpeza completa em componentes internos e externos do notebook.', 120.00, 0, 1, 2, 1)," +

"('Reparo em placa-mãe de computador', 'Diagnóstico e reparo técnico em placas-mãe com defeito, incluindo substituição de componentes.', 200.00, 1, 1, 1, 2)," +

"('Instalação de software e antivírus', 'Instalação de softwares essenciais, pacotes de escritório e antivírus com configuração inicial.', 90.00, 0, 1, 1, 2)," +

"('Troca de tela de notebook', 'Substituição da tela quebrada ou defeituosa de notebook por uma nova.', 350.00, 1, 1, 2, 1)," +

"('Montagem de PC gamer', 'Montagem completa de computador gamer com cable management e testes de desempenho.', 180.00, 1, 1, 2, 1)," +

"('Backup e recuperação de dados', 'Serviço de backup ou recuperação de dados excluídos ou corrompidos de HDs e SSDs.', 160.00, 1, 1, 2, 1)," +

"('Atualização de hardware', 'Troca ou instalação de novos componentes como memória RAM, SSD ou placas.', 110.00, 0, 1, 1, 2)," +

"('Configuração de rede doméstica', 'Instalação e configuração de roteadores, Wi-Fi e compartilhamento de dispositivos.', 95.00, 0, 1, 2, 2)," +

"('Instalação de impressoras', 'Configuração de impressoras em rede ou local, drivers e testes de impressão.', 85.00, 0, 1, 2, 2)," +

"('Troca de pasta térmica e limpeza de cooler', 'Limpeza e substituição da pasta térmica em CPU e GPU para evitar superaquecimento.', 70.00, 1, 1, 1, 1)," +

"('Manutenção preventiva empresarial', 'Visita técnica para manutenção preventiva em estações de trabalho e servidores.', 300.00, 0, 1, 1, 2);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql("DELETE FROM Servicos");
        }
    }
}
