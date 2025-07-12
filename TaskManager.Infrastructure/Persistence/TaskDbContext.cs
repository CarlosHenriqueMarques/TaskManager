using Microsoft.EntityFrameworkCore;
using TaskManager.Domain;

namespace TaskManager.Infrastructure.Persistence;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) { }

    public DbSet<Project> Projetos => Set<Project>();
    public DbSet<TaskItem> Tarefas => Set<TaskItem>();
}
