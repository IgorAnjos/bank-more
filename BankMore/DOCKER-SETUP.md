# 🐳 BankMore - Guia de Execução via Docker

Este guia explica como executar toda a aplicação BankMore usando Docker Compose.

## 📋 Pré-requisitos

- **Docker Desktop** instalado e rodando
  - Windows: [Download Docker Desktop](https://www.docker.com/products/docker-desktop/)
  - Mínimo: 4GB RAM, 20GB disco livre
- **PowerShell** (já vem no Windows)

## 🚀 Início Rápido (3 comandos)

```powershell
# 1. Entre no diretório do projeto
cd c:\GitHub\Teste\BankMore

# 2. Execute o script de inicialização
.\docker-start.ps1

# 3. Acesse o app
# Web: http://localhost:5000
# API Conta: http://localhost:5003/swagger
# API Transferência: http://localhost:5004/swagger
```

## 📦 Serviços Incluídos

A aplicação completa inclui **8 serviços**:

| Serviço | Porta | Descrição |
|---------|-------|-----------|
| **web** | 5000 | Interface Blazor Server |
| **api-conta** | 5003 | API REST Conta Corrente |
| **api-transferencia** | 5004 | API REST Transferências |
| **worker-tarifas** | - | Worker consumidor Kafka (Tarifas) |
| **kafka** | 9092 | Message Broker Apache Kafka |
| **zookeeper** | 2181 | Coordenação do Kafka |
| **redis** | 6379 | Cache e Idempotência |

## 🔧 Comandos Disponíveis

### Iniciar aplicação (primeira vez)
```powershell
.\docker-start.ps1
```

### Iniciar com limpeza completa (rebuild)
```powershell
.\docker-start.ps1 -Clean
```

### Iniciar e ver logs
```powershell
.\docker-start.ps1 -Logs
```

### Ver logs de um serviço específico
```powershell
docker-compose logs -f api-conta
docker-compose logs -f api-transferencia
docker-compose logs -f worker-tarifas
```

### Ver status dos containers
```powershell
docker-compose ps
```

### Parar todos os serviços
```powershell
docker-compose down
```

### Parar e remover volumes (limpar banco de dados)
```powershell
docker-compose down -v
```

### Reiniciar um serviço específico
```powershell
docker-compose restart api-conta
```

### Acessar shell de um container
```powershell
docker exec -it bankmore-api-conta sh
```

## 🧪 Testando a Aplicação

### 1. Via Web App (Blazor)
1. Acesse: http://localhost:5000
2. Cadastre uma conta
3. Faça login
4. Realize operações (depósito, saque, transferência)

### 2. Via Swagger (APIs)

**API Conta Corrente:** http://localhost:5003/swagger

```bash
# 1. Cadastrar conta
POST /api/v1/contas
{
  "nome": "João Silva",
  "cpf": "12345678901",
  "senha": "Senha@123"
}

# 2. Login
POST /api/v1/auth/login
{
  "cpf": "12345678901",
  "senha": "Senha@123"
}
# Copie o token retornado

# 3. Depositar (use o token no header Authorization: Bearer {token})
POST /api/v1/movimentos/deposito
{
  "valor": 1000.00
}
```

**API Transferência:** http://localhost:5004/swagger

```bash
# Realizar transferência (precisa de 2 contas criadas)
POST /api/v1/transferencias
{
  "idContaCorrenteOrigem": "guid-conta-1",
  "idContaCorrenteDestino": "guid-conta-2",
  "valor": 100.00
}
```

### 3. Verificar Kafka

```powershell
# Ver tópicos criados
docker exec -it banco-kafka kafka-topics --list --bootstrap-server localhost:9092

# Consumir eventos do tópico
docker exec -it banco-kafka kafka-console-consumer \
  --bootstrap-server localhost:9092 \
  --topic contas-criadas \
  --from-beginning
```

### 4. Verificar Redis

```powershell
# Conectar ao Redis
docker exec -it banco-redis redis-cli

# Ver todas as chaves de idempotência
KEYS *

# Ver TTL de uma chave
TTL chave-idempotencia
```

## 🔍 Troubleshooting

### Containers não sobem
```powershell
# Verificar logs de erro
docker-compose logs

# Verificar se portas estão em uso
netstat -ano | findstr "5000 5003 5004 9092 6379"

# Limpar tudo e recomeçar
docker-compose down -v
docker system prune -a --volumes -f
.\docker-start.ps1 -Clean
```

### Erro "port is already allocated"
```powershell
# Parar processo usando a porta (exemplo: 5003)
$port = 5003
Get-Process -Id (Get-NetTCPConnection -LocalPort $port).OwningProcess | Stop-Process -Force
```

### Kafka não conecta
```powershell
# Verificar se Kafka está rodando
docker-compose ps kafka

# Ver logs do Kafka
docker-compose logs -f kafka

# Reiniciar Kafka e Zookeeper
docker-compose restart zookeeper kafka
```

### APIs retornam 500
```powershell
# Ver logs da API
docker-compose logs -f api-conta

# Verificar se banco de dados foi criado
docker exec -it bankmore-api-conta ls -la /app/data

# Reiniciar API
docker-compose restart api-conta
```

### Worker Tarifas não processa
```powershell
# Ver logs do worker
docker-compose logs -f worker-tarifas

# Verificar se tópico existe
docker exec -it banco-kafka kafka-topics --describe --topic transferencias-realizadas --bootstrap-server localhost:9092

# Verificar consumer group
docker exec -it banco-kafka kafka-consumer-groups --bootstrap-server localhost:9092 --describe --group tarifas-consumer-group
```

## 📊 Monitoramento

### Ver uso de recursos
```powershell
docker stats
```

### Ver volumes criados
```powershell
docker volume ls | findstr banco
```

### Ver redes
```powershell
docker network ls | findstr banco
```

## 🔄 Atualizando Código

Após modificar o código:

```powershell
# 1. Parar containers
docker-compose down

# 2. Rebuild apenas do serviço modificado
docker-compose build api-conta

# 3. Reiniciar
docker-compose up -d
```

## 🗑️ Limpeza Completa

```powershell
# Parar tudo
docker-compose down -v

# Remover imagens do projeto
docker images "bankmore*" -q | ForEach-Object { docker rmi $_ -f }

# Limpar sistema Docker (cuidado: remove tudo)
docker system prune -a --volumes -f
```

## 🏗️ Arquitetura dos Containers

```
┌─────────────────────────────────────────────────────────────┐
│                        Docker Network                        │
│                       (banco-network)                        │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌──────────┐    ┌──────────┐    ┌──────────┐             │
│  │   Web    │───▶│ API      │───▶│ API      │             │
│  │  :5000   │    │ Conta    │    │Transfer  │             │
│  │ (Blazor) │    │ :5003    │    │ :5004    │             │
│  └──────────┘    └────┬─────┘    └────┬─────┘             │
│                       │                 │                    │
│                       │   ┌─────────────┤                   │
│                       │   │             │                    │
│                       ▼   ▼             ▼                    │
│                  ┌─────────────┐  ┌──────────┐             │
│                  │   Kafka     │  │  Redis   │             │
│                  │   :9092     │  │  :6379   │             │
│                  └──────┬──────┘  └──────────┘             │
│                         │                                    │
│                         ▼                                    │
│                  ┌─────────────┐                            │
│                  │   Worker    │                            │
│                  │  Tarifas    │                            │
│                  └─────────────┘                            │
│                                                              │
│  ┌──────────┐                                               │
│  │Zookeeper │ (coordenação Kafka)                          │
│  │  :2181   │                                               │
│  └──────────┘                                               │
└─────────────────────────────────────────────────────────────┘
```

## 📝 Variáveis de Ambiente

Todas configuradas no `docker-compose.yml`:

- **JWT_SECRET**: Chave de assinatura dos tokens
- **ENCRYPTION_KEY**: Chave para criptografia de CPF (AES-256)
- **KAFKA_BOOTSTRAP_SERVERS**: Endereço do Kafka
- **REDIS_CONNECTION**: String de conexão do Redis
- **API_CONTA_URL**: URL da API Conta Corrente
- **DATABASE_PATH**: Caminho do SQLite (/app/data)

## 🎯 Próximos Passos

Após rodar a aplicação:

1. ✅ Teste o fluxo completo via Web App
2. ✅ Verifique eventos no Kafka
3. ✅ Valide processamento de tarifas
4. ✅ Teste idempotência (enviar mesma requisição 2x)
5. ✅ Verifique Outbox Pattern (tabela `outbox_events`)
6. ✅ Simule falhas (parar Kafka e ver recuperação)

## 📞 Suporte

Em caso de problemas:

1. Verifique logs: `docker-compose logs`
2. Consulte a seção Troubleshooting
3. Limpe e reinicie: `.\docker-start.ps1 -Clean`

---

**✨ Pronto! Sua aplicação bancária completa está rodando em containers Docker.**
