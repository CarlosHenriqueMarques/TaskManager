namespace TaskManager.Api.Models;

public class UpdateTarefaDto
{
    public string? Titulo { get; set; }
    public string? Descricao { get; set; }
    public Domain.Enum.TaskStatus? Status { get; set; }
    public string Usuario { get; set; } = string.Empty;
}
