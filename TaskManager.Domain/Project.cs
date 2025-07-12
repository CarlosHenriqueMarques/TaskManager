namespace TaskManager.Domain;

public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; }
    public List<TaskItem> Tarefas { get; set; } = new();
}
