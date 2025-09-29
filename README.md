DotnetBlueprints

Status: 🚧 Work in Progress (WIP) — under active development.
The goal is to provide a reusable .NET 8 blueprint for real-world enterprise projects.
Architecture follows DDD + Clean Architecture + CQRS with an event-driven approach.

✨ Features & Scope

Layered design: Domain, Application, Infrastructure, API

DDD patterns: AggregateRoot, Entity, ValueObject, Domain Events

CQRS & MediatR: Command/Query separation, pipeline behaviors (validation, logging, caching)

Data access: EF Core (migrations) + Dapper (for selected queries)

Messaging: RabbitMQ + MassTransit, Outbox pattern

Caching: Redis (including role/permission cache)

Background jobs: Hangfire

Observability: Serilog (+ Elastic/Graylog integration)

Real-time communication: SignalR / SSE (use-case driven)

Auth/Permissions: Role/Permission model, policy-based authorization

CI/CD & Containers: Docker, Azure Pipelines (YAML)

Example modules include Auth, Sales, and a shared SharedKernel.

📁 Project Structure (overview)
src/
  DotnetBlueprints.Auth.Domain/
  DotnetBlueprints.Auth.Application/
  DotnetBlueprints.Auth.Infrastructure/
  DotnetBlueprints.Auth.Api/

  DotnetBlueprints.Sales.Domain/
  DotnetBlueprints.Sales.Application/
  DotnetBlueprints.Sales.Infrastructure/
  DotnetBlueprints.Sales.Api/

  DotnetBlueprints.SharedKernel/

build/
  pipelines/           # Azure DevOps YAML examples
  docker/              # Dockerfiles & compose files

tests/
  DotnetBlueprints.Auth.Tests/
  DotnetBlueprints.Sales.Tests/

🧭 Roadmap

 Layered architecture skeleton

 CQRS + MediatR + Pipeline behaviors

 Role/Permission model and caching strategy

 Outbox + MassTransit basic flow

 Multi-module scenarios (Sales/Offer)

 Advanced observability (structured logging + dashboards)

 Integration tests & fakes

 End-to-end CI/CD pipeline

🚀 Getting Started (Local)

Prerequisites: .NET 8 SDK, Docker, SQL Server or PostgreSQL, Redis, RabbitMQ

# 1) Clone the repository
git clone https://github.com/Faik442/dotnetblueprints.git
cd dotnetblueprints

# 2) Start required containers (example compose file)
docker compose -f build/docker/compose.dev.yml up -d

# 3) Build & run (example API)
dotnet build
dotnet run --project src/DotnetBlueprints.Auth.Api


Connection strings and secrets are managed via .env / user-secrets.
Do not push sensitive information into the repo.
