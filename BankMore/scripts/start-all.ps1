# Script para iniciar todos os serviços do BankMore
# Execute este script para iniciar automaticamente todas as APIs e o frontend

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "    BankMore - Iniciando Serviços   " -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

$baseDir = "c:\GitHub\Teste\BankMore\src"

# Função para iniciar um serviço em nova janela do PowerShell
function Start-Service {
    param(
        [string]$ServiceName,
        [string]$Path,
        [string]$Color
    )
    
    Write-Host "Iniciando $ServiceName..." -ForegroundColor $Color
    
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$Path'; Write-Host '$ServiceName' -ForegroundColor $Color; dotnet run"
    
    Start-Sleep -Seconds 2
}

# Iniciar API Conta Corrente
Start-Service -ServiceName "API Conta Corrente (Porta 5003)" `
              -Path "$baseDir\BankMore.ContaCorrente\Api" `
              -Color "Green"

# Iniciar API Transferência
Start-Service -ServiceName "API Transferência (Porta 5004)" `
              -Path "$baseDir\BankMore.Transferencia\Api" `
              -Color "Yellow"

# Aguardar APIs iniciarem
Write-Host ""
Write-Host "Aguardando APIs iniciarem (10 segundos)..." -ForegroundColor Magenta
Start-Sleep -Seconds 10

# Iniciar Interface Web
Start-Service -ServiceName "Interface Web Blazor (Porta 5000/5001)" `
              -Path "$baseDir\BankMore.Web" `
              -Color "Cyan"

Write-Host ""
Write-Host "=====================================" -ForegroundColor Green
Write-Host "  ✓ Todos os serviços iniciados!    " -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""
Write-Host "Acesse a aplicação:" -ForegroundColor White
Write-Host "  • Interface Web: " -NoNewline -ForegroundColor White
Write-Host "http://localhost:5000" -ForegroundColor Cyan
Write-Host "  • Swagger Conta: " -NoNewline -ForegroundColor White
Write-Host "http://localhost:5003" -ForegroundColor Green
Write-Host "  • Swagger Transferência: " -NoNewline -ForegroundColor White
Write-Host "http://localhost:5004" -ForegroundColor Yellow
Write-Host ""
Write-Host "Pressione qualquer tecla para abrir o navegador..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

# Abrir navegador
Start-Process "http://localhost:5000"

Write-Host ""
Write-Host "Navegador aberto!" -ForegroundColor Green
Write-Host "Para encerrar os serviços, feche as janelas do PowerShell." -ForegroundColor Gray
