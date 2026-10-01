using Curriculos.Api.Data;
using Curriculos.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Curriculos.Api.Controllers;

[ApiController]
[Route("api/candidatos")]
public class CandidatosController : ControllerBase
{
    private readonly AppDbContext _context;

    public CandidatosController(AppDbContext context)
    {
        _context = context;
    }

    // Salva um novo candidato 
    [HttpPost]
    public async Task<IActionResult> Cadastrar(CandidatoRequest dados)
    {
        Candidato candidato = new Candidato();
        candidato.NomeCompleto = dados.NomeCompleto!.Trim();
        candidato.Email = dados.Email!.Trim().ToLower();
        candidato.Telefone = dados.Telefone?.Trim();
        candidato.AreaInteresse = dados.AreaInteresse?.Trim();
        candidato.ResumoProfissional = dados.ResumoProfissional?.Trim();

        _context.Candidatos.Add(candidato);
        await _context.SaveChangesAsync();

        return Created($"/api/candidatos/{candidato.Id}", candidato);
    }

    // Lista os candidatos, do mais novo para o mais antigo 
    [HttpGet]
    public async Task<IActionResult> Listar(string? busca)
    {
        var consulta = _context.Candidatos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(busca))
        {
            consulta = consulta.Where(c => c.NomeCompleto.Contains(busca) || c.Email.Contains(busca));
        }

        var lista = await consulta.OrderByDescending(c => c.CriadoEm).ToListAsync();
        return Ok(lista);
    }

    // Mostra os detalhes de um candidato
    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var candidato = await _context.Candidatos.FindAsync(id);

        if (candidato == null)
        {
            return NotFound("Candidato não encontrado.");
        }

        return Ok(candidato);
    }
}