namespace TaskManager.Api.Models;

public class Tarefa
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public StatusTarefa Status { get; set; } = StatusTarefa.Pendente;
    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadaEm { get; set; }
}
