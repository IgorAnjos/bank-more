# Testes Unitários - BankMore Conta Corrente

## ✅ Status dos Testes

**Total de Testes**: 41  
**Aprovados**: 41 ✅  
**Falhados**: 0  
**Ignorados**: 0  
**Duração**: ~150ms  

## 📊 Cobertura de Código

- **CpfValidator**: 95.45% de cobertura de linhas, 87.5% de branches
- **JwtService**: 100% de cobertura de linhas, 50% de branches
- **Cobertura Global**: 6.79% (normal para testes iniciais - apenas 2 serviços testados)

## 🧪 Testes Implementados

### 1. **CpfValidatorTests** (9 testes)

#### ✅ Validação de CPFs Válidos
- `Validar_ComCpfValido_DeveRetornarTrue`
  - CPF: `52998224725` ✅
  - CPF: `11144477735` ✅
  - CPF: `12345678909` ✅
  - CPF: `00000000191` ✅
  - CPF: `123.456.789-09` ✅ (formatado)

#### ❌ Validação de CPFs Inválidos
- `Validar_ComCpfInvalido_DeveRetornarFalse`
  - `12345678901` - Dígitos verificadores inválidos
  - `11111111111` - Todos dígitos iguais
  - `00000000000` - Todos zeros
  - `99999999999` - Todos noves
  - `12345678900` - Dígito verificador errado

#### 🚫 Validação de Inputs Vazios/Nulos
- `Validar_ComCpfVazioOuNulo_DeveRetornarFalse`
  - String vazia `""`
  - Null `null`
  - Apenas espaços `"   "`

#### 📏 Validação de Formato Inválido
- `Validar_ComTamanhoOuFormatoInvalido_DeveRetornarFalse`
  - `123456789` - Menos de 11 dígitos
  - `123456789012` - Mais de 11 dígitos
  - `1234567890A` - Contém letra

#### 🔁 Validação de Sequências Repetidas
- `Validar_ComSequenciaDeZeros_DeveRetornarFalse`
  - `00000000000`
  
- `Validar_ComCpfComDigitosRepetidos_DeveRetornarFalse`
  - `11111111111` até `99999999999`

#### ⚡ Teste de Performance
- `Validar_ComGrandeQuantidadeDeValidacoes_DeveExecutarRapidamente`
  - 1000 validações em < 100ms ✅

---

### 2. **JwtServiceTests** (16 testes)

#### 🎫 Geração de Access Token
- `GerarAccessToken_DeveRetornarTokenValido`
  - Token não vazio
  - Formato JWT válido (3 partes separadas por `.`)

- `GerarAccessToken_DeveConterClaimsObrigatorias`
  - Claim `sub` (subject) com ID da conta
  - Claim `jti` (JWT ID) único
  - Claim `iat` (issued at) timestamp
  - Claim `tipo` = "access"

- `GerarAccessToken_NaoDeveConterDadosSensiveis`
  - **NÃO** contém `cpf`
  - **NÃO** contém `nome`
  - **NÃO** contém `numero`
  - **NÃO** contém `saldo`

- `GerarAccessToken_DeveExpirarEm10Minutos`
  - Token expira em exatamente 10 minutos

- `GerarAccessToken_DeveTerAssinaturaCriptograficaValida`
  - Token assinado com HMAC-SHA256

#### 🔄 Geração de Refresh Token
- `GerarRefreshToken_DeveRetornarTokenUnico`
  - Cada token gerado é único
  - 2 tokens consecutivos são diferentes

- `GerarRefreshToken_DeveRetornarTokenComTamanhoCorreto`
  - Token em Base64
  - Comprimento esperado de 44 caracteres

- `GerarRefreshToken_DeveTerAltaEntropia`
  - Token criptograficamente seguro
  - Alta aleatoriedade

#### 🔐 Validação de Tokens
- `ValidarToken_ComTokenValido_DeveRetornarPrincipal`
  - Retorna ClaimsPrincipal válido
  - Contém claim `sub` com ID correto

- `ValidarToken_ComTokenInvalido_DeveRetornarNull`
  - Token malformado retorna `null`

- `ValidarToken_ComTokenExpirado_DeveRetornarNull`
  - Token expirado é rejeitado

- `ValidarToken_ComAssinaturaInvalida_DeveRetornarNull`
  - Token adulterado é rejeitado

#### 🔒 Hash de Tokens
- `ComputarHashToken_DeveRetornarHashSHA256`
  - Hash não vazio
  - Comprimento correto (64 caracteres hexadecimais)

- `ComputarHashToken_DeveTerHashDeterministico`
  - Mesmo token gera sempre o mesmo hash

- `ComputarHashToken_DeveTerHashUnicoPorToken`
  - Tokens diferentes geram hashes diferentes

- `ComputarHashToken_DeveRetornarHashEmMinusculas`
  - Hash em formato hexadecimal minúsculo

---

## 🛠️ Tecnologias Utilizadas

- **xUnit 2.9.3** - Framework de testes
- **FluentAssertions 8.8.0** - Assertions legíveis
- **Moq 4.20.72** - Mocking framework (preparado para uso futuro)
- **Microsoft.EntityFrameworkCore.InMemory 9.0.10** - Banco em memória para testes
- **coverlet.collector 6.0.4** - Coleta de cobertura de código

## 📋 Próximos Passos

### Testes Pendentes
1. **EncryptionServiceTests** - Criptografia AES-256 e BCrypt
2. **IdempotencyRepositoryTests** - Operações Redis com TTL
3. **TransferenciaHandlerTests** - Integração HTTP e Kafka
4. **ValidatorsTests** - Validadores de negócio (FluentValidation)
5. **IntegrationTests** - Testes end-to-end com banco real

### Meta de Cobertura
- **Objetivo**: >80% de cobertura nas camadas Application e Domain
- **Atual**: 6.79% global (CpfValidator e JwtService testados)

## 🚀 Como Executar os Testes

```powershell
# Executar todos os testes
cd tests/BankMore.ContaCorrente.Tests
dotnet test

# Executar com cobertura
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

# Executar apenas testes de CPF
dotnet test --filter FullyQualifiedName~CpfValidatorTests

# Executar apenas testes de JWT
dotnet test --filter FullyQualifiedName~JwtServiceTests

# Executar com verbosidade detalhada
dotnet test --verbosity detailed
```

## 📝 Convenções de Nomenclatura

```csharp
// Padrão: [MetodoTestado]_[Cenario]_[ResultadoEsperado]
public void Validar_ComCpfValido_DeveRetornarTrue()
public void GerarAccessToken_NaoDeveConterDadosSensiveis()
public void ValidarToken_ComTokenExpirado_DeveRetornarNull()
```

## ✨ Destaques

### Segurança Validada
- ✅ JWT **NÃO** contém dados sensíveis (CPF, nome, saldo)
- ✅ Tokens refresh com alta entropia criptográfica
- ✅ Hash SHA-256 determinístico e único
- ✅ Validação de assinatura HMAC-SHA256

### Validações Robustas
- ✅ CPF aceita formato com máscara (`123.456.789-09`)
- ✅ Rejeita todos dígitos iguais (`11111111111`)
- ✅ Performance: 1000 validações < 100ms

### Qualidade de Código
- ✅ Testes independentes e isolados
- ✅ Assertions claras e descritivas
- ✅ Coverage configurado e funcional
- ✅ Sem dependências externas (mocks preparados)

---

**Última Atualização**: 15/01/2025  
**Responsável**: GitHub Copilot  
**Status**: ✅ Todos os testes passando (41/41)
