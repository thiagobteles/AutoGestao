-- =============================================
-- Script de Insert: Lançamentos Contábeis
-- Descrição: 50 lançamentos contábeis com partidas dobradas
-- =============================================

INSERT INTO lancamentos_contabeis (id, data_lancamento, tipo_lancamento, empresa_cliente_id, conta_debito_id, conta_credito_id, valor, historico, complemento, numero_documento, nota_fiscal_id, conciliado, data_conciliacao, id_empresa, ativo, data_cadastro, data_alteracao) VALUES
-- Outubro 2024 - Lançamentos de vendas
(1, '2024-10-05', 1, 1, 5, 31, 5280.50, 'Venda de mercadorias a prazo', 'Ref. NF-e 1001', 'NF-001001', 1, true, '2024-10-06', 1, true, NOW(), NOW()),
(2, '2024-10-08', 1, 2, 5, 32, 8750.00, 'Venda de produtos de tecnologia', 'Ref. NF-e 1002', 'NF-001002', 2, true, '2024-10-09', 1, true, NOW(), NOW()),
(3, '2024-10-10', 1, 3, 43, 14, 3500.00, 'Compra de insumos para restaurante', 'Ref. NF-e 1003', 'NF-001003', 3, true, '2024-10-11', 1, true, NOW(), NOW()),
(4, '2024-10-12', 1, 4, 7, 34, 12500.00, 'Receita de serviços de consultoria', 'Ref. NF-e 1004', 'NF-001004', 4, true, '2024-10-13', 1, true, NOW(), NOW()),
(5, '2024-10-15', 1, 5, 4, 32, 2150.00, 'Venda à vista de pães e confeitaria', 'Ref. NF-e 1005', 'NF-001005', 5, true, '2024-10-16', 1, true, NOW(), NOW()),

-- Outubro 2024 - Lançamentos de compras
(6, '2024-10-18', 2, 6, 10, 14, 15800.00, 'Compra de medicamentos a prazo', 'Ref. NF-e 2001', 'NF-002001', 6, true, '2024-10-19', 1, true, NOW(), NOW()),
(7, '2024-10-20', 1, 7, 5, 34, 9500.00, 'Receita de honorários advocatícios', 'Ref. NF-e 2002', 'NF-002002', 7, true, '2024-10-21', 1, true, NOW(), NOW()),
(8, '2024-10-22', 1, 8, 4, 31, 1875.50, 'Venda à vista de produtos de mercearia', 'Ref. NF-e 2003', 'NF-002003', 8, true, '2024-10-23', 1, true, NOW(), NOW()),
(9, '2024-10-25', 1, 9, 7, 34, 18000.00, 'Serviços contábeis mensais a receber', 'Ref. NF-e 2004', 'NF-002004', 9, true, '2024-10-26', 1, true, NOW(), NOW()),
(10, '2024-10-28', 1, 10, 4, 31, 680.00, 'Venda à vista de lanches', 'Ref. NF-e 2005', 'NF-002005', 10, true, '2024-10-29', 1, true, NOW(), NOW()),

-- Novembro 2024 - Lançamentos de vendas
(11, '2024-11-02', 1, 11, 7, 31, 4500.00, 'Venda de roupas a prazo', 'Ref. NF-e 3001', 'NF-003001', 11, true, '2024-11-03', 1, true, NOW(), NOW()),
(12, '2024-11-04', 1, 12, 5, 31, 28500.00, 'Venda de móveis planejados', 'Ref. NF-e 3002', 'NF-003002', 12, true, '2024-11-05', 1, true, NOW(), NOW()),
(13, '2024-11-06', 1, 13, 4, 31, 2800.00, 'Venda à vista de calçados', 'Ref. NF-e 3003', 'NF-003003', 13, true, '2024-11-07', 1, true, NOW(), NOW()),
(14, '2024-11-08', 1, 14, 5, 34, 3200.00, 'Mensalidades de academia', 'Ref. NF-e 3004', 'NF-003004', 14, true, '2024-11-09', 1, true, NOW(), NOW()),
(15, '2024-11-10', 2, 15, 10, 14, 45000.00, 'Compra de insumos agrícolas', 'Ref. NF-e 3005', 'NF-003005', 15, true, '2024-11-11', 1, true, NOW(), NOW()),

