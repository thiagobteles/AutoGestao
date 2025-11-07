-- =============================================
-- Script de Insert: Plano de Contas
-- Descrição: Estrutura contábil hierárquica
-- =============================================

-- Contas de Ativo
INSERT INTO plano_contas (id, codigo, descricao, tipo_conta, natureza, conta_pai_id, nivel, conta_analitica, aceita_lancamento, exibir_na_dre, exibir_no_balancete, observacoes, id_empresa, ativo, data_cadastro, data_alteracao) VALUES
(1, '1', 'ATIVO', 1, 1, NULL, 1, false, false, false, true, 'Grupo principal de Ativo', 1, true, NOW(), NOW()),
(2, '1.1', 'ATIVO CIRCULANTE', 1, 1, 1, 2, false, false, false, true, 'Ativos de curto prazo', 1, true, NOW(), NOW()),
(3, '1.1.01', 'Disponibilidades', 1, 1, 2, 3, false, false, false, true, 'Caixa e bancos', 1, true, NOW(), NOW()),
(4, '1.1.01.001', 'Caixa Geral', 1, 1, 3, 4, true, true, false, true, 'Dinheiro em espécie', 1, true, NOW(), NOW()),
(5, '1.1.01.002', 'Bancos Conta Movimento', 1, 1, 3, 4, true, true, false, true, 'Contas bancárias', 1, true, NOW(), NOW()),
(6, '1.1.02', 'Clientes', 1, 1, 2, 3, false, false, false, true, 'Contas a receber', 1, true, NOW(), NOW()),
(7, '1.1.02.001', 'Clientes Nacionais', 1, 1, 6, 4, true, true, false, true, 'Vendas a prazo Brasil', 1, true, NOW(), NOW()),
(8, '1.1.02.002', 'Duplicatas a Receber', 1, 1, 6, 4, true, true, false, true, 'Duplicatas emitidas', 1, true, NOW(), NOW()),
(9, '1.1.03', 'Estoques', 1, 1, 2, 3, false, false, false, true, 'Mercadorias e produtos', 1, true, NOW(), NOW()),
(10, '1.1.03.001', 'Estoque de Mercadorias', 1, 1, 9, 4, true, true, false, true, 'Mercadorias para revenda', 1, true, NOW(), NOW()),

-- Contas de Passivo
(11, '2', 'PASSIVO', 2, 2, NULL, 1, false, false, false, true, 'Grupo principal de Passivo', 1, true, NOW(), NOW()),
(12, '2.1', 'PASSIVO CIRCULANTE', 2, 2, 11, 2, false, false, false, true, 'Obrigações de curto prazo', 1, true, NOW(), NOW()),
(13, '2.1.01', 'Fornecedores', 2, 2, 12, 3, false, false, false, true, 'Contas a pagar', 1, true, NOW(), NOW()),
(14, '2.1.01.001', 'Fornecedores Nacionais', 2, 2, 13, 4, true, true, false, true, 'Compras a prazo', 1, true, NOW(), NOW()),
(15, '2.1.02', 'Obrigações Trabalhistas', 2, 2, 12, 3, false, false, false, true, 'Salários e encargos', 1, true, NOW(), NOW()),
(16, '2.1.02.001', 'Salários a Pagar', 2, 2, 15, 4, true, true, false, true, 'Folha de pagamento', 1, true, NOW(), NOW()),
(17, '2.1.03', 'Obrigações Fiscais', 2, 2, 12, 3, false, false, false, true, 'Impostos a recolher', 1, true, NOW(), NOW()),
(18, '2.1.03.001', 'ICMS a Recolher', 2, 2, 17, 4, true, true, false, true, 'ICMS sobre vendas', 1, true, NOW(), NOW()),
(19, '2.1.03.002', 'ISS a Recolher', 2, 2, 17, 4, true, true, false, true, 'ISS sobre serviços', 1, true, NOW(), NOW()),
(20, '2.1.03.003', 'IRPJ a Recolher', 2, 2, 17, 4, true, true, false, true, 'Imposto de Renda PJ', 1, true, NOW(), NOW()),

-- Patrimônio Líquido
(21, '3', 'PATRIMÔNIO LÍQUIDO', 3, 2, NULL, 1, false, false, false, true, 'Grupo de PL', 1, true, NOW(), NOW()),
(22, '3.1', 'Capital Social', 3, 2, 21, 2, false, false, false, true, 'Capital dos sócios', 1, true, NOW(), NOW()),
(23, '3.1.01', 'Capital Subscrito', 3, 2, 22, 3, true, true, false, true, 'Capital registrado', 1, true, NOW(), NOW()),
(24, '3.2', 'Reservas', 3, 2, 21, 2, false, false, false, true, 'Reservas de lucro', 1, true, NOW(), NOW()),
(25, '3.2.01', 'Reserva Legal', 3, 2, 24, 3, true, true, false, true, 'Reserva obrigatória', 1, true, NOW(), NOW()),
(26, '3.3', 'Lucros/Prejuízos Acumulados', 3, 2, 21, 2, false, false, false, true, 'Resultado do exercício', 1, true, NOW(), NOW()),
(27, '3.3.01', 'Lucros Acumulados', 3, 2, 26, 3, true, true, false, true, 'Lucros não distribuídos', 1, true, NOW(), NOW()),

