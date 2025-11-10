# BankMore.Web - Interface Web Blazor WebAssembly

Interface de usuário web para o sistema BankMore, desenvolvida com Blazor WebAssembly.

## 🎯 Funcionalidades

### 1. Autenticação
- **Login**: Acesso com número da conta ou CPF e senha
- **Cadastro**: Criação de nova conta corrente
- **Logout**: Encerramento de sessão

### 2. Gestão de Conta
- **Visualização de Dados**: Informações da conta (número, CPF, nome, status)
- **Consulta de Saldo**: Saldo atual com data/hora da consulta
- **Extrato**: Histórico de movimentações com paginação
- **Movimentações**: Criar créditos e débitos na conta

### 3. Transferências
- **Nova Transferência**: Realizar transferências entre contas
- **Histórico**: Visualizar transferências realizadas com paginação
- **Detalhamento**: Valor, tarifa aplicada, data e status

## 🏗️ Arquitetura

```
BankMore.Web/
├── Models/              # DTOs e modelos de dados
│   ├── AuthDto.cs
│   ├── ContaDto.cs
│   ├── TransferenciaDto.cs
│   └── MovimentacaoRequest.cs
├── Services/            # Serviços de comunicação com APIs
│   ├── AuthService.cs
│   ├── ContaService.cs
│   ├── TransferenciaService.cs
│   └── TokenService.cs
├── Pages/               # Páginas Blazor
│   ├── Login.razor
│   ├── Cadastro.razor
│   ├── MinhaConta.razor
│   └── Transferencias.razor
├── Layout/              # Componentes de layout
│   ├── MainLayout.razor
│   └── NavMenu.razor
└── wwwroot/            # Arquivos estáticos
    └── appsettings.json
```

## 🚀 Como Executar

### Pré-requisitos

- .NET 9.0 SDK
- APIs BankMore.ContaCorrente (porta 5003) e BankMore.Transferencia (porta 5004) em execução

### Executar localmente

```powershell
cd c:\GitHub\Teste\BankMore\src\BankMore.Web
dotnet run
```

Acesse: `https://localhost:5001` ou `http://localhost:5000`

### Executar com Docker

```powershell
# Build da imagem
cd c:\GitHub\Teste\BankMore\src\BankMore.Web
docker build -t bankmore-web .

# Executar container
docker run -d -p 8080:80 --name bankmore-web bankmore-web
```

Acesse: `http://localhost:8080`

## ⚙️ Configuração

### appsettings.json

Configure as URLs das APIs:

```json
{
  "ApiUrls": {
    "ContaCorrente": "http://localhost:5003",
    "Transferencia": "http://localhost:5004"
  }
}
```

### Program.cs

Os serviços HTTP são configurados automaticamente:

```csharp
builder.Services.AddHttpClient<ContaService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5003");
});

builder.Services.AddHttpClient<TransferenciaService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5004");
});
```

## 🔐 Autenticação

O aplicativo utiliza JWT Bearer Token:

1. **Login**: Token é obtido da API de autenticação
2. **Armazenamento**: Token salvo no localStorage do navegador
3. **Autorização**: Token enviado no header `Authorization: Bearer {token}` em todas as requisições
4. **Expiração**: Token tem validade limitada (configurada nas APIs)

## 📱 Páginas

### Login (`/login`)
- Formulário de autenticação
- Validação de credenciais
- Redirecionamento para página principal após login

### Cadastro (`/cadastro`)
- Formulário de criação de conta
- Validação de CPF e dados
- Mensagem de sucesso com número da conta criada

### Minha Conta (`/` ou `/conta`)
- Card com dados da conta
- Card com saldo atual
- Formulário para nova movimentação
- Tabela com extrato paginado

### Transferências (`/transferencias`)
- Formulário para nova transferência
- Tabela com histórico de transferências
- Paginação e atualização em tempo real

## 🎨 Interface

O aplicativo utiliza **Bootstrap 5** para estilização:

- Design responsivo
- Cards para organização de conteúdo
- Formulários com validação
- Tabelas paginadas
- Alertas de sucesso/erro
- Spinners de carregamento

## 🔧 Serviços

### AuthService
- `LoginAsync()`: Autenticação de usuário
- `CadastrarContaAsync()`: Criação de nova conta
- `Logout()`: Encerramento de sessão
- `IsAuthenticated()`: Verificação de autenticação

### ContaService
- `ObterContaAsync()`: Buscar dados da conta
- `ConsultarSaldoAsync()`: Consultar saldo
- `ListarMovimentosAsync()`: Listar movimentos com paginação
- `CriarMovimentoAsync()`: Criar nova movimentação

### TransferenciaService
- `RealizarTransferenciaAsync()`: Realizar transferência
- `ObterTransferenciaAsync()`: Buscar transferência específica
- `ListarTransferenciasAsync()`: Listar transferências com paginação

### TokenService
- `GetToken()`: Obter token JWT
- `SetToken()`: Salvar token
- `GetIdConta()`: Obter ID da conta
- `ClearToken()`: Limpar token (logout)

## 🚧 Próximos Passos

- [ ] Implementar loading states globais
- [ ] Adicionar tratamento de erros mais robusto
- [ ] Criar componente de notificações toast
- [ ] Implementar refresh automático de token
- [ ] Adicionar página de consulta de tarifas
- [ ] Implementar dark mode
- [ ] Adicionar gráficos de movimentações
- [ ] Criar testes unitários com bUnit
- [ ] Adicionar PWA (Progressive Web App) support
- [ ] Implementar internacionalização (i18n)

## 📦 Dependências

- Microsoft.AspNetCore.Components.WebAssembly (9.0.0)
- Microsoft.Extensions.Http (9.0.0)
- System.Net.Http.Json (9.0.0)

## 🌐 CORS

As APIs backend precisam ter CORS configurado para aceitar requisições do aplicativo web:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorWasm", policy =>
    {
        policy.WithOrigins("http://localhost:5000", "https://localhost:5001")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

## 📝 Notas

- O aplicativo roda inteiramente no navegador (client-side)
- Todas as chamadas HTTP são feitas diretamente para as APIs
- Não há servidor backend específico para o Blazor WebAssembly
- O estado da aplicação é mantido localmente no navegador

---

**BankMore.Web** - Interface moderna e responsiva para gestão bancária 🏦✨
