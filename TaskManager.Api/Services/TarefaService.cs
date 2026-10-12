using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.DTOs;
using TaskManager.Api.Models;

namespace TaskManager.Api.Services;

public class TarefaService(AppDbContext db) : ITarefaService
{
    public async Task<IEnumerable<TarefaResponseDto>> ListarAsync(StatusTarefa? status)
    {
        var query = db.Tarefas.AsNoTracking();

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        var tarefas = await query
            .OrderByDescending(t => t.CriadaEm)
            .ToListAsync();

        return tarefas.Select(ToDto);
    }

    public async Task<TarefaResponseDto> ObterPorIdAsync(int id)
        => ToDto(await BuscarOuFalharAsync(id));

    public async Task<TarefaResponseDto> CriarAsync(CriarTarefaDto dto)
    {
        var tarefa = new Tarefa
        {
            Titulo = dto.Titulo.Trim(),
            Descricao = dto.Descricao?.Trim()
        };

        db.Tarefas.Add(tarefa);
        await db.SaveChangesAsync();

        return ToDto(tarefa);
    }

    public async Task<TarefaResponseDto> AtualizarAsync(int id, AtualizarTarefaDto dto)
    {
        var tarefa = await BuscarOuFalharAsync(id);

        tarefa.Titulo = dto.Titulo.Trim();
        tarefa.Descricao = dto.Descricao?.Trim();
        tarefa.Status = dto.Status;
        tarefa.AtualizadaEm = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return ToDto(tarefa);
    }

    public async Task<TarefaResponseDto> AtualizarStatusAsync(int id, AtualizarStatusDto dto)
    {
        var tarefa = await BuscarOuFalharAsync(id);

        tarefa.Status = dto.Status;
        tarefa.AtualizadaEm = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return ToDto(tarefa);
    }

    public async Task RemoverAsync(int id)
    {
        var tarefa = await BuscarOuFalharAsync(id);

        db.Tarefas.Remove(tarefa);
        await db.SaveChangesAsync();
    }

    // Lança KeyNotFoundException -> o GlobalExceptionHandler transforma em 404
    private async Task<Tarefa> BuscarOuFalharAsync(int id)
        => await db.Tarefas.FindAsync(id)
           ?? throw new KeyNotFoundException($"Tarefa {id} não encontrada.");

    private static TarefaResponseDto ToDto(Tarefa t) => new()
    {
        Id = t.Id,
        Titulo = t.Titulo,
        Descricao = t.Descricao,
        Status = t.Status,
        CriadaEm = t.CriadaEm,
        AtualizadaEm = t.AtualizadaEm
    };
}
