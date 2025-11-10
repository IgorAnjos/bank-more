# 🧪 Dados de Teste - BankMore

## Contas de Exemplo

Execute estes comandos para criar contas de teste:

### Conta 1 - João Silva
```bash
curl -X POST http://localhost:5003/api/v1/contas \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "João Silva",
    "cpf": "12345678901",
    "senha": "Senha@123"
  }'
```

### Conta 2 - Maria Santos
```bash
curl -X POST http://localhost:5003/api/v1/contas \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Maria Santos",
    "cpf": "98765432100",
    "senha": "Senha@456"
  }'
```

### Conta 3 - Pedro Oliveira
```bash
curl -X POST http://localhost:5003/api/v1/contas \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Pedro Oliveira",
    "cpf": "11122233344",
    "senha": "Senha@789"
  }'
```

## Fluxo Completo de Teste

### 1. Login (Conta 1)
```bash
curl -X POST http://localhost:5003/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "cpf": "12345678901",
    "senha": "Senha@123"
  }'
```

**Resposta:**
```json
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "refresh-token-guid",
  "expiresIn": 600,
  "tokenType": "Bearer"
}
```

> 💡 Copie o `accessToken` e use em todas as próximas requisições

### 2. Depósito
```bash
curl -X POST http://localhost:5003/api/v1/movimentos/deposito \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -d '{
    "valor": 1000.00
  }'
```

### 3. Consultar Saldo
```bash
curl -X GET http://localhost:5003/api/v1/contas/saldo \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

### 4. Login (Conta 2)
```bash
curl -X POST http://localhost:5003/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "cpf": "98765432100",
    "senha": "Senha@456"
  }'
```

### 5. Transferência (Conta 1 → Conta 2)
```bash
# Use o token da Conta 1
curl -X POST http://localhost:5004/api/v1/transferencias \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TOKEN_CONTA_1" \
  -d '{
    "idContaCorrenteOrigem": "GUID_CONTA_1",
    "idContaCorrenteDestino": "GUID_CONTA_2",
    "valor": 100.00
  }'
```

**Resultado esperado:**
- Conta 1: R$ 1000 - R$ 100 - R$ 2 (tarifa) = R$ 898
- Conta 2: R$ 0 + R$ 100 = R$ 100
- Evento `TransferenciaRealizadaEvent` publicado no Kafka
- Worker Tarifas processa evento
- Tarifa de R$ 2,00 debitada automaticamente

### 6. Teste de Idempotência
Execute a MESMA transferência 2x com a mesma chave:

```bash
# Requisição 1 (processa)
curl -X POST http://localhost:5004/api/v1/transferencias \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TOKEN" \
  -H "X-Idempotency-Key: transferencia-teste-123" \
  -d '{
    "idContaCorrenteOrigem": "GUID_1",
    "idContaCorrenteDestino": "GUID_2",
    "valor": 50.00
  }'

# Requisição 2 (retorna resultado da 1ª sem processar)
curl -X POST http://localhost:5004/api/v1/transferencias \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TOKEN" \
  -H "X-Idempotency-Key: transferencia-teste-123" \
  -d '{
    "idContaCorrenteOrigem": "GUID_1",
    "idContaCorrenteDestino": "GUID_2",
    "valor": 50.00
  }'
```

**Resultado:** Mesma resposta, mas só 1 transferência processada!

### 7. Refresh Token
```bash
curl -X POST http://localhost:5003/api/v1/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{
    "refreshToken": "SEU_REFRESH_TOKEN"
  }'
```

## PowerShell Scripts

### Criar 3 contas rapidamente
```powershell
# Conta 1
$conta1 = Invoke-RestMethod -Method Post -Uri "http://localhost:5003/api/v1/contas" `
  -ContentType "application/json" `
  -Body '{"nome":"João Silva","cpf":"12345678901","senha":"Senha@123"}'

# Conta 2
$conta2 = Invoke-RestMethod -Method Post -Uri "http://localhost:5003/api/v1/contas" `
  -ContentType "application/json" `
  -Body '{"nome":"Maria Santos","cpf":"98765432100","senha":"Senha@456"}'

# Login Conta 1
$login1 = Invoke-RestMethod -Method Post -Uri "http://localhost:5003/api/v1/auth/login" `
  -ContentType "application/json" `
  -Body '{"cpf":"12345678901","senha":"Senha@123"}'

$token1 = $login1.accessToken

