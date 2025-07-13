
# TaskManager API

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
   git clone https://github.com/seu-usuario/taskmanager-api.git
   cd taskmanager-api
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
   docker run -p 5000:5000 taskmanager-api
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


## Refinamento e Perguntas ao Product Owner

Para futuras implementações e melhorias, seguem algumas dúvidas e pontos a esclarecer com o PO:

- Devemos suportar múltiplos usuários e permissões por projeto/tarefa?
- Qual o nível esperado de auditoria e rastreamento de alterações? Devemos armazenar versões completas das tarefas?
- Existe necessidade de integração com outras ferramentas (ex: calendários, sistemas externos)?
- O sistema deverá escalar para múltiplos clientes ou será usado internamente?
- Existe previsão para suporte a outras linguagens, internacionalização ou personalização?


## Pontos de Melhoria e Visão para o Projeto

Algumas sugestões para evoluir e profissionalizar o projeto:

- **Persistência**: Migrar de banco InMemory para banco relacional (SQL Server, PostgreSQL) com migrações EF Core.
- **Autenticação e Autorização**: Implementar Identity para controle de acesso, suporte a múltiplos usuários e permissões. (Se futuramente for um requisito)
- **API**: Adicionar versionamento da API para facilitar manutenção futura.
- **Logs e Monitoramento**: Integrar logs estruturados, tracing distribuído e monitoramento de desempenho.
- **Deploy e Infraestrutura**: Automatizar deploy com CI/CD, considerar orquestração com Kubernetes e uso de cloud (Azure, AWS).
- **Escalabilidade**: Preparar para escalabilidade horizontal, caching e otimização de consultas. (Se futuramente for um requisito/necessario)
- **Internacionalização**: Suporte a múltiplos idiomas para alcance global. (Se futuramente for um requisito)
