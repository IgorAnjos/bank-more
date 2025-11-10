# Instruções para Agentes de IA - Banco Digital Ana

## 🎯 Visão Geral do Projeto

Este é um projeto de **Banco Digital** desenvolvido em C#/.NET Core com foco em operações bancárias básicas: contas correntes, movimentações, transferências e tarifas. O projeto segue uma abordagem incremental e gradual de desenvolvimento.

## 🗄️ Arquitetura de Dados

### Schema do Banco de Dados (SQLite/SQL Server)
O projeto utiliza as seguintes tabelas principais (veja `sql/`):

- **contacorrente**: Gestão de contas (id UUID, número, nome, senha com salt, status ativo/inativo)
- **movimento**: Registros de crédito/débito (tipo 'C' ou 'D', vinculados a conta corrente)
- **transferencia**: Transferências entre contas (origem, destino, valor, data)
- **tarifa**: Cobranças de tarifas sobre operações
- **idempotencia**: Controle de requisições duplicadas (chave única, requisição, resultado)

### Convenções de Dados Importantes
- **IDs**: Usar TEXT(37) para UUIDs em todas as tabelas
- **Datas**: Formato obrigatório `DD/MM/YYYY` como TEXT(25)
- **Valores monetários**: REAL com **duas casas decimais sempre**
- **Status booleanos**: INTEGER(1) com CHECK constraint (0 = inativo/false, 1 = ativo/true)
- **Idempotência**: Todas as operações críticas devem verificar a tabela `idempotencia` antes de executar

## 🏗️ Padrões de Arquitetura

### Stack Técnico Obrigatório (Ailos)
- **Backend**: C# + .NET 8
- **ORM**: Dapper (padrão Ailos) - Entity Framework Core aceitável
- **API**: RESTful com Swagger/OpenAPI completo (todos atributos, exemplos, respostas documentadas)
- **Autenticação**: JWT obrigatório em TODOS os endpoints (exceto cadastro/login)
- **Mensageria**: KafkaFlow (biblioteca .NET para Kafka)
- **Database**: SQLite (recomendado) ou Oracle (produção Ailos)
- **Testes**: xUnit ou NUnit (unitários + integração)
- **Containerização**: Docker + Docker Compose (um container por API + Kafka + DB)
- **Cache**: Redis recomendado (diferencial)

### Arquitetura Obrigatória
1. **DDD (Domain-Driven Design)** em todos os microsserviços
2. **CQRS (Command Query Responsibility Segregation)**
3. **Mediator Pattern** para reduzir dependências
4. **Repository Pattern** para acesso a dados
5. **Idempotência** em todas operações críticas (usa chave_idempotencia)

### Estrutura de Microsserviços
```
BancoDigitalAna/
├── BancoDigitalAna.ContaCorrente/
│   ├── Api/                    # Controllers, Middlewares
│   ├── Application/            # Commands, Queries, Handlers (CQRS)
│   ├── Domain/                 # Entities, ValueObjects, Interfaces
│   ├── Infrastructure/         # Repositories, Dapper, DbContext
│   └── Tests/                  # xUnit (unit + integration)
├── BancoDigitalAna.Transferencia/
│   ├── Api/
│   ├── Application/
│   ├── Domain/
│   ├── Infrastructure/
│   └── Tests/
├── BancoDigitalAna.Tarifas/    # OPCIONAL - Consumidor Kafka
│   ├── Application/
│   ├── Infrastructure/
│   └── appsettings.json        # Valor da tarifa configurável
├── sql/                        # Scripts de schema
└── docker-compose.yml          # API Conta + API Transfer + Kafka + SQLite
```

### Comunicação entre Microsserviços
- **Síncrona**: API Transferência → API Conta Corrente (REST + JWT repassado)
- **Assíncrona**: Kafka Topics:
  - `transferencias-realizadas` (Transferência → Tarifas)
  - `tarifas-realizadas` (Tarifas → Conta Corrente)

## 🔐 Segurança

- **JWT Obrigatório**: TODOS os endpoints (exceto cadastro e login) exigem token no header
- **HTTP 403**: Retornar quando token inválido/expirado
- **Senhas**: Sempre usar hashing seguro (BCrypt/Argon2) + salt único por conta
- **Dados Sensíveis**: CPF e número da conta NÃO podem transitar entre microsserviços (restrição da Seg. Informação)
- **Validação**: Contas inativas (`ativo = 0`) não podem realizar operações
- **Auditoria**: Todo `movimento` deve registrar data/hora exata
- **SQL Injection**: Use sempre **parâmetros** em queries (Dapper protege com @parametros)

