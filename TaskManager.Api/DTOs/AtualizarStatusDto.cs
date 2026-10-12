using System.ComponentModel.DataAnnotations;
using TaskManager.Api.Models;

namespace TaskManager.Api.DTOs;

public class AtualizarStatusDto
{
    [EnumDataType(typeof(StatusTarefa), ErrorMessage = "Status inválido.")]
    public StatusTarefa Status { get; set; }
}
