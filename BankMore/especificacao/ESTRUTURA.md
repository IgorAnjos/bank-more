# Estrutura do Projeto - Banco Digital Ana

```
BancoDigitalAna/
│
├── .github/
│   └── copilot-instructions.md          # Instruções para agentes de IA
│
├── .dockerignore                         # Arquivos a ignorar no build Docker
├── docker-compose.yml                    # Orquestração de containers
├── DOCKER.md                             # Guia de uso do Docker
├── README.md                             # Documentação principal
├── BancoDigitalAna.sln                   # Solution .NET
├── test-api.ps1                          # Script de testes de integração
│
├── agentes/
│   └── agente-desenvolvedor.md           # Diretrizes de desenvolvimento
│
├── especificacao/
│   └── Teste-Desenvolvedor-CSharp-API-v4.docx  # Especificação completa
│
├── sql/
│   ├── contacorrente.sql                 # Schema banco Conta Corrente
│   ├── transferencia.sql                 # Schema banco Transferência
│   └── tarifas.sql                       # Schema banco Tarifas
│
└── src/
    │
    ├── BancoDigitalAna.ContaCorrente/    # 🏦 API Conta Corrente (EF Core)
    │   ├── Api/
    │   │   ├── Controllers/
    │   │   │   ├── AuthController.cs     # POST /api/auth/login
    │   │   │   ├── ContaController.cs    # POST /api/conta, PUT /api/conta/inativar
    │   │   │   └── MovimentacaoController.cs  # POST /api/movimentacao, GET /api/conta/saldo
    │   │   └── Middlewares/
    │   │       └── ExceptionHandlerMiddleware.cs
    │   │
    │   ├── Application/
    │   │   ├── Commands/
    │   │   │   ├── CadastrarContaCommand.cs
    │   │   │   ├── InativarContaCommand.cs
    │   │   │   ├── LoginCommand.cs
    │   │   │   └── MovimentacaoCommand.cs
    │   │   ├── Handlers/
    │   │   │   ├── CadastrarContaHandler.cs
    │   │   │   ├── InativarContaHandler.cs
    │   │   │   ├── LoginHandler.cs
    │   │   │   └── MovimentacaoHandler.cs
    │   │   └── Queries/
    │   │       ├── ConsultarSaldoQuery.cs
    │   │       └── ConsultarSaldoHandler.cs
    │   │
    │   ├── Domain/
    │   │   ├── Entities/
    │   │   │   ├── ContaCorrente.cs
    │   │   │   ├── Movimento.cs
    │   │   │   └── Idempotencia.cs
    │   │   ├── Interfaces/
    │   │   │   ├── IContaCorrenteRepository.cs
    │   │   │   └── IJwtService.cs
    │   │   └── ValueObjects/
    │   │       └── Cpf.cs
    │   │
    │   ├── Infrastructure/
    │   │   ├── Data/
    │   │   │   ├── ContaCorrenteDbContext.cs
    │   │   │   └── Configurations/
    │   │   │       ├── ContaCorrenteConfiguration.cs
    │   │   │       ├── MovimentoConfiguration.cs
    │   │   │       └── IdempotenciaConfiguration.cs
    │   │   ├── Repositories/
    │   │   │   └── ContaCorrenteRepository.cs
    │   │   └── Services/
    │   │       ├── JwtService.cs
    │   │       └── CpfValidator.cs
    │   │
    │   ├── appsettings.json
    │   ├── Program.cs
    │   ├── Dockerfile
    │   └── BancoDigitalAna.ContaCorrente.csproj
    │
    ├── BancoDigitalAna.Transferencia/    # 💸 API Transferência (Dapper + Kafka Producer)
    │   ├── Api/
    │   │   ├── Controllers/
    │   │   │   └── TransferenciaController.cs  # POST /api/transferencia
    │   │   └── Middlewares/
    │   │       └── ExceptionHandlerMiddleware.cs
    │   │
    │   ├── Application/
    │   │   ├── Commands/
    │   │   │   └── RealizarTransferenciaCommand.cs
    │   │   └── Handlers/
    │   │       └── RealizarTransferenciaHandler.cs  # Lógica de rollback
    │   │
    │   ├── Domain/
    │   │   ├── Entities/
    │   │   │   ├── Transferencia.cs
    │   │   │   └── Idempotencia.cs
    │   │   ├── Events/
    │   │   │   └── TransferenciaRealizadaEvent.cs  # Evento Kafka
    │   │   └── Interfaces/
    │   │       ├── ITransferenciaRepository.cs
    │   │       ├── IContaCorrenteService.cs
    │   │       └── IKafkaProducerService.cs
    │   │
    │   ├── Infrastructure/
    │   │   ├── Data/
    │   │   │   └── DatabaseInitializer.cs
    │   │   ├── Repositories/
    │   │   │   └── TransferenciaRepository.cs  # Dapper raw SQL
    │   │   ├── Services/
    │   │   │   └── ContaCorrenteService.cs  # HttpClient para API Conta
    │   │   └── Messaging/
    │   │       └── KafkaProducerService.cs
    │   │
    │   ├── appsettings.json
    │   ├── Program.cs
    │   ├── Dockerfile
    │   └── BancoDigitalAna.Transferencia.csproj
    │
    └── BancoDigitalAna.Tarifas/          # 📊 Worker Service Tarifas (Kafka Consumer)
        ├── Models/
        │   ├── Tarifa.cs
        │   └── TransferenciaRealizadaEvent.cs
        │
        ├── Data/
        │   ├── TarifaRepository.cs        # Dapper raw SQL
        │   └── DatabaseInitializer.cs
        │
        ├── Handlers/
        │   └── TransferenciaConsumerHandler.cs  # Processa mensagens Kafka
        │
        ├── Services/
        │   └── ContaCorrenteService.cs    # HttpClient para debitar tarifa
        │
        ├── Worker.cs                      # BackgroundService
        ├── appsettings.json
        ├── Program.cs                     # Configuração do KafkaFlow Consumer
        ├── Dockerfile
        └── BancoDigitalAna.Tarifas.csproj
```

