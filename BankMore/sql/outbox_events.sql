-- Tabela para Outbox Pattern - Garante consistência eventual entre banco e Kafka
-- Eventos são salvos no banco dentro da mesma transação e processados de forma assíncrona

CREATE TABLE IF NOT EXISTS outbox_events (
    id TEXT(37) PRIMARY KEY NOT NULL,
    topic TEXT(100) NOT NULL,
    event_type TEXT(100) NOT NULL,
    payload TEXT NOT NULL,
    partition_key TEXT(100),
    created_at TEXT NOT NULL,
    processed_at TEXT,
    processed INTEGER NOT NULL DEFAULT 0,
    retry_count INTEGER NOT NULL DEFAULT 0,
    error_message TEXT(500)
);

-- Índice para buscar eventos não processados ordenados por data de criação
CREATE INDEX IF NOT EXISTS IX_outbox_events_processed_created_at 
ON outbox_events(processed, created_at);

-- Índice para filtrar por tópico
CREATE INDEX IF NOT EXISTS IX_outbox_events_topic 
ON outbox_events(topic);

-- Comentários para documentação
-- processed: 0 = não processado, 1 = processado
-- retry_count: Número de tentativas de processamento (máximo 5)
-- partition_key: Chave usada para garantir ordem das mensagens no Kafka (geralmente IdContaCorrente)