-- Novembro 2024 - Mais lançamentos
(16, '2024-11-12', 1, 16, 4, 31, 850.00, 'Venda de material escolar', 'Ref. NF-e 4001', 'NF-004001', 16, true, '2024-11-13', 1, true, NOW(), NOW()),
(17, '2024-11-14', 1, 17, 7, 34, 7800.00, 'Receita de frete de mercadorias', 'Ref. NF-e 4002', 'NF-004002', 17, true, '2024-11-15', 1, true, NOW(), NOW()),
(18, '2024-11-16', 1, 18, 5, 34, 1200.00, 'Mensalidade escolar', 'Ref. NF-e 4003', 'NF-004003', 18, true, '2024-11-17', 1, true, NOW(), NOW()),
(19, '2024-11-18', 1, 19, 5, 34, 5600.00, 'Serviços de buffet', 'Ref. NF-e 4004', 'NF-004004', 19, false, NULL, 1, true, NOW(), NOW()),
(20, '2024-11-20', 1, 20, 7, 34, 8900.00, 'Campanha publicitária', 'Ref. NF-e 4005', 'NF-004005', 20, false, NULL, 1, true, NOW(), NOW()),

-- Lançamentos de despesas operacionais
(21, '2024-10-05', 3, 1, 47, 5, 8500.00, 'Pagamento de salários outubro', 'Folha de pagamento do mês', 'FOLHA-10/24', NULL, true, '2024-10-06', 1, true, NOW(), NOW()),
(22, '2024-10-10', 3, 2, 47, 5, 12300.00, 'Pagamento de salários outubro', 'Folha de pagamento do mês', 'FOLHA-10/24', NULL, true, '2024-10-11', 1, true, NOW(), NOW()),
(23, '2024-10-15', 3, 3, 50, 5, 1500.00, 'Pagamento de energia elétrica', 'Conta de luz referente a setembro', 'ENERGIA-09/24', NULL, true, '2024-10-16', 1, true, NOW(), NOW()),
(24, '2024-10-20', 3, 4, 50, 5, 980.00, 'Pagamento de energia elétrica', 'Conta de luz referente a setembro', 'ENERGIA-09/24', NULL, true, '2024-10-21', 1, true, NOW(), NOW()),
(25, '2024-10-25', 3, 5, 47, 5, 5600.00, 'Pagamento de salários outubro', 'Folha de pagamento do mês', 'FOLHA-10/24', NULL, true, '2024-10-26', 1, true, NOW(), NOW()),

-- Novembro - Despesas operacionais
(26, '2024-11-05', 3, 6, 47, 5, 18500.00, 'Pagamento de salários novembro', 'Folha de pagamento do mês', 'FOLHA-11/24', NULL, true, '2024-11-06', 1, true, NOW(), NOW()),
(27, '2024-11-10', 3, 7, 47, 5, 22000.00, 'Pagamento de salários novembro', 'Folha de pagamento do mês', 'FOLHA-11/24', NULL, false, NULL, 1, true, NOW(), NOW()),
(28, '2024-11-15', 3, 8, 50, 5, 850.00, 'Pagamento de energia elétrica', 'Conta de luz referente a outubro', 'ENERGIA-10/24', NULL, false, NULL, 1, true, NOW(), NOW()),
(29, '2024-11-20', 3, 9, 50, 5, 1200.00, 'Pagamento de energia elétrica', 'Conta de luz referente a outubro', 'ENERGIA-10/24', NULL, false, NULL, 1, true, NOW(), NOW()),
(30, '2024-11-25', 3, 10, 47, 5, 4500.00, 'Pagamento de salários novembro', 'Folha de pagamento do mês', 'FOLHA-11/24', NULL, false, NULL, 1, true, NOW(), NOW()),

