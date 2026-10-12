using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.DTOs;
using TaskManager.Api.Models;
using TaskManager.Api.Services;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // -> /api/tarefas
[Produces("application/json")]
public class TarefasController(ITarefaService service) : ControllerBase
{
    /// <summary>Lista as tarefas, com filtro opcional por status.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TarefaResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TarefaResponseDto>>> Listar([FromQuery] StatusTarefa? status)
        => Ok(await service.ListarAsync(status));

    /// <summary>Obtém uma tarefa pelo id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TarefaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TarefaResponseDto>> ObterPorId(int id)
        => Ok(await service.ObterPorIdAsync(id));

    /// <summary>Cria uma nova tarefa (status inicial: Pendente).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TarefaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TarefaResponseDto>> Criar(CriarTarefaDto dto)
    {
        var criada = await service.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = criada.Id }, criada);
    }

    /// <summary>Atualiza todos os dados de uma tarefa.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TarefaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TarefaResponseDto>> Atualizar(int id, AtualizarTarefaDto dto)
        => Ok(await service.AtualizarAsync(id, dto));

    /// <summary>Altera somente o status de uma tarefa.</summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(TarefaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TarefaResponseDto>> AtualizarStatus(int id, AtualizarStatusDto dto)
        => Ok(await service.AtualizarStatusAsync(id, dto));

    /// <summary>Remove uma tarefa.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id)
    {
        await service.RemoverAsync(id);
        return NoContent();
    }
}
