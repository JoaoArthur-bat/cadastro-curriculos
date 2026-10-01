using System.ComponentModel.DataAnnotations;

namespace Curriculos.Api.Models;

public class CandidatoRequest
{
    [Required(ErrorMessage = "O nome completo é obrigatório.")]
    [StringLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres.")]
    public string? NomeCompleto { get; set; }

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail em formato válido.")]
    [StringLength(200, ErrorMessage = "O e-mail deve ter no máximo 200 caracteres.")]
    public string? Email { get; set; }

    [StringLength(30, ErrorMessage = "O telefone deve ter no máximo 30 caracteres.")]
    public string? Telefone { get; set; }

    [StringLength(150, ErrorMessage = "A área de interesse deve ter no máximo 150 caracteres.")]
    public string? AreaInteresse { get; set; }

    [StringLength(2000, ErrorMessage = "O resumo deve ter no máximo 2000 caracteres.")]
    public string? ResumoProfissional { get; set; }
}