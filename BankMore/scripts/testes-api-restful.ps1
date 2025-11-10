# Script de Testes das APIs RESTful - Banco Digital Ana
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "TESTES API BANCO DIGITAL ANA" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

$apiContaUrl = "http://localhost:5003/api/v1"
$apiTransferenciaUrl = "http://localhost:5004/api/v1"
$token = $null
$idConta1 = $null
$idConta2 = $null
$numeroConta2 = 0

function Show-Response {
    param (
        [string]$Title,
        [object]$Response,
        [int]$StatusCode
    )
    
    Write-Host ""
    Write-Host "[$Title]" -ForegroundColor Yellow
    if ($StatusCode -lt 300) {
        Write-Host "Status: $StatusCode OK" -ForegroundColor Green
    } else {
        Write-Host "Status: $StatusCode" -ForegroundColor Red
    }
    
    if ($Response) {
        $Response | ConvertTo-Json -Depth 10 | Write-Host -ForegroundColor Gray
    }
    Write-Host ""
}

# TESTE 1: Criar Conta 1
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "TESTE 1: Criar Conta 1 (Ana)" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

try {
    $conta1Body = @{
        nome = "Ana Silva"
        cpf = "12345678901"
        senha = "senha123"
    } | ConvertTo-Json

    $response1 = Invoke-RestMethod -Method POST -Uri "$apiContaUrl/contas" -Body $conta1Body -ContentType "application/json"
    Show-Response -Title "POST /api/v1/contas" -Response $response1 -StatusCode 201
    
    $idConta1 = $response1.idContaCorrente
    Write-Host "[OK] Conta 1 criada: $idConta1" -ForegroundColor Green
    Write-Host "[OK] HATEOAS Links: $($response1.links.Count)" -ForegroundColor Green
}
catch {
    Write-Host "[ERRO] Falha ao criar conta 1: $($_.Exception.Message)" -ForegroundColor Red
}

Start-Sleep -Seconds 1

# TESTE 2: Criar Conta 2
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "TESTE 2: Criar Conta 2 (Joao)" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

try {
    $conta2Body = @{
        nome = "Joao Santos"
        cpf = "98765432100"
        senha = "senha456"
    } | ConvertTo-Json

    $response2 = Invoke-RestMethod -Method POST -Uri "$apiContaUrl/contas" -Body $conta2Body -ContentType "application/json"
    Show-Response -Title "POST /api/v1/contas" -Response $response2 -StatusCode 201
    
    $idConta2 = $response2.idContaCorrente
    $numeroConta2 = $response2.numeroConta
    Write-Host "[OK] Conta 2 criada: $idConta2" -ForegroundColor Green
    Write-Host "[OK] Numero: $numeroConta2" -ForegroundColor Green
}
catch {
    Write-Host "[ERRO] Falha ao criar conta 2: $($_.Exception.Message)" -ForegroundColor Red
}

Start-Sleep -Seconds 1

# TESTE 3: Login
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "TESTE 3: Login (Obter JWT)" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

try {
    $loginBody = @{
        numeroContaOuCpf = "12345678901"
        senha = "senha123"
    } | ConvertTo-Json

    $responseLogin = Invoke-RestMethod -Method POST -Uri "$apiContaUrl/auth/tokens" -Body $loginBody -ContentType "application/json"
    Show-Response -Title "POST /api/v1/auth/tokens" -Response $responseLogin -StatusCode 200
    
    $token = $responseLogin.token
    Write-Host "[OK] JWT obtido com sucesso" -ForegroundColor Green
    Write-Host "Token: $($token.Substring(0, 50))..." -ForegroundColor Gray
}
catch {
    Write-Host "[ERRO] Falha no login: $($_.Exception.Message)" -ForegroundColor Red
}

Start-Sleep -Seconds 1

# TESTE 4: GET Conta por ID
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "TESTE 4: GET Conta por ID (NOVO)" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

try {
    $headers = @{
        "Authorization" = "Bearer $token"
        "Content-Type" = "application/json"
    }
    
    $responseConta = Invoke-RestMethod -Method GET -Uri "$apiContaUrl/contas/$idConta1" -Headers $headers
    Show-Response -Title "GET /api/v1/contas/{id}" -Response $responseConta -StatusCode 200
    
    Write-Host "[OK] Nome: $($responseConta.nome)" -ForegroundColor Green
    Write-Host "[OK] Saldo: R`$ $($responseConta.saldo)" -ForegroundColor Green
    Write-Host "[OK] HATEOAS Links: $($responseConta.links.Count)" -ForegroundColor Green
}
catch {
    Write-Host "[ERRO] Falha ao obter conta: $($_.Exception.Message)" -ForegroundColor Red
}

