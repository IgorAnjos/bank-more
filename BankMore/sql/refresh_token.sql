-- Create refresh_token table for JWT refresh token management
CREATE TABLE IF NOT EXISTS refresh_token (
    id TEXT(37) NOT NULL PRIMARY KEY,
    id_conta_corrente TEXT(37) NOT NULL,
    token_hash TEXT(64) NOT NULL,
    data_criacao TEXT NOT NULL,
    data_expiracao TEXT NOT NULL,
    revogado INTEGER NOT NULL DEFAULT 0,
    data_revogacao TEXT,
    motivo_revogacao TEXT(500)
);

-- Create indices for performance
CREATE UNIQUE INDEX IF NOT EXISTS IX_refresh_token_token_hash ON refresh_token(token_hash);
CREATE INDEX IF NOT EXISTS IX_refresh_token_id_conta_corrente ON refresh_token(id_conta_corrente);
CREATE INDEX IF NOT EXISTS IX_refresh_token_data_expiracao ON refresh_token(data_expiracao);

-- Mark migrations as applied in __EFMigrationsHistory
INSERT OR IGNORE INTO __EFMigrationsHistory (MigrationId, ProductVersion)
VALUES 
    ('20251109155940_AddCpfToContaCorrente', '9.0.10'),
    ('20251109162220_EnhanceIdempotenciaEntity', '9.0.10'),
    ('20251109165822_AddRefreshTokenTable', '9.0.10');
