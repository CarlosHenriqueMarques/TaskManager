using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Interfaces;
using TaskManager.Domain;
using TaskManager.Infrastructure.Persistence;
using static TaskManager.Domain.Enum;
using TaskStatus = TaskManager.Domain.Enum.TaskStatus;

namespace TaskManager.Application.Services;

public class ProjectService : IProjectService
{
    private readonly TaskDbContext _context;

    public ProjectService(TaskDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CriarProjetoAsync(string nome)
    {
        var projeto = new Project { Nome = nome };
        _context.Projetos.Add(projeto);
        await _context.SaveChangesAsync();
        return projeto.Id;
    }

    public async Task<IEnumerable<Project>> ListarProjetosAsync()
    {
        return await _context.Projetos
            .Include(p => p.Tarefas)
            .ToListAsync();
    }

    public async Task<Project?> ObterProjetoAsync(Guid id)
    {
        return await _context.Projetos
            .Include(p => p.Tarefas)
                .ThenInclude(t => t.Historico)
            .Include(p => p.Tarefas)
                .ThenInclude(t => t.Comentarios)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Guid> AdicionarTarefaAsync(Guid projetoId, string titulo, string descricao, DateTime vencimento, TaskPriority prioridade)
    {
        var projeto = await _context.Projetos.FirstOrDefaultAsync(p => p.Id == projetoId);

        if (projeto == null)
            throw new InvalidOperationException("Projeto não encontrado");

        var totalTarefas = await _context.Tarefas.CountAsync(t => t.ProjetoId == projetoId);
        if (totalTarefas >= 20)
            throw new InvalidOperationException("Limite de 20 tarefas por projeto atingido.");

        var tarefa = new TaskItem
        {
            Titulo = titulo,
            Descricao = descricao,
            DataVencimento = vencimento,
            Status = TaskStatus.Pendente,
            Prioridade = prioridade,
            ProjetoId = projetoId
        };

        await _context.Tarefas.AddAsync(tarefa);
        await _context.SaveChangesAsync();

        return tarefa.Id;
    }


    public async Task AtualizarTarefaAsync(Guid tarefaId, string? titulo, string? descricao, TaskStatus? status, string usuario)
    {
        var tarefa = await _context.Tarefas
            .Include(t => t.Historico)
            .FirstOrDefaultAsync(t => t.Id == tarefaId);

        if (tarefa == null)
            throw new InvalidOperationException("Tarefa não encontrada.");

        var alteracoes = new List<string>();

        if (titulo != null && titulo != tarefa.Titulo)
        {
            alteracoes.Add($"Título: '{tarefa.Titulo}' → '{titulo}'");
            tarefa.Titulo = titulo;
        }

        if (descricao != null && descricao != tarefa.Descricao)
        {
            alteracoes.Add($"Descrição: '{tarefa.Descricao}' → '{descricao}'");
            tarefa.Descricao = descricao;
        }

        if (status.HasValue && status.Value != tarefa.Status)
        {
            alteracoes.Add($"Status: {tarefa.Status} → {status.Value}");
            tarefa.Status = status.Value;
        }

        if (alteracoes.Any())
        {
            var historico = new TaskHistory
            {
                Alteracao = string.Join("; ", alteracoes),
                Usuario = usuario,
                DataModificacao = DateTime.UtcNow,
                TarefaId = tarefa.Id
            };

            tarefa.Historico.Add(historico);
            await _context.SaveChangesAsync();
        }
    }


    public async Task RemoverTarefaAsync(Guid tarefaId)
    {
        var tarefa = await _context.Tarefas.FirstOrDefaultAsync(t => t.Id == tarefaId);
        if (tarefa == null)
            throw new InvalidOperationException("Tarefa não encontrada.");

        _context.Tarefas.Remove(tarefa);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverProjetoAsync(Guid projetoId)
    {
        var projeto = await _context.Projetos
            .Include(p => p.Tarefas)
            .FirstOrDefaultAsync(p => p.Id == projetoId);

        if (projeto == null)
            throw new InvalidOperationException("Projeto não encontrado.");

        if (projeto.Tarefas.Any(t => t.Status != TaskStatus.Concluida))
            throw new InvalidOperationException("Não é possível remover o projeto com tarefas pendentes.");

        _context.Projetos.Remove(projeto);
        await _context.SaveChangesAsync();
    }

    public async Task AdicionarComentarioAsync(Guid tarefaId, string texto, string usuario)
    {
        var tarefa = await _context.Tarefas.FirstOrDefaultAsync(t => t.Id == tarefaId);

        if (tarefa == null)
            throw new InvalidOperationException("Tarefa não encontrada.");

        var comentario = new TaskComment
        {
            Texto = texto,
            Usuario = usuario,
            Data = DateTime.UtcNow,
            TarefaId = tarefa.Id
        };
        _context.Set<TaskComment>().Add(comentario);

        var historico = new TaskHistory
        {
            Alteracao = $"Comentário adicionado: \"{texto}\"",
            Usuario = usuario,
            DataModificacao = DateTime.UtcNow,
            TarefaId = tarefa.Id
        };
        _context.Set<TaskHistory>().Add(historico);

        await _context.SaveChangesAsync();
    }
}
