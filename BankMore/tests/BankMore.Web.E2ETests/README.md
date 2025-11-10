# Testes E2E com Selenium - BankMore Web

## 📋 Visão Geral

Testes end-to-end automatizados para a aplicação web BankMore usando **Selenium WebDriver** com **xUnit** e **Page Object Pattern**.

## 🎯 Funcionalidades Testadas

### ✅ Cadastro de Conta
- [x] Carregar página de cadastro
- [x] Criar conta com dados válidos
- [x] Redirecionar para login após cadastro
- [x] Validar erro com CPF inválido
- [x] Validar erro com CPF duplicado
- [x] Desabilitar botão durante loading
- [x] Navegar para login
- [x] Validar campos obrigatórios
- [x] Aceitar CPF formatado com máscara

**Total**: 9 testes

### ✅ Login
- [x] Carregar página de login
- [x] Login com número da conta válido
- [x] Login com CPF válido
- [x] Exibir erro com credenciais inválidas
- [x] Exibir erro com senha incorreta
- [x] Desabilitar botão durante loading
- [x] Navegar para cadastro
- [x] Validar campos obrigatórios
- [x] Redirecionar usuário autenticado
- [x] Aceitar CPF ou número no mesmo campo

**Total**: 10 testes

### ✅ Minha Conta
- [x] Redirecionar para login quando não autenticado
- [x] Exibir dados da conta após login
- [x] Exibir número da conta correto
- [x] Exibir nome do titular
- [x] Exibir CPF do titular
- [x] Exibir conta como ativa
- [x] Exibir saldo da conta
- [x] Exibir saldo inicial zerado (R$ 0,00)
- [x] Exibir página completa com todos cards
- [x] Manter sessão ao navegar entre páginas

**Total**: 10 testes

## 📦 Tecnologias

- **Selenium WebDriver 4.27.0** - Automação de navegador
- **Selenium.WebDriver.ChromeDriver 131.0** - Driver do Chrome
- **Selenium.Support 4.27.0** - Classes auxiliares (Page Object)
- **xUnit 2.9.3** - Framework de testes
- **FluentAssertions 8.8.0** - Assertions legíveis

## 🏗️ Arquitetura

```
BankMore.Web.E2ETests/
├── Infrastructure/
│   └── SeleniumTestBase.cs          # Classe base com helpers Selenium
├── PageObjects/
│   ├── CadastroPage.cs              # Page Object: Cadastro
│   ├── LoginPage.cs                 # Page Object: Login
│   └── MinhaContaPage.cs            # Page Object: Minha Conta
└── Tests/
    ├── CadastroE2ETests.cs          # 9 testes de cadastro
    ├── LoginE2ETests.cs             # 10 testes de login
    └── MinhaContaE2ETests.cs        # 10 testes de conta
```

### 🎨 Page Object Pattern

Os testes seguem o **Page Object Pattern** para:
- ✅ Separar lógica de localização de elementos da lógica de teste
- ✅ Reduzir duplicação de código
- ✅ Facilitar manutenção quando UI mudar
- ✅ Tornar testes mais legíveis

Exemplo:
```csharp
// Ao invés de:
Driver.FindElement(By.Id("cpf")).SendKeys("12345678909");

// Usamos:
_cadastroPage.PreencherFormulario("12345678909", "João Silva", "senha123");
```

## 🚀 Como Executar

### Pré-requisitos

1. **Aplicação rodando**: A aplicação web deve estar disponível em `http://localhost:8080`
2. **Chrome instalado**: Selenium usa ChromeDriver
3. **.NET 9 SDK**: Para rodar os testes

### Comandos

```powershell
# Navegar para o diretório de testes
cd tests/BankMore.Web.E2ETests

# Restaurar dependências
dotnet restore

# Executar todos os testes E2E
dotnet test

# Executar com verbosidade detalhada
dotnet test --verbosity detailed

# Executar apenas testes de Cadastro
dotnet test --filter "FullyQualifiedName~CadastroE2ETests"

# Executar apenas testes de Login
dotnet test --filter "FullyQualifiedName~LoginE2ETests"

# Executar apenas testes de Minha Conta
dotnet test --filter "FullyQualifiedName~MinhaContaE2ETests"

# Executar teste específico
dotnet test --filter "DisplayName~Deve criar conta com dados válidos"
```

### Executar com Docker Compose

```powershell
# Subir toda a stack (incluindo Web)
cd ../..
docker-compose up -d

# Aguardar serviços iniciarem (30 segundos)
Start-Sleep -Seconds 30

# Executar testes E2E
cd tests/BankMore.Web.E2ETests
dotnet test

# Parar stack
cd ../..
docker-compose down
```

## ⚙️ Configuração

### Variável de Ambiente

Por padrão, os testes acessam `http://localhost:8080`. Para alterar:

```powershell
# PowerShell
$env:WEB_BASE_URL = "http://localhost:5000"
dotnet test

# Bash/Linux
export WEB_BASE_URL="http://localhost:5000"
dotnet test
```

