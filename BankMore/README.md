# 🏦 BankMore - Sistema Bancário Completo# BankMore - Sistema Bancário Completo



Sistema bancário digital com **arquitetura de microsserviços**, **comunicação assíncrona via Kafka**, **processamento de tarifas em tempo real**, **interface web moderna com Blazor WebAssembly** e **observabilidade completa** (Serilog + Seq + Prometheus + Grafana).Sistema bancário com arquitetura de microsserviços, comunicação assíncrona via Kafka, processamento de tarifas em tempo real e interface web moderna com Blazor WebAssembly.



---## 🏗️ Arquitetura



## 📑 Índice```

┌─────────────────────┐

- [Arquitetura](#-arquitetura)│   Blazor WebAssembly│

- [Tecnologias](#-tecnologias)│   (Interface Web)   │

- [Funcionalidades](#-funcionalidades)│    Port: 5000       │

- [Pré-requisitos](#-pré-requisitos)└──────────┬──────────┘

- [Como Executar](#-como-executar-com-docker)           │ HTTP/JWT

- [Estrutura do Projeto](#-estrutura-do-projeto)           ├──────────────────────┐

- [Observabilidade](#-observabilidade)           ▼                      ▼

- [Testes](#-testes)┌─────────────────┐      HTTP      ┌──────────────────────┐

- [APIs Disponíveis](#-apis-disponíveis)│   API Conta     │ ◄───────────── │ API Transferência    │

- [Troubleshooting](#-troubleshooting)│   Corrente      │                │                      │

│  (EF Core)      │                │     (Dapper)         │

---│  Port: 5003     │                │    Port: 5004        │

└─────────────────┘                └──────────────────────┘

## 🏗️ Arquitetura        ▲                                    │

        │                                    │ publish

```        │ HTTP (débito tarifa)               ▼

┌─────────────────────────────────────────────────────────────┐        │                          ┌─────────────────────┐

│                    CAMADA DE APRESENTAÇÃO                   │┌───────┴──────────┐               │   Kafka Topic:      │

│                                                               ││  Worker Tarifas  │ ◄─────────── │ transferencias-     │

│  ┌─────────────────────┐      ┌─────────────────────────┐   ││   (Consumer)     │   consume     │    realizadas       │

│  │  Blazor WebAssembly │      │     Swagger/OpenAPI     │   │└──────────────────┘               └─────────────────────┘

│  │   (Interface Web)   │      │   (Documentação APIs)   │   │```

│  │    Port: 8080       │      │   Ports: 5003/5004      │   │

│  └──────────┬──────────┘      └─────────────────────────┘   │### Componentes

└─────────────┼──────────────────────────────────────────────┘

              │ HTTP/REST + JWT1. **Interface Web Blazor** (`BankMore.Web`) 🆕

┌─────────────┼──────────────────────────────────────────────┐   - Interface de usuário moderna com Blazor WebAssembly

│             │          CAMADA DE SERVIÇOS                   │   - Autenticação JWT com LocalStorage

│             ├──────────────────────┐                        │   - Funcionalidades: Login, Cadastro, Consulta de Conta, Movimentações e Transferências

│             ▼                      ▼                        │   - Design responsivo com Bootstrap 5

│   ┌─────────────────┐      ┌──────────────────────┐        │

│   │   API Conta     │◄────►│ API Transferência    │        │2. **API Conta Corrente** (`BankMore.ContaCorrente`)

│   │   Corrente      │ HTTP │                      │        │   - Gerencia contas, autenticação (JWT), movimentações e saldo

│   │  (EF Core)      │      │     (Dapper)         │        │   - Entity Framework Core + SQLite

│   │  Port: 5003     │      │    Port: 5004        │        │   - RESTful com HATEOAS, versionamento e Problem Details

│   └────────┬────────┘      └──────────┬───────────┘        │   - CORS habilitado para frontend

└────────────┼────────────────────────────┼──────────────────┘

             │                            │3. **API Transferência** (`BankMore.Transferencia`)

             │ HTTP (débito tarifa)       │ publish event   - Processa transferências entre contas com rollback automático

             │                            ▼   - Dapper (raw SQL) + SQLite

┌────────────┼────────────────────────────────────────────────┐   - Kafka Producer: publica eventos de transferências realizadas

│            │             MENSAGERIA                          │   - Integração HTTP com API Conta Corrente

│            │      ┌─────────────────────────┐               │   - CORS habilitado para frontend

│            │      │   Apache Kafka          │               │

│            │      │   Topic: transferencias-│               │4. **Worker Tarifas** (`BankMore.Tarifas`)

│            │      │        realizadas       │               │   - Background Service que consome eventos do Kafka

│            │      │    Port: 9092           │               │   - Persiste tarifas no banco de dados

│            │      └──────────┬──────────────┘               │   - Debita automaticamente tarifas na conta origem

└────────────┼─────────────────┼──────────────────────────────┘   - Idempotência garantida por `idtransferencia`

             │                 │ consume

             │                 ▼## 🚀 Como Executar

┌────────────┼─────────────────────────────────────────────────┐

│            │       CAMADA DE PROCESSAMENTO                   │### Opção 1: Script Automático (Recomendado) ⚡

│            │      ┌──────────────────┐                       │

│            └─────►│  Worker Tarifas  │                       │Execute o script PowerShell que inicia todos os serviços automaticamente:

│                   │   (Background    │                       │

│                   │    Service)      │                       │```powershell

│                   └──────────────────┘                       │cd c:\GitHub\Teste\BankMore

└──────────────────────────────────────────────────────────────┘.\start-all.ps1

```

┌──────────────────────────────────────────────────────────────┐

│                CAMADA DE DADOS (SQLite)                      │O script irá:

│                                                               │1. Iniciar API Conta Corrente (porta 5003)

│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐       │2. Iniciar API Transferência (porta 5004)

│  │contacorrente │  │transferencia │  │   tarifas    │       │3. Iniciar Interface Web (porta 5000/5001)

│  │     .db      │  │     .db      │  │     .db      │       │4. Abrir o navegador automaticamente

│  └──────────────┘  └──────────────┘  └──────────────┘       │

└──────────────────────────────────────────────────────────────┘### Opção 2: Manual (3 Terminais)



┌──────────────────────────────────────────────────────────────┐#### Terminal 1 - API Conta Corrente

│              CAMADA DE OBSERVABILIDADE                       │```powershell

│                                                               │cd src\BankMore.ContaCorrente\Api

│  ┌─────────┐  ┌──────────┐  ┌──────────┐  ┌────────────┐   │dotnet run

│  │ Serilog │─►│   Seq    │  │Prometheus│─►│  Grafana   │   │```

│  │  Logs   │  │ (5341)   │  │  (9090)  │  │   (3000)   │   │

│  └─────────┘  └──────────┘  └──────────┘  └────────────┘   │#### Terminal 2 - API Transferência

└──────────────────────────────────────────────────────────────┘```powershell

```cd src\BankMore.Transferencia\Api

dotnet run

### 📦 Componentes```



| Componente | Tecnologia | Porta | Descrição |#### Terminal 3 - Interface Web

|------------|-----------|-------|-----------|```powershell

| **Blazor Web** | WebAssembly | 8080 | Interface de usuário moderna e responsiva |cd src\BankMore.Web

| **API Conta** | ASP.NET Core + EF Core | 5003 | Gerencia contas, auth JWT, movimentações |dotnet run

| **API Transferência** | ASP.NET Core + Dapper | 5004 | Processa transferências com rollback |```

| **Worker Tarifas** | Background Service | - | Consome eventos Kafka e debita tarifas |

| **Kafka** | Apache Kafka | 9092 | Mensageria assíncrona |### Acessar o Sistema

| **Zookeeper** | Apache Zookeeper | 2181 | Coordenação do Kafka |

| **Redis** | Redis Cache | 6379 | Idempotência e cache distribuído |- **🌐 Interface Web**: http://localhost:5000 ou https://localhost:5001

| **Seq** | Seq Logs | 5341 | Agregação e visualização de logs |- **📖 Swagger Conta**: http://localhost:5003

| **Prometheus** | Prometheus | 9090 | Coleta de métricas |- **📖 Swagger Transferência**: http://localhost:5004

| **Grafana** | Grafana | 3000 | Dashboards e visualizações |

## 📘 Documentação

---

- **[Guia de Execução Web](GUIA-EXECUCAO-WEB.md)**: Tutorial completo com fluxo de teste

## 🚀 Tecnologias- **[README da Interface Web](src/BankMore.Web/README.md)**: Documentação específica do frontend



### Backend## 🎯 Funcionalidades da Interface Web

- **.NET 9.0** - Framework principal

- **ASP.NET Core** - APIs RESTful### Autenticação

- **Entity Framework Core 9.0.10** - ORM (API Conta)- ✅ Login com CPF ou número da conta

- **Dapper 2.1.66** - Micro-ORM (API Transferência)- ✅ Cadastro de nova conta

- **MediatR 13.1.0** - CQRS pattern- ✅ Logout

- **KafkaFlow 4.0.1** - Cliente Kafka- ✅ JWT Token armazenado no LocalStorage

- **BCrypt.Net 4.0.3** - Hashing de senhas

- **SQLite** - Banco de dados### Gestão de Conta

- ✅ Visualizar dados da conta (CPF, nome, status)

### Frontend- ✅ Consultar saldo em tempo real

- **Blazor WebAssembly** - SPA client-side- ✅ Criar movimentações (crédito/débito)

- **Bootstrap 5** - UI responsiva- ✅ Visualizar extrato com paginação

- **HttpClient** - Comunicação com APIs

- **JWT Authentication** - Autenticação stateless### Transferências

- ✅ Realizar transferências entre contas

### Observabilidade- ✅ Visualizar histórico de transferências

- **Serilog 9.0.0** - Logging estruturado- ✅ Informações de tarifa (R$ 2,00)

- **Serilog.Sinks.Seq** - Sink para Seq- ✅ Paginação de resultados

- **Serilog.Sinks.Console** - Sink para Console

- **prometheus-net 8.2.1** - Métricas Prometheus## 🎨 Tecnologias

- **Health Checks** - Monitoramento de saúde

  - SQLite, Redis, Kafka### Frontend

- **Blazor WebAssembly** (client-side)

### DevOps- **Bootstrap 5** (UI responsiva)

- **Docker & Docker Compose** - Containerização- **HttpClient** (comunicação com APIs)

- **Nginx** - Web server para Blazor- **JWT Authentication**

- **Seq** - Agregação de logs

- **Prometheus** - Métricas### Backend

- **Grafana** - Visualização- **.NET 9.0**

- **Entity Framework Core 9.0.10**

### Testes- **Dapper 2.1.66**

- **xUnit 2.9.3** - Framework de testes- **KafkaFlow 4.0.1** (opcional)

- **FluentAssertions 8.8.0** - Assertions legíveis- **SQLite**

- **Moq 4.20.72** - Mocking- **JWT Bearer Authentication**

- **Selenium WebDriver 4.27.0** - Testes E2E- **Swagger/OpenAPI**

- **coverlet** - Code coverage- **BCrypt.Net**

- **MediatR** (CQRS pattern)

---

## 🔧 Pré-requisitos

## ✨ Funcionalidades

- .NET 9.0 SDK

### 🔐 Autenticação e Segurança- Docker Desktop (opcional, para Kafka)

- ✅ Cadastro de conta com CPF e senha- PowerShell ou terminal compatível

- ✅ Login com CPF ou número da conta- Navegador web moderno

- ✅ JWT Token com refresh token

- ✅ Senha criptografada com BCrypt## 📦 Estrutura do Projeto

- ✅ CPF criptografado com AES-256-CBC

- ✅ Token **NÃO** contém dados sensíveis```

- ✅ LogoutBankMore/

├── src/

### 💳 Gestão de Conta│   ├── BankMore.Web/                    # 🆕 Interface Blazor WebAssembly

- ✅ Criar conta corrente│   │   ├── Models/                      # DTOs

- ✅ Consultar dados da conta│   │   ├── Services/                    # HTTP Services

- ✅ Visualizar saldo em tempo real│   │   ├── Pages/                       # Páginas Razor

- ✅ Ativar/desativar conta│   │   └── Layout/                      # Layout e Menu

- ✅ Histórico de movimentações│   ├── BankMore.ContaCorrente/         # API Conta Corrente

│   │   ├── Api/                         # Controllers e Program

### 💸 Movimentações│   │   ├── Application/                 # CQRS (MediatR)

- ✅ Crédito (depósito)│   │   ├── Domain/                      # Entidades e interfaces

- ✅ Débito (saque)│   │   └── Infrastructure/              # Repositórios e DbContext

- ✅ Extrato com paginação│   ├── BankMore.Transferencia/         # API Transferência

- ✅ Filtros por tipo e período│   │   ├── Api/                         # Controllers e Program

│   │   ├── Application/                 # CQRS (MediatR)

### 🔄 Transferências│   │   ├── Domain/                      # Entidades e interfaces

- ✅ Transferência entre contas│   │   └── Infrastructure/              # Repositórios Dapper

- ✅ Validação de saldo│   └── BankMore.Tarifas/               # Worker Tarifas

- ✅ Rollback automático em caso de falha│       ├── Handlers/                    # Event Handlers

- ✅ Tarifa de R$ 2,00 por transferência│       └── Services/                    # Business Services

- ✅ Histórico de transferências├── tests/                               # Testes automatizados

- ✅ Idempotência garantida├── start-all.ps1                        # 🆕 Script de inicialização

├── GUIA-EXECUCAO-WEB.md                # 🆕 Tutorial completo

### ⚙️ Processamento Assíncrono└── README.md                            # Este arquivo

- ✅ Worker que consome eventos Kafka```

- ✅ Persistência de tarifas no banco

- ✅ Débito automático de tarifas## 🧪 Fluxo de Teste Rápido

- ✅ Retry e dead letter queue

1. Execute `.\start-all.ps1`

### 📊 Observabilidade2. Acesse http://localhost:5000

- ✅ Logs estruturados (Serilog + Seq)3. Clique em "Criar Conta"

- ✅ Métricas Prometheus4. Cadastre-se com CPF, nome e senha

- ✅ Health Checks5. Faça login

- ✅ Dashboards Grafana (opcional)6. Adicione saldo (Crédito de R$ 1.000)

- ✅ Correlation ID para rastreamento7. Realize uma transferência

8. Verifique o extrato e histórico

### 🧪 Testes

- ✅ **41 testes unitários** (xUnit)## 🐛 Troubleshooting

  - CpfValidator (9 testes)

  - JwtService (16 testes)### Erro de CORS

  - Cobertura: 95%+- Certifique-se de que as APIs estão rodando

- ✅ **29 testes E2E** (Selenium)- CORS já está configurado nas APIs

  - Cadastro (9 testes)

  - Login (10 testes)### Token Expirado

  - Minha Conta (10 testes)- Faça logout e login novamente

- Tokens JWT têm validade de 24 horas

---

### Porta em Uso

## 📋 Pré-requisitos- Verifique se não há outros serviços nas portas 5000, 5003 ou 5004

- Use `netstat -ano | findstr :5000` para verificar

### Obrigatórios

- ✅ **Docker Desktop** instalado e rodando## 📝 Próximos Passos

- ✅ **Git** (para clonar o repositório)

- ✅ **Navegador Web** moderno (Chrome, Edge, Firefox)### Interface Web

- [ ] Página de consulta de tarifas

### Opcionais (para desenvolvimento)- [ ] Gráficos de movimentações

- ⚙️ **.NET 9.0 SDK** (para rodar fora do Docker)- [ ] Dark mode

- ⚙️ **Visual Studio 2022** ou **VS Code**- [ ] PWA (Progressive Web App)

- ⚙️ **PowerShell** (Windows) ou **Bash** (Linux/Mac)- [ ] Notificações toast

- [ ] Testes com bUnit

---

### APIs

## 🐋 Como Executar com Docker- [ ] Docker Compose completo (APIs + Kafka + Worker + Web)

- [ ] Testes unitários com xUnit

### 1️⃣ Clonar o Repositório- [ ] Testes de integração end-to-end

- [ ] Health checks e retry policies

```bash- [ ] Dead Letter Queue para mensagens falhadas

git clone https://github.com/seu-usuario/BankMore.git

cd BankMore---

```

**BankMore** - Sistema bancário completo com interface moderna 🏦✨

### 2️⃣ Subir Toda a Stack

```powershell

```bash# Criar docker-compose.yml na raiz do projeto (veja próxima seção)

# Buildar e iniciar todos os containersdocker-compose up -d

docker-compose up -d --build```



# Aguardar serviços iniciarem (~30 segundos)### 2️⃣ Executar as APIs

# No Windows (PowerShell):

Start-Sleep -Seconds 30```powershell

# Terminal 1 - API Conta Corrente

# No Linux/Mac:cd src\BancoDigitalAna.ContaCorrente

sleep 30dotnet run

```

# Terminal 2 - API Transferência

### 3️⃣ Verificar Status dos Containerscd src\BancoDigitalAna.Transferencia

dotnet run

```bash

docker-compose ps# Terminal 3 - Worker Tarifas

```cd src\BancoDigitalAna.Tarifas

dotnet run

**Saída esperada** (10 containers rodando):```



```### 3️⃣ Testar o Sistema

NAME                           STATUS          PORTS

bankmore-web-1                 Up             0.0.0.0:8080->80/tcpAcesse os Swaggers:

bankmore-api-conta-1           Up             0.0.0.0:5003->8080/tcp- **API Conta**: http://localhost:5003/swagger

bankmore-api-transferencia-1   Up             0.0.0.0:5004->8080/tcp- **API Transferência**: http://localhost:5004/swagger

bankmore-worker-tarifas-1      Up

kafka                          Up             0.0.0.0:9092->9092/tcp#### Fluxo Completo de Teste

zookeeper                      Up             0.0.0.0:2181->2181/tcp

redis                          Up             0.0.0.0:6379->6379/tcp```powershell

seq                            Up             0.0.0.0:5341->80/tcp# 1. Cadastrar conta origem

prometheus                     Up             0.0.0.0:9090->9090/tcpcurl -X POST http://localhost:5003/api/conta `

grafana                        Up             0.0.0.0:3000->3000/tcp  -H "Content-Type: application/json" `

```  -d '{

    "cpf": "12345678901",

### 4️⃣ Acessar o Sistema    "nome": "João Silva",

    "senha": "senha123"

| Interface | URL | Credenciais |  }'

|-----------|-----|-------------|

| **🌐 Aplicação Web** | http://localhost:8080 | - |# 2. Cadastrar conta destino

| **📖 API Conta (Swagger)** | http://localhost:5003 | - |curl -X POST http://localhost:5003/api/conta `

| **📖 API Transferência (Swagger)** | http://localhost:5004 | - |  -H "Content-Type: application/json" `

| **📊 Seq (Logs)** | http://localhost:5341 | - |  -d '{

| **📈 Prometheus** | http://localhost:9090 | - |    "cpf": "98765432100",

| **📊 Grafana** | http://localhost:3000 | admin/admin |    "nome": "Maria Santos",

| **🔴 Redis** | localhost:6379 | - |    "senha": "senha456"

| **📨 Kafka** | localhost:9092 | - |  }'



### 5️⃣ Fluxo de Teste Completo# 3. Fazer login

curl -X POST http://localhost:5003/api/auth/login `

#### **Passo 1: Cadastrar Conta**  -H "Content-Type: application/json" `

  -d '{

1. Acesse http://localhost:8080    "numeroContaOuCpf": "12345678901",

2. Clique em **"Criar Conta"**    "senha": "senha123"

3. Preencha:  }'

   - **CPF**: `12345678909` (válido)# Copie o token JWT retornado

   - **Nome**: `João Silva`

   - **Senha**: `senha123`# 4. Fazer uma movimentação de crédito (adicionar R$ 1000)

4. Clique em **"Criar Conta"**curl -X POST http://localhost:5003/api/movimentacao `

5. Anote o **número da conta** exibido  -H "Content-Type: application/json" `

  -H "Authorization: Bearer SEU_TOKEN_JWT" `

#### **Passo 2: Fazer Login**  -d '{

    "chaveIdempotencia": "mov-001",

1. Clique em **"Fazer Login"**    "tipoMovimento": "C",

2. Digite:    "valor": 1000.00

   - **Conta ou CPF**: `12345678909` (ou número da conta)  }'

   - **Senha**: `senha123`

3. Clique em **"Entrar"**# 5. Realizar transferência

curl -X POST http://localhost:5004/api/transferencia `

#### **Passo 3: Adicionar Saldo**  -H "Content-Type: application/json" `

  -H "Authorization: Bearer SEU_TOKEN_JWT" `

1. Na tela **"Minha Conta"**, clique em **"Adicionar Movimentação"**  -d '{

2. Selecione **"Crédito"**    "chaveIdempotencia": "trans-001",

3. Digite **R$ 1.000,00**    "idContaCorrenteDestino": "ID_CONTA_DESTINO",

4. Clique em **"Adicionar"**    "valor": 100.00

5. Verifique que o saldo foi atualizado  }'



#### **Passo 4: Criar Segunda Conta (Destino)**# 6. Consultar saldo (deve ter descontado R$ 100 + R$ 2 de tarifa)

curl -X GET http://localhost:5003/api/conta/saldo `

1. Faça **Logout**  -H "Authorization: Bearer SEU_TOKEN_JWT"

2. Crie uma nova conta com CPF diferente: `98765432100````

3. Anote o **número da conta destino**

## 📊 Bancos de Dados

#### **Passo 5: Realizar Transferência**

O sistema cria automaticamente 3 bancos SQLite:

1. Faça login novamente com a **primeira conta**

2. Vá para **"Transferências"**1. **contacorrente.db** - API Conta

3. Clique em **"Nova Transferência"**   - Tables: `contacorrente`, `movimento`, `idempotencia`

4. Preencha:

   - **Conta Destino**: (número da segunda conta)2. **transferencia.db** - API Transferência

   - **Valor**: `R$ 100,00`   - Tables: `transferencia`, `idempotencia`

5. Clique em **"Transferir"**

3. **tarifas.db** - Worker Tarifas

#### **Passo 6: Verificar Tarifa**   - Tables: `tarifa`



1. Vá para **"Minha Conta"**## ⚙️ Configurações

2. Verifique o saldo:

   - **Antes**: R$ 1.000,00### API Conta Corrente (`appsettings.json`)

   - **Depois**: R$ 898,00 (R$ 100 + R$ 2 de tarifa)

3. Consulte o **Extrato** para ver:```json

   - Débito de R$ 100,00 (transferência){

   - Débito de R$ 2,00 (tarifa)  "ConnectionStrings": {

    "DefaultConnection": "Data Source=contacorrente.db"

#### **Passo 7: Validar Logs no Seq**  },

  "Jwt": {

1. Acesse http://localhost:5341    "Key": "sua-chave-secreta-jwt-com-no-minimo-32-caracteres-para-seguranca",

2. Busque por:    "Issuer": "BancoDigitalAna",

   - `Transferência realizada`    "Audience": "BancoDigitalAna.Api"

   - `Tarifa debitada`  }

3. Verifique **Correlation ID** para rastreamento}

```

#### **Passo 8: Verificar Métricas no Prometheus**

### API Transferência (`appsettings.json`)

1. Acesse http://localhost:9090

2. Execute queries:```json

   ```promql{

   # Total de requisições HTTP  "ConnectionStrings": {

   http_requests_received_total    "DefaultConnection": "Data Source=transferencia.db"

     },

   # Duração das requisições  "ApiContaCorrente": {

   http_request_duration_seconds    "BaseUrl": "http://localhost:5003"

     },

   # Health checks  "Kafka": {

   health_check_status    "BootstrapServers": "localhost:9092"

   ```  },

  "Tarifa": {

### 6️⃣ Parar o Sistema    "Valor": 2.00

  }

```bash}

# Parar containers (preserva dados)```

docker-compose stop

### Worker Tarifas (`appsettings.json`)

# Parar e remover containers (limpa tudo)

docker-compose down```json

{

# Remover containers E volumes (apaga banco de dados)  "ConnectionStrings": {

docker-compose down -v    "DefaultConnection": "Data Source=tarifas.db"

```  },

  "Kafka": {

---    "BootstrapServers": "localhost:9092"

  },

## 🗂️ Estrutura do Projeto  "ApiContaCorrente": {

    "BaseUrl": "http://localhost:5003"

```  }

BankMore/}

├── 📁 src/```

│   ├── 📁 BankMore.Web/                      # Interface Blazor WebAssembly

│   │   ├── Pages/                            # Páginas Razor## 🔍 Logs e Monitoramento

│   │   │   ├── Cadastro.razor               # Tela de cadastro

│   │   │   ├── Login.razor                  # Tela de loginOs logs são exibidos no console de cada aplicação:

│   │   │   ├── MinhaConta.razor             # Dashboard da conta

│   │   │   └── Transferencias.razor         # Gestão de transferências- **API Conta**: Operações de conta, autenticação, movimentações

│   │   ├── Services/                         # HTTP Services- **API Transferência**: Transferências, rollbacks, publicação Kafka

│   │   │   ├── AuthService.cs               # Autenticação JWT- **Worker Tarifas**: Consumo de mensagens, persistência, débitos

│   │   │   ├── ContaService.cs              # Operações de conta

│   │   │   └── TokenService.cs              # Gerenciamento de tokens## 🐛 Troubleshooting

│   │   ├── Models/                           # DTOs

│   │   ├── Layout/                           # Layout e componentes### Kafka não conecta

│   │   ├── Dockerfile                        # Imagem Docker

│   │   └── nginx.conf                        # Configuração Nginx```powershell

│   │# Verificar se o Kafka está rodando

│   ├── 📁 BankMore.ContaCorrente/           # Microsserviço Contadocker ps | Select-String kafka

│   │   ├── Api/                              # Controllers e Program.cs

│   │   │   ├── Controllers/# Reiniciar containers

│   │   │   │   ├── ContaController.cs       # CRUD de contasdocker-compose restart

│   │   │   │   ├── AuthController.cs        # Login/Logout```

│   │   │   │   └── MovimentacaoController.cs # Movimentações

│   │   │   ├── Program.cs                   # Configuração da API### Worker não consome mensagens

│   │   │   └── Dockerfile                   # Imagem Docker

│   │   ├── Application/                      # CQRS (MediatR)- Verificar se o tópico `transferencias-realizadas` existe

│   │   │   ├── Handlers/                    # Command/Query Handlers- Conferir `BootstrapServers` no `appsettings.json`

│   │   │   ├── Services/- Checar logs do Worker para erros de conexão

│   │   │   │   ├── JwtService.cs            # Geração JWT

│   │   │   │   ├── CpfValidator.cs          # Validação CPF### Tarifa não é debitada

│   │   │   │   └── EncryptionService.cs     # AES-256 + BCrypt

│   │   │   └── Validators/                  # FluentValidation- Verificar se o Worker está rodando

│   │   ├── Domain/                           # Entidades e interfaces- Conferir URL da API Conta no Worker

│   │   │   ├── Entities/- Validar que a conta tem saldo suficiente

│   │   │   │   ├── ContaCorrente.cs- Verificar idempotência (transferência já processada)

│   │   │   │   ├── Movimento.cs

│   │   │   │   └── IdempotenciaChave.cs## 📝 Próximos Passos

│   │   │   └── Interfaces/                  # Repositórios

│   │   └── Infrastructure/                   # EF Core- [ ] Docker Compose completo (APIs + Kafka + Worker)

│   │       ├── Data/- [ ] Testes unitários com xUnit

│   │       │   └── AppDbContext.cs          # DbContext- [ ] Testes de integração end-to-end

│   │       └── Repositories/                # Implementações- [ ] Health checks e retry policies

│   │- [ ] Dead Letter Queue para mensagens falhadas

│   ├── 📁 BankMore.Transferencia/           # Microsserviço Transferência- [ ] Autenticação service-to-service (Worker → API Conta)

│   │   ├── Api/

│   │   │   ├── Controllers/## 📚 Tecnologias Utilizadas

│   │   │   │   └── TransferenciaController.cs

│   │   │   ├── Program.cs- **.NET 10.0** (preview)

│   │   │   └── Dockerfile- **Entity Framework Core 9.0.10**

│   │   ├── Application/- **Dapper 2.1.66**

│   │   │   ├── Handlers/- **KafkaFlow 4.0.1**

│   │   │   │   └── RealizarTransferenciaHandler.cs- **SQLite**

│   │   │   └── Services/- **JWT Bearer Authentication**

│   │   │       ├── ContaCorrenteHttpService.cs # HTTP Client- **Swagger/OpenAPI**

│   │   │       └── KafkaProducerService.cs     # Kafka Producer- **BCrypt.Net** (hashing de senhas)

│   │   ├── Domain/- **MediatR** (CQRS pattern)

│   │   │   └── Entities/

│   │   │       └── Transferencia.cs---

│   │   └── Infrastructure/

│   │       └── Repositories/                # Dapper**Banco Digital Ana** - Sistema de microsserviços com processamento de tarifas em tempo real 🏦✨

│   │
│   └── 📁 BankMore.Tarifas/                 # Worker Tarifas
│       ├── Worker.cs                         # Background Service
│       ├── Handlers/
│       │   └── TransferenciaRealizadaHandler.cs
│       ├── Services/
│       │   ├── TarifaService.cs             # Lógica de negócio
│       │   └── ContaHttpService.cs          # HTTP Client
│       ├── Data/
│       │   └── TarifasDbContext.cs
│       ├── Dockerfile
│       └── Program.cs
│
├── 📁 tests/
│   ├── 📁 BankMore.ContaCorrente.Tests/     # Testes Unitários
│   │   ├── Services/
│   │   │   ├── CpfValidatorTests.cs         # 9 testes
│   │   │   └── JwtServiceTests.cs           # 16 testes
│   │   └── README-TESTES.md                 # Documentação
│   │
│   └── 📁 BankMore.Web.E2ETests/            # Testes E2E (Selenium)
│       ├── Infrastructure/
│       │   └── SeleniumTestBase.cs          # Base class com helpers
│       ├── PageObjects/
│       │   ├── CadastroPage.cs              # Page Object: Cadastro
│       │   ├── LoginPage.cs                 # Page Object: Login
│       │   └── MinhaContaPage.cs            # Page Object: Minha Conta
│       ├── Tests/
│       │   ├── CadastroE2ETests.cs          # 9 testes E2E
│       │   ├── LoginE2ETests.cs             # 10 testes E2E
│       │   └── MinhaContaE2ETests.cs        # 10 testes E2E
│       └── README.md                         # Documentação
│
├── 📁 sql/                                   # Scripts SQL
│   ├── contacorrente.sql                    # Schema conta
│   ├── transferencia.sql                    # Schema transferência
│   ├── tarifas.sql                          # Schema tarifas
│   ├── refresh_token.sql                    # Tokens
│   └── outbox_events.sql                    # Outbox pattern
│
├── 📁 especificacao/                         # Documentação técnica
│   ├── RESUMO-IMPLEMENTACAO-COMPLETA.md     # Resumo completo (8000+ linhas)
│   ├── ESTRUTURA.md                          # Arquitetura
│   └── teste-desevolvedor-csharp-api.md     # Guia de desenvolvimento
│
├── 📄 docker-compose.yml                     # Orquestração Docker (10 serviços)
├── 📄 prometheus.yml                         # Configuração Prometheus
├── 📄 README.md                              # Este arquivo
├── 📄 VERSION.md                             # Controle de versão
├── 📄 CHANGELOG.md                           # Histórico de mudanças
│
└── 📁 Scripts PowerShell/
    ├── start-all.ps1                         # Inicia todos os serviços
    ├── docker-start.ps1                      # Inicia Docker Compose
    ├── docker-check.ps1                      # Verifica containers
    ├── test-api.ps1                          # Testa APIs
    └── version-info.ps1                      # Informações de versão
```

### 📊 Estatísticas do Projeto

- **Total de Arquivos C#**: ~80 arquivos
- **Linhas de Código**: ~15.000+ linhas
- **Testes Unitários**: 41 testes (95%+ cobertura)
- **Testes E2E**: 29 testes (Selenium)
- **Microsserviços**: 3 (Conta, Transferência, Tarifas)
- **Containers Docker**: 10 serviços
- **Endpoints REST**: 25+ endpoints
- **Documentação**: 10.000+ linhas

---

## 📊 Observabilidade

### 📝 Logs Estruturados (Serilog + Seq)

#### Acessar Seq
- **URL**: http://localhost:5341
- **Funcionalidades**:
  - Busca full-text
  - Filtros por nível (Info, Warning, Error)
  - Correlation ID para rastreamento
  - Agregações e estatísticas

#### Exemplos de Queries no Seq

```sql
-- Todas as transferências realizadas
@MessageTemplate = "Transferência realizada"

-- Erros nas últimas 24h
@Level = "Error" and @Timestamp > Now() - 1d

-- Operações de uma conta específica
NumeroContaCorrente = "12345"

-- Rastrear uma requisição completa
CorrelationId = "abc-123-def"
```

### 📈 Métricas (Prometheus)

#### Acessar Prometheus
- **URL**: http://localhost:9090
- **Métricas Disponíveis**:
  - `http_requests_received_total` - Total de requisições HTTP
  - `http_request_duration_seconds` - Duração das requisições
  - `process_cpu_seconds_total` - Uso de CPU
  - `process_working_set_bytes` - Memória utilizada
  - `health_check_status` - Status dos health checks

#### Exemplos de Queries PromQL

```promql
# Taxa de requisições por segundo (últimos 5 minutos)
rate(http_requests_received_total[5m])

# Percentil 95 de duração de requisições
histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m]))

# Total de requisições com erro (5xx)
sum(rate(http_requests_received_total{code=~"5.."}[5m]))

# Health checks com falha
health_check_status{status="Unhealthy"}
```

### 🏥 Health Checks

#### Endpoints Disponíveis

| Endpoint | Descrição |
|----------|-----------|
| `/health` | Health check geral (aggregate) |
| `/health/ready` | Readiness probe (Kubernetes) |
| `/health/live` | Liveness probe (Kubernetes) |

#### Exemplo de Resposta

```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.523",
  "entries": {
    "sqlite": {
      "status": "Healthy",
      "duration": "00:00:00.123"
    },
    "redis": {
      "status": "Healthy",
      "duration": "00:00:00.089"
    },
    "kafka": {
      "status": "Healthy",
      "duration": "00:00:00.311"
    }
  }
}
```

### 📊 Grafana (Opcional)

#### Acessar Grafana
- **URL**: http://localhost:3000
- **Credenciais**: `admin` / `admin`

#### Configurar Datasource
1. Acesse **Configuration** → **Data Sources**
2. Clique em **Add data source**
3. Selecione **Prometheus**
4. Configure URL: `http://prometheus:9090`
5. Clique em **Save & Test**

#### Importar Dashboards
1. Acesse **Dashboards** → **Import**
2. Use IDs de dashboards públicos:
   - **ASP.NET Core**: ID `10915`
   - **Prometheus**: ID `2`
   - **Node Exporter**: ID `1860`

---

## 🧪 Testes

### Testes Unitários (xUnit)

#### Executar Testes Unitários

```bash
# Navegar para o projeto de testes
cd tests/BankMore.ContaCorrente.Tests

# Executar todos os testes
dotnet test

# Executar com cobertura
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

# Executar apenas testes de CPF
dotnet test --filter "FullyQualifiedName~CpfValidatorTests"

# Executar apenas testes de JWT
dotnet test --filter "FullyQualifiedName~JwtServiceTests"
```

#### Testes Implementados

**CpfValidatorTests** (9 testes)
- ✅ Validar CPF válido
- ✅ Rejeitar CPF inválido
- ✅ Rejeitar CPF com dígitos repetidos
- ✅ Aceitar CPF formatado (`123.456.789-09`)
- ✅ Performance: 1000 validações < 100ms

**JwtServiceTests** (16 testes)
- ✅ Gerar Access Token com claims obrigatórias
- ✅ **NÃO** incluir dados sensíveis no token
- ✅ Gerar Refresh Token criptograficamente seguro
- ✅ Validar token válido/inválido/expirado
- ✅ Hash SHA-256 determinístico

#### Cobertura Atual
- **CpfValidator**: 95.45%
- **JwtService**: 100%

### Testes E2E (Selenium)

#### Pré-requisitos
- Aplicação rodando em `http://localhost:8080`
- Chrome instalado

#### Executar Testes E2E

```bash
# Navegar para o projeto de testes E2E
cd tests/BankMore.Web.E2ETests

# Executar todos os testes E2E
dotnet test

# Executar com verbosidade
dotnet test --verbosity detailed

# Executar apenas testes de Cadastro
dotnet test --filter "FullyQualifiedName~CadastroE2ETests"

# Executar apenas testes de Login
dotnet test --filter "FullyQualifiedName~LoginE2ETests"

# Executar apenas testes de Minha Conta
dotnet test --filter "FullyQualifiedName~MinhaContaE2ETests"
```

#### Testes Implementados

**CadastroE2ETests** (9 testes)
- ✅ Criar conta com dados válidos
- ✅ Redirecionar para login após cadastro
- ✅ Validar erro com CPF inválido
- ✅ Validar erro com CPF duplicado

**LoginE2ETests** (10 testes)
- ✅ Login com número da conta
- ✅ Login com CPF
- ✅ Erro com credenciais inválidas
- ✅ Aceitar CPF ou número no mesmo campo

**MinhaContaE2ETests** (10 testes)
- ✅ Exibir dados da conta após login
- ✅ Exibir saldo atualizado
- ✅ Manter sessão entre páginas

#### Executar Testes E2E com Docker

```bash
# 1. Subir aplicação
docker-compose up -d

# 2. Aguardar serviços
sleep 30

# 3. Executar testes E2E
cd tests/BankMore.Web.E2ETests
dotnet test

# 4. Parar aplicação
cd ../..
docker-compose down
```

### Relatórios de Testes

#### Gerar Relatório de Cobertura

```bash
# Gerar coverage XML
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

# Instalar ReportGenerator (primeira vez)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Gerar relatório HTML
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coveragereport -reporttypes:Html

# Abrir relatório no navegador
# Windows:
start coveragereport/index.html

# Linux/Mac:
open coveragereport/index.html
```

---

## 🌐 APIs Disponíveis

### API Conta Corrente (Port 5003)

#### Autenticação

```http
POST /api/auth/login
Content-Type: application/json

{
  "numeroContaOuCpf": "12345678909",
  "senha": "senha123"
}

Response 200 OK:
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "abc123...",
  "expiresIn": 86400
}
```

#### Cadastrar Conta

```http
POST /api/conta
Content-Type: application/json

{
  "cpf": "12345678909",
  "nome": "João Silva",
  "senha": "senha123"
}

Response 201 Created:
{
  "id": "guid-123",
  "numeroContaCorrente": "12345",
  "cpf": "***.***.***-09",
  "nome": "João Silva",
  "ativo": true,
  "dataCriacao": "2025-01-15T10:30:00Z"
}
```

#### Consultar Saldo

```http
GET /api/conta/saldo
Authorization: Bearer {token}

Response 200 OK:
{
  "numeroContaCorrente": "12345",
  "saldo": 1000.00,
  "dataConsulta": "2025-01-15T10:30:00Z"
}
```

#### Criar Movimentação

```http
POST /api/movimentacao
Authorization: Bearer {token}
Content-Type: application/json

{
  "chaveIdempotencia": "mov-001",
  "tipoMovimento": "C",
  "valor": 1000.00
}

Response 201 Created:
{
  "id": "guid-456",
  "tipoMovimento": "C",
  "valor": 1000.00,
  "dataMovimento": "2025-01-15T10:30:00Z"
}
```

### API Transferência (Port 5004)

#### Realizar Transferência

```http
POST /api/transferencia
Authorization: Bearer {token}
Content-Type: application/json

{
  "chaveIdempotencia": "trans-001",
  "idContaCorrenteDestino": "guid-destino",
  "valor": 100.00
}

Response 201 Created:
{
  "id": "guid-789",
  "idContaCorrenteOrigem": "guid-origem",
  "idContaCorrenteDestino": "guid-destino",
  "valor": 100.00,
  "tarifa": 2.00,
  "dataTransferencia": "2025-01-15T10:30:00Z",
  "status": "Realizada"
}
```

#### Consultar Transferências

```http
GET /api/transferencia?pagina=1&tamanhoPagina=10
Authorization: Bearer {token}

Response 200 OK:
{
  "items": [...],
  "paginaAtual": 1,
  "tamanhoPagina": 10,
  "totalItens": 50,
  "totalPaginas": 5
}
```

### Swagger/OpenAPI

Acesse a documentação interativa:
- **API Conta**: http://localhost:5003/swagger
- **API Transferência**: http://localhost:5004/swagger

---

## 🐛 Troubleshooting

### Problema: Containers não iniciam

```bash
# Verificar logs
docker-compose logs

# Verificar logs de um serviço específico
docker-compose logs api-conta

# Reiniciar serviços
docker-compose restart

# Rebuild completo
docker-compose down -v
docker-compose up -d --build
```

### Problema: Kafka não conecta

```bash
# Verificar se Kafka e Zookeeper estão rodando
docker-compose ps kafka zookeeper

# Restart Kafka
docker-compose restart kafka zookeeper

# Aguardar Kafka inicializar completamente
sleep 30
```

### Problema: Worker não consome mensagens

**Sintomas**: Transferências realizadas mas tarifas não debitadas

**Soluções**:
1. Verificar se o Worker está rodando:
   ```bash
   docker-compose ps worker-tarifas
   ```

2. Verificar logs do Worker:
   ```bash
   docker-compose logs worker-tarifas
   ```

3. Verificar se o tópico Kafka existe:
   ```bash
   docker exec -it kafka kafka-topics.sh --list --bootstrap-server localhost:9092
   ```

4. Verificar conectividade Worker → Kafka:
   ```bash
   docker-compose logs worker-tarifas | grep -i kafka
   ```

### Problema: API retorna 401 Unauthorized

**Causa**: Token JWT expirado ou inválido

**Solução**:
1. Fazer logout no frontend
2. Fazer login novamente
3. Verificar se o token está sendo enviado no header `Authorization: Bearer {token}`

### Problema: Erro de CORS no Frontend

**Sintomas**: Console do navegador mostra erro de CORS

**Solução**:
1. Verificar se as APIs estão rodando
2. CORS já está configurado nas APIs para aceitar `http://localhost:8080`
3. Se usar porta diferente, atualizar configuração CORS nas APIs

### Problema: Seq não mostra logs

```bash
# Verificar se Seq está rodando
docker-compose ps seq

# Verificar URL do Seq nas APIs
docker-compose logs api-conta | grep -i seq

# Acessar Seq e verificar filtros
# URL: http://localhost:5341
```

### Problema: Prometheus não coleta métricas

```bash
# Verificar targets no Prometheus
# Acesse: http://localhost:9090/targets
# Status deve ser "UP"

# Se status "DOWN", verificar endpoints /metrics das APIs
curl http://localhost:5003/metrics
curl http://localhost:5004/metrics
```

### Problema: Portas em uso

```bash
# Windows (PowerShell)
netstat -ano | findstr :8080
netstat -ano | findstr :5003
netstat -ano | findstr :5004

# Linux/Mac
lsof -i :8080
lsof -i :5003
lsof -i :5004

# Matar processo
# Windows
taskkill /PID <PID> /F

# Linux/Mac
kill -9 <PID>
```

### Problema: Banco de dados corrompido

```bash
# Remover volumes e recriar
docker-compose down -v
docker-compose up -d --build

# ⚠️ ATENÇÃO: Isso apaga todos os dados!
```

### Problema: Testes E2E falhando

**Soluções**:
1. Verificar se aplicação está rodando: `curl http://localhost:8080`
2. Verificar se Chrome está instalado
3. Aumentar timeouts em `SeleniumTestBase.cs`
4. Executar em modo não-headless para debug (comentar `--headless`)

---

## 📚 Documentação Adicional

### Documentos Técnicos

- **[RESUMO-IMPLEMENTACAO-COMPLETA.md](especificacao/RESUMO-IMPLEMENTACAO-COMPLETA.md)** - Documentação completa de 8.000+ linhas
- **[ESTRUTURA.md](especificacao/ESTRUTURA.md)** - Arquitetura detalhada
- **[README-TESTES.md](tests/README-TESTES.md)** - Documentação de testes unitários
- **[README E2E](tests/BankMore.Web.E2ETests/README.md)** - Documentação de testes E2E

### Diagramas

#### Fluxo de Transferência

```
1. Cliente → API Transferência: POST /api/transferencia
2. API Transferência valida dados
3. API Transferência → API Conta (HTTP): Débito na origem
4. API Conta verifica saldo e debita
5. API Transferência → API Conta (HTTP): Crédito no destino
6. Se falha: rollback do débito (idempotência)
7. API Transferência → Kafka: Publish TransferenciaRealizada
8. API Transferência → Cliente: Response 201 Created
9. Worker Tarifas ← Kafka: Consume TransferenciaRealizada
10. Worker Tarifas persiste tarifa no banco
11. Worker Tarifas → API Conta (HTTP): Débito da tarifa
```

#### Fluxo de Autenticação

```
1. Cliente → API Conta: POST /api/auth/login {cpf, senha}
2. API Conta valida credenciais
3. API Conta verifica senha (BCrypt)
4. API Conta gera JWT Access Token (10min)
5. API Conta gera Refresh Token (1 dia)
6. API Conta → Cliente: {accessToken, refreshToken}
7. Cliente armazena tokens no LocalStorage
8. Cliente → API: Requisições com Authorization: Bearer {accessToken}
9. API valida JWT em cada requisição
10. Se token expirado: usar refresh token
```

---

## 🤝 Contribuindo

### Como Contribuir

1. **Fork** o repositório
2. Crie uma **branch** para sua feature (`git checkout -b feature/MinhaFeature`)
3. **Commit** suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)
4. **Push** para a branch (`git push origin feature/MinhaFeature`)
5. Abra um **Pull Request**

