namespace TaskManager.Api.Models;

public class TaskItemDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public DateTime DataVencimento { get; set; }
    public string Status { get; set; } = null!;
    public string Prioridade { get; set; } = null!;
}
