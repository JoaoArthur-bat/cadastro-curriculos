using System.ComponentModel.DataAnnotations;
using Curriculos.Api.Models;

namespace Curriculos.Tests;

public class CandidatoRequestTests
{
    // Roda as regras de validação do CandidatoRequest e devolve a lista de erros
    private List<ValidationResult> Validar(CandidatoRequest dados)
    {
        var erros = new List<ValidationResult>();
        var contexto = new ValidationContext(dados);
        Validator.TryValidateObject(dados, contexto, erros, true);
        return erros;
    }

    [Fact]
    public void Aceita_quando_so_nome_e_email_estao_preenchidos()
    {
        var dados = new CandidatoRequest();
        dados.NomeCompleto = "Maria Silva";
        dados.Email = "maria@exemplo.com";

        var erros = Validar(dados);

        Assert.Empty(erros);
    }

    [Fact]
    public void Rejeita_quando_o_nome_esta_vazio()
    {
        var dados = new CandidatoRequest();
        dados.Email = "maria@exemplo.com";

        var erros = Validar(dados);

        Assert.Contains(erros, e => e.MemberNames.Contains("NomeCompleto"));
    }

    [Fact]
    public void Rejeita_quando_o_email_esta_vazio()
    {
        var dados = new CandidatoRequest();
        dados.NomeCompleto = "Maria Silva";

        var erros = Validar(dados);

        Assert.Contains(erros, e => e.MemberNames.Contains("Email"));
    }

    [Fact]
    public void Rejeita_email_com_formato_invalido()
    {
        var dados = new CandidatoRequest();
        dados.NomeCompleto = "Maria Silva";
        dados.Email = "isso-nao-e-email";

        var erros = Validar(dados);

        Assert.Contains(erros, e => e.MemberNames.Contains("Email"));
    }
}