-- Receitas
(28, '4', 'RECEITAS', 4, 2, NULL, 1, false, false, true, true, 'Grupo de Receitas', 1, true, NOW(), NOW()),
(29, '4.1', 'RECEITA OPERACIONAL', 4, 2, 28, 2, false, false, true, true, 'Receitas da atividade', 1, true, NOW(), NOW()),
(30, '4.1.01', 'Receita Bruta de Vendas', 4, 2, 29, 3, false, false, true, true, 'Vendas de mercadorias', 1, true, NOW(), NOW()),
(31, '4.1.01.001', 'Vendas de Produtos', 4, 2, 30, 4, true, true, true, true, 'Receita com produtos', 1, true, NOW(), NOW()),
(32, '4.1.01.002', 'Vendas de Mercadorias', 4, 2, 30, 4, true, true, true, true, 'Receita com mercadorias', 1, true, NOW(), NOW()),
(33, '4.1.02', 'Receita de Serviços', 4, 2, 29, 3, false, false, true, true, 'Prestação de serviços', 1, true, NOW(), NOW()),
(34, '4.1.02.001', 'Serviços Prestados', 4, 2, 33, 4, true, true, true, true, 'Receita de serviços', 1, true, NOW(), NOW()),
(35, '4.2', 'DEDUÇÕES DA RECEITA', 4, 1, 28, 2, false, false, true, true, 'Impostos sobre vendas', 1, true, NOW(), NOW()),
(36, '4.2.01', 'Impostos sobre Vendas', 4, 1, 35, 3, false, false, true, true, 'ICMS, ISS, PIS, COFINS', 1, true, NOW(), NOW()),
(37, '4.2.01.001', 'ICMS sobre Vendas', 4, 1, 36, 4, true, true, true, true, 'ICMS das vendas', 1, true, NOW(), NOW()),
(38, '4.2.01.002', 'ISS sobre Serviços', 4, 1, 36, 4, true, true, true, true, 'ISS dos serviços', 1, true, NOW(), NOW()),
(39, '4.2.01.003', 'PIS sobre Receitas', 4, 1, 36, 4, true, true, true, true, 'PIS faturamento', 1, true, NOW(), NOW()),
(40, '4.2.01.004', 'COFINS sobre Receitas', 4, 1, 36, 4, true, true, true, true, 'COFINS faturamento', 1, true, NOW(), NOW()),

-- Despesas e Custos
(41, '5', 'CUSTOS E DESPESAS', 5, 1, NULL, 1, false, false, true, true, 'Grupo de Custos e Despesas', 1, true, NOW(), NOW()),
(42, '5.1', 'CUSTO DAS VENDAS', 6, 1, 41, 2, false, false, true, true, 'CMV e CSP', 1, true, NOW(), NOW()),
(43, '5.1.01', 'Custo Mercadorias Vendidas', 6, 1, 42, 3, true, true, true, true, 'CMV', 1, true, NOW(), NOW()),
(44, '5.1.02', 'Custo Serviços Prestados', 6, 1, 42, 3, true, true, true, true, 'CSP', 1, true, NOW(), NOW()),
(45, '5.2', 'DESPESAS OPERACIONAIS', 5, 1, 41, 2, false, false, true, true, 'Despesas administrativas e vendas', 1, true, NOW(), NOW()),
(46, '5.2.01', 'Despesas com Pessoal', 5, 1, 45, 3, false, false, true, true, 'Salários e encargos', 1, true, NOW(), NOW()),
(47, '5.2.01.001', 'Salários e Ordenados', 5, 1, 46, 4, true, true, true, true, 'Folha de pagamento', 1, true, NOW(), NOW()),
(48, '5.2.01.002', 'Encargos Sociais', 5, 1, 46, 4, true, true, true, true, 'INSS, FGTS', 1, true, NOW(), NOW()),
(49, '5.2.02', 'Despesas Administrativas', 5, 1, 45, 3, false, false, true, true, 'Despesas gerais', 1, true, NOW(), NOW()),
(50, '5.2.02.001', 'Energia Elétrica', 5, 1, 49, 4, true, true, true, true, 'Conta de luz', 1, true, NOW(), NOW());

-- Atualizar a sequência
SELECT setval('plano_contas_id_seq', 50, true);
