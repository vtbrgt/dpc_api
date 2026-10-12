using System.ComponentModel.DataAnnotations;
using TaskManager.Api.Models;

namespace TaskManager.Api.DTOs;

public class AtualizarTarefaDto
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(150, ErrorMessage = "O título deve ter no máximo 150 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "A descrição deve ter no máximo 1000 caracteres.")]
    public string? Descricao { get; set; }

    [EnumDataType(typeof(StatusTarefa), ErrorMessage = "Status inválido.")]
    public StatusTarefa Status { get; set; }
}
