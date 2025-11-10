# Teste de Integracao APIs
Write-Host "TESTE DE INTEGRACAO" -ForegroundColor Cyan

$apiConta = "http://localhost:5003"
$apiTransferencia = "http://localhost:5004"

# 1. Cadastrar Conta Origem
Write-Host "[1] Cadastrando Conta Origem..." -ForegroundColor Yellow
$cadastroOrigem = @{
    cpf = "11144477735"
    nome = "Joao Silva"
    senha = "senha123"
} | ConvertTo-Json

$responseOrigem = Invoke-RestMethod -Uri "$apiConta/api/conta" -Method Post -Body $cadastroOrigem -ContentType "application/json"
$numeroContaOrigem = $responseOrigem.numeroConta
Write-Host "Conta Origem criada: $numeroContaOrigem" -ForegroundColor Green

# 2. Cadastrar Conta Destino
Write-Host "[2] Cadastrando Conta Destino..." -ForegroundColor Yellow
$cadastroDestino = @{
    cpf = "52998224725"
    nome = "Maria Santos"
    senha = "senha456"
} | ConvertTo-Json

$responseDestino = Invoke-RestMethod -Uri "$apiConta/api/conta" -Method Post -Body $cadastroDestino -ContentType "application/json"
$numeroContaDestino = $responseDestino.numeroConta
Write-Host "Conta Destino criada: $numeroContaDestino" -ForegroundColor Green

# 3. Login
Write-Host "[3] Fazendo login na Conta Origem..." -ForegroundColor Yellow
$login = @{
    numeroConta = $numeroContaOrigem
    senha = "senha123"
} | ConvertTo-Json

$responseLogin = Invoke-RestMethod -Uri "$apiConta/api/auth/login" -Method Post -Body $login -ContentType "application/json"
$token = $responseLogin.token
Write-Host "Token obtido" -ForegroundColor Green

# 4. Creditar
Write-Host "[4] Creditando R$ 500,00..." -ForegroundColor Yellow
$credito = @{
    chaveIdempotencia = [guid]::NewGuid().ToString()
    tipoMovimento = "C"
    valor = 500.00
} | ConvertTo-Json

$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

Invoke-RestMethod -Uri "$apiConta/api/movimentacao" -Method Post -Body $credito -Headers $headers | Out-Null
Write-Host "Credito realizado" -ForegroundColor Green

# 5. Consultar Saldo
Write-Host "[5] Consultando saldo..." -ForegroundColor Yellow
$saldoOrigem = Invoke-RestMethod -Uri "$apiConta/api/conta/saldo" -Method Get -Headers $headers
Write-Host "Saldo atual: R$ $($saldoOrigem.saldo)" -ForegroundColor Green

# 6. Transferir
Write-Host "[6] Realizando transferencia de R$ 150,00..." -ForegroundColor Yellow
$transferencia = @{
    chaveIdempotencia = [guid]::NewGuid().ToString()
    numeroContaDestino = $numeroContaDestino
    valor = 150.00
} | ConvertTo-Json

try {
    $responseTransf = Invoke-RestMethod -Uri "$apiTransferencia/api/transferencia" -Method Post -Body $transferencia -Headers $headers
    Write-Host "Transferencia OK!" -ForegroundColor Green
} catch {
    Write-Host "ERRO: $($_.Exception.Message)" -ForegroundColor Red
}

# 7. Verificar Saldo Final
Write-Host "[7] Verificando saldo final..." -ForegroundColor Yellow
$saldoFinal = Invoke-RestMethod -Uri "$apiConta/api/conta/saldo" -Method Get -Headers $headers
Write-Host "Saldo final Origem: R$ $($saldoFinal.saldo)" -ForegroundColor Green

# 8. Login Destino
Write-Host "[8] Verificando saldo Destino..." -ForegroundColor Yellow
$loginDestino = @{
    numeroConta = $numeroContaDestino
    senha = "senha456"
} | ConvertTo-Json

$responseLoginDestino = Invoke-RestMethod -Uri "$apiConta/api/auth/login" -Method Post -Body $loginDestino -ContentType "application/json"
$tokenDestino = $responseLoginDestino.token
$headersDestino = @{
    "Authorization" = "Bearer $tokenDestino"
}

$saldoDestino = Invoke-RestMethod -Uri "$apiConta/api/conta/saldo" -Method Get -Headers $headersDestino
Write-Host "Saldo Destino: R$ $($saldoDestino.saldo)" -ForegroundColor Green

Write-Host "`nTESTE CONCLUIDO!" -ForegroundColor Cyan
Write-Host "Origem: $numeroContaOrigem - Saldo: R$ $($saldoFinal.saldo)" -ForegroundColor White
Write-Host "Destino: $numeroContaDestino - Saldo: R$ $($saldoDestino.saldo)" -ForegroundColor White
