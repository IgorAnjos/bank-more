#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Verifica pré-requisitos para rodar BankMore via Docker

.DESCRIPTION
    Verifica se Docker está instalado, rodando, e se as portas necessárias estão disponíveis
#>

$ErrorActionPreference = "Continue"

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "   BankMore - Verificação de Requisitos" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

$allOk = $true

# 1. Verificar Docker
Write-Host "1. Verificando Docker..." -ForegroundColor Yellow
try {
    $dockerVersion = docker --version 2>$null
    if ($dockerVersion) {
        Write-Host "   ✓ Docker instalado: $dockerVersion" -ForegroundColor Green
    } else {
        Write-Host "   ✗ Docker não está instalado" -ForegroundColor Red
        Write-Host "     Baixe em: https://www.docker.com/products/docker-desktop/" -ForegroundColor Yellow
        $allOk = $false
    }
} catch {
    Write-Host "   ✗ Docker não está instalado ou acessível" -ForegroundColor Red
    $allOk = $false
}

# 2. Verificar se Docker está rodando
Write-Host "`n2. Verificando se Docker está rodando..." -ForegroundColor Yellow
try {
    docker ps 2>$null | Out-Null
    Write-Host "   ✓ Docker está rodando" -ForegroundColor Green
} catch {
    Write-Host "   ✗ Docker não está rodando" -ForegroundColor Red
    Write-Host "     Inicie o Docker Desktop" -ForegroundColor Yellow
    $allOk = $false
}

# 3. Verificar Docker Compose
Write-Host "`n3. Verificando Docker Compose..." -ForegroundColor Yellow
try {
    $composeVersion = docker-compose --version 2>$null
    if ($composeVersion) {
        Write-Host "   ✓ Docker Compose: $composeVersion" -ForegroundColor Green
    } else {
        Write-Host "   ✗ Docker Compose não encontrado" -ForegroundColor Red
        $allOk = $false
    }
} catch {
    Write-Host "   ✗ Docker Compose não disponível" -ForegroundColor Red
    $allOk = $false
}

# 4. Verificar portas disponíveis
Write-Host "`n4. Verificando portas necessárias..." -ForegroundColor Yellow
$ports = @(5000, 5003, 5004, 9092, 6379, 2181)
$portsOk = $true

foreach ($port in $ports) {
    try {
        $connection = Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue
        if ($connection) {
            Write-Host "   ✗ Porta $port está em uso (PID: $($connection.OwningProcess))" -ForegroundColor Red
            $portsOk = $false
        } else {
            Write-Host "   ✓ Porta $port disponível" -ForegroundColor Green
        }
    } catch {
        Write-Host "   ✓ Porta $port disponível" -ForegroundColor Green
    }
}

if (-not $portsOk) {
    Write-Host "`n   Para liberar uma porta, use:" -ForegroundColor Yellow
    Write-Host "   Get-Process -Id (Get-NetTCPConnection -LocalPort PORTA).OwningProcess | Stop-Process -Force" -ForegroundColor Gray
    $allOk = $false
}

# 5. Verificar espaço em disco
Write-Host "`n5. Verificando espaço em disco..." -ForegroundColor Yellow
$drive = Get-PSDrive C
$freeSpaceGB = [math]::Round($drive.Free / 1GB, 2)
if ($freeSpaceGB -gt 10) {
    Write-Host "   ✓ Espaço livre: $freeSpaceGB GB" -ForegroundColor Green
} else {
    Write-Host "   ⚠ Espaço livre: $freeSpaceGB GB (recomendado: 10GB+)" -ForegroundColor Yellow
}

# 6. Verificar memória
Write-Host "`n6. Verificando memória disponível..." -ForegroundColor Yellow
$os = Get-CimInstance Win32_OperatingSystem
$freeMemoryGB = [math]::Round($os.FreePhysicalMemory / 1MB, 2)
if ($freeMemoryGB -gt 2) {
    Write-Host "   ✓ Memória livre: $freeMemoryGB GB" -ForegroundColor Green
} else {
    Write-Host "   ⚠ Memória livre: $freeMemoryGB GB (recomendado: 4GB+)" -ForegroundColor Yellow
}

# 7. Verificar se arquivos necessários existem
Write-Host "`n7. Verificando arquivos do projeto..." -ForegroundColor Yellow
$files = @(
    "docker-compose.yml",
    "src/BankMore.ContaCorrente/Dockerfile",
    "src/BankMore.Transferencia/Dockerfile",
    "src/BankMore.Tarifas/Dockerfile",
    "src/BankMore.Web/Dockerfile"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "   ✓ $file" -ForegroundColor Green
    } else {
        Write-Host "   ✗ $file não encontrado" -ForegroundColor Red
        $allOk = $false
    }
}

# Resumo
Write-Host "`n============================================" -ForegroundColor Cyan
if ($allOk) {
    Write-Host "   ✓ Tudo pronto! Pode executar:" -ForegroundColor Green
    Write-Host "   .\docker-start.ps1" -ForegroundColor White
} else {
    Write-Host "   ✗ Corrija os problemas acima antes de continuar" -ForegroundColor Red
}
Write-Host "============================================" -ForegroundColor Cyan