### Guidelines

- ✅ Siga os padrões de código existentes
- ✅ Adicione testes para novas funcionalidades
- ✅ Atualize a documentação
- ✅ Use commits semânticos (Conventional Commits)

### Conventional Commits

```
feat: adiciona nova funcionalidade
fix: corrige bug
docs: atualiza documentação
test: adiciona ou corrige testes
refactor: refatora código sem mudar comportamento
perf: melhora performance
chore: tarefas de manutenção
```

---

## 📄 Licença

Este projeto é um **sistema de demonstração educacional** desenvolvido para fins de aprendizado e portfólio.

---

## 👨‍💻 Autor

**Desenvolvido com ❤️ usando:**
- .NET 9.0
- Blazor WebAssembly
- Docker & Docker Compose
- Apache Kafka
- Serilog + Seq + Prometheus + Grafana
- xUnit + Selenium WebDriver

---

## 🎯 Roadmap

### V1.0 ✅ (Atual)
- [x] Interface Blazor WebAssembly
- [x] APIs RESTful (Conta + Transferência)
- [x] Worker de Tarifas
- [x] Observabilidade completa
- [x] Testes unitários e E2E
- [x] Docker Compose

### V1.1 🚧 (Em Desenvolvimento)
- [ ] Autenticação OAuth2
- [ ] API Gateway (Ocelot)
- [ ] Circuit Breaker (Polly)
- [ ] Outbox Pattern
- [ ] Saga Pattern

