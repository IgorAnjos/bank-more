# Script de Teste: JWT Security - Access + Refresh Tokens
# Valida que claims não contêm dados sensíveis

$ErrorActionPreference = "Stop"
$baseUrl = "http://localhost:5001/api/v1"

Write-Host "`n=== TESTE: JWT SECURITY COMPLIANCE ===" -ForegroundColor Cyan
Write-Host "Validando que tokens JWT não contêm dados sensíveis (PII, financeiros, etc.)`n" -ForegroundColor Yellow

# 1. LOGIN
Write-Host "[1/5] LOGIN: Autenticando usuário..." -ForegroundColor Green
$loginBody = @{
    numeroOuCpf = "12345678901"
    senha = "SenhaForte@123"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/auth/tokens" -Method POST -Body $loginBody -ContentType "application/json"
    Write-Host "✅ Login realizado com sucesso" -ForegroundColor Green
    Write-Host "   - Access Token recebido: $($loginResponse.accessToken.Substring(0, 50))..." -ForegroundColor Gray
    Write-Host "   - Refresh Token recebido: $($loginResponse.refreshToken.Substring(0, 30))..." -ForegroundColor Gray
    Write-Host "   - Expira em: $($loginResponse.expiresInMinutes) minutos`n" -ForegroundColor Gray
} catch {
    Write-Host "❌ Erro no login: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 2. DECODIFICAR JWT (sem validar assinatura - apenas para inspeção)
Write-Host "[2/5] INSPECIONANDO CLAIMS: Verificando conteúdo do token..." -ForegroundColor Green

# Extrai payload (parte 2 do JWT: header.PAYLOAD.signature)
$tokenParts = $loginResponse.accessToken -split '\.'
if ($tokenParts.Length -ne 3) {
    Write-Host "❌ Token JWT inválido (não possui 3 partes)" -ForegroundColor Red
    exit 1
}

# Decodifica Base64Url (adiciona padding se necessário)
$payload = $tokenParts[1]
$payload = $payload.Replace('-', '+').Replace('_', '/')
switch ($payload.Length % 4) {
    2 { $payload += '==' }
    3 { $payload += '=' }
}

try {
    $decodedBytes = [System.Convert]::FromBase64String($payload)
    $decodedJson = [System.Text.Encoding]::UTF8.GetString($decodedBytes)
    $claims = $decodedJson | ConvertFrom-Json
    
    Write-Host "✅ Token decodificado com sucesso`n" -ForegroundColor Green
    Write-Host "📋 CLAIMS ENCONTRADAS:" -ForegroundColor Cyan
    $claims.PSObject.Properties | ForEach-Object {
        Write-Host "   - $($_.Name): $($_.Value)" -ForegroundColor Gray
    }
    Write-Host ""
} catch {
    Write-Host "❌ Erro ao decodificar token: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 3. VALIDAÇÃO DE SEGURANÇA
Write-Host "[3/5] VALIDAÇÃO DE SEGURANÇA: Verificando compliance..." -ForegroundColor Green

$violations = @()

# Claims obrigatórias (✅ DEVE TER)
$requiredClaims = @('sub', 'jti', 'iat', 'exp', 'iss', 'aud')
foreach ($claim in $requiredClaims) {
    if (-not $claims.PSObject.Properties.Name.Contains($claim)) {
        $violations += "❌ MISSING CLAIM: '$claim' obrigatória não encontrada"
    }
}

# Claims proibidas (❌ NÃO DEVE TER)
$forbiddenClaims = @(
    @{Name='nome'; Desc='Nome completo (PII)'},
    @{Name='cpf'; Desc='CPF (PII crítico)'},
    @{Name='email'; Desc='Email (PII)'},
    @{Name='telefone'; Desc='Telefone (PII)'},
    @{Name='numero'; Desc='Número da conta (dado financeiro)'},
    @{Name='numero_conta'; Desc='Número da conta (dado financeiro)'},
    @{Name='saldo'; Desc='Saldo (dado financeiro)'},
    @{Name='agencia'; Desc='Agência (dado financeiro)'},
    @{Name='cartao'; Desc='Cartão (dado sensível)'},
    @{Name='senha'; Desc='Senha (credencial)'},
    @{Name='api_key'; Desc='API Key (credencial)'},
    @{Name='data_nascimento'; Desc='Data nascimento (PII)'}
)

foreach ($forbidden in $forbiddenClaims) {
    if ($claims.PSObject.Properties.Name.Contains($forbidden.Name)) {
        $violations += "🚨 VIOLAÇÃO CRÍTICA: Claim '$($forbidden.Name)' encontrada - $($forbidden.Desc)"
    }
}

# Validação de identificador opaco (sub deve ser UUID)
if ($claims.sub -notmatch '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$') {
    $violations += "⚠️ ATENÇÃO: 'sub' não é UUID (identificador pode não ser opaco)"
}

# Validação de expiração (access token deve ser curto)
$iat = [DateTimeOffset]::FromUnixTimeSeconds($claims.iat).DateTime
$exp = [DateTimeOffset]::FromUnixTimeSeconds($claims.exp).DateTime
$duration = ($exp - $iat).TotalMinutes

if ($duration -gt 15) {
    $violations += "⚠️ ATENÇÃO: Access token expira em $([math]::Round($duration, 2)) minutos (recomendado <= 15 minutos)"
}

# Resultado da validação
if ($violations.Count -eq 0) {
    Write-Host "✅ COMPLIANCE OK: Nenhuma violação de segurança detectada!" -ForegroundColor Green
    Write-Host "   - Claims obrigatórias presentes: $($requiredClaims -join ', ')" -ForegroundColor Gray
    Write-Host "   - Nenhuma claim proibida encontrada" -ForegroundColor Gray
    Write-Host "   - Identificador opaco (UUID) confirmado" -ForegroundColor Gray
    Write-Host "   - Expiração adequada: $([math]::Round($duration, 2)) minutos`n" -ForegroundColor Gray
} else {
    Write-Host "❌ VIOLAÇÕES DETECTADAS:" -ForegroundColor Red
    foreach ($violation in $violations) {
        Write-Host "   $violation" -ForegroundColor Red
    }
    Write-Host ""
    exit 1
}

# 4. REFRESH TOKEN
Write-Host "[4/5] REFRESH TOKEN: Renovando access token..." -ForegroundColor Green

$refreshBody = @{
    refreshToken = $loginResponse.refreshToken
} | ConvertTo-Json

try {
    $refreshResponse = Invoke-RestMethod -Uri "$baseUrl/auth/refresh" -Method POST -Body $refreshBody -ContentType "application/json"
    Write-Host "✅ Token renovado com sucesso" -ForegroundColor Green
    Write-Host "   - Novo Access Token: $($refreshResponse.accessToken.Substring(0, 50))..." -ForegroundColor Gray
    Write-Host "   - Novo Refresh Token: $($refreshResponse.refreshToken.Substring(0, 30))...`n" -ForegroundColor Gray
} catch {
    Write-Host "❌ Erro ao renovar token: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 5. VALIDAR NOVO TOKEN TAMBÉM É SEGURO
Write-Host "[5/5] VALIDANDO NOVO TOKEN: Verificando claims após refresh..." -ForegroundColor Green

$newTokenParts = $refreshResponse.accessToken -split '\.'
$newPayload = $newTokenParts[1]
$newPayload = $newPayload.Replace('-', '+').Replace('_', '/')
switch ($newPayload.Length % 4) {
    2 { $newPayload += '==' }
    3 { $newPayload += '=' }
}

$newDecodedBytes = [System.Convert]::FromBase64String($newPayload)
$newDecodedJson = [System.Text.Encoding]::UTF8.GetString($newDecodedBytes)
$newClaims = $newDecodedJson | ConvertFrom-Json

$newViolations = @()
foreach ($forbidden in $forbiddenClaims) {
    if ($newClaims.PSObject.Properties.Name.Contains($forbidden.Name)) {
        $newViolations += "🚨 VIOLAÇÃO: Claim '$($forbidden.Name)' no novo token"
    }
}

if ($newViolations.Count -eq 0) {
    Write-Host "✅ Novo token também está em compliance" -ForegroundColor Green
} else {
    Write-Host "❌ Novo token contém violações:" -ForegroundColor Red
    foreach ($violation in $newViolations) {
        Write-Host "   $violation" -ForegroundColor Red
    }
    exit 1
}

# RESUMO FINAL
Write-Host "`n=== RESULTADO FINAL ===" -ForegroundColor Cyan
Write-Host "✅ JWT Security Compliance: APROVADO" -ForegroundColor Green
Write-Host "✅ Access Token: Contém APENAS identificadores opacos" -ForegroundColor Green
Write-Host "✅ Refresh Token: Funcionando corretamente" -ForegroundColor Green
Write-Host "✅ LGPD: Nenhum dado pessoal (PII) em tokens" -ForegroundColor Green
Write-Host "✅ OWASP: Seguindo boas práticas de segurança" -ForegroundColor Green
Write-Host "`n🎉 Todos os testes de segurança passaram com sucesso!`n" -ForegroundColor Green
