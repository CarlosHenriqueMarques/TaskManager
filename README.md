# TaskManager
API RESTful para gerenciamento de projetos e tarefas, desenvolvida com .NET 7 Minimal API e Entity Framework Core.

---

## Sumário

- [Descrição](#descrição)  
- [Tecnologias](#tecnologias)  
- [Configuração](#configuração)  
- [Execução Local](#execução-local)  
- [Executando com Docker](#executando-com-docker)  
- [Endpoints](#endpoints)  
- [Limitações e Regras](#limitações-e-regras)  
- [Testes](#testes)  
- [Contribuição](#contribuição)  

---

## Descrição

Esta API permite criar projetos, adicionar tarefas a projetos, atualizar tarefas, remover tarefas e projetos, além de adicionar comentários e manter histórico de alterações. É ideal para gerenciamento simples e eficiente de atividades.

---

## Tecnologias

- .NET 7 Minimal API  
- Entity Framework Core (InMemory para testes)  
- xUnit para testes unitários  
- Docker para containerização  

---

## Configuração

1. Clone o repositório:  
   ```bash
   git clone https://github.com/CarlosHenriqueMarques/TaskManager.git
   cd TaskManager
   ```

2. Restore pacotes NuGet:  
   ```bash
   dotnet restore
   ```

---

## Execução Local

Para executar a API localmente:  
```bash
dotnet run --project TaskManager.Api
```

A API estará disponível por padrão em `http://localhost:5000` (ou outra porta conforme configuração).

---

## Executando com Docker

1. Build da imagem Docker:  
   ```bash
   docker build -t taskmanager-api .
   ```

2. Rodar container:  
   ```bash
   docker run -p 5000:80 taskmanager-api
   ```

3. Acesse a API em: `http://localhost:5000`

---

## Endpoints

| Método | Rota                        | Descrição                                   |
|--------|-----------------------------|---------------------------------------------|
| GET    | `/projetos`                  | Lista todos os projetos com suas tarefas    |
| POST   | `/projetos`                  | Cria um novo projeto (envie nome no body)  |
| POST   | `/projetos/{id}/tarefas`    | Adiciona uma tarefa ao projeto especificado |
| PUT    | `/tarefas/{id}`             | Atualiza uma tarefa pelo id                  |
| GET    | `/projetos/{id}/tarefas`    | Obtém as tarefas de um projeto               |

---

### Modelo de dados importantes

- Projeto: Id, Nome, lista de Tarefas  
- Tarefa: Id, Titulo, Descrição, DataVencimento, Status, Prioridade, Comentários, Histórico  
- Comentário: Texto, Usuário, Data  
- Histórico: Descrição da alteração, Usuário, DataModificacao  

---

## Limitações e Regras

- Cada projeto pode conter no máximo **20 tarefas**.  
- Não é possível remover projetos que tenham tarefas **não concluídas**.  
- Histórico registra alterações em tarefas e adição de comentários com usuário e timestamp.  

---

## Testes

Executar todos os testes unitários com:  
```bash
dotnet test
```

Os testes cobrem criação, listagem, atualização, exclusão de tarefas e projetos, e verificam o histórico de alterações.
