namespace Curriculos.Api.Models;

public class Candidato
{
    // Identificador único do candidato 
    public int Id { get; set; }
    // Nome completo do candidato 
    public string NomeCompleto { get; set; } = string.Empty;
    // E-mail do candidato 
    public string Email { get; set; } = string.Empty;
    // Telefone de contato 
    public string? Telefone { get; set; }
    // Cargo ou área de interesse profissional 
    public string? AreaInteresse { get; set; }
    // Resumo profissional extraído do PDF ou digitado manualmente 
    public string? ResumoProfissional { get; set; }
    // Data e hora em que o registro foi criado no sistema
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}