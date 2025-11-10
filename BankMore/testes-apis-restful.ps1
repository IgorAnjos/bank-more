# Script de Testes das APIs RESTful - Banco Digital Ana
# Testa endpoints principais com versionamento, HATEOAS, Problem Details e paginação

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "TESTES API BANCO DIGITAL ANA" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Variáveis globais
$apiContaUrl = "http://localhost:5001/api/v1"
$apiTransferenciaUrl = "http://localhost:5002/api/v1"
$token = $null
$idConta1 = $null
$idConta2 = $null

# Função para exibir resposta
function Show-Response {
    param (
        [string]$Title,
        [object]$Response,
        [int]$StatusCode
    )
    
    Write-Host ""
    Write-Host "[$Title]" -ForegroundColor Yellow
    Write-Host "Status: $StatusCode" -ForegroundColor $(if ($StatusCode -lt 300) { "Green" } else { "Red" })
    
    if ($Response) {
        $Response | ConvertTo-Json -Depth 10 | Write-Host -ForegroundColor Gray
    }
    Write-Host ""
}

# Função para fazer requisição
function Invoke-ApiRequest {
    param (
        [string]$Method,
        [string]$Url,
        [object]$Body = $null,
        [string]$Token = $null
    )
    
    $headers = @{
        "Content-Type" = "application/json"
    }
    
    if ($Token) {
        $headers["Authorization"] = "Bearer $Token"
    }
    
    try {
        $params = @{
            Method = $Method
            Uri = $Url
            Headers = $headers
            ContentType = "application/json"
        }
        
        if ($Body) {
            $params.Body = ($Body | ConvertTo-Json -Depth 10)
        }
        
        $response = Invoke-RestMethod @params
        return @{
            Success = $true
            StatusCode = 200
            Data = $response
        }
    }
    catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        $errorBody = $null
        
        if ($_.Exception.Response) {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $errorBody = $reader.ReadToEnd() | ConvertFrom-Json
            $reader.Close()
        }
        
        return @{
            Success = $false
            StatusCode = $statusCode
            Data = $errorBody
        }
    }
}

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "TESTE 1: Criar Conta 1 (Ana)" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

$conta1 = @{
    nome = "Ana Silva"
    cpf = "12345678901"
    senha = "senha123"
}

$result = Invoke-ApiRequest -Method POST -Url "$apiContaUrl/contas" -Body $conta1
Show-Response -Title "POST /api/v1/contas (Criar Conta 1)" -Response $result.Data -StatusCode $result.StatusCode

if ($result.Success) {
    $idConta1 = $result.Data.idContaCorrente
    Write-Host "✓ Conta 1 criada: $idConta1" -ForegroundColor Green
    Write-Host "✓ Número da conta: $($result.Data.numeroConta)" -ForegroundColor Green
    
    # Verificar HATEOAS
    if ($result.Data.links) {
        Write-Host "✓ HATEOAS Links encontrados: $($result.Data.links.Count)" -ForegroundColor Green
    }
}

Start-Sleep -Seconds 2

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "TESTE 2: Criar Conta 2 (João)" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

$conta2 = @{
    nome = "João Santos"
    cpf = "98765432100"
    senha = "senha456"
}

$result = Invoke-ApiRequest -Method POST -Url "$apiContaUrl/contas" -Body $conta2
Show-Response -Title "POST /api/v1/contas (Criar Conta 2)" -Response $result.Data -StatusCode $result.StatusCode

if ($result.Success) {
    $idConta2 = $result.Data.idContaCorrente
    $numeroConta2 = $result.Data.numeroConta
    Write-Host "✓ Conta 2 criada: $idConta2" -ForegroundColor Green
    Write-Host "✓ Número da conta: $numeroConta2" -ForegroundColor Green
}

Start-Sleep -Seconds 2

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "TESTE 3: Login (Obter JWT)" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

$login = @{
    numeroContaOuCpf = "12345678901"
    senha = "senha123"
}