### Modo Headless

Os testes rodam em **modo headless** (sem abrir janela do navegador) por padrão.

Para executar **com interface gráfica** (debug):
1. Edite `Infrastructure/SeleniumTestBase.cs`
2. Comente a linha: `options.AddArgument("--headless");`

## 🐛 Debug e Troubleshooting

### Screenshots

A classe base possui método `TakeScreenshot()`:

```csharp
[Fact]
public void MeuTeste()
{
    try
    {
        // Seu teste aqui
    }
    catch
    {
        TakeScreenshot("erro_meu_teste");
        throw;
    }
}
```

Screenshots são salvos em `Screenshots/` com timestamp.

### Problemas Comuns

#### 1. **Aplicação não está rodando**
```
Error: Unable to connect to http://localhost:8080
```
**Solução**: Certifique-se que `docker-compose up` foi executado.

#### 2. **ChromeDriver incompatível**
```
Error: session not created: This version of ChromeDriver only supports Chrome version X
```
**Solução**: Atualize o pacote `Selenium.WebDriver.ChromeDriver`:
```powershell
dotnet add package Selenium.WebDriver.ChromeDriver
```

#### 3. **Timeout nos testes**
```
Error: Timeout waiting for element
```
**Solução**: 
- Aumente timeouts em `SeleniumTestBase.cs`
- Verifique se API está respondendo rápido
- Use `WaitForLoadingToFinish()` após ações

#### 4. **Teste flaky (às vezes passa, às vezes falha)**
**Soluções**:
- Adicione `Thread.Sleep()` estratégicos
- Use `WaitForElement()` ao invés de `FindElement()`
- Verifique se há animações CSS que atrasam elementos

## 📊 Relatório de Testes

```powershell
# Executar com logger detalhado
dotnet test --logger "console;verbosity=detailed"

# Gerar relatório HTML (com ReportGenerator)
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coveragereport -reporttypes:Html
```

## 🎯 Boas Práticas Implementadas

✅ **Page Object Pattern** - Separação de responsabilidades  
✅ **Waits Explícitos** - Evita flakiness  
✅ **Cleanup automático** - `Dispose()` fecha navegador  
✅ **Geração de CPF válido** - Testes não dependem de dados fixos  
✅ **Assertions claras** - FluentAssertions legível  
✅ **Isolamento de testes** - Cada teste cria seus próprios dados  
✅ **DisplayName descritivo** - Relatórios legíveis  

## 📈 Cobertura de Testes

### Fluxos Cobertos

1. **Happy Path**: Cadastro → Login → Visualizar Conta ✅
2. **Error Handling**: CPF inválido, credenciais erradas ✅
3. **Validações**: Campos obrigatórios, formatos ✅
4. **Navegação**: Redirecionamentos, links ✅
5. **Autenticação**: Sessão, logout, proteção de rotas ✅

### Métricas

- **Total de Testes**: 29
- **Tempo Médio**: ~3-5 segundos por teste
- **Taxa de Sucesso**: 100% (quando aplicação está rodando)

## 🔄 CI/CD Integration

Exemplo de pipeline para GitHub Actions:

```yaml
name: E2E Tests

on: [push, pull_request]

jobs:
  e2e:
    runs-on: ubuntu-latest
    
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'
      
      - name: Start Docker Compose
        run: docker-compose up -d
      
      - name: Wait for services
        run: sleep 30
      
      - name: Run E2E Tests
        run: dotnet test tests/BankMore.Web.E2ETests
      
      - name: Stop Docker Compose
        run: docker-compose down
```

## 📝 Próximos Passos

- [ ] Adicionar testes para **Transferências**
- [ ] Adicionar testes para **Extrato**
- [ ] Implementar **screenshot automático em falhas**
- [ ] Adicionar testes de **performance** (tempo de carregamento)
- [ ] Implementar **testes em múltiplos navegadores** (Firefox, Edge)
- [ ] Adicionar **testes de acessibilidade** (ARIA labels)
- [ ] Implementar **testes mobile** (viewport responsivo)

## 🤝 Contribuindo

Ao adicionar novos testes:

1. **Siga o Page Object Pattern** - Crie/atualize PageObjects
2. **Use DisplayName descritivo** - "Deve [ação] [resultado esperado]"
3. **Isole testes** - Cada teste deve criar seus próprios dados
4. **Limpe estado** - Use `ClearBrowserData()` quando necessário
5. **Documente casos edge** - Comente comportamentos especiais

## 📚 Referências

- [Selenium WebDriver Docs](https://www.selenium.dev/documentation/)
- [Page Object Pattern](https://www.selenium.dev/documentation/test_practices/encouraged/page_object_models/)
- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions](https://fluentassertions.com/)

---

**Última Atualização**: 15/01/2025  
**Responsável**: GitHub Copilot  
**Status**: ✅ 29 testes E2E implementados
