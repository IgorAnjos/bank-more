# 🏦 BankMore - Sistema Bancário Digital# 🏦 BankMore - Sistema Bancário Completo# BankMore - Sistema Bancário Completo



[![.NET](https://img.shields.io/badge/.NET-9.0-purple?logo=dotnet)](https://dotnet.microsoft.com/)

[![Blazor](https://img.shields.io/badge/Blazor-WebAssembly-blueviolet?logo=blazor)](https://blazor.net/)

[![Docker](https://img.shields.io/badge/Docker-Ready-blue?logo=docker)](https://www.docker.com/)Sistema bancário digital com **arquitetura de microsserviços**, **comunicação assíncrona via Kafka**, **processamento de tarifas em tempo real**, **interface web moderna com Blazor WebAssembly** e **observabilidade completa** (Serilog + Seq + Prometheus + Grafana).Sistema bancário com arquitetura de microsserviços, comunicação assíncrona via Kafka, processamento de tarifas em tempo real e interface web moderna com Blazor WebAssembly.

[![Kafka](https://img.shields.io/badge/Kafka-4.0-black?logo=apache-kafka)](https://kafka.apache.org/)

[![Tests](https://img.shields.io/badge/Tests-70%20passing-brightgreen?logo=xunit)](tests/)

[![License](https://img.shields.io/badge/License-Educational-yellow)](LICENSE)

---## 🏗️ Arquitetura

Sistema bancário completo com **arquitetura de microsserviços**, **comunicação assíncrona via Kafka**, **processamento de tarifas em tempo real**, **interface web moderna com Blazor WebAssembly** e **observabilidade completa** (Serilog + Seq + Prometheus + Grafana).



---

## 📑 Índice```

## 📑 Índice

┌─────────────────────┐

- [🎯 Visão Geral](#-visão-geral)

- [🏗️ Arquitetura](#️-arquitetura)- [Arquitetura](#-arquitetura)│   Blazor WebAssembly│

- [🚀 Tecnologias](#-tecnologias)

- [✨ Funcionalidades](#-funcionalidades)- [Tecnologias](#-tecnologias)│   (Interface Web)   │

- [📋 Pré-requisitos](#-pré-requisitos)

- [🐋 Como Executar](#-como-executar-com-docker)- [Funcionalidades](#-funcionalidades)│    Port: 5000       │

- [🧪 Testes](#-testes)

- [📊 Observabilidade](#-observabilidade)- [Pré-requisitos](#-pré-requisitos)└──────────┬──────────┘

- [🌐 APIs Disponíveis](#-apis-disponíveis)

- [🗂️ Estrutura do Projeto](#️-estrutura-do-projeto)- [Como Executar](#-como-executar-com-docker)           │ HTTP/JWT

- [🐛 Troubleshooting](#-troubleshooting)

- [🤝 Contribuindo](#-contribuindo)- [Estrutura do Projeto](#-estrutura-do-projeto)           ├──────────────────────┐



---- [Observabilidade](#-observabilidade)           ▼                      ▼



## 🎯 Visão Geral- [Testes](#-testes)┌─────────────────┐      HTTP      ┌──────────────────────┐



O **BankMore** é um sistema bancário digital moderno desenvolvido para demonstrar práticas avançadas de desenvolvimento de software, incluindo:- [APIs Disponíveis](#-apis-disponíveis)│   API Conta     │ ◄───────────── │ API Transferência    │



- **Arquitetura de Microsserviços** com segregação de responsabilidades- [Troubleshooting](#-troubleshooting)│   Corrente      │                │                      │

- **Event-Driven Architecture** com Apache Kafka

- **CQRS Pattern** com MediatR│  (EF Core)      │                │     (Dapper)         │

- **Observabilidade Completa** (Logs, Métricas, Health Checks)

- **Segurança Robusta** (JWT, BCrypt, AES-256)---│  Port: 5003     │                │    Port: 5004        │

- **Testes Abrangentes** (Unitários + E2E)

- **Containerização** com Docker Compose└─────────────────┘                └──────────────────────┘



### 📊 Estatísticas do Projeto## 🏗️ Arquitetura        ▲                                    │



```        │                                    │ publish

📦 3 Microsserviços            🧪 70 Testes (41 Unit + 29 E2E)

🌐 1 Frontend Blazor           📄 80.000+ linhas de código```        │ HTTP (débito tarifa)               ▼

🐋 10 Containers Docker        📚 10.000+ linhas de documentação

📨 1 Message Queue (Kafka)     ⚡ 95%+ de cobertura de testes┌─────────────────────────────────────────────────────────────┐        │                          ┌─────────────────────┐

🔍 3 Ferramentas Observabilidade

```│                    CAMADA DE APRESENTAÇÃO                   │┌───────┴──────────┐               │   Kafka Topic:      │



---│                                                               ││  Worker Tarifas  │ ◄─────────── │ transferencias-     │



## 🏗️ Arquitetura│  ┌─────────────────────┐      ┌─────────────────────────┐   ││   (Consumer)     │   consume     │    realizadas       │



### Diagrama Geral│  │  Blazor WebAssembly │      │     Swagger/OpenAPI     │   │└──────────────────┘               └─────────────────────┘



```│  │   (Interface Web)   │      │   (Documentação APIs)   │   │```

┌──────────────────────────────────────────────────────────────────┐

│                     CAMADA DE APRESENTAÇÃO                       ││  │    Port: 8080       │      │   Ports: 5003/5004      │   │

│                                                                  │

│  ┌─────────────────────┐       ┌──────────────────────────┐    ││  └──────────┬──────────┘      └─────────────────────────┘   │### Componentes

│  │  Blazor WebAssembly │       │    Swagger/OpenAPI       │    │

│  │   (Interface Web)   │       │  (Documentação APIs)     │    │└─────────────┼──────────────────────────────────────────────┘

│  │    Port: 8080       │       │   Ports: 5003/5004       │    │

│  └──────────┬──────────┘       └──────────────────────────┘    │              │ HTTP/REST + JWT1. **Interface Web Blazor** (`BankMore.Web`) 🆕

└─────────────┼────────────────────────────────────────────────────┘

              │ HTTP/REST + JWT┌─────────────┼──────────────────────────────────────────────┐   - Interface de usuário moderna com Blazor WebAssembly

┌─────────────┼────────────────────────────────────────────────────┐

│             │             CAMADA DE SERVIÇOS                     ││             │          CAMADA DE SERVIÇOS                   │   - Autenticação JWT com LocalStorage

│             ├────────────────────────┐                           │

│             ▼                        ▼                           ││             ├──────────────────────┐                        │   - Funcionalidades: Login, Cadastro, Consulta de Conta, Movimentações e Transferências

│   ┌─────────────────┐        ┌──────────────────────┐           │

│   │   API Conta     │◄──────►│ API Transferência    │           ││             ▼                      ▼                        │   - Design responsivo com Bootstrap 5

│   │   Corrente      │  HTTP  │                      │           │

│   │  (EF Core)      │        │     (Dapper)         │           ││   ┌─────────────────┐      ┌──────────────────────┐        │

│   │  Port: 5003     │        │    Port: 5004        │           │

│   └────────┬────────┘        └──────────┬───────────┘           ││   │   API Conta     │◄────►│ API Transferência    │        │2. **API Conta Corrente** (`BankMore.ContaCorrente`)

└────────────┼──────────────────────────────┼─────────────────────┘

             │                              ││   │   Corrente      │ HTTP │                      │        │   - Gerencia contas, autenticação (JWT), movimentações e saldo

             │ HTTP (débito tarifa)         │ publish event

             │                              ▼│   │  (EF Core)      │      │     (Dapper)         │        │   - Entity Framework Core + SQLite

┌────────────┼──────────────────────────────────────────────────────┐

│            │                  MENSAGERIA                           ││   │  Port: 5003     │      │    Port: 5004        │        │   - RESTful com HATEOAS, versionamento e Problem Details

│            │         ┌─────────────────────────┐                  │

│            │         │   Apache Kafka          │                  ││   └────────┬────────┘      └──────────┬───────────┘        │   - CORS habilitado para frontend

│            │         │   Topic: transferencias-│                  │

│            │         │        realizadas       │                  │└────────────┼────────────────────────────┼──────────────────┘

│            │         │    Port: 9092           │                  │

│            │         └──────────┬──────────────┘                  │             │                            │3. **API Transferência** (`BankMore.Transferencia`)

└────────────┼────────────────────┼───────────────────────────────┘

             │                    │ consume             │ HTTP (débito tarifa)       │ publish event   - Processa transferências entre contas com rollback automático

             │                    ▼

┌────────────┼────────────────────────────────────────────────────┐             │                            ▼   - Dapper (raw SQL) + SQLite

│            │          CAMADA DE PROCESSAMENTO                   │

│            │         ┌──────────────────┐                       │┌────────────┼────────────────────────────────────────────────┐   - Kafka Producer: publica eventos de transferências realizadas

│            └────────►│  Worker Tarifas  │                       │

│                      │   (Background    │                       ││            │             MENSAGERIA                          │   - Integração HTTP com API Conta Corrente

│                      │    Service)      │                       │

│                      └──────────────────┘                       ││            │      ┌─────────────────────────┐               │   - CORS habilitado para frontend

└─────────────────────────────────────────────────────────────────┘

│            │      │   Apache Kafka          │               │

┌─────────────────────────────────────────────────────────────────┐

│                    CAMADA DE DADOS (SQLite)                     ││            │      │   Topic: transferencias-│               │4. **Worker Tarifas** (`BankMore.Tarifas`)

│                                                                 │

│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐         ││            │      │        realizadas       │               │   - Background Service que consome eventos do Kafka

│  │contacorrente │  │transferencia │  │   tarifas    │         │

│  │     .db      │  │     .db      │  │     .db      │         ││            │      │    Port: 9092           │               │   - Persiste tarifas no banco de dados

│  └──────────────┘  └──────────────┘  └──────────────┘         │

└─────────────────────────────────────────────────────────────────┘│            │      └──────────┬──────────────┘               │   - Debita automaticamente tarifas na conta origem



┌─────────────────────────────────────────────────────────────────┐└────────────┼─────────────────┼──────────────────────────────┘   - Idempotência garantida por `idtransferencia`

│                CAMADA DE OBSERVABILIDADE                        │

│                                                                 │             │                 │ consume

│  ┌─────────┐  ┌──────────┐  ┌──────────┐  ┌────────────┐     │

│  │ Serilog │─►│   Seq    │  │Prometheus│─►│  Grafana   │     │             │                 ▼## 🚀 Como Executar

│  │  Logs   │  │ (5341)   │  │  (9090)  │  │   (3000)   │     │

│  └─────────┘  └──────────┘  └──────────┘  └────────────┘     │┌────────────┼─────────────────────────────────────────────────┐

└─────────────────────────────────────────────────────────────────┘

```│            │       CAMADA DE PROCESSAMENTO                   │### Opção 1: Script Automático (Recomendado) ⚡



### 📦 Componentes│            │      ┌──────────────────┐                       │



| Componente | Tecnologia | Porta | Descrição |│            └─────►│  Worker Tarifas  │                       │Execute o script PowerShell que inicia todos os serviços automaticamente:

|------------|-----------|-------|-----------|

| **Blazor Web** | WebAssembly | 8080 | Interface de usuário moderna e responsiva |│                   │   (Background    │                       │

| **API Conta** | ASP.NET Core + EF Core | 5003 | Gerencia contas, autenticação JWT, movimentações |

| **API Transferência** | ASP.NET Core + Dapper | 5004 | Processa transferências com rollback automático |│                   │    Service)      │                       │```powershell

| **Worker Tarifas** | Background Service | - | Consome eventos Kafka e debita tarifas |

| **Kafka** | Apache Kafka | 9092 | Mensageria assíncrona |│                   └──────────────────┘                       │cd c:\GitHub\Teste\BankMore

| **Zookeeper** | Apache Zookeeper | 2181 | Coordenação do Kafka |

| **Redis** | Redis Cache | 6379 | Idempotência e cache distribuído |└──────────────────────────────────────────────────────────────┘.\start-all.ps1

| **Seq** | Seq Logs | 5341 | Agregação e visualização de logs |

| **Prometheus** | Prometheus | 9090 | Coleta de métricas |```

| **Grafana** | Grafana | 3000 | Dashboards e visualizações |

┌──────────────────────────────────────────────────────────────┐

---

│                CAMADA DE DADOS (SQLite)                      │O script irá:

## 🚀 Tecnologias

│                                                               │1. Iniciar API Conta Corrente (porta 5003)

### Backend

- **.NET 9.0** - Framework principal│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐       │2. Iniciar API Transferência (porta 5004)

- **ASP.NET Core** - APIs RESTful

- **Entity Framework Core 9.0.10** - ORM para API Conta│  │contacorrente │  │transferencia │  │   tarifas    │       │3. Iniciar Interface Web (porta 5000/5001)

- **Dapper 2.1.66** - Micro-ORM para API Transferência

- **MediatR 13.1.0** - CQRS pattern│  │     .db      │  │     .db      │  │     .db      │       │4. Abrir o navegador automaticamente

- **KafkaFlow 4.0.1** - Cliente Kafka

- **BCrypt.Net 4.0.3** - Hashing de senhas│  └──────────────┘  └──────────────┘  └──────────────┘       │

- **SQLite** - Banco de dados

└──────────────────────────────────────────────────────────────┘### Opção 2: Manual (3 Terminais)

### Frontend

- **Blazor WebAssembly** - SPA client-side

- **Bootstrap 5** - UI responsiva

- **HttpClient** - Comunicação com APIs┌──────────────────────────────────────────────────────────────┐#### Terminal 1 - API Conta Corrente

- **JWT Authentication** - Autenticação stateless

│              CAMADA DE OBSERVABILIDADE                       │```powershell

### Observabilidade

- **Serilog 9.0.0** - Logging estruturado│                                                               │cd src\BankMore.ContaCorrente\Api

- **Serilog.Sinks.Seq** - Sink para Seq

- **prometheus-net 8.2.1** - Métricas Prometheus│  ┌─────────┐  ┌──────────┐  ┌──────────┐  ┌────────────┐   │dotnet run

- **Health Checks** - Monitoramento (SQLite, Redis, Kafka)

│  │ Serilog │─►│   Seq    │  │Prometheus│─►│  Grafana   │   │```

### DevOps

- **Docker & Docker Compose** - Containerização│  │  Logs   │  │ (5341)   │  │  (9090)  │  │   (3000)   │   │

- **Nginx** - Web server para Blazor

│  └─────────┘  └──────────┘  └──────────┘  └────────────┘   │#### Terminal 2 - API Transferência

### Testes

- **xUnit 2.9.3** - Framework de testes└──────────────────────────────────────────────────────────────┘```powershell

- **FluentAssertions 8.8.0** - Assertions legíveis

- **Moq 4.20.72** - Mocking```cd src\BankMore.Transferencia\Api

- **Selenium WebDriver 4.27.0** - Testes E2E

- **coverlet** - Code coveragedotnet run



---### 📦 Componentes```



## ✨ Funcionalidades



### 🔐 Autenticação e Segurança| Componente | Tecnologia | Porta | Descrição |#### Terminal 3 - Interface Web

- ✅ Cadastro de conta com CPF e senha

- ✅ Login com CPF ou número da conta|------------|-----------|-------|-----------|```powershell

- ✅ JWT Token com refresh token

- ✅ Senha criptografada com BCrypt| **Blazor Web** | WebAssembly | 8080 | Interface de usuário moderna e responsiva |cd src\BankMore.Web

- ✅ CPF criptografado com AES-256-CBC

- ✅ Token **NÃO** contém dados sensíveis| **API Conta** | ASP.NET Core + EF Core | 5003 | Gerencia contas, auth JWT, movimentações |dotnet run

- ✅ Logout com revogação de token

| **API Transferência** | ASP.NET Core + Dapper | 5004 | Processa transferências com rollback |```

### 💳 Gestão de Conta

- ✅ Criar conta corrente| **Worker Tarifas** | Background Service | - | Consome eventos Kafka e debita tarifas |

- ✅ Consultar dados da conta

- ✅ Visualizar saldo em tempo real| **Kafka** | Apache Kafka | 9092 | Mensageria assíncrona |### Acessar o Sistema

- ✅ Ativar/desativar conta

- ✅ Histórico de movimentações| **Zookeeper** | Apache Zookeeper | 2181 | Coordenação do Kafka |



### 💸 Movimentações| **Redis** | Redis Cache | 6379 | Idempotência e cache distribuído |- **🌐 Interface Web**: http://localhost:5000 ou https://localhost:5001

- ✅ Crédito (depósito)

- ✅ Débito (saque)| **Seq** | Seq Logs | 5341 | Agregação e visualização de logs |- **📖 Swagger Conta**: http://localhost:5003

- ✅ Extrato com paginação

- ✅ Filtros por tipo e período| **Prometheus** | Prometheus | 9090 | Coleta de métricas |- **📖 Swagger Transferência**: http://localhost:5004



### 🔄 Transferências| **Grafana** | Grafana | 3000 | Dashboards e visualizações |

- ✅ Transferência entre contas

- ✅ Validação de saldo## 📘 Documentação

- ✅ Rollback automático em caso de falha

- ✅ Tarifa de R$ 2,00 por transferência---

- ✅ Histórico de transferências

- ✅ Idempotência garantida- **[Guia de Execução Web](GUIA-EXECUCAO-WEB.md)**: Tutorial completo com fluxo de teste



### ⚙️ Processamento Assíncrono## 🚀 Tecnologias- **[README da Interface Web](src/BankMore.Web/README.md)**: Documentação específica do frontend

- ✅ Worker que consome eventos Kafka

- ✅ Persistência de tarifas no banco

- ✅ Débito automático de tarifas

- ✅ Retry e dead letter queue### Backend## 🎯 Funcionalidades da Interface Web



### 📊 Observabilidade- **.NET 9.0** - Framework principal

- ✅ Logs estruturados (Serilog + Seq)

- ✅ Métricas Prometheus- **ASP.NET Core** - APIs RESTful### Autenticação

- ✅ Health Checks (/health, /health/ready, /health/live)

- ✅ Dashboards Grafana- **Entity Framework Core 9.0.10** - ORM (API Conta)- ✅ Login com CPF ou número da conta

- ✅ Correlation ID para rastreamento

- **Dapper 2.1.66** - Micro-ORM (API Transferência)- ✅ Cadastro de nova conta

---

- **MediatR 13.1.0** - CQRS pattern- ✅ Logout

## 📋 Pré-requisitos

- **KafkaFlow 4.0.1** - Cliente Kafka- ✅ JWT Token armazenado no LocalStorage

### Obrigatórios

- ✅ **Docker Desktop** instalado e rodando- **BCrypt.Net 4.0.3** - Hashing de senhas

- ✅ **Git** para clonar o repositório

- ✅ **Navegador Web** moderno (Chrome, Edge, Firefox)- **SQLite** - Banco de dados### Gestão de Conta



### Opcionais (para desenvolvimento)- ✅ Visualizar dados da conta (CPF, nome, status)

- ⚙️ **.NET 9.0 SDK**

- ⚙️ **Visual Studio 2022** ou **VS Code**### Frontend- ✅ Consultar saldo em tempo real

- ⚙️ **PowerShell** (Windows) ou **Bash** (Linux/Mac)

- **Blazor WebAssembly** - SPA client-side- ✅ Criar movimentações (crédito/débito)

---

- **Bootstrap 5** - UI responsiva- ✅ Visualizar extrato com paginação

## 🐋 Como Executar com Docker

- **HttpClient** - Comunicação com APIs

### 1️⃣ Clonar o Repositório

- **JWT Authentication** - Autenticação stateless### Transferências

```bash

git clone https://github.com/IgorAnjos/bank-more.git- ✅ Realizar transferências entre contas

cd bank-more

```### Observabilidade- ✅ Visualizar histórico de transferências



### 2️⃣ Subir Toda a Stack- **Serilog 9.0.0** - Logging estruturado- ✅ Informações de tarifa (R$ 2,00)



```bash- **Serilog.Sinks.Seq** - Sink para Seq- ✅ Paginação de resultados

# Buildar e iniciar todos os containers

docker-compose up -d --build- **Serilog.Sinks.Console** - Sink para Console



# Aguardar serviços iniciarem (~30 segundos)- **prometheus-net 8.2.1** - Métricas Prometheus## 🎨 Tecnologias

# Windows (PowerShell):

Start-Sleep -Seconds 30- **Health Checks** - Monitoramento de saúde



# Linux/Mac:  - SQLite, Redis, Kafka### Frontend

sleep 30

```- **Blazor WebAssembly** (client-side)



### 3️⃣ Verificar Status dos Containers### DevOps- **Bootstrap 5** (UI responsiva)



```bash- **Docker & Docker Compose** - Containerização- **HttpClient** (comunicação com APIs)

docker-compose ps

```- **Nginx** - Web server para Blazor- **JWT Authentication**



**Saída esperada** (10 containers rodando):- **Seq** - Agregação de logs



```- **Prometheus** - Métricas### Backend

NAME                           STATUS          PORTS

bankmore-web-1                 Up             0.0.0.0:8080->80/tcp- **Grafana** - Visualização- **.NET 9.0**

bankmore-api-conta-1           Up             0.0.0.0:5003->8080/tcp

bankmore-api-transferencia-1   Up             0.0.0.0:5004->8080/tcp- **Entity Framework Core 9.0.10**

bankmore-worker-tarifas-1      Up

kafka                          Up             0.0.0.0:9092->9092/tcp### Testes- **Dapper 2.1.66**

zookeeper                      Up             0.0.0.0:2181->2181/tcp

redis                          Up             0.0.0.0:6379->6379/tcp- **xUnit 2.9.3** - Framework de testes- **KafkaFlow 4.0.1** (opcional)

seq                            Up             0.0.0.0:5341->80/tcp

prometheus                     Up             0.0.0.0:9090->9090/tcp- **FluentAssertions 8.8.0** - Assertions legíveis- **SQLite**

grafana                        Up             0.0.0.0:3000->3000/tcp

```- **Moq 4.20.72** - Mocking- **JWT Bearer Authentication**



### 4️⃣ Acessar o Sistema- **Selenium WebDriver 4.27.0** - Testes E2E- **Swagger/OpenAPI**



| Interface | URL | Credenciais |- **coverlet** - Code coverage- **BCrypt.Net**

|-----------|-----|-------------|

| **🌐 Aplicação Web** | http://localhost:8080 | - |- **MediatR** (CQRS pattern)

| **📖 API Conta (Swagger)** | http://localhost:5003 | - |

| **📖 API Transferência (Swagger)** | http://localhost:5004 | - |---

| **📊 Seq (Logs)** | http://localhost:5341 | - |

| **📈 Prometheus** | http://localhost:9090 | - |## 🔧 Pré-requisitos

| **📊 Grafana** | http://localhost:3000 | admin/admin |

## ✨ Funcionalidades

### 5️⃣ Fluxo de Teste Completo

- .NET 9.0 SDK

#### **A. Cadastrar Conta**

1. Acesse http://localhost:8080### 🔐 Autenticação e Segurança- Docker Desktop (opcional, para Kafka)

2. Clique em **"Criar Conta"**

3. Preencha: CPF `12345678909`, Nome `João Silva`, Senha `senha123`- ✅ Cadastro de conta com CPF e senha- PowerShell ou terminal compatível

4. Anote o número da conta exibido

- ✅ Login com CPF ou número da conta- Navegador web moderno

#### **B. Fazer Login**

1. Clique em **"Fazer Login"**- ✅ JWT Token com refresh token

2. Digite CPF ou número da conta + senha

3. Acesse o dashboard- ✅ Senha criptografada com BCrypt## 📦 Estrutura do Projeto



#### **C. Adicionar Saldo**- ✅ CPF criptografado com AES-256-CBC

1. Na tela "Minha Conta", adicione R$ 1.000,00 (Crédito)

- ✅ Token **NÃO** contém dados sensíveis```

#### **D. Realizar Transferência**

1. Crie uma segunda conta (CPF diferente)- ✅ LogoutBankMore/

2. Faça login com a primeira conta

3. Vá para "Transferências" e transfira R$ 100,00├── src/

4. Verifique o débito da tarifa (R$ 2,00) no extrato

### 💳 Gestão de Conta│   ├── BankMore.Web/                    # 🆕 Interface Blazor WebAssembly

#### **E. Verificar Observabilidade**

1. **Logs no Seq**: http://localhost:5341- ✅ Criar conta corrente│   │   ├── Models/                      # DTOs

   - Busque por "Transferência realizada"

2. **Métricas no Prometheus**: http://localhost:9090- ✅ Consultar dados da conta│   │   ├── Services/                    # HTTP Services

   - Query: `http_requests_received_total`

- ✅ Visualizar saldo em tempo real│   │   ├── Pages/                       # Páginas Razor

### 6️⃣ Parar o Sistema

- ✅ Ativar/desativar conta│   │   └── Layout/                      # Layout e Menu

```bash

# Parar containers (preserva dados)- ✅ Histórico de movimentações│   ├── BankMore.ContaCorrente/         # API Conta Corrente

docker-compose stop

│   │   ├── Api/                         # Controllers e Program

# Parar e remover containers

docker-compose down### 💸 Movimentações│   │   ├── Application/                 # CQRS (MediatR)



# Remover containers E volumes (apaga banco de dados)- ✅ Crédito (depósito)│   │   ├── Domain/                      # Entidades e interfaces

docker-compose down -v

```- ✅ Débito (saque)│   │   └── Infrastructure/              # Repositórios e DbContext



---- ✅ Extrato com paginação│   ├── BankMore.Transferencia/         # API Transferência



## 🧪 Testes- ✅ Filtros por tipo e período│   │   ├── Api/                         # Controllers e Program



### Testes Unitários (xUnit)│   │   ├── Application/                 # CQRS (MediatR)



```bash### 🔄 Transferências│   │   ├── Domain/                      # Entidades e interfaces

cd tests/BankMore.ContaCorrente.Tests

dotnet test- ✅ Transferência entre contas│   │   └── Infrastructure/              # Repositórios Dapper



# Com cobertura- ✅ Validação de saldo│   └── BankMore.Tarifas/               # Worker Tarifas

dotnet test --collect:"XPlat Code Coverage"

```- ✅ Rollback automático em caso de falha│       ├── Handlers/                    # Event Handlers



#### Testes Implementados- ✅ Tarifa de R$ 2,00 por transferência│       └── Services/                    # Business Services



**CpfValidatorTests** (9 testes)- ✅ Histórico de transferências├── tests/                               # Testes automatizados

- ✅ Validar CPF válido

- ✅ Rejeitar CPF inválido- ✅ Idempotência garantida├── start-all.ps1                        # 🆕 Script de inicialização

- ✅ Rejeitar CPF com dígitos repetidos

- ✅ Aceitar CPF formatado├── GUIA-EXECUCAO-WEB.md                # 🆕 Tutorial completo

- ✅ Performance: 1000 validações < 100ms

### ⚙️ Processamento Assíncrono└── README.md                            # Este arquivo

**JwtServiceTests** (16 testes)

- ✅ Gerar Access Token com claims obrigatórias- ✅ Worker que consome eventos Kafka```

- ✅ **NÃO** incluir dados sensíveis no token

- ✅ Gerar Refresh Token criptograficamente seguro- ✅ Persistência de tarifas no banco

- ✅ Validar token válido/inválido/expirado

- ✅ Hash SHA-256 determinístico- ✅ Débito automático de tarifas## 🧪 Fluxo de Teste Rápido



**Cobertura**: 95%+ nas principais funcionalidades- ✅ Retry e dead letter queue



### Testes E2E (Selenium)1. Execute `.\start-all.ps1`



```bash### 📊 Observabilidade2. Acesse http://localhost:5000

# Pré-requisito: aplicação rodando em http://localhost:8080

cd tests/BankMore.Web.E2ETests- ✅ Logs estruturados (Serilog + Seq)3. Clique em "Criar Conta"

dotnet test

```- ✅ Métricas Prometheus4. Cadastre-se com CPF, nome e senha



#### Testes Implementados- ✅ Health Checks5. Faça login



- **CadastroE2ETests** (9 testes) - Cadastro de conta- ✅ Dashboards Grafana (opcional)6. Adicione saldo (Crédito de R$ 1.000)

- **LoginE2ETests** (10 testes) - Login e autenticação

- **MinhaContaE2ETests** (10 testes) - Dashboard e operações- ✅ Correlation ID para rastreamento7. Realize uma transferência



**Total**: 29 testes E2E com Page Object Pattern8. Verifique o extrato e histórico



---### 🧪 Testes



## 📊 Observabilidade- ✅ **41 testes unitários** (xUnit)## 🐛 Troubleshooting



### 📝 Logs Estruturados (Serilog + Seq)  - CpfValidator (9 testes)



**Acessar**: http://localhost:5341  - JwtService (16 testes)### Erro de CORS



**Exemplos de Queries**:  - Cobertura: 95%+- Certifique-se de que as APIs estão rodando

```sql

-- Todas as transferências- ✅ **29 testes E2E** (Selenium)- CORS já está configurado nas APIs

@MessageTemplate = "Transferência realizada"

  - Cadastro (9 testes)

-- Erros nas últimas 24h

@Level = "Error" and @Timestamp > Now() - 1d  - Login (10 testes)### Token Expirado



-- Rastrear requisição  - Minha Conta (10 testes)- Faça logout e login novamente

CorrelationId = "abc-123-def"

```- Tokens JWT têm validade de 24 horas



### 📈 Métricas (Prometheus)---



**Acessar**: http://localhost:9090### Porta em Uso



**Queries PromQL**:## 📋 Pré-requisitos- Verifique se não há outros serviços nas portas 5000, 5003 ou 5004

```promql

# Taxa de requisições/segundo- Use `netstat -ano | findstr :5000` para verificar

rate(http_requests_received_total[5m])

### Obrigatórios

# Percentil 95 de duração

histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m]))- ✅ **Docker Desktop** instalado e rodando## 📝 Próximos Passos



# Health checks com falha- ✅ **Git** (para clonar o repositório)

health_check_status{status="Unhealthy"}

```- ✅ **Navegador Web** moderno (Chrome, Edge, Firefox)### Interface Web



### 🏥 Health Checks- [ ] Página de consulta de tarifas



| Endpoint | Descrição |### Opcionais (para desenvolvimento)- [ ] Gráficos de movimentações

|----------|-----------|

| `/health` | Health check geral |- ⚙️ **.NET 9.0 SDK** (para rodar fora do Docker)- [ ] Dark mode

| `/health/ready` | Readiness probe |

| `/health/live` | Liveness probe |- ⚙️ **Visual Studio 2022** ou **VS Code**- [ ] PWA (Progressive Web App)



**Exemplo de Resposta**:- ⚙️ **PowerShell** (Windows) ou **Bash** (Linux/Mac)- [ ] Notificações toast

```json

{- [ ] Testes com bUnit

  "status": "Healthy",

  "totalDuration": "00:00:00.523",---

  "entries": {

    "sqlite": { "status": "Healthy" },### APIs

    "redis": { "status": "Healthy" },

    "kafka": { "status": "Healthy" }## 🐋 Como Executar com Docker- [ ] Docker Compose completo (APIs + Kafka + Worker + Web)

  }

}- [ ] Testes unitários com xUnit

```

### 1️⃣ Clonar o Repositório- [ ] Testes de integração end-to-end

---

- [ ] Health checks e retry policies

## 🌐 APIs Disponíveis

```bash- [ ] Dead Letter Queue para mensagens falhadas

### API Conta Corrente (Port 5003)

git clone https://github.com/seu-usuario/BankMore.git

**Autenticação**

```httpcd BankMore---

POST /api/auth/login

Content-Type: application/json```



{**BankMore** - Sistema bancário completo com interface moderna 🏦✨

  "numeroContaOuCpf": "12345678909",

  "senha": "senha123"### 2️⃣ Subir Toda a Stack

}

``````powershell



**Cadastrar Conta**```bash# Criar docker-compose.yml na raiz do projeto (veja próxima seção)

```http

POST /api/conta# Buildar e iniciar todos os containersdocker-compose up -d

Content-Type: application/json

docker-compose up -d --build```

{

  "cpf": "12345678909",

  "nome": "João Silva",

  "senha": "senha123"# Aguardar serviços iniciarem (~30 segundos)### 2️⃣ Executar as APIs

}

```# No Windows (PowerShell):



**Consultar Saldo**Start-Sleep -Seconds 30```powershell

```http

GET /api/conta/saldo# Terminal 1 - API Conta Corrente

Authorization: Bearer {token}

```# No Linux/Mac:cd src\BancoDigitalAna.ContaCorrente



**Criar Movimentação**sleep 30dotnet run

```http

POST /api/movimentacao```

Authorization: Bearer {token}

Content-Type: application/json# Terminal 2 - API Transferência



{### 3️⃣ Verificar Status dos Containerscd src\BancoDigitalAna.Transferencia

  "chaveIdempotencia": "mov-001",

  "tipoMovimento": "C",dotnet run

  "valor": 1000.00

}```bash

```

docker-compose ps# Terminal 3 - Worker Tarifas

### API Transferência (Port 5004)

```cd src\BancoDigitalAna.Tarifas

**Realizar Transferência**

```httpdotnet run

POST /api/transferencia

Authorization: Bearer {token}**Saída esperada** (10 containers rodando):```

Content-Type: application/json



{

  "chaveIdempotencia": "trans-001",```### 3️⃣ Testar o Sistema

  "idContaCorrenteDestino": "guid-destino",

  "valor": 100.00NAME                           STATUS          PORTS

}

```bankmore-web-1                 Up             0.0.0.0:8080->80/tcpAcesse os Swaggers:



**Swagger/OpenAPI**: bankmore-api-conta-1           Up             0.0.0.0:5003->8080/tcp- **API Conta**: http://localhost:5003/swagger

- http://localhost:5003/swagger

- http://localhost:5004/swaggerbankmore-api-transferencia-1   Up             0.0.0.0:5004->8080/tcp- **API Transferência**: http://localhost:5004/swagger



---bankmore-worker-tarifas-1      Up



## 🗂️ Estrutura do Projetokafka                          Up             0.0.0.0:9092->9092/tcp#### Fluxo Completo de Teste



```zookeeper                      Up             0.0.0.0:2181->2181/tcp

BankMore/

├── 📁 src/redis                          Up             0.0.0.0:6379->6379/tcp```powershell

│   ├── BankMore.Web/                    # Blazor WebAssembly

│   ├── BankMore.ContaCorrente/          # Microsserviço Contaseq                            Up             0.0.0.0:5341->80/tcp# 1. Cadastrar conta origem

│   │   ├── Api/                         # Controllers + Program.cs

│   │   ├── Application/                 # CQRS (MediatR)prometheus                     Up             0.0.0.0:9090->9090/tcpcurl -X POST http://localhost:5003/api/conta `

│   │   ├── Domain/                      # Entidades + Interfaces

│   │   └── Infrastructure/              # EF Core + Repositóriosgrafana                        Up             0.0.0.0:3000->3000/tcp  -H "Content-Type: application/json" `

│   ├── BankMore.Transferencia/          # Microsserviço Transferência

│   │   ├── Api/```  -d '{

│   │   ├── Application/

│   │   ├── Domain/    "cpf": "12345678901",

│   │   └── Infrastructure/              # Dapper

│   └── BankMore.Tarifas/                # Worker Tarifas### 4️⃣ Acessar o Sistema    "nome": "João Silva",

├── 📁 tests/

│   ├── BankMore.ContaCorrente.Tests/    # 41 testes unitários    "senha": "senha123"

│   └── BankMore.Web.E2ETests/           # 29 testes E2E (Selenium)

├── 📁 sql/                              # Scripts SQL| Interface | URL | Credenciais |  }'

├── 📁 especificacao/                    # Documentação técnica

├── docker-compose.yml                   # 10 serviços|-----------|-----|-------------|

├── prometheus.yml

└── README.md| **🌐 Aplicação Web** | http://localhost:8080 | - |# 2. Cadastrar conta destino

```

| **📖 API Conta (Swagger)** | http://localhost:5003 | - |curl -X POST http://localhost:5003/api/conta `

**Estatísticas**:

- 80+ arquivos C#| **📖 API Transferência (Swagger)** | http://localhost:5004 | - |  -H "Content-Type: application/json" `

- 15.000+ linhas de código

- 10.000+ linhas de documentação| **📊 Seq (Logs)** | http://localhost:5341 | - |  -d '{



---| **📈 Prometheus** | http://localhost:9090 | - |    "cpf": "98765432100",



## 🐛 Troubleshooting| **📊 Grafana** | http://localhost:3000 | admin/admin |    "nome": "Maria Santos",



### Containers não iniciam| **🔴 Redis** | localhost:6379 | - |    "senha": "senha456"



```bash| **📨 Kafka** | localhost:9092 | - |  }'

# Verificar logs

docker-compose logs



# Rebuild completo### 5️⃣ Fluxo de Teste Completo# 3. Fazer login

docker-compose down -v

docker-compose up -d --buildcurl -X POST http://localhost:5003/api/auth/login `

```

#### **Passo 1: Cadastrar Conta**  -H "Content-Type: application/json" `

### Kafka não conecta

  -d '{

```bash

# Restart Kafka1. Acesse http://localhost:8080    "numeroContaOuCpf": "12345678901",

docker-compose restart kafka zookeeper

2. Clique em **"Criar Conta"**    "senha": "senha123"

# Aguardar inicialização

sleep 303. Preencha:  }'

```

   - **CPF**: `12345678909` (válido)# Copie o token JWT retornado

### Worker não consome mensagens

   - **Nome**: `João Silva`

```bash

# Verificar logs do Worker   - **Senha**: `senha123`# 4. Fazer uma movimentação de crédito (adicionar R$ 1000)

docker-compose logs worker-tarifas

4. Clique em **"Criar Conta"**curl -X POST http://localhost:5003/api/movimentacao `

# Verificar tópico Kafka

docker exec -it kafka kafka-topics.sh --list --bootstrap-server localhost:90925. Anote o **número da conta** exibido  -H "Content-Type: application/json" `

```

  -H "Authorization: Bearer SEU_TOKEN_JWT" `

### API retorna 401 Unauthorized

#### **Passo 2: Fazer Login**  -d '{

**Causa**: Token JWT expirado

    "chaveIdempotencia": "mov-001",

**Solução**: Fazer logout e login novamente

1. Clique em **"Fazer Login"**    "tipoMovimento": "C",

### Portas em uso

2. Digite:    "valor": 1000.00

```bash

# Windows (PowerShell)   - **Conta ou CPF**: `12345678909` (ou número da conta)  }'

netstat -ano | findstr :8080

   - **Senha**: `senha123`

# Linux/Mac

lsof -i :80803. Clique em **"Entrar"**# 5. Realizar transferência



# Matar processocurl -X POST http://localhost:5004/api/transferencia `

taskkill /PID <PID> /F  # Windows

kill -9 <PID>           # Linux/Mac#### **Passo 3: Adicionar Saldo**  -H "Content-Type: application/json" `

```

  -H "Authorization: Bearer SEU_TOKEN_JWT" `

### Banco de dados corrompido

1. Na tela **"Minha Conta"**, clique em **"Adicionar Movimentação"**  -d '{

```bash

# Remover volumes e recriar2. Selecione **"Crédito"**    "chaveIdempotencia": "trans-001",

docker-compose down -v

docker-compose up -d --build3. Digite **R$ 1.000,00**    "idContaCorrenteDestino": "ID_CONTA_DESTINO",



# ⚠️ ATENÇÃO: Isso apaga todos os dados!4. Clique em **"Adicionar"**    "valor": 100.00

```

5. Verifique que o saldo foi atualizado  }'

---



## 🤝 Contribuindo

#### **Passo 4: Criar Segunda Conta (Destino)**# 6. Consultar saldo (deve ter descontado R$ 100 + R$ 2 de tarifa)

Contribuições são bem-vindas! Por favor, siga estas diretrizes:

curl -X GET http://localhost:5003/api/conta/saldo `

1. **Fork** o repositório

2. Crie uma **branch** (`git checkout -b feature/MinhaFeature`)1. Faça **Logout**  -H "Authorization: Bearer SEU_TOKEN_JWT"

3. **Commit** suas mudanças (`git commit -m 'feat: Adiciona MinhaFeature'`)

4. **Push** para a branch (`git push origin feature/MinhaFeature`)2. Crie uma nova conta com CPF diferente: `98765432100````

5. Abra um **Pull Request**

3. Anote o **número da conta destino**

### Conventional Commits

## 📊 Bancos de Dados

```

feat: adiciona nova funcionalidade#### **Passo 5: Realizar Transferência**

fix: corrige bug

docs: atualiza documentaçãoO sistema cria automaticamente 3 bancos SQLite:

test: adiciona testes

refactor: refatora código1. Faça login novamente com a **primeira conta**

perf: melhora performance

chore: tarefas de manutenção2. Vá para **"Transferências"**1. **contacorrente.db** - API Conta

```

3. Clique em **"Nova Transferência"**   - Tables: `contacorrente`, `movimento`, `idempotencia`

---

4. Preencha:

## 📚 Documentação Adicional

   - **Conta Destino**: (número da segunda conta)2. **transferencia.db** - API Transferência

- **[RESUMO-IMPLEMENTACAO-COMPLETA.md](especificacao/RESUMO-IMPLEMENTACAO-COMPLETA.md)** - Documentação técnica completa (8.000+ linhas)

- **[ESTRUTURA.md](especificacao/ESTRUTURA.md)** - Arquitetura detalhada   - **Valor**: `R$ 100,00`   - Tables: `transferencia`, `idempotencia`

- **[README-TESTES.md](tests/README-TESTES.md)** - Documentação de testes unitários

- **[README E2E](tests/BankMore.Web.E2ETests/README.md)** - Documentação de testes E2E5. Clique em **"Transferir"**



---3. **tarifas.db** - Worker Tarifas



## 🎯 Roadmap#### **Passo 6: Verificar Tarifa**   - Tables: `tarifa`



### V1.0 ✅ (Atual)

- [x] Microsserviços (Conta, Transferência, Tarifas)

- [x] Interface Blazor WebAssembly1. Vá para **"Minha Conta"**## ⚙️ Configurações

- [x] Observabilidade completa

- [x] Testes unitários (41) e E2E (29)2. Verifique o saldo:

- [x] Docker Compose

   - **Antes**: R$ 1.000,00### API Conta Corrente (`appsettings.json`)

### V1.1 🚧 (Próxima)

- [ ] API Gateway (Ocelot)   - **Depois**: R$ 898,00 (R$ 100 + R$ 2 de tarifa)

- [ ] Circuit Breaker (Polly)

- [ ] Outbox Pattern completo3. Consulte o **Extrato** para ver:```json

- [ ] Saga Pattern para transações distribuídas

   - Débito de R$ 100,00 (transferência){

### V2.0 📋 (Futuro)

- [ ] Kubernetes (Helm Charts)   - Débito de R$ 2,00 (tarifa)  "ConnectionStrings": {

- [ ] CI/CD (GitHub Actions)

- [ ] Testes de carga (k6)    "DefaultConnection": "Data Source=contacorrente.db"

- [ ] Autenticação OAuth2

#### **Passo 7: Validar Logs no Seq**  },

---

  "Jwt": {

## 📄 Licença

1. Acesse http://localhost:5341    "Key": "sua-chave-secreta-jwt-com-no-minimo-32-caracteres-para-seguranca",

Este projeto é um **sistema de demonstração educacional** desenvolvido para fins de aprendizado e portfólio.

2. Busque por:    "Issuer": "BancoDigitalAna",

---

   - `Transferência realizada`    "Audience": "BancoDigitalAna.Api"

## 👨‍💻 Autor

   - `Tarifa debitada`  }

Desenvolvido por **Igor Anjos**

3. Verifique **Correlation ID** para rastreamento}

**Stack Tecnológica**:

- .NET 9.0 + ASP.NET Core```

- Blazor WebAssembly

- Apache Kafka + Redis#### **Passo 8: Verificar Métricas no Prometheus**

- Docker & Docker Compose

- Serilog + Seq + Prometheus + Grafana### API Transferência (`appsettings.json`)

- xUnit + Selenium WebDriver

1. Acesse http://localhost:9090

---

2. Execute queries:```json

<div align="center">

   ```promql{

### 🏦 BankMore - Sistema Bancário Moderno 🚀

   # Total de requisições HTTP  "ConnectionStrings": {

[![GitHub](https://img.shields.io/badge/GitHub-IgorAnjos%2Fbank--more-181717?logo=github)](https://github.com/IgorAnjos/bank-more)

[![Docker](https://img.shields.io/badge/Docker-Ready-blue?logo=docker)](https://www.docker.com/)   http_requests_received_total    "DefaultConnection": "Data Source=transferencia.db"

[![.NET](https://img.shields.io/badge/.NET-9.0-purple?logo=dotnet)](https://dotnet.microsoft.com/)

     },

**[⬆️ Voltar ao topo](#-bankmore---sistema-bancário-digital)**

   # Duração das requisições  "ApiContaCorrente": {

</div>

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
