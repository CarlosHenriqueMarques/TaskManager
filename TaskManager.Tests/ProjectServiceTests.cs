using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Services;
using TaskManager.Infrastructure.Persistence;
using static TaskManager.Domain.Enum;
using TaskStatus = TaskManager.Domain.Enum.TaskStatus;

namespace TaskManager.Tests;

public class ProjectServiceTests
{
    private readonly TaskDbContext _context;
    private readonly ProjectService _service;

    public ProjectServiceTests()
    {
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new TaskDbContext(options);
        _service = new ProjectService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task CriarProjetoAsync_DeveCriarProjeto()
    {
        var nome = "Projeto Teste";

        var id = await _service.CriarProjetoAsync(nome);
        var projeto = await _service.ObterProjetoAsync(id);

        Assert.NotNull(projeto);
        Assert.Equal(nome, projeto.Nome);
    }

    [Fact]
    public async Task ListarProjetosAsync_DeveRetornarProjetos()
    {
        await _service.CriarProjetoAsync("Projeto 1");
        await _service.CriarProjetoAsync("Projeto 2");

        var lista = (await _service.ListarProjetosAsync()).ToList();

        Assert.True(lista.Count >= 2);
    }

    [Fact]
    public async Task AdicionarTarefaAsync_DeveAdicionarTarefa()
    {
        var projetoId = await _service.CriarProjetoAsync("Projeto");

        var tarefaId = await _service.AdicionarTarefaAsync(
            projetoId, "Tarefa 1", "Descrição", DateTime.UtcNow.AddDays(1), TaskPriority.Alta);

        var projeto = await _service.ObterProjetoAsync(projetoId);

        Assert.Contains(projeto.Tarefas, t => t.Id == tarefaId && t.Titulo == "Tarefa 1");
    }

    [Fact]
    public async Task AdicionarTarefaAsync_ProjetoNaoEncontrado_DeveLancarExcecao()
    {
        var fakeProjetoId = Guid.NewGuid();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.AdicionarTarefaAsync(fakeProjetoId, "Tarefa", "Desc", DateTime.UtcNow, TaskPriority.Media));
    }

    [Fact]
    public async Task AdicionarTarefaAsync_Limite20Tarefas_DeveLancarExcecao()
    {
        // Arrange
        var projetoId = await _service.CriarProjetoAsync("Projeto");

        for (int i = 0; i < 20; i++)
        {
            await _service.AdicionarTarefaAsync(projetoId, $"Tarefa {i}", "Desc", DateTime.UtcNow, TaskPriority.Media);
        }

        // Act + Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _service.AdicionarTarefaAsync(projetoId, "Tarefa 21", "Desc", DateTime.UtcNow, TaskPriority.Media);
        });

        Assert.Equal("Limite de 20 tarefas por projeto atingido.", exception.Message);
    }

    [Fact]
    public async Task AtualizarTarefaAsync_DeveAtualizarEAdicionarHistorico()
    {
        var projetoId = await _service.CriarProjetoAsync("Projeto");
        var tarefaId = await _service.AdicionarTarefaAsync(projetoId, "OldTitle", "OldDesc", DateTime.UtcNow, TaskPriority.Media);

        await _service.AtualizarTarefaAsync(tarefaId, "NewTitle", "NewDesc", TaskStatus.EmAndamento, "Carlos");

        var tarefa = await _context.Tarefas.Include(t => t.Historico).FirstOrDefaultAsync(t => t.Id == tarefaId);

        Assert.Equal("NewTitle", tarefa.Titulo);
        Assert.Equal("NewDesc", tarefa.Descricao);
        Assert.Equal(TaskStatus.EmAndamento, tarefa.Status);
        Assert.Single(tarefa.Historico);
        Assert.Contains("Título", tarefa.Historico[0].Alteracao);
    }

    [Fact]
    public async Task AtualizarTarefaAsync_TarefaNaoEncontrada_DeveLancarExcecao()
    {
        var fakeTarefaId = Guid.NewGuid();
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.AtualizarTarefaAsync(fakeTarefaId, "X", "X", TaskStatus.Concluida, "User"));
    }

    [Fact]
    public async Task RemoverTarefaAsync_DeveRemoverTarefa()
    {
        var projetoId = await _service.CriarProjetoAsync("Projeto");
        var tarefaId = await _service.AdicionarTarefaAsync(projetoId, "Tarefa", "Desc", DateTime.UtcNow, TaskPriority.Media);

        await _service.RemoverTarefaAsync(tarefaId);

        var tarefa = await _context.Tarefas.FindAsync(tarefaId);
        Assert.Null(tarefa);
    }

    [Fact]
    public async Task RemoverTarefaAsync_TarefaNaoEncontrada_DeveLancarExcecao()
    {
        var fakeTarefaId = Guid.NewGuid();
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RemoverTarefaAsync(fakeTarefaId));
    }

    [Fact]
    public async Task RemoverProjetoAsync_DeveRemoverProjeto()
    {
        var projetoId = await _service.CriarProjetoAsync("Projeto");
        // Adiciona tarefa concluída
        var tarefaId = await _service.AdicionarTarefaAsync(projetoId, "Tarefa", "Desc", DateTime.UtcNow, TaskPriority.Media);

        var tarefa = await _context.Tarefas.FindAsync(tarefaId);
        tarefa.Status = TaskStatus.Concluida;
        await _context.SaveChangesAsync();

        await _service.RemoverProjetoAsync(projetoId);

        var projeto = await _context.Projetos.FindAsync(projetoId);
        Assert.Null(projeto);
    }

    [Fact]
    public async Task RemoverProjetoAsync_TarefasPendentes_DeveLancarExcecao()
    {
        var projetoId = await _service.CriarProjetoAsync("Projeto");
        await _service.AdicionarTarefaAsync(projetoId, "Tarefa", "Desc", DateTime.UtcNow, TaskPriority.Media);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RemoverProjetoAsync(projetoId));
    }

    [Fact]
    public async Task RemoverProjetoAsync_ProjetoNaoEncontrado_DeveLancarExcecao()
    {
        var fakeProjetoId = Guid.NewGuid();
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RemoverProjetoAsync(fakeProjetoId));
    }

    [Fact]
    public async Task AdicionarComentarioAsync_DeveAdicionarComentarioEHistorico()
    {
        var projetoId = await _service.CriarProjetoAsync("Projeto");
        var tarefaId = await _service.AdicionarTarefaAsync(projetoId, "Tarefa", "Desc", DateTime.UtcNow, TaskPriority.Media);

        await _service.AdicionarComentarioAsync(tarefaId, "Comentário Teste", "Carlos");

        var tarefa = await _context.Tarefas
            .Include(t => t.Comentarios)
            .Include(t => t.Historico)
            .FirstOrDefaultAsync(t => t.Id == tarefaId);

        Assert.Single(tarefa.Comentarios);
        Assert.Single(tarefa.Historico);
        Assert.Equal("Comentário Teste", tarefa.Comentarios[0].Texto);
    }

    [Fact]
    public async Task AdicionarComentarioAsync_TarefaNaoEncontrada_DeveLancarExcecao()
    {
        var fakeTarefaId = Guid.NewGuid();
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AdicionarComentarioAsync(fakeTarefaId, "X", "User"));
    }
}
