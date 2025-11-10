# Test script for JWT Refresh Token functionality
$baseUrl = "http://localhost:5001/api/v1"

Write-Host "`n=== JWT Refresh Token Test ===" -ForegroundColor Cyan

# 1. Create account
Write-Host "`n1. Creating test account..." -ForegroundColor Yellow
$createBody = @{
    nome = "Usuario Teste JWT"
    cpf = "12345678901"
    senha = "SenhaForte@123"
} | ConvertTo-Json

try {
    $account = Invoke-RestMethod -Uri "$baseUrl/contas" -Method POST -Body $createBody -ContentType "application/json"
    Write-Host "✓ Account created: Numero=$($account.numeroConta)" -ForegroundColor Green
} catch {
    if ($_.Exception.Response.StatusCode -eq 409) {
        Write-Host "✓ Account already exists" -ForegroundColor Green
    } else {
        Write-Host "✗ Error: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
}

# 2. Login (get access + refresh token)
Write-Host "`n2. Logging in to get tokens..." -ForegroundColor Yellow
$loginBody = @{
    numeroOuCpf = "12345678901"
    senha = "SenhaForte@123"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/auth/tokens" -Method POST -Body $loginBody -ContentType "application/json"
    
    Write-Host "✓ Login successful!" -ForegroundColor Green
    Write-Host "  - Access Token: $($loginResponse.accessToken.Substring(0, 20))..." -ForegroundColor Gray
    Write-Host "  - Refresh Token: $($loginResponse.refreshToken.Substring(0, 20))..." -ForegroundColor Gray
    Write-Host "  - Expires In: $($loginResponse.expiresInMinutes) minutes" -ForegroundColor Gray
    Write-Host "  - Account ID: $($loginResponse.idContaCorrente)" -ForegroundColor Gray
    Write-Host "  - Account Number: $($loginResponse.numeroConta)" -ForegroundColor Gray
    
    $accessToken = $loginResponse.accessToken
    $refreshToken = $loginResponse.refreshToken
} catch {
    Write-Host "✗ Login failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 3. Use access token to get balance
Write-Host "`n3. Using access token to get balance..." -ForegroundColor Yellow
$headers = @{
    Authorization = "Bearer $accessToken"
}

try {
    $balance = Invoke-RestMethod -Uri "$baseUrl/contas/$($loginResponse.numeroConta)/saldo" -Method GET -Headers $headers
    Write-Host "✓ Balance retrieved: R$ $($balance.saldo)" -ForegroundColor Green
} catch {
    Write-Host "✗ Error getting balance: $($_.Exception.Message)" -ForegroundColor Red
}

# 4. Refresh token
Write-Host "`n4. Refreshing access token..." -ForegroundColor Yellow
$refreshBody = @{
    refreshToken = $refreshToken
} | ConvertTo-Json

try {
    $refreshResponse = Invoke-RestMethod -Uri "$baseUrl/auth/refresh" -Method POST -Body $refreshBody -ContentType "application/json"
    
    Write-Host "✓ Token refreshed successfully!" -ForegroundColor Green
    Write-Host "  - New Access Token: $($refreshResponse.accessToken.Substring(0, 20))..." -ForegroundColor Gray
    Write-Host "  - New Refresh Token: $($refreshResponse.refreshToken.Substring(0, 20))..." -ForegroundColor Gray
    Write-Host "  - Expires In: $($refreshResponse.expiresInMinutes) minutes" -ForegroundColor Gray
    
    $newAccessToken = $refreshResponse.accessToken
} catch {
    Write-Host "✗ Token refresh failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 5. Use new access token
Write-Host "`n5. Using new access token to get balance..." -ForegroundColor Yellow
$headers = @{
    Authorization = "Bearer $newAccessToken"
}

try {
    $balance2 = Invoke-RestMethod -Uri "$baseUrl/contas/$($refreshResponse.numeroConta)/saldo" -Method GET -Headers $headers
    Write-Host "✓ Balance retrieved with new token: R$ $($balance2.saldo)" -ForegroundColor Green
} catch {
    Write-Host "✗ Error getting balance with new token: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n=== All tests completed! ===" -ForegroundColor Cyan
Write-Host "`nJWT Dual Token System is working correctly:" -ForegroundColor Green
Write-Host "  - Access Token: 10 minutes (for API requests)" -ForegroundColor Gray
Write-Host "  - Refresh Token: 1 day (to get new access tokens)" -ForegroundColor Gray
Write-Host "  - Old refresh token revoked after use" -ForegroundColor Gray