### V2.0 📋 (Planejado)
- [ ] Kubernetes (Helm Charts)
- [ ] CI/CD (GitHub Actions)
- [ ] Testes de carga (k6)
- [ ] Documentação OpenAPI 3.0
- [ ] Webhooks

---

## 📞 Suporte

- **Issues**: [GitHub Issues](https://github.com/seu-usuario/BankMore/issues)
- **Discussões**: [GitHub Discussions](https://github.com/seu-usuario/BankMore/discussions)
- **Email**: seu-email@exemplo.com

---

<div align="center">

### 🏦 BankMore - Sistema Bancário Moderno 🚀

**[⬆️ Voltar ao topo](#-bankmore---sistema-bancário-completo)**

---

Made with ❤️ and ☕ by **[Seu Nome]**

[![Docker](https://img.shields.io/badge/Docker-Ready-blue?logo=docker)](https://www.docker.com/)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple?logo=dotnet)](https://dotnet.microsoft.com/)
[![Blazor](https://img.shields.io/badge/Blazor-WebAssembly-blueviolet?logo=blazor)](https://blazor.net/)
[![Kafka](https://img.shields.io/badge/Kafka-4.0-black?logo=apache-kafka)](https://kafka.apache.org/)
[![Tests](https://img.shields.io/badge/Tests-70%20passing-brightgreen?logo=xunit)](tests/)

</div>
