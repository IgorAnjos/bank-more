# Changelog

Todas as mudanças notáveis neste projeto serão documentadas neste arquivo.

O formato é baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.0.0/),
e este projeto adere ao [Semantic Versioning](https://semver.org/lang/pt-BR/).

---

## [1.0.0] - 2025-11-10

### Adicionado
- **API Conta Corrente** (v1.0.0)
  - Cadastro de contas com CPF criptografado (AES-256)
  - Autenticação JWT (Access + Refresh Token)
  - Operações de depósito e saque
  - Consulta de saldo e movimentações
  - Idempotência via Redis (24h TTL)
  - Versionamento de API (v1)
  - Swagger/OpenAPI 3.0
  - HATEOAS parcial
  - Problem Details (RFC 7807)

- **API Transferência** (v1.0.0)
  - Transferências entre contas
  - Validação de saldo
  - Cobrança de tarifa (R$ 2,00)
  - Compensação com retry automático
  - Idempotência via Redis
  - Publicação de eventos no Kafka
  - DLQ para transferências falhadas

- **Worker Tarifas** (v1.0.0)
  - Consumer Kafka (topic: transferencias-realizadas)
  - Processamento assíncrono de tarifas
  - Chamada à API Conta para débito
  - Idempotência com Redis
  - Retry com backoff exponencial
  - DLQ para tarifas não processadas

- **Outbox Pattern**
  - Tabela `outbox_events` em ambas APIs
  - Background Service (polling 5s)
  - Processamento em lotes (100 eventos)
  - Retry até 5 tentativas
  - Auto-cleanup de eventos antigos (7 dias)
  - Garantia de at-least-once delivery

- **Infraestrutura**
  - Docker Compose com 8 serviços
  - Kafka + Zookeeper
  - Redis para cache/idempotência
  - Volumes persistentes (SQLite)
  - Health checks
  - Scripts PowerShell (docker-start.ps1, docker-check.ps1)

- **Segurança**
  - JWT com claims mínimas (LGPD compliant)
  - Criptografia AES-256-CBC para CPF
  - BCrypt para senhas (salt único por usuário)
  - Refresh Token com revogação
  - Audit trail via tabela de movimentos

- **Documentação**
  - DOCKER-SETUP.md (guia completo)
  - DADOS-TESTE.md (exemplos de testes)
  - JWT-SECURITY-GUIDELINES.md
  - VERSION.md (versionamento)
  - CHANGELOG.md (este arquivo)

### Tecnologias
- .NET 9.0
- Entity Framework Core 9.0.10
- Dapper 2.1.35
- Confluent.Kafka 2.8.0
- StackExchange.Redis 2.8.16
- SQLite 3.x
- Docker + Docker Compose
- Blazor Server (Web)

### Segurança e Compliance
- ✅ LGPD: Dados sensíveis criptografados
- ✅ OWASP: JWT sem PII
- ✅ BCrypt: Senhas com salt único
- ✅ TLS Ready: Preparado para HTTPS
- ✅ Idempotência: Prevenção de duplicatas

---

## [Unreleased]

### Planejado para v1.1.0
- Suporte a Oracle Database (produção)
- Testes de integração com Testcontainers
- Web App Blazor completamente integrado
- Consumer Kafka genérico (base class)
- Métricas e observabilidade básica
- CI/CD com GitHub Actions

### Planejado para v1.2.0
- API Gateway (Ocelot ou YARP)
- Circuit Breaker (Polly)
- Rate Limiting
- Distributed Caching
- Logging estruturado (Serilog)

### Planejado para v2.0.0
- Migração para gRPC
- Event Sourcing completo
- CQRS pattern
- Projeções assíncronas
- Saga Pattern para transações distribuídas

---

## Tipos de Mudanças

- **Adicionado** para novas funcionalidades
- **Modificado** para mudanças em funcionalidades existentes
- **Obsoleto** para funcionalidades que serão removidas
- **Removido** para funcionalidades removidas
- **Corrigido** para correções de bugs
- **Segurança** para vulnerabilidades corrigidas

---

## Formato de Versão

Usamos [Semantic Versioning](https://semver.org/):

- **MAJOR**: Mudanças incompatíveis na API
- **MINOR**: Novas funcionalidades compatíveis
- **PATCH**: Correções de bugs compatíveis

Exemplo: `1.2.3` = Major 1, Minor 2, Patch 3

---

## Links

- [Repositório](https://github.com/bankmore/bankmore)
- [Issues](https://github.com/bankmore/bankmore/issues)
- [Releases](https://github.com/bankmore/bankmore/releases)