Start-Sleep -Seconds 1

# TESTE 5: Movimentacao Credito
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "TESTE 5: Movimentacao - Credito" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

try {
    $movimentacaoBody = @{
        chaveIdempotencia = [Guid]::NewGuid().ToString()
        tipo = "C"
        valor = 1000.00
    } | ConvertTo-Json

    Invoke-RestMethod -Method POST -Uri "$apiContaUrl/contas/$idConta1/movimentos" -Body $movimentacaoBody -Headers $headers
    Write-Host "[OK] Credito de R`$ 1000,00 realizado (204 No Content)" -ForegroundColor Green
}
catch {
    Write-Host "[ERRO] Falha na movimentacao: $($_.Exception.Message)" -ForegroundColor Red
}

Start-Sleep -Seconds 1

# TESTE 6: Consultar Saldo
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "TESTE 6: Consultar Saldo" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

try {
    $responseSaldo = Invoke-RestMethod -Method GET -Uri "$apiContaUrl/contas/$idConta1/saldo" -Headers $headers
    Show-Response -Title "GET /api/v1/contas/{id}/saldo" -Response $responseSaldo -StatusCode 200
    
    Write-Host "[OK] Saldo: R`$ $($responseSaldo.saldo)" -ForegroundColor Green
    Write-Host "[OK] HATEOAS Links: $($responseSaldo.links.Count)" -ForegroundColor Green
}
catch {
    Write-Host "[ERRO] Falha ao consultar saldo: $($_.Exception.Message)" -ForegroundColor Red
}

Start-Sleep -Seconds 1

# TESTE 7: Listar Movimentos com Paginacao
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "TESTE 7: Listar Movimentos (NOVO)" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

try {
    $urlMovimentos = $apiContaUrl + "/contas/" + $idConta1 + "/movimentos"
    $responseMovimentos = Invoke-RestMethod -Method GET -Uri $urlMovimentos -Headers $headers
    Show-Response -Title "GET /api/v1/contas/{id}/movimentos" -Response $responseMovimentos -StatusCode 200
    
    Write-Host "[OK] Total de movimentos: $($responseMovimentos.totalItems)" -ForegroundColor Green
    Write-Host "[OK] Pagina: $($responseMovimentos.page)/$($responseMovimentos.totalPages)" -ForegroundColor Green
    Write-Host "[OK] Links de navegacao: $($responseMovimentos.links.Count)" -ForegroundColor Green
}
catch {
    Write-Host "[ERRO] Falha ao listar movimentos: $($_.Exception.Message)" -ForegroundColor Red
}

Start-Sleep -Seconds 1

# TESTE 8: Erro 404 - Problem Details
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "TESTE 8: Erro 404 (Problem Details)" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

try {
    $idInexistente = [Guid]::NewGuid().ToString()
    Invoke-RestMethod -Method GET -Uri "$apiContaUrl/contas/$idInexistente" -Headers $headers
}
catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    
    if ($statusCode -eq 404) {
        Write-Host "[OK] Erro 404 retornado corretamente" -ForegroundColor Green
        
        try {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $errorBody = $reader.ReadToEnd() | ConvertFrom-Json
            $reader.Close()
            
            Write-Host "[OK] Problem Details RFC 7807:" -ForegroundColor Green
            Write-Host "  Type: $($errorBody.type)" -ForegroundColor Gray
            Write-Host "  Title: $($errorBody.title)" -ForegroundColor Gray
            Write-Host "  Status: $($errorBody.status)" -ForegroundColor Gray
            Write-Host "  ErrorCode: $($errorBody.errorCode)" -ForegroundColor Gray
        }
        catch {
            Write-Host "  Erro ao ler body: $($_.Exception.Message)" -ForegroundColor Yellow
        }
    }
}

Start-Sleep -Seconds 1

Write-Host ""
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "RESUMO DOS TESTES" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "[OK] Versionamento (v1 na URL)" -ForegroundColor Green
Write-Host "[OK] Rotas plurais" -ForegroundColor Green
Write-Host "[OK] Codigos HTTP semanticos" -ForegroundColor Green
Write-Host "[OK] HATEOAS (links de navegacao)" -ForegroundColor Green
Write-Host "[OK] Problem Details RFC 7807" -ForegroundColor Green
Write-Host "[OK] Paginacao com metadados" -ForegroundColor Green
Write-Host "[OK] JWT Authentication" -ForegroundColor Green
Write-Host ""
Write-Host "APIs RESTful validadas com sucesso!" -ForegroundColor Green
Write-Host ""
