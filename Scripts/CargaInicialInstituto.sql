-- ====================================================================
-- 1. CLIENTES
-- ====================================================================
INSERT INTO clientes (id_empresa, id, tipo_pessoa, nome, cpf, cnpj, rg, email, telefone, celular, endereco, cidade, estado, cep, numero, complemento, bairro, ativo, observacoes, data_cadastro, data_alteracao) VALUES
(1, 1, 1, 'João Silva Santos', '12345678901', NULL, '1234567890', 'joao.silva@email.com', '1133334444', '11987654321', 'Rua das Flores, 123', 'São Paulo', 25, '01234567', '123', 'Apto 45', 'Centro', true, 'Cliente preferenciel', NOW(), NOW());

-- ====================================================================
-- 2. Despesas
-- ====================================================================
INSERT INTO despesa_tipos (id_empresa, id, descricao, data_cadastro, data_alteracao, ativo) VALUES
(1, 1, 'Revisão', NOW(), NOW(), true),
(1, 2, 'Reparo', NOW(), NOW(), true),
(1, 3, 'Pintura', NOW(), NOW(), true),
(1, 4, 'Lavagem', NOW(), NOW(), true),
(1, 5, 'Documentação', NOW(), NOW(), true),
(1, 6, 'Despachante', NOW(), NOW(), true);

-- ====================================================================
-- 3. VENDEDORES
-- ====================================================================
INSERT INTO vendedores (id_empresa, id, nome, cpf, email, telefone, celular, percentual_comissao, meta, ativo, data_cadastro, data_alteracao) VALUES
(1, 1, 'Carlos', '12312312312', 'carlos.vendas@autogestao.com.br', '1133445566', '11988776655', 5.50, 50000.00, true, NOW(), NOW());

-- ====================================================================
-- 5. TAREFAS
-- ====================================================================
INSERT INTO tarefas (id_empresa, id, titulo, descricao, status, prioridade, data_criacao, data_vencimento, data_conclusao, id_responsavel, data_cadastro, data_alteracao, ativo) VALUES
(1, 1, 'Revisar estoque de veículos em pátio', 'Fazer levantamento completo de todos os veículos no pátio, verificar documentação e condições gerais', 1, 2, '2024-09-01 08:00:00', '2024-09-18 18:00:00', NULL, 1, NOW(), NOW(), true);