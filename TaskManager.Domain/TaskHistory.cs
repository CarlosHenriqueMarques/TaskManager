namespace TaskManager.Domain;

public class TaskHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Alteracao { get; set; }
    public string Usuario { get; set; }
    public DateTime DataModificacao { get; set; }
}