-- Lançamentos diversos
(31, '2024-11-22', 1, 21, 4, 34, 1200.00, 'Serviços mecânicos à vista', 'Ref. NF-e 5001', 'NF-005001', 21, false, NULL, 1, true, NOW(), NOW()),
(32, '2024-11-24', 1, 22, 5, 34, 950.00, 'Consulta médica', 'Ref. NF-e 5002', 'NF-005002', 22, false, NULL, 1, true, NOW(), NOW()),
(33, '2024-11-26', 1, 23, 7, 34, 75000.00, 'Serviços de construção civil', 'Ref. NF-e 5003', 'NF-005003', 23, false, NULL, 1, true, NOW(), NOW()),
(34, '2024-11-28', 1, 24, 5, 34, 2800.00, 'Serviços de segurança', 'Ref. NF-e 5004', 'NF-005004', 24, false, NULL, 1, true, NOW(), NOW()),
(35, '2024-11-29', 1, 25, 4, 31, 680.00, 'Venda de produtos pet', 'Ref. NF-e 5005', 'NF-005005', 25, false, NULL, 1, true, NOW(), NOW()),

-- NFC-e lançamentos
(36, '2024-11-01', 1, 26, 4, 31, 350.00, 'Venda PDV joalheria', 'Ref. NFC-e 101', 'NFCE-000101', 26, true, '2024-11-02', 1, true, NOW(), NOW()),
(37, '2024-11-02', 1, 27, 4, 31, 180.00, 'Venda PDV floricultura', 'Ref. NFC-e 102', 'NFCE-000102', 27, true, '2024-11-03', 1, true, NOW(), NOW()),
(38, '2024-11-03', 1, 28, 4, 34, 450.00, 'Venda PDV limpeza', 'Ref. NFC-e 103', 'NFCE-000103', 28, true, '2024-11-04', 1, true, NOW(), NOW()),
(39, '2024-11-04', 1, 29, 4, 31, 920.00, 'Venda PDV bebidas', 'Ref. NFC-e 104', 'NFCE-000104', 29, true, '2024-11-05', 1, true, NOW(), NOW()),
(40, '2024-11-05', 1, 30, 4, 34, 1200.00, 'Venda PDV gráfica', 'Ref. NFC-e 105', 'NFCE-000105', 30, true, '2024-11-06', 1, true, NOW(), NOW()),

-- NFS-e lançamentos
(41, '2024-11-06', 1, 31, 5, 34, 2400.00, 'Curso profissionalizante', 'Ref. NFS-e 201', 'NFSE-000201', 31, false, NULL, 1, true, NOW(), NOW()),
(42, '2024-11-07', 1, 32, 5, 34, 1800.00, 'Aulas de direção', 'Ref. NFS-e 202', 'NFSE-000202', 32, false, NULL, 1, true, NOW(), NOW()),
(43, '2024-11-08', 1, 33, 5, 34, 850.00, 'Exames laboratoriais', 'Ref. NFS-e 203', 'NFSE-000203', 33, false, NULL, 1, true, NOW(), NOW()),
(44, '2024-11-09', 1, 34, 7, 34, 12000.00, 'Importação de produtos', 'Ref. NFS-e 204', 'NFSE-000204', 34, false, NULL, 1, true, NOW(), NOW()),
(45, '2024-11-10', 1, 35, 5, 34, 3500.00, 'Hospedagem hoteleira', 'Ref. NFS-e 205', 'NFSE-000205', 35, false, NULL, 1, true, NOW(), NOW()),

-- Lançamentos recentes
(46, '2024-11-28', 1, 41, 4, 31, 890.00, 'Venda de pizzas', 'Ref. NF-e 7001', 'NF-007001', 41, false, NULL, 1, true, NOW(), NOW()),
(47, '2024-11-29', 1, 42, 5, 34, 4500.00, 'Seguro de vida', 'Ref. NF-e 7002', 'NF-007002', 42, false, NULL, 1, true, NOW(), NOW()),
(48, '2024-11-30', 1, 43, 4, 34, 380.00, 'Lavanderia express', 'Ref. NF-e 7003', 'NF-007003', 43, false, NULL, 1, true, NOW(), NOW()),
(49, '2024-10-30', 1, 44, 4, 31, 480.00, 'Venda de sorvetes', 'Ref. NF-e 7004', 'NF-007004', 44, true, '2024-10-31', 1, true, NOW(), NOW()),
(50, '2024-10-31', 1, 45, 5, 34, 2200.00, 'Curso de inglês', 'Ref. NF-e 7005', 'NF-007005', 45, true, '2024-11-01', 1, true, NOW(), NOW());

-- Atualizar a sequência
SELECT setval('lancamentos_contabeis_id_seq', 50, true);
