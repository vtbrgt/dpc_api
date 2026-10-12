using TaskManager.Api.DTOs;
using TaskManager.Api.Models;

namespace TaskManager.Api.Services;

public interface ITarefaService
{
    Task<IEnumerable<TarefaResponseDto>> ListarAsync(StatusTarefa? status);
    Task<TarefaResponseDto> ObterPorIdAsync(int id);
    Task<TarefaResponseDto> CriarAsync(CriarTarefaDto dto);
    Task<TarefaResponseDto> AtualizarAsync(int id, AtualizarTarefaDto dto);
    Task<TarefaResponseDto> AtualizarStatusAsync(int id, AtualizarStatusDto dto);
    Task RemoverAsync(int id);
}
