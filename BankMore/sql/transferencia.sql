CREATE TABLE transferencia (
	idtransferencia TEXT(37) PRIMARY KEY, -- identificacao unica da transferencia
	idcontacorrente_origem TEXT(37) NOT NULL, -- identificacao unica da conta corrente de origem
	idcontacorrente_destino TEXT(37) NOT NULL, -- identificacao unica da conta corrente de destino
	datamovimento TEXT(25) NOT NULL, -- data do transferencia no formato DD/MM/YYYY
	valor REAL NOT NULL, -- valor da transferencia. Usar duas casas decimais.
	FOREIGN KEY(idtransferencia) REFERENCES transferencia(idtransferencia)
);

CREATE TABLE compensacao_pendente (
	id TEXT(37) PRIMARY KEY, -- identificacao unica da compensacao
	id_transferencia TEXT(37) NOT NULL, -- id da transferencia que falhou
	chave_idempotencia TEXT(37) NOT NULL, -- chave de idempotencia original
	id_conta_origem TEXT(37) NOT NULL, -- conta que deve receber o estorno
	valor_estorno REAL NOT NULL, -- valor a ser estornado
	tentativas_realizadas INTEGER NOT NULL DEFAULT 0, -- numero de tentativas automaticas
	data_criacao TEXT NOT NULL, -- quando foi criado
	data_ultima_retentativa TEXT, -- ultima tentativa de processamento
	data_resolucao TEXT, -- quando foi resolvido
	status TEXT(20) NOT NULL, -- Pendente, Processando, Resolvido, EscaladoManual
	motivo_falha TEXT(2000), -- descricao do erro
	observacoes_operador TEXT(2000), -- observacoes da mesa de operacoes
	FOREIGN KEY(id_transferencia) REFERENCES transferencia(idtransferencia)
);
