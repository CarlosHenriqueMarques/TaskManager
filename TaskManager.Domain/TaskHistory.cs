namespace TaskManager.Domain;

public class TaskHistory
{
    public Guid Id { get; set; }
    public string Alteracao { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public DateTime DataModificacao { get; set; }
    public Guid TarefaId { get; set; } 
    public TaskItem Tarefa { get; set; } = null!;
}
