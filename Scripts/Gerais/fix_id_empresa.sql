-- Verificar dados na tabela usuario_empresa_cliente
SELECT * FROM usuario_empresa_cliente;

-- Atualizar id_empresa para 1 onde estiver 0 ou NULL
UPDATE usuario_empresa_cliente
SET id_empresa = 1
WHERE id_empresa = 0 OR id_empresa IS NULL;

-- Verificar após atualização
SELECT * FROM usuario_empresa_cliente;
