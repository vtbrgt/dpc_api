using TaskManager.Api.Models;

namespace TaskManager.Api.DTOs;

public class TarefaResponseDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public StatusTarefa Status { get; set; }
    public DateTime CriadaEm { get; set; }
    public DateTime? AtualizadaEm { get; set; }
}