## 📦 Bancos de Dados SQLite

Gerados automaticamente na primeira execução:

```
src/BancoDigitalAna.ContaCorrente/
└── contacorrente.db              # Tables: contacorrente, movimento, idempotencia

src/BancoDigitalAna.Transferencia/
└── transferencia.db              # Tables: transferencia, idempotencia

src/BancoDigitalAna.Tarifas/
└── tarifas.db                    # Tables: tarifa
```

## 🔗 Fluxo de Comunicação

```
1. Cliente HTTP
   ↓ POST /api/transferencia (JWT)
   
2. API Transferência (Dapper)
   ↓ Validações
   ↓ Débito conta origem (HTTP → API Conta)
   ↓ Crédito conta destino (HTTP → API Conta)
   ↓ Se falhar crédito → Estorno (rollback manual)
   ↓ Persistir transferência (SQLite)
   ↓ Publicar evento Kafka ✉️
   
3. Kafka Topic: transferencias-realizadas
   ↓ TransferenciaRealizadaEvent
   
4. Worker Tarifas (Consumer)
   ↓ Recebe evento
   ↓ Verifica idempotência
   ↓ Persiste tarifa (Dapper + SQLite)
   ↓ Débito tarifa (HTTP → API Conta)
   ↓ Log de sucesso ✅
```

## 🛠️ Tecnologias por Projeto

| Projeto | ORM | Mensageria | Autenticação | Porta |
|---------|-----|------------|--------------|-------|
| **API Conta** | EF Core 9.0 | - | JWT Bearer | 5003 |
| **API Transfer** | Dapper 2.1 | KafkaFlow 4.0 (Producer) | JWT Bearer | 5004 |
| **Worker Tarifas** | Dapper 2.1 | KafkaFlow 4.0 (Consumer) | - | - |

## 📋 Endpoints Implementados

### API Conta Corrente (5 endpoints)

```
POST   /api/conta                 # Cadastrar nova conta
POST   /api/auth/login            # Login (retorna JWT)
PUT    /api/conta/inativar        # Inativar conta (requer JWT + senha)
POST   /api/movimentacao          # Crédito/Débito (requer JWT)
GET    /api/conta/saldo           # Consultar saldo (requer JWT)
```

### API Transferência (1 endpoint)

```
POST   /api/transferencia         # Transferir entre contas (requer JWT)
```

## 🎯 Padrões de Arquitetura Aplicados

✅ **DDD (Domain-Driven Design)**
- Separação clara: Domain → Application → Infrastructure → Api
- Entities, ValueObjects, Repositories

✅ **CQRS (Command Query Responsibility Segregation)**
- Commands para writes (CadastrarContaCommand, MovimentacaoCommand)
- Queries para reads (ConsultarSaldoQuery)

✅ **Mediator Pattern**
- MediatR para desacoplar Controllers de Handlers

✅ **Repository Pattern**
- Abstração de acesso a dados (IContaCorrenteRepository, ITransferenciaRepository)

✅ **Idempotência**
- Tabela `idempotencia` em todos os bancos
- Chave única `chave_idempotencia` para prevenir duplicação

✅ **Event-Driven Architecture**
- Kafka para comunicação assíncrona
- Producer (API Transferência) → Consumer (Worker Tarifas)

✅ **Transactional Outbox Pattern** (parcial)
- Transferências com rollback manual

---

**Total de Arquivos**: ~60 arquivos C#
**Total de Linhas de Código**: ~3.500 linhas
**Tempo de Desenvolvimento**: Implementação incremental e validada
