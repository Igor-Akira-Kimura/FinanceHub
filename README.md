# FinanceHub

Backend de uma plataforma de gestão financeira desenvolvida com C# e
ASP.NET Core, aplicando princípios de arquitetura de software,
separação de responsabilidades, testes automatizados, mensageria,
processamento assíncrono, containers e CI/CD.

## Tecnologias

### Backend

- C#
- .NET 9
- ASP.NET Core
- Entity Framework Core
- FluentValidation
- JWT
- LINQ

### Banco de dados

- SQL Server

### Mensageria e processamento assíncrono

- RabbitMQ
- Outbox Pattern
- Workers

### Testes

- xUnit
- Testes unitários
- Testes de integração

### Infraestrutura

- Docker
- Docker Compose
- GitHub Actions
- GitHub Container Registry
- AWS

## Arquitetura

O projeto utiliza uma arquitetura baseada na separação de responsabilidades
entre apresentação, aplicação, domínio e infraestrutura.

```text
FinanceHub
│
├── FinanceHub.Api
├── FinanceHub.Application
├── FinanceHub.Domain
├── FinanceHub.Infrastructure
├── FinanceHub.Carteira.Worker
├── FinanceHub.Outbox.Worker
└── FinanceHub.Tests

FinanceHub.Api

Responsável pela exposição da API HTTP e pela entrada das requisições
da aplicação.

Responsabilidades:

Controllers
Autenticação e autorização
Configuração da aplicação
Health Checks
Middleware
FinanceHub.Application

Responsável pelos casos de uso e pela lógica de aplicação.

Responsabilidades:

Casos de uso
DTOs
Validações
Interfaces
Orquestração das operações
FinanceHub.Domain

Contém o núcleo do domínio e as regras de negócio.

Responsabilidades:

Entidades
Value Objects
Regras de negócio
Exceções de domínio
FinanceHub.Infrastructure

Responsável pelas integrações com recursos externos.

Responsabilidades:

Entity Framework Core
SQL Server
Repositórios
Persistência
Mensageria
Workers

O projeto possui workers separados para processamento assíncrono.

FinanceHub.Carteira.Worker
FinanceHub.Outbox.Worker

Esses processos permitem retirar determinadas tarefas do fluxo
síncrono da API e realizar seu processamento em background.

Funcionalidades
Autenticação utilizando JWT
Gerenciamento de usuários
Gerenciamento de carteiras
Gerenciamento de ativos
Compra de ativos
Venda de ativos
Validação das operações
Persistência em SQL Server
Processamento assíncrono
Mensageria com RabbitMQ
Processamento de eventos através de workers
Outbox
Health Checks
Logs
Testes automatizados
Segurança

A API utiliza autenticação baseada em JSON Web Token (JWT).

As requisições protegidas precisam apresentar um token válido para
acessar os recursos que exigem autenticação.

Mensageria e processamento assíncrono

O RabbitMQ é utilizado para comunicação assíncrona entre componentes
da aplicação.

A utilização de workers permite que tarefas que não precisam ser
executadas diretamente durante a requisição HTTP sejam processadas
separadamente.

Essa abordagem reduz o acoplamento entre a API e processos assíncronos
e permite maior controle sobre o processamento das mensagens.

Outbox

O projeto possui um worker dedicado ao processamento da Outbox.

A abordagem permite separar o registro dos eventos da publicação e
processamento assíncrono, aumentando a confiabilidade da comunicação
entre os componentes.

Testes

O projeto possui uma suíte de testes automatizados.

Os testes são executados durante o pipeline de CI e incluem testes
das regras de negócio e testes de integração.

Docker

O projeto utiliza Docker para padronizar o ambiente de execução.

Também possui Docker Compose para facilitar a execução dos serviços
necessários ao ambiente local.

docker compose up -d
CI/CD

O projeto utiliza GitHub Actions para automatizar o processo de
integração e entrega.

O pipeline realiza:

Checkout do código
Configuração do ambiente .NET
Restore das dependências
Build da solução
Execução dos testes
Build da imagem Docker
Publicação da imagem no GitHub Container Registry
Execução da imagem Docker no ambiente de deploy
Health Check

A aplicação possui um endpoint de Health Check para verificar
a disponibilidade da API.

Exemplo:

GET /health
Objetivo do projeto

O FinanceHub foi desenvolvido para aplicar, em um projeto backend
completo, conceitos utilizados no desenvolvimento profissional de
software.

Entre os principais conceitos aplicados estão:

Clean Architecture
DDD
SOLID
APIs REST
Autenticação JWT
Entity Framework Core
SQL Server
Mensageria
Processamento assíncrono
Outbox Pattern
Testes automatizados
Docker
CI/CD
GitHub Actions
Observabilidade básica