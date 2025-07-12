using static TaskManager.Domain.Enum;

namespace TaskManager.Domain;

public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public DateTime DataVencimento { get; set; }
    public Enum.TaskStatus Status { get; set; }
    public TaskPriority Prioridade { get; set; }
    public List<TaskHistory> Historico { get; set; } = new();
    public List<TaskComment> Comentarios { get; set; } = new();
}
