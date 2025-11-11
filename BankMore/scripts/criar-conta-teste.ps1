# Script para criar conta de teste
$body = @{
    cpf = "52998224725"
    nome = "Usuário Teste"
    senha = "Senha123!"
} | ConvertTo-Json

Write-Host "Criando conta de teste..." -ForegroundColor Yellow
Write-Host "Body: $body" -ForegroundColor Cyan

try {
    $response = Invoke-RestMethod -Method POST `
        -Uri "http://localhost:5003/api/v1/contas" `
        -ContentType "application/json" `
        -Body $body
    
    Write-Host "`nConta criada com sucesso!" -ForegroundColor Green
    Write-Host "ID: $($response.id)" -ForegroundColor White
    Write-Host "Número da Conta: $($response.numeroContaCorrente)" -ForegroundColor White
    Write-Host "Nome: $($response.nomeCompleto)" -ForegroundColor White
    
    # Agora tenta fazer login
    Write-Host "`n----------------------------" -ForegroundColor Gray
    Write-Host "Testando login..." -ForegroundColor Yellow
    
    $loginBody = @{
        numeroOuCpf = "52998224725"
        senha = "Senha123!"
    } | ConvertTo-Json
    
    $loginResponse = Invoke-RestMethod -Method POST `
        -Uri "http://localhost:5003/api/v1/auth/tokens" `
        -ContentType "application/json" `
        -Body $loginBody
    
    Write-Host "Login realizado com sucesso!" -ForegroundColor Green
    Write-Host "Token: $($loginResponse.token.Substring(0, 50))..." -ForegroundColor White
}
catch {
    Write-Host "`nErro:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $reader.BaseStream.Position = 0
        $reader.DiscardBufferedData()
        $responseBody = $reader.ReadToEnd()
        Write-Host "Detalhes: $responseBody" -ForegroundColor Yellow
    }
}
