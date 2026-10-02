using Curriculos.Api.Models;
using Curriculos.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Curriculos.Api.Controllers;

[ApiController]
[Route("api/candidatos")]
public class ExtracaoPdfController : ControllerBase
{
    private const long TamanhoMaximo = 5 * 1024 * 1024; // 5 MB

    // Recebe o PDF, lê o texto e devolve o que conseguiu identificar. Não salva nada.
    [HttpPost("extrair-pdf")]
    public async Task<IActionResult> ExtrairPdf(IFormFile? arquivo)
    {
        if (arquivo == null || arquivo.Length == 0)
        {
            return BadRequest(new { mensagem = "Nenhum arquivo foi enviado." });
        }

        if (arquivo.Length > TamanhoMaximo)
        {
            return BadRequest(new { mensagem = "O arquivo é maior que 5 MB." });
        }

        if (Path.GetExtension(arquivo.FileName).ToLower() != ".pdf")
        {
            return BadRequest(new { mensagem = "Envie um arquivo no formato PDF." });
        }

        // Copia o arquivo para a memória
        byte[] conteudo;
        using (var memoria = new MemoryStream())
        {
            await arquivo.CopyToAsync(memoria);
            conteudo = memoria.ToArray();
        }

        if (!ExtratorDeCurriculo.PareceUmPdf(conteudo))
        {
            return BadRequest(new { mensagem = "O arquivo enviado não é um PDF válido." });
        }

        string texto;
        try
        {
            texto = ExtratorDeCurriculo.LerTexto(conteudo);
        }
        catch (Exception)
        {
            return UnprocessableEntity(new
            {
                mensagem = "Não foi possível ler o PDF. Você pode preencher o formulário manualmente."
            });
        }

        if (string.IsNullOrWhiteSpace(texto))
        {
            return UnprocessableEntity(new
            {
                mensagem = "O PDF não tem texto para ler (pode ser uma imagem escaneada). Preencha o formulário manualmente."
            });
        }

        DadosExtraidos dados = ExtratorDeCurriculo.IdentificarDados(texto);

        bool achouAlgo = dados.NomeCompleto != null || dados.Email != null || dados.Telefone != null;
        if (achouAlgo)
        {
            dados.Mensagem = "Confira os dados identificados e corrija o que for preciso.";
        }
        else
        {
            dados.Mensagem = "Não conseguimos identificar nome, e-mail ou telefone. Preencha o formulário manualmente.";
        }

        return Ok(dados);
    }
}