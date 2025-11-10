#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Exibe informações de versão de todos os componentes BankMore

.DESCRIPTION
    Script para visualizar versões das APIs, Workers e imagens Docker
#>

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "   BankMore - Informações de Versão" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Versão da solução
Write-Host "SOLUÇÃO" -ForegroundColor Yellow
Write-Host "  Versão: 1.0.0" -ForegroundColor White
Write-Host "  Data: 2025-11-10" -ForegroundColor Gray
Write-Host "  Framework: .NET 9.0" -ForegroundColor Gray
Write-Host ""

# Componentes
Write-Host "COMPONENTES" -ForegroundColor Yellow
$components = @(
    @{Name="API Conta Corrente"; Version="1.0.0"; Port="5003"; Path="src/BankMore.ContaCorrente"},
    @{Name="API Transferência"; Version="1.0.0"; Port="5004"; Path="src/BankMore.Transferencia"},
    @{Name="Worker Tarifas"; Version="1.0.0"; Port="-"; Path="src/BankMore.Tarifas"},
    @{Name="Web Application"; Version="1.0.0"; Port="5000"; Path="src/BankMore.Web"}
)

foreach ($comp in $components) {
    Write-Host "  $($comp.Name)" -ForegroundColor White
    Write-Host "    Versão: $($comp.Version)" -ForegroundColor Gray
    Write-Host "    Porta: $($comp.Port)" -ForegroundColor Gray
    Write-Host ""
}

# Verificar containers rodando
Write-Host "CONTAINERS DOCKER" -ForegroundColor Yellow
try {
    $containers = docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}" --filter "name=bankmore" 2>$null
    if ($containers) {
        Write-Host $containers -ForegroundColor Gray
    } else {
        Write-Host "  Nenhum container rodando" -ForegroundColor Gray
        Write-Host "  Execute: .\docker-start.ps1" -ForegroundColor Yellow
    }
} catch {
    Write-Host "  Docker não disponível" -ForegroundColor Gray
}
Write-Host ""

# Verificar imagens
Write-Host "IMAGENS DOCKER" -ForegroundColor Yellow
try {
    $images = docker images --format "table {{.Repository}}\t{{.Tag}}\t{{.Size}}" --filter "reference=bankmore*" 2>$null
    if ($images) {
        Write-Host $images -ForegroundColor Gray
    } else {
        Write-Host "  Nenhuma imagem construída" -ForegroundColor Gray
        Write-Host "  Execute: docker-compose build" -ForegroundColor Yellow
    }
} catch {
    Write-Host "  Docker não disponível" -ForegroundColor Gray
}
Write-Host ""

# Endpoints de versão
Write-Host "ENDPOINTS DE VERSÃO (quando rodando)" -ForegroundColor Yellow
Write-Host "  API Conta: http://localhost:5003/api/v1/info" -ForegroundColor White
Write-Host "  API Transfer: http://localhost:5004/api/v1/info" -ForegroundColor White
Write-Host ""

# Health checks
Write-Host "HEALTH CHECKS (quando rodando)" -ForegroundColor Yellow
Write-Host "  API Conta: http://localhost:5003/api/v1/info/health" -ForegroundColor White
Write-Host "  API Transfer: http://localhost:5004/api/v1/info/health" -ForegroundColor White
Write-Host ""

# Testar se APIs estão respondendo
Write-Host "STATUS DAS APIs" -ForegroundColor Yellow
try {
    $responseConta = Invoke-RestMethod -Uri "http://localhost:5003/api/v1/info" -TimeoutSec 2 -ErrorAction SilentlyContinue
    if ($responseConta) {
        Write-Host "  ✓ API Conta: v$($responseConta.Version) - $($responseConta.Environment)" -ForegroundColor Green
    }
} catch {
    Write-Host "  ✗ API Conta: Offline" -ForegroundColor Gray
}

try {
    $responseTransfer = Invoke-RestMethod -Uri "http://localhost:5004/api/v1/info" -TimeoutSec 2 -ErrorAction SilentlyContinue
    if ($responseTransfer) {
        Write-Host "  ✓ API Transfer: v$($responseTransfer.Version) - $($responseTransfer.Environment)" -ForegroundColor Green
    }
} catch {
    Write-Host "  ✗ API Transfer: Offline" -ForegroundColor Gray
}
Write-Host ""

# Arquivos de versão
Write-Host "DOCUMENTAÇÃO" -ForegroundColor Yellow
Write-Host "  VERSION.md - Histórico de versões" -ForegroundColor Gray
Write-Host "  CHANGELOG.md - Log de mudanças" -ForegroundColor Gray
Write-Host "  Directory.Build.props - Configurações de build" -ForegroundColor Gray
Write-Host ""

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Para mais informações, consulte VERSION.md" -ForegroundColor Gray
Write-Host "============================================" -ForegroundColor Cyan
