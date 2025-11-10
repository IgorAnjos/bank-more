#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Script para iniciar toda a aplicação BankMore via Docker Compose

.DESCRIPTION
    Este script:
    1. Para e remove containers antigos
    2. Limpa volumes (opcional)
    3. Reconstrói as imagens Docker
    4. Inicia todos os serviços
    5. Aguarda os health checks
    6. Exibe logs e URLs de acesso

.PARAMETER Clean
    Remove volumes e faz rebuild completo das imagens

.PARAMETER Logs
    Exibe logs dos containers após inicialização

.EXAMPLE
    .\docker-start.ps1
    .\docker-start.ps1 -Clean
    .\docker-start.ps1 -Logs
#>

param(
    [switch]$Clean,
    [switch]$Logs
)

$ErrorActionPreference = "Stop"

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "   BankMore - Iniciando via Docker Compose" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Verificar se Docker está rodando
Write-Host "Verificando Docker..." -ForegroundColor Yellow
try {
    docker version | Out-Null
    Write-Host "✓ Docker está rodando" -ForegroundColor Green
} catch {
    Write-Host "✗ Docker não está rodando. Por favor, inicie o Docker Desktop." -ForegroundColor Red
    exit 1
}

# Parar containers existentes
Write-Host "`nParando containers existentes..." -ForegroundColor Yellow
docker-compose down 2>$null

if ($Clean) {
    Write-Host "`nRemovendo volumes (limpeza completa)..." -ForegroundColor Yellow
    docker-compose down -v
    
    Write-Host "`nRemovendo imagens antigas..." -ForegroundColor Yellow
    docker images "bankmore*" -q | ForEach-Object { docker rmi $_ -f 2>$null }
}

# Construir imagens
Write-Host "`nConstruindo imagens Docker..." -ForegroundColor Yellow
Write-Host "(Isso pode levar alguns minutos na primeira vez)" -ForegroundColor Gray
docker-compose build --no-cache

if ($LASTEXITCODE -ne 0) {
    Write-Host "`n✗ Erro ao construir as imagens" -ForegroundColor Red
    exit 1
}

Write-Host "✓ Imagens construídas com sucesso" -ForegroundColor Green

# Iniciar serviços
Write-Host "`nIniciando serviços..." -ForegroundColor Yellow
docker-compose up -d

if ($LASTEXITCODE -ne 0) {
    Write-Host "`n✗ Erro ao iniciar os serviços" -ForegroundColor Red
    exit 1
}

# Aguardar serviços ficarem prontos
Write-Host "`nAguardando serviços ficarem prontos..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Verificar status dos containers
Write-Host "`nStatus dos containers:" -ForegroundColor Yellow
docker-compose ps

# URLs de acesso
Write-Host "`n============================================" -ForegroundColor Cyan
Write-Host "   Aplicação iniciada com sucesso!" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "URLs de Acesso:" -ForegroundColor Yellow
Write-Host "  • Web App (Blazor):        http://localhost:5000" -ForegroundColor White
Write-Host "  • API Conta Corrente:      http://localhost:5003/swagger" -ForegroundColor White
Write-Host "  • API Transferência:       http://localhost:5004/swagger" -ForegroundColor White
Write-Host ""
Write-Host "Serviços Internos:" -ForegroundColor Yellow
Write-Host "  • Kafka:                   localhost:9092" -ForegroundColor Gray
Write-Host "  • Zookeeper:               localhost:2181" -ForegroundColor Gray
Write-Host "  • Redis:                   localhost:6379" -ForegroundColor Gray
Write-Host ""
Write-Host "Comandos úteis:" -ForegroundColor Yellow
Write-Host "  • Ver logs:                docker-compose logs -f [serviço]" -ForegroundColor Gray
Write-Host "  • Parar tudo:              docker-compose down" -ForegroundColor Gray
Write-Host "  • Reiniciar serviço:       docker-compose restart [serviço]" -ForegroundColor Gray
Write-Host "  • Ver status:              docker-compose ps" -ForegroundColor Gray
Write-Host ""

if ($Logs) {
    Write-Host "Exibindo logs (Ctrl+C para sair)..." -ForegroundColor Yellow
    docker-compose logs -f
}
