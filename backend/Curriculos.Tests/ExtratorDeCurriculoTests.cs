using System.Text;
using Curriculos.Api.Services;

namespace Curriculos.Tests;

public class ExtratorDeCurriculoTests
{
    [Fact]
    public void Identifica_nome_email_e_telefone()
    {
        string texto = "Maria Silva\nDesenvolvedora\nmaria.silva@exemplo.com\n(41) 99999-0000";

        var dados = ExtratorDeCurriculo.IdentificarDados(texto);

        Assert.Equal("Maria Silva", dados.NomeCompleto);
        Assert.Equal("maria.silva@exemplo.com", dados.Email);
        Assert.Equal("(41) 99999-0000", dados.Telefone);
    }

    [Fact]
    public void Ignora_o_titulo_do_curriculo_ao_procurar_o_nome()
    {
        string texto = "CURRÍCULO\nJoão Pedro Santos\njoao@exemplo.com";

        var dados = ExtratorDeCurriculo.IdentificarDados(texto);

        Assert.Equal("João Pedro Santos", dados.NomeCompleto);
    }

    [Fact]
    public void Identifica_telefone_fixo_sem_parenteses()
    {
        var dados = ExtratorDeCurriculo.IdentificarDados("Ana Costa\n41 3333-4444");

        Assert.Equal("41 3333-4444", dados.Telefone);
    }

    [Fact]
    public void Devolve_nulo_quando_nao_encontra_nada()
    {
        var dados = ExtratorDeCurriculo.IdentificarDados("1234\n!!!");

        Assert.Null(dados.NomeCompleto);
        Assert.Null(dados.Email);
        Assert.Null(dados.Telefone);
    }

    [Fact]
    public void Reconhece_o_inicio_de_um_pdf()
    {
        byte[] pdf = Encoding.ASCII.GetBytes("%PDF-1.4 conteudo qualquer");
        byte[] texto = Encoding.ASCII.GetBytes("isto e um texto comum");

        Assert.True(ExtratorDeCurriculo.PareceUmPdf(pdf));
        Assert.False(ExtratorDeCurriculo.PareceUmPdf(texto));
    }
}