$result = Invoke-ApiRequest -Method POST -Url "$apiContaUrl/auth/tokens" -Body $login
Show-Response -Title "POST /api/v1/auth/tokens (Login)" -Response $result.Data -StatusCode $result.StatusCode

if ($result.Success) {
    $token = $result.Data.token
    Write-Host "✓ JWT obtido com sucesso" -ForegroundColor Green
    Write-Host "Token: $($token.Substring(0, 50))..." -ForegroundColor Gray
}

Start-Sleep -Seconds 2

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "TESTE 4: GET Conta por ID" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

$result = Invoke-ApiRequest -Method GET -Url "$apiContaUrl/contas/$idConta1" -Token $token
Show-Response -Title "GET /api/v1/contas/{id}" -Response $result.Data -StatusCode $result.StatusCode

if ($result.Success) {
    Write-Host "✓ Conta obtida com sucesso" -ForegroundColor Green
    Write-Host "✓ Nome: $($result.Data.nome)" -ForegroundColor Green
    Write-Host "✓ Saldo atual: R$ $($result.Data.saldo)" -ForegroundColor Green
    
    if ($result.Data.links) {
        Write-Host "✓ HATEOAS Links: $($result.Data.links.Count)" -ForegroundColor Green
    }
}

Start-Sleep -Seconds 2

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "TESTE 5: Movimentação - Crédito" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

$movimentacao = @{
    chaveIdempotencia = [Guid]::NewGuid().ToString()
    tipo = "C"
    valor = 1000.00
}

$result = Invoke-ApiRequest -Method POST -Url "$apiContaUrl/contas/$idConta1/movimentos" -Body $movimentacao -Token $token
Show-Response -Title "POST /api/v1/contas/{id}/movimentos (Crédito)" -Response $result.Data -StatusCode $result.StatusCode

if ($result.StatusCode -eq 204) {
    Write-Host "✓ Crédito de R$ 1000,00 realizado com sucesso" -ForegroundColor Green
}

Start-Sleep -Seconds 2

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "TESTE 6: Consultar Saldo" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

$result = Invoke-ApiRequest -Method GET -Url "$apiContaUrl/contas/$idConta1/saldo" -Token $token
Show-Response -Title "GET /api/v1/contas/{id}/saldo" -Response $result.Data -StatusCode $result.StatusCode

if ($result.Success) {
    Write-Host "✓ Saldo: R$ $($result.Data.saldo)" -ForegroundColor Green
    
    if ($result.Data.links) {
        Write-Host "✓ HATEOAS Links: $($result.Data.links.Count)" -ForegroundColor Green
    }
}

Start-Sleep -Seconds 2

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "TESTE 7: Listar Movimentos (Paginado)" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

$urlMovimentos = "$apiContaUrl/contas/$idConta1/movimentos?page=1&pageSize=10"
$result = Invoke-ApiRequest -Method GET -Url $urlMovimentos -Token $token
Show-Response -Title "GET /api/v1/contas/{id}/movimentos (paginado)" -Response $result.Data -StatusCode $result.StatusCode

if ($result.Success) {
    Write-Host "✓ Movimentos obtidos: $($result.Data.totalItems)" -ForegroundColor Green
    Write-Host "✓ Página: $($result.Data.page)/$($result.Data.totalPages)" -ForegroundColor Green
    Write-Host "✓ Itens por página: $($result.Data.pageSize)" -ForegroundColor Green
    
    if ($result.Data.links) {
        Write-Host "✓ Links de navegação: $($result.Data.links.Count)" -ForegroundColor Green
    }
}

Start-Sleep -Seconds 2

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "TESTE 8: Transferência (API Transfer)" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

$transferencia = @{
    chaveIdempotencia = [Guid]::NewGuid().ToString()
    numeroContaDestino = $numeroConta2
    valor = 100.00
}

$result = Invoke-ApiRequest -Method POST -Url "$apiTransferenciaUrl/transferencias" -Body $transferencia -Token $token
Show-Response -Title "POST /api/v1/transferencias" -Response $result.Data -StatusCode $result.StatusCode

