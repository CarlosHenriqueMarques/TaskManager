using TaskManager.Domain;
using static TaskManager.Domain.Enum;

namespace TaskManager.Application.Interfaces;

public interface IProjectService
{
    Task<Guid> CriarProjetoAsync(string nome);
    Task<IEnumerable<Project>> ListarProjetosAsync();
    Task<Project?> ObterProjetoAsync(Guid id);
    Task<Guid> AdicionarTarefaAsync(Guid projetoId, string titulo, string descricao, DateTime vencimento, TaskPriority prioridade);
    Task AtualizarTarefaAsync(Guid tarefaId, string? titulo, string? descricao, Domain.Enum.TaskStatus? status, string usuario);
    Task RemoverTarefaAsync(Guid tarefaId);
    Task RemoverProjetoAsync(Guid projetoId);
    Task AdicionarComentarioAsync(Guid tarefaId, string texto, string usuario);
}
