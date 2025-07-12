using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;
using TaskManager.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// InMemory
builder.Services.AddDbContext<TaskDbContext>(opt =>
    opt.UseInMemoryDatabase("TaskDb"));

builder.Services.AddScoped<IProjectService, ProjectService>();


builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Corrigido: habilita Swagger e UI
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/projetos", async (IProjectService service) =>
    Results.Ok(await service.ListarProjetosAsync()));

app.MapPost("/projetos", async (string nome, IProjectService service) =>
    Results.Ok(await service.CriarProjetoAsync(nome)));

app.MapPost("/projetos/{id}/tarefas", async (Guid id, TarefaDto dto, IProjectService service) =>
    Results.Ok(await service.AdicionarTarefaAsync(id, dto.Titulo, dto.Descricao, dto.DataVencimento, dto.Prioridade)));

app.MapPut("/tarefas/{id}", async (Guid id, UpdateTarefaDto dto, IProjectService service) =>
{
    await service.AtualizarTarefaAsync(id, dto.Titulo, dto.Descricao, dto.Status, dto.Usuario);
    return Results.NoContent();
});

app.Run();