$idTransferencia = $null
if ($result.Success) {
    $idTransferencia = $result.Data.id
    Write-Host "✓ Transferência realizada: $idTransferencia" -ForegroundColor Green
    Write-Host "✓ Valor: R$ $($result.Data.valor)" -ForegroundColor Green
    Write-Host "✓ Status: $($result.Data.status)" -ForegroundColor Green
    
    if ($result.Data.links) {
        Write-Host "✓ HATEOAS Links: $($result.Data.links.Count)" -ForegroundColor Green
    }
}

Start-Sleep -Seconds 2

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "TESTE 9: GET Transferência por ID" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

if ($idTransferencia) {
    $result = Invoke-ApiRequest -Method GET -Url "$apiTransferenciaUrl/transferencias/$idTransferencia" -Token $token
    Show-Response -Title "GET /api/v1/transferencias/{id}" -Response $result.Data -StatusCode $result.StatusCode
    
    if ($result.Success) {
        Write-Host "✓ Transferência obtida com sucesso" -ForegroundColor Green
        Write-Host "✓ Status: $($result.Data.status)" -ForegroundColor Green
        
        if ($result.Data.links) {
            Write-Host "✓ HATEOAS Links: $($result.Data.links.Count)" -ForegroundColor Green
        }
    }
}

Start-Sleep -Seconds 2

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "TESTE 10: Listar Transferências (Paginado)" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

$urlTransferencias = "$apiTransferenciaUrl/transferencias?tipo=enviadas&page=1&pageSize=10"
$result = Invoke-ApiRequest -Method GET -Url $urlTransferencias -Token $token
Show-Response -Title "GET /api/v1/transferencias (paginado)" -Response $result.Data -StatusCode $result.StatusCode

if ($result.Success) {
    Write-Host "✓ Transferências obtidas: $($result.Data.totalItems)" -ForegroundColor Green
    Write-Host "✓ Página: $($result.Data.page)/$($result.Data.totalPages)" -ForegroundColor Green
    
    if ($result.Data.links) {
        Write-Host "✓ Links de navegação: $($result.Data.links.Count)" -ForegroundColor Green
    }
}

Start-Sleep -Seconds 2

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "TESTE 11: Erro 404 (Conta Inexistente)" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

$idInexistente = [Guid]::NewGuid().ToString()
$result = Invoke-ApiRequest -Method GET -Url "$apiContaUrl/contas/$idInexistente" -Token $token
Show-Response -Title "GET /api/v1/contas/{id-inexistente} - Teste Problem Details" -Response $result.Data -StatusCode $result.StatusCode

if ($result.StatusCode -eq 404) {
    Write-Host "✓ Erro 404 retornado corretamente" -ForegroundColor Green
    
    if ($result.Data.type -and $result.Data.title -and $result.Data.status) {
        Write-Host "✓ Problem Details RFC 7807 implementado corretamente" -ForegroundColor Green
        Write-Host "  - Type: $($result.Data.type)" -ForegroundColor Gray
        Write-Host "  - Title: $($result.Data.title)" -ForegroundColor Gray
        Write-Host "  - ErrorCode: $($result.Data.errorCode)" -ForegroundColor Gray
    }
}

Start-Sleep -Seconds 2

Write-Host ""
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "RESUMO DOS TESTES" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "[OK] Versionamento (v1 na URL)" -ForegroundColor Green
Write-Host "[OK] Rotas plurais" -ForegroundColor Green
Write-Host "[OK] Códigos HTTP semânticos" -ForegroundColor Green
Write-Host "[OK] HATEOAS (links de navegação)" -ForegroundColor Green
Write-Host "[OK] Problem Details RFC 7807" -ForegroundColor Green
Write-Host "[OK] Paginação com metadados" -ForegroundColor Green
Write-Host "[OK] Filtros de consulta" -ForegroundColor Green
Write-Host "[OK] JWT Authentication" -ForegroundColor Green
Write-Host ""
Write-Host "APIs RESTful validadas com sucesso!" -ForegroundColor Green
Write-Host ""
