using static TaskManager.Domain.Enum;

namespace TaskManager.Api.Models;

public class TarefaDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataVencimento { get; set; }
    public TaskPriority Prioridade { get; set; }
}
