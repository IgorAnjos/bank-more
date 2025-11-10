# BankMore - Controle de Versões

## Versão Atual: 1.0.0

### Formato de Versionamento

Seguimos o **Semantic Versioning 2.0.0** (https://semver.org/)

**MAJOR.MINOR.PATCH**

- **MAJOR**: Mudanças incompatíveis na API
- **MINOR**: Novas funcionalidades compatíveis
- **PATCH**: Correções de bugs compatíveis

---

## Histórico de Versões

### v1.0.0 - 2025-11-10

**Primeira versão de produção**

#### Features
- ✅ API Conta Corrente com autenticação JWT
- ✅ API Transferência com idempotência
- ✅ Worker Tarifas (Kafka Consumer)
- ✅ Outbox Pattern para consistência eventual
- ✅ Kafka para mensageria (4 tópicos)
- ✅ Redis para idempotência (24h TTL)
- ✅ Criptografia AES-256 para CPF
- ✅ Refresh Token com revogação
- ✅ Docker Compose completo

#### Componentes e Versões
| Componente | Versão | Descrição |
|------------|--------|-----------|
| BankMore.ContaCorrente.Api | 1.0.0 | API REST - Contas e Movimentos |
| BankMore.Transferencia.Api | 1.0.0 | API REST - Transferências |
| BankMore.Tarifas | 1.0.0 | Worker Service - Processamento Tarifas |
| BankMore.Web | 1.0.0 | Interface Blazor Server |

#### Tecnologias
- .NET 9.0
- Confluent.Kafka 2.8.0
- StackExchange.Redis 2.8.16
- Entity Framework Core 9.0.10
- Dapper 2.1.35
- SQLite (dev)
- Docker Compose

#### Segurança
- JWT com claims mínimas (LGPD compliant)
- Criptografia AES-256-CBC para dados sensíveis
- BCrypt para senhas
- Refresh Token com 1 dia de validade
- Access Token com 10 minutos

---

## Próximas Versões (Roadmap)

### v1.1.0 (Planejado)
- [ ] Suporte a Oracle para produção
- [ ] Kafka Consumer base genérico
- [ ] Integração completa Web ↔ APIs
- [ ] Testes de integração com Testcontainers
- [ ] Métricas e observabilidade

### v1.2.0 (Futuro)
- [ ] API Gateway
- [ ] Circuit Breaker
- [ ] Rate Limiting
- [ ] Distributed Tracing (OpenTelemetry)

### v2.0.0 (Futuro)
- [ ] Migração para gRPC
- [ ] Event Sourcing
- [ ] CQRS completo

---

## Como Atualizar a Versão

1. Atualize este arquivo `VERSION.md`
2. Atualize `Directory.Build.props`
3. Atualize cada `.csproj` (se necessário)
4. Crie tag no Git: `git tag v1.0.0`
5. Atualize `CHANGELOG.md`

---

## Compatibilidade

### v1.0.0
- ✅ .NET 9.0+
- ✅ Docker 20.10+
- ✅ Docker Compose 2.0+
- ✅ Redis 7.0+
- ✅ Kafka 3.5+