# Depósito
Invoke-RestMethod -Method Post -Uri "http://localhost:5003/api/v1/movimentos/deposito" `
  -Headers @{ Authorization = "Bearer $token1" } `
  -ContentType "application/json" `
  -Body '{"valor":1000.00}'

# Transferência
Invoke-RestMethod -Method Post -Uri "http://localhost:5004/api/v1/transferencias" `
  -Headers @{ Authorization = "Bearer $token1" } `
  -ContentType "application/json" `
  -Body "{`"idContaCorrenteOrigem`":`"$($conta1.data.id)`",`"idContaCorrenteDestino`":`"$($conta2.data.id)`",`"valor`":100.00}"
```

## Verificar Eventos no Kafka

### Listar tópicos
```powershell
docker exec -it banco-kafka kafka-topics --list --bootstrap-server localhost:9092
```

### Consumir eventos (contas criadas)
```powershell
docker exec -it banco-kafka kafka-console-consumer `
  --bootstrap-server localhost:9092 `
  --topic contas-criadas `
  --from-beginning `
  --property print.key=true `
  --property print.timestamp=true
```

### Consumir eventos (transferências)
```powershell
docker exec -it banco-kafka kafka-console-consumer `
  --bootstrap-server localhost:9092 `
  --topic transferencias-realizadas `
  --from-beginning
```

### Ver consumer groups
```powershell
docker exec -it banco-kafka kafka-consumer-groups `
  --bootstrap-server localhost:9092 `
  --list

docker exec -it banco-kafka kafka-consumer-groups `
  --bootstrap-server localhost:9092 `
  --describe `
  --group tarifas-consumer-group
```

## Verificar Redis (Idempotência)

```powershell
# Conectar ao Redis
docker exec -it banco-redis redis-cli

# Ver todas as chaves
KEYS *

# Ver valor de uma chave
GET "bankmore:idempotency:chave-teste"

# Ver TTL
TTL "bankmore:idempotency:chave-teste"

# Ver quantas chaves existem
DBSIZE
```

## Verificar Outbox Pattern

### Conectar ao banco ContaCorrente
```powershell
docker exec -it bankmore-api-conta sh
cd /app/data
sqlite3 contacorrente.db

# Ver eventos não processados
SELECT id, topic, event_type, created_at, processed, retry_count 
FROM outbox_events 
WHERE processed = 0
ORDER BY created_at;

# Ver eventos processados recentes
SELECT id, topic, event_type, created_at, processed_at 
FROM outbox_events 
WHERE processed = 1
ORDER BY processed_at DESC
LIMIT 10;
```

## Teste de Falha e Recuperação

### 1. Simular Kafka indisponível
```powershell
# Parar Kafka
docker-compose stop kafka

# Fazer uma transferência (vai para outbox)
# Requisição será aceita mas não publicada

# Ver eventos pendentes no outbox
docker exec -it bankmore-api-transferencia sh
sqlite3 /app/data/transferencia.db "SELECT * FROM outbox_events WHERE processed = 0"

# Reiniciar Kafka
docker-compose start kafka

# OutboxProcessor vai processar automaticamente
```

### 2. Teste de Retry
```powershell
# Ver evento com retry
docker exec -it bankmore-api-conta sh
sqlite3 /app/data/contacorrente.db "SELECT id, retry_count, error_message FROM outbox_events WHERE retry_count > 0"
```

## Métricas e Monitoramento

### Ver logs em tempo real
```powershell
# Todos os serviços
docker-compose logs -f

# Apenas API Conta
docker-compose logs -f api-conta

# Apenas Worker Tarifas
docker-compose logs -f worker-tarifas

# Últimas 100 linhas
docker-compose logs --tail=100 api-conta
```

### Ver uso de recursos
```powershell
docker stats
```

### Ver conexões Redis
```powershell
docker exec -it banco-redis redis-cli CLIENT LIST
```

---

## 🎯 Cenários de Teste Importantes

1. ✅ **Fluxo Feliz**: Criar conta → Login → Depósito → Transferência
2. ✅ **Idempotência**: Mesma requisição 2x com mesma chave
3. ✅ **Autenticação**: Acesso sem token, token expirado, refresh token
4. ✅ **Validações**: Saldo insuficiente, valor negativo, conta inexistente
5. ✅ **Eventos**: Verificar publicação no Kafka e processamento
6. ✅ **Outbox Pattern**: Kafka offline → eventos na outbox → recuperação
7. ✅ **Worker Tarifas**: Verificar cobrança automática após transferência
8. ✅ **Concorrência**: Múltiplas transferências simultâneas
