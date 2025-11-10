# 📜 Scripts PowerShell - BankMore

Coleção de scripts PowerShell para automação de tarefas de desenvolvimento e teste.

---

## 🐋 Scripts Docker

### `docker-start.ps1`
**Descrição**: Inicia todos os containers Docker via docker-compose
- Executa `docker-compose up -d --build`
- Verifica status dos containers
- Exibe logs de inicialização

**Uso**:
```powershell
.\docker-start.ps1
```

---

### `docker-check.ps1`
**Descrição**: Verifica status e saúde dos containers Docker
- Lista containers em execução
- Verifica health checks
- Exibe uso de recursos (CPU/Memória)

**Uso**:
```powershell
.\docker-check.ps1
```

---

## 🚀 Scripts de Inicialização

### `start-all.ps1`
**Descrição**: Inicia todos os serviços do BankMore em modo desenvolvimento
- API Conta Corrente (Port 5003)
- API Transferência (Port 5004)
- Worker Tarifas
- Blazor Web (Port 5000)

**Uso**:
```powershell
.\start-all.ps1
```

**Observação**: Abre cada serviço em uma nova janela do PowerShell

---

## 🧪 Scripts de Teste

### `test-api.ps1`
**Descrição**: Testa endpoints básicos das APIs
- Verifica se APIs estão respondendo
- Testa endpoint `/health`
- Valida status HTTP 200

**Uso**:
```powershell
.\test-api.ps1
```

---

### `testes-api-restful.ps1`
**Descrição**: Testes completos da API de Conta Corrente
- Cadastro de conta
- Login
- Consulta de saldo
- Movimentações (crédito/débito)
- Extrato

**Uso**:
```powershell
.\testes-api-restful.ps1
```

---

### `testes-apis-restful.ps1`
**Descrição**: Testes integrados de todas as APIs
- API Conta Corrente
- API Transferência
- Fluxo completo de transferência
- Validação de tarifas

**Uso**:
```powershell
.\testes-apis-restful.ps1
```

---

## 🔐 Scripts de Segurança JWT

### `test-jwt-security.ps1`
**Descrição**: Testa segurança da autenticação JWT
- Valida geração de tokens
- Verifica se dados sensíveis estão criptografados
- Testa expiração de tokens
- Valida refresh token

**Uso**:
```powershell
.\test-jwt-security.ps1
```

---

### `test-jwt-refresh.ps1`
**Descrição**: Testa fluxo de refresh token
- Login e obtenção de access token
- Simulação de expiração
- Refresh token para renovação
- Validação de novo token

**Uso**:
```powershell
.\test-jwt-refresh.ps1
```

---

## 📊 Scripts de Informação

### `version-info.ps1`
**Descrição**: Exibe informações de versão do projeto
- Versão do BankMore
- Versões das dependências (.NET, Docker, etc)
- Informações do ambiente
- Changelog recente

**Uso**:
```powershell
.\version-info.ps1
```

---

## 🛠️ Pré-requisitos

Para executar estes scripts, você precisa ter:

- ✅ **PowerShell 5.1+** (Windows) ou **PowerShell Core 7+** (multiplataforma)
- ✅ **.NET 9.0 SDK** instalado
- ✅ **Docker Desktop** (para scripts Docker)
- ✅ **Git** configurado

---

## 📝 Notas

- Todos os scripts assumem que você está na pasta raiz do projeto BankMore
- Alguns scripts podem requerer permissões de administrador
- Para scripts Docker, certifique-se de que o Docker Desktop está rodando

---

## 🔧 Troubleshooting

### Erro: "script não pode ser carregado porque a execução de scripts foi desabilitada"

**Solução**:
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Erro: "Acesso negado"

**Solução**: Execute o PowerShell como Administrador

---

## 📚 Documentação Adicional

- [README Principal](../README.md)
- [Documentação de Testes](../tests/README-TESTES.md)
- [Documentação E2E](../tests/BankMore.Web.E2ETests/README.md)
