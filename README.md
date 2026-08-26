# FinanceHub

Backend de uma plataforma de gestão financeira desenvolvida com C# e ASP.NET Core, aplicando conceitos de arquitetura, DDD, SOLID, mensageria, processamento assíncrono, testes automatizados, Docker, CI/CD e Cloud.

## Tecnologias

### Backend
- C#
- .NET 9
- ASP.NET Core
- Entity Framework Core
- FluentValidation
- LINQ

### Banco de dados
- SQL Server
- Entity Framework Core Migrations

### Mensageria e processamento assíncrono
- RabbitMQ
- Outbox Pattern
- Workers
- Idempotência

### Cache
- Redis

### Testes
- xUnit
- Testes unitários
- Testes de integração

### Infraestrutura e Cloud
- Docker
- Docker Compose
- GitHub Actions
- GitHub Container Registry
- AWS EC2
- AWS RDS
- AWS VPC
- AWS Security Groups
- Ubuntu Server

## Arquitetura

O projeto utiliza separação de responsabilidades entre API, aplicação, domínio e infraestrutura.

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
Controllers
Autenticação e autorização
JWT
Health Checks
Middleware
Exception Handling
Observabilidade
FinanceHub.Application
Casos de uso
DTOs
Validações
Interfaces
Orquestração das operações
FinanceHub.Domain
Entidades
Value Objects
Regras de negócio
Exceções de domínio
FinanceHub.Infrastructure
Entity Framework Core
SQL Server
Redis
Repositórios
RabbitMQ
BCrypt
Persistência
Funcionalidades
Autenticação com JWT
Refresh Token
Gerenciamento de usuários
Gerenciamento de carteiras
Gerenciamento de ativos
Compra e venda de ativos
Validação das operações
Persistência em SQL Server
Cache com Redis
Mensageria com RabbitMQ
Processamento assíncrono
Outbox Pattern
Idempotência
Health Checks
Correlation ID
Testes automatizados
Cloud / Deploy

A aplicação foi implantada na AWS utilizando EC2 para execução da API e RDS para o banco SQL Server.

                 AWS
                  │
        ┌─────────┴─────────┐
        │                   │
       EC2                 RDS
    Ubuntu + Docker      SQL Server
        │                   │
        └──── FinanceHub ───┘
                 API

O ambiente foi validado com:

API executando em Docker na EC2
SQL Server hospedado no RDS
Entity Framework Core Migrations aplicadas no RDS
Comunicação EC2 → RDS
Autenticação JWT
Acesso externo à API através do Security Group
Health Check funcionando externamente
CI/CD

O pipeline utiliza GitHub Actions para:

Restore das dependências
Build
Execução dos testes
Build da imagem Docker
Publicação no GitHub Container Registry
Deploy da aplicação

Fluxo:

GitHub
   ↓
GitHub Actions
   ↓
Build + Test
   ↓
Docker Build
   ↓
GitHub Container Registry
   ↓
AWS EC2
   ↓
FinanceHub API
Docker

Para executar o ambiente local:

docker compose up -d
Health Check
GET /health

Endpoint utilizado para verificar a disponibilidade da API.

Objetivo

Projeto desenvolvido para praticar conceitos utilizados no desenvolvimento profissional de aplicações backend, incluindo:

Clean Architecture
DDD
SOLID
APIs REST
JWT
Entity Framework Core
SQL Server
Redis
RabbitMQ
Outbox Pattern
Idempotência
Testes automatizados
Docker
CI/CD
AWS
Observabilidade