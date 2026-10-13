# Gerenciador de Tarefas: Backend (API)

API REST do Gerenciador de Tarefas, desenvolvida em **C# / .NET** como parte de um desafio técnico para a vaga de Desenvolvedor Full Stack. Ela cuida das regras de negócio, das validações e da persistência das tarefas em PostgreSQL.

## Sumário

1. [Visão geral](#1-visão-geral)
2. [Tecnologias utilizadas](#2-tecnologias-utilizadas)
3. [Como executar a aplicação](#3-como-executar-a-aplicação)
4. [Como usar a API](#4-como-usar-a-api)
5. [Endpoints](#5-endpoints)
6. [Decisões técnicas](#6-decisões-técnicas)
7. [Aplicação publicada (deploy)](#7-aplicação-publicada-deploy)
8. [Como foi feito: dificuldades e facilidades](#8-como-foi-feito-dificuldades-e-facilidades)

## 1. Visão geral

O desafio consiste em uma aplicação de gerenciamento de tarefas, dividida em dois repositórios:

| Parte | Repositório | Publicação |
|---|---|---|
| **Backend** (este repositório) | `https://github.com/vtbrgt/dpc_api` | API: `https://dpc-api-jhrm.onrender.com` / Swagger: `https://dpc-api-jhrm.onrender.com/swagger/index.html` |
| Frontend (React) | `https://github.com/vtbrgt/dpc_app/` | `https://dpc-app-seven.vercel.app/` |

**O que a API oferece**

- CRUD completo de tarefas (criar, listar, consultar, editar e excluir).
- Controle de status: `Pendente`, `EmAndamento` e `Concluida`, com endpoint próprio para alterá-lo.
- Filtro de listagem por status.
- Persistência em PostgreSQL, com migrations versionadas.
- Validação dos dados de entrada e tratamento global de erros em formato padronizado.
- Documentação interativa dos endpoints via Swagger.

## 2. Tecnologias utilizadas

* C# / .NET 10 
* ASP.NET Core Web API
* Entity Framework Core 10
* Npgsql (provider PostgreSQL)
* PostgreSQL ([Neon](https://neon.com))
* Swashbuckle
* Render
* Claude

## 3. Como executar a aplicação

### Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- Um banco **PostgreSQL** acessível, que pode ser:
  - um banco gratuito no [Neon](https://neon.com), ou
  - um PostgreSQL local.

### 3.1 Obter o código

```bash
git clone https://github.com/vtbrgt/dpc_api
cd dpc_api/TaskManager.Api
dotnet restore
```

### 3.2 Configurar a conexão com o banco

A aplicação lê a connection string da chave `ConnectionStrings:DefaultConnection`. **Por segurança, nenhuma credencial é versionada no repositório**, então é preciso informá-la por uma das formas abaixo.

Formato da connection string:

```
# Neon
Host=ep-xxxx.sa-east-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=SUA_SENHA;SSL Mode=Require

# PostgreSQL local
Host=localhost;Port=5432;Database=taskmanager;Username=postgres;Password=postgres
```

> No Neon, use a conexão **direta** (desative a opção "Connection pooling" ao copiar os dados).

**Opção A: User Secrets**

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<SUA_CONNECTION_STRING>"
```

**Opção B: variável de ambiente**

```bash
# Linux, macOS ou Git Bash
export ConnectionStrings__DefaultConnection="<SUA_CONNECTION_STRING>"

# PowerShell
$env:ConnectionStrings__DefaultConnection="<SUA_CONNECTION_STRING>"
```

### 3.3 Executar

```bash
dotnet run
```

- Na primeira execução, as **migrations são aplicadas automaticamente** e a tabela `tarefas` é criada.
- A API sobe por padrão em **`http://localhost:8080`** (o terminal mostra o endereço na linha `Now listening on`).
- O Swagger fica em **`http://localhost:8080/swagger`** (a raiz `/` redireciona para ele).

### 3.4 Variáveis de configuração

| Variável | Para que serve | Padrão |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | Connection string do PostgreSQL | (obrigatória) |
| `PORT` | Porta em que a API escuta (definida automaticamente pelo Render) | `8080` |
| `AllowedOrigins__0`, `AllowedOrigins__1`, ... | Origens do frontend publicado liberadas no CORS, sem barra no final (ex.: `https://meu-app.vercel.app`) | nenhuma |

Origens `localhost` e `127.0.0.1`, em qualquer porta, já são aceitas para desenvolvimento.

## 4. Como usar a API

A forma mais simples de explorar a API é pelo **Swagger** (`/swagger`). Roteiro rápido:

1. Abra o Swagger no navegador.
2. Em `POST /api/tarefas`, clique em **Try it out**, informe `{"titulo": "Minha primeira tarefa", "descricao": "teste"}` e execute. Deve retornar **201** com `status: "Pendente"`.
3. Em `GET /api/tarefas`, confira que a tarefa aparece na lista.
4. Em `PATCH /api/tarefas/{id}/status`, envie `{"status": "EmAndamento"}`. Deve retornar **200** com o novo status.
5. Em `GET /api/tarefas?status=EmAndamento`, confira o filtro.
6. Em `PUT /api/tarefas/{id}`, atualize título, descrição e status.
7. Em `DELETE /api/tarefas/{id}`, remova a tarefa (retorna **204**).

**Para testar os erros:**

- `GET /api/tarefas/999` retorna **404**.
- `POST /api/tarefas` com `{"titulo": ""}` retorna **400** com a mensagem de validação.
- `PATCH /api/tarefas/{id}/status` com `{"status": "Qualquer"}` retorna **400**.

## 5. Endpoints

| Método | Rota | Descrição | Respostas |
|---|---|---|---|
| `GET` | `/api/tarefas` | Lista as tarefas. Aceita `?status=Pendente\|EmAndamento\|Concluida` | 200 |
| `GET` | `/api/tarefas/{id}` | Consulta uma tarefa | 200, 404 |
| `POST` | `/api/tarefas` | Cria uma tarefa (status inicial *Pendente*) | 201, 400 |
| `PUT` | `/api/tarefas/{id}` | Atualiza título, descrição e status | 200, 400, 404 |
| `PATCH` | `/api/tarefas/{id}/status` | Altera somente o status | 200, 400, 404 |
| `DELETE` | `/api/tarefas/{id}` | Remove a tarefa | 204, 404 |

**Regras de validação**

- `titulo`: obrigatório, até 150 caracteres.
- `descricao`: opcional, até 1000 caracteres.
- `status`: um dos valores `Pendente`, `EmAndamento` ou `Concluida`.

**Exemplo: criação**

```http
POST /api/tarefas
Content-Type: application/json

{
  "titulo": "Preparar relatório mensal",
  "descricao": "Consolidar os dados do mês"
}
```

```json
{
  "id": 1,
  "titulo": "Preparar relatório mensal",
  "descricao": "Consolidar os dados do mês",
  "status": "Pendente",
  "criadaEm": "2026-10-12T14:30:00Z",
  "atualizadaEm": null
}
```

**Formato dos erros**

```json
{
  "status": 404,
  "title": "Recurso não encontrado",
  "detail": "Tarefa 999 não encontrada."
}
```

Erros de validação (400) retornam, além disso, a lista de campos inválidos com as respectivas mensagens. Erros inesperados (500) são registrados em log e devolvem uma mensagem genérica, sem expor detalhes internos.

## 6. Decisões técnicas

- **Organização em camadas por pastas, em um único projeto.** Controller (HTTP), Service (regras e acesso ao banco) e DTOs/Models separados.
- **DTOs separados da entidade.** A API só expõe o que está declarado nos DTOs, e o formato do banco pode evoluir sem quebrar o contrato com o frontend.
- **Validação com Data Annotations.** Com `[ApiController]`, dados inválidos retornam 400 automaticamente, com mensagens por campo.
- **Tratamento global de erros** (`IExceptionHandler`). Um único ponto converte exceções em respostas padronizadas: `KeyNotFoundException` vira 404, `ArgumentException` vira 400 e qualquer outra falha vira 500, sem vazar detalhes internos.
- **Endpoint dedicado para status** (`PATCH /api/tarefas/{id}/status`). Atende a alteração de status e permite trocar a situação sem reenviar a tarefa inteira.
- **PostgreSQL no Neon.** Escolhido para ter persistência real e permitir o deploy. O acesso passa pelo Entity Framework Core, então o código de negócio não depende do banco. Como o plano gratuito suspende o banco quando ocioso, a conexão usa `EnableRetryOnFailure()` para tolerar a primeira conexão lenta.
- **Datas em UTC.** `timestamp with time zone` no banco e `DateTime.UtcNow` na aplicação, o que mantém os horários consistentes entre ambientes.
- **Credenciais fora do repositório.** A connection string vem de User Secrets ou de variável de ambiente.
- **CORS configurável.** `localhost` é liberado para desenvolvimento, e as origens publicadas entram pela configuração `AllowedOrigins`, sem alterar o código.
- **Porta configurável.** A aplicação lê a variável `PORT` (usada pelo Render) e, na ausência dela, escuta na 8080.
- **Swagger com `ProducesResponseType`**, documentando os códigos de retorno de cada endpoint.

## 7. Aplicação publicada (deploy)

- **API (Render):** `https://dpc-api-jhrm.onrender.com`, com Swagger em `https://dpc-api-jhrm.onrender.com/swagger`.
- **Banco (Neon):** PostgreSQL gerenciado.
- **Frontend:** `https://dpc-app-seven.vercel.app/`.

Variáveis configuradas no serviço do Render: `ConnectionStrings__DefaultConnection` e `AllowedOrigins__0` (a porta é fornecida pelo próprio Render).

> A API e o banco estão em planos gratuitos, que suspendem a aplicação quando ela fica sem uso. O **primeiro acesso** depois de um tempo parado pode demorar mais que o normal.

---

## 8. Como foi feito: dificuldades e facilidades

**Ordem de desenvolvimento**

1. Estudo da stack e definição da arquitetura.
2. Modelagem da entidade, configuração do Entity Framework Core e migrations.
3. Implementação do CRUD, do endpoint de status, das validações e do tratamento global de erros.
4. Validação dos endpoints pelo Swagger.
5. Migração do banco para PostgreSQL (Neon) e preparação para o deploy no Render.

**Facilidades**

- Minha experiência prévia com NestJS ajudou bastante: controllers, injeção de dependência, DTOs com validação e exception filters têm muitas semelhanças com o ASP.NET Core.
- O Entity Framework Core e suas migrations seguem uma lógica parecida com a dos ORMs que já utilizei antes.
- O Swagger ser praticamente nativo no ecossistema .NET, facilitou bastante o teste da API durante o desenvolvimento.

**Dificuldades**

- Foi o meu primeiro projeto em C# / .NET, então precisei aprender a estrutura de um projeto ASP.NET Core: o papel do `Program.cs`, o registro de serviços na injeção de dependência, a ordem dos middlewares e o fluxo das migrations.
- A configuração do PostgreSQL com Docker deu trabalho no início. Comecei com SQLite para validar a aplicação e, depois, migrei para o PostgreSQL no Neon, o que exigiu recriar as migrations, já que as do SQLite não serviam para o Postgres.
- A configuração de porta e de CORS entre ambiente local e deploy exigiu atenção: a porta fixa do deploy escondia a porta local e fazia o frontend não encontrar a API.
- Entender o que cada comando da CLI do .NET faz (criação do projeto, migrations, execução) levou algum tempo no início.