## 💰 Regras de Negócio e Validações

### Política de Tarifas
- **Transferências**: R$ 0,00 (PIX) a R$ 10,00 (TED entre bancos) - configurável no `appsettings.json`
- **Saques**: Gratuitos até X por mês, depois R$ 2,00-5,00
- **Manutenção de conta**: R$ 0,00 (digital) a R$ 30,00/mês (tradicional)
- **Momento da cobrança**: No momento da operação, debitado automaticamente

### Fluxo de Transferência (Crítico!)
1. Validar conta origem (cadastrada, ativa, saldo suficiente)
2. Validar conta destino (cadastrada, ativa)
3. Debitar da conta origem (chamada à API Conta Corrente)
4. Creditar na conta destino (chamada à API Conta Corrente)
5. **Se falhar o crédito**: ESTORNAR o débito (rollback manual)
6. Persistir registro em `transferencia`
7. Publicar mensagem Kafka "Transferências Realizadas" (opcional)
8. Consumir tarifa via Kafka e debitar (opcional)

### Validações por Endpoint

#### Cadastro de Conta
- Validar CPF (retornar `INVALID_DOCUMENT` se inválido)
- Gerar salt único e hash seguro da senha (BCrypt/Argon2)

#### Login
- Aceitar número da conta OU CPF + senha
- Retornar JWT com `idcontacorrente` (retornar `USER_UNAUTHORIZED` se falhar)

#### Movimentação
- `INVALID_ACCOUNT`: Conta não cadastrada
- `INACTIVE_ACCOUNT`: Conta com `ativo = 0`
- `INVALID_VALUE`: Valor <= 0
- `INVALID_TYPE`: Tipo diferente de 'C' ou 'D'
- `INVALID_TYPE`: Débito em conta que não é a do usuário logado (só crédito é permitido)

#### Saldo
- Calculado: `SUM(créditos) - SUM(débitos) - SUM(tarifas)`
- Retornar `0.00` se não houver movimentos

#### Transferência
- `INVALID_ACCOUNT`: Conta origem/destino não cadastrada
- `INACTIVE_ACCOUNT`: Conta origem/destino inativa
- `INVALID_VALUE`: Valor <= 0

## 🧪 Testes

- Testar cenários de idempotência (requisições duplicadas devem retornar o mesmo resultado)
- Testar transações (rollback em caso de falha parcial)
- Mockar DbContext com InMemory Database do EF Core
- Validar formatos de data e precisão decimal

## 🚀 Fluxo de Desenvolvimento

**Metodologia**: Desenvolvimento **incremental e gradual** (veja `agentes/agente-desenvolvedor.md`)
- Implemente uma feature completa por vez
- Sempre peça validação antes de prosseguir para a próxima
- Priorize funcionalidade básica antes de otimizações

## 📚 Referências Essenciais

- Schemas SQL completos: `sql/*.sql`
- Diretrizes gerais do desenvolvedor: `agentes/agente-desenvolvedor.md`
- Especificação completa do projeto: `especificacao/teste-desevolvedor-csharp-api.md`

## 📋 Endpoints da API Conta Corrente

1. **POST /conta** - Cadastrar conta (recebe CPF + senha, retorna número da conta)
2. **POST /login** - Login (recebe número/CPF + senha, retorna JWT)
3. **PUT /conta/inativar** - Inativar conta (requer JWT + senha, retorna 204)
4. **POST /movimentacao** - Movimentação (requer JWT, tipo C/D, valor, opcional: número conta)
5. **GET /saldo** - Consultar saldo (requer JWT, retorna saldo calculado)

## 📋 Endpoints da API Transferência

1. **POST /transferencia** - Transferência entre contas (requer JWT, conta destino, valor)
   - Chama API Conta: débito origem → crédito destino (ou estorna se falhar)
   - Publica Kafka: `transferencias-realizadas`

## 📋 Aplicação Tarifas (OPCIONAL)

- **Consumidor Kafka**: Tópico `transferencias-realizadas`
- **Configuração**: `appsettings.json` com valor da tarifa (ex: 2.00)
- **Persiste**: Tabela `tarifa` (idcontacorrente, valor, datamovimento)
- **Produtor Kafka**: Tópico `tarifas-realizadas` → API Conta debita automaticamente

## ⚠️ Avisos Importantes

- **Nunca** altere o schema SQL sem validar compatibilidade com dados existentes
- **Sempre** use transações para operações que afetam múltiplas tabelas
- **Sempre** valide datas no formato DD/MM/YYYY antes de persistir
- Ao gerar GUIDs, use formato string de 36 caracteres + hífens (adequado para TEXT(37))
