using System.Text;
using System.Text.RegularExpressions;
using Curriculos.Api.Models;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace Curriculos.Api.Services;

public static class ExtratorDeCurriculo
{
    // Todo PDF de verdade começa com os caracteres "%PDF-"
    public static bool PareceUmPdf(byte[] conteudo)
    {
        if (conteudo.Length < 5)
        {
            return false;
        }

        string inicio = Encoding.ASCII.GetString(conteudo, 0, 5);
        return inicio == "%PDF-";
    }

    // Abre o PDF e junta o texto de todas as páginas
    public static string LerTexto(byte[] conteudo)
    {
        var texto = new StringBuilder();

        using (var pdf = PdfDocument.Open(conteudo))
        {
            foreach (var pagina in pdf.GetPages())
            {
                texto.AppendLine(ContentOrderTextExtractor.GetText(pagina));
            }
        }

        return texto.ToString();
    }

    // Procura nome, e-mail e telefone dentro do texto
    public static DadosExtraidos IdentificarDados(string texto)
    {
        var dados = new DadosExtraidos();

        // E-mail: algo + @ + algo + ponto + algo
        var email = Regex.Match(texto, @"[\w.+\-]+@[\w\-]+\.[\w.\-]+");
        if (email.Success)
        {
            dados.Email = email.Value.TrimEnd('.');
        }

        // Telefone: DDD (com ou sem parênteses) + 8 ou 9 dígitos
        var telefone = Regex.Match(texto, @"\(?\d{2}\)?\s?\d{4,5}-?\d{4}");
        if (telefone.Success)
        {
            dados.Telefone = telefone.Value;
        }

        // Nome: a primeira linha com 2 ou mais palavras, sem números e sem @
        foreach (string linhaOriginal in texto.Split('\n'))
        {
            string linha = linhaOriginal.Trim();
            bool temNumero = linha.Any(char.IsDigit);
            bool temArroba = linha.Contains('@');
            bool temDuasPalavras = linha.Split(' ').Length >= 2;

            if (temDuasPalavras && !temNumero && !temArroba)
            {
                dados.NomeCompleto = linha;
                break;
            }
        }

        return dados;
    }
}