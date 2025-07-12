namespace TaskManager.Domain;

public class TaskComment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Texto { get; set; }
    public string Usuario { get; set; }
    public DateTime Data { get; set; }
}
