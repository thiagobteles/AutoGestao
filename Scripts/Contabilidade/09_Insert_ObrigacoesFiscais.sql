-- =============================================
-- Script de Insert: Obrigações Fiscais
-- Descrição: Controle de obrigações acessórias
-- =============================================

INSERT INTO obrigacoes_fiscais (id, empresa_cliente_id, tipo_obrigacao, periodicidade, competencia, data_vencimento, status, data_entrega, numero_recibo, arquivo_enviado, arquivo_recibo, observacoes, id_empresa, ativo, data_cadastro, data_alteracao) VALUES
-- SPED Fiscal
(1, 1, 1, 1, '2024-10-01', '2024-11-20', 3, '2024-11-15 14:30:00', 'SPED-FISCAL-001-2024', 'sped_fiscal_10_2024.txt', 'recibo_001.pdf', 'Entregue dentro do prazo', 1, true, NOW(), NOW()),
(2, 2, 1, 1, '2024-10-01', '2024-11-20', 2, NULL, NULL, NULL, NULL, 'Em processamento', 1, true, NOW(), NOW()),
(3, 3, 1, 1, '2024-11-01', '2024-12-20', 1, NULL, NULL, NULL, NULL, 'Aguardando fechamento', 1, true, NOW(), NOW()),

-- SPED Contribuições
(4, 4, 2, 1, '2024-10-01', '2024-11-10', 3, '2024-11-08 16:45:00', 'SPED-CONTRIB-004-2024', 'sped_contrib_10_2024.txt', 'recibo_004.pdf', 'Entregue', 1, true, NOW(), NOW()),
(5, 5, 2, 1, '2024-10-01', '2024-11-10', 2, NULL, NULL, NULL, NULL, 'Gerando arquivo', 1, true, NOW(), NOW()),

-- SPED Contábil (ECD)
(6, 6, 3, 4, '2023-12-31', '2024-05-31', 3, '2024-05-20 10:00:00', 'ECD-006-2023', 'ecd_2023.txt', 'recibo_006.pdf', 'Exercício 2023 entregue', 1, true, NOW(), NOW()),
(7, 9, 3, 4, '2024-12-31', '2025-05-31', 1, NULL, NULL, NULL, NULL, 'Exercício 2024 - futuro', 1, true, NOW(), NOW()),

-- DCTF
(8, 10, 4, 1, '2024-10-01', '2024-11-15', 3, '2024-11-12 09:30:00', 'DCTF-008-10-2024', 'dctf_10_2024.dec', 'recibo_008.pdf', 'Sem débitos', 1, true, NOW(), NOW()),
(9, 11, 4, 1, '2024-11-01', '2024-12-15', 1, NULL, NULL, NULL, NULL, 'Aguardando', 1, true, NOW(), NOW()),

-- DCTFWeb
(10, 12, 5, 1, '2024-10-01', '2024-11-15', 3, '2024-11-13 11:20:00', 'DCTFWEB-010-2024', 'dctfweb_10_2024.xml', 'recibo_010.pdf', 'Transmitido', 1, true, NOW(), NOW()),
(11, 13, 5, 1, '2024-11-01', '2024-12-15', 1, NULL, NULL, NULL, NULL, 'Pendente', 1, true, NOW(), NOW()),

-- eSocial
(12, 14, 6, 1, '2024-10-01', '2024-11-07', 3, '2024-11-05 08:15:00', 'ESOCIAL-012-10-2024', 'esocial_folha_10_2024.xml', 'recibo_012.pdf', 'Folha enviada', 1, true, NOW(), NOW()),
(13, 15, 6, 1, '2024-11-01', '2024-12-07', 2, NULL, NULL, NULL, NULL, 'Folha em validação', 1, true, NOW(), NOW()),
(14, 16, 6, 1, '2024-11-01', '2024-12-07', 1, NULL, NULL, NULL, NULL, 'Aguardando fechamento folha', 1, true, NOW(), NOW()),

-- DIRF
(15, 17, 7, 4, '2023-12-31', '2024-02-28', 3, '2024-02-20 15:00:00', 'DIRF-015-2023', 'dirf_2023.dec', 'recibo_015.pdf', 'Ano-calendário 2023', 1, true, NOW(), NOW()),
(16, 18, 7, 4, '2024-12-31', '2025-02-28', 1, NULL, NULL, NULL, NULL, 'Ano-calendário 2024 - futuro', 1, true, NOW(), NOW()),

-- DARF (Mensal)
(17, 19, 8, 1, '2024-10-01', '2024-11-20', 3, '2024-11-18 13:45:00', 'DARF-017-10-2024', NULL, 'darf_017.pdf', 'IRPJ e CSLL pagos', 1, true, NOW(), NOW()),
(18, 20, 8, 1, '2024-11-01', '2024-12-20', 1, NULL, NULL, NULL, NULL, 'Aguardando vencimento', 1, true, NOW(), NOW()),

-- DAS (Simples Nacional)
(19, 5, 9, 1, '2024-10-01', '2024-11-20', 3, '2024-11-19 10:30:00', 'DAS-019-10-2024', NULL, 'das_019.pdf', 'Simples Nacional pago', 1, true, NOW(), NOW()),
(20, 5, 9, 1, '2024-11-01', '2024-12-20', 1, NULL, NULL, NULL, NULL, 'A vencer', 1, true, NOW(), NOW()),

-- GFIP
(21, 21, 10, 1, '2024-10-01', '2024-11-07', 3, '2024-11-06 16:00:00', 'GFIP-021-10-2024', 'gfip_10_2024.xml', 'recibo_021.pdf', 'FGTS e Previdência', 1, true, NOW(), NOW()),
(22, 22, 10, 1, '2024-11-01', '2024-12-07', 1, NULL, NULL, NULL, NULL, 'Pendente', 1, true, NOW(), NOW()),

-- DEFIS
(23, 25, 11, 4, '2023-12-31', '2024-03-31', 3, '2024-03-25 12:00:00', 'DEFIS-023-2023', 'defis_2023.dec', 'recibo_023.pdf', 'Exercício 2023 - Simples', 1, true, NOW(), NOW()),
(24, 5, 11, 4, '2024-12-31', '2025-03-31', 1, NULL, NULL, NULL, NULL, 'Exercício 2024 - futuro', 1, true, NOW(), NOW()),

-- DeSTDA
(25, 1, 12, 1, '2024-10-01', '2024-11-20', 3, '2024-11-17 14:20:00', 'DESTDA-025-10-2024', 'destda_10_2024.xml', 'recibo_025.pdf', 'Substituição tributária', 1, true, NOW(), NOW()),

-- GIA
(26, 26, 13, 1, '2024-10-01', '2024-11-10', 3, '2024-11-08 09:45:00', 'GIA-026-10-2024', 'gia_sp_10_2024.txt', 'recibo_026.pdf', 'GIA-SP entregue', 1, true, NOW(), NOW()),
(27, 27, 13, 1, '2024-11-01', '2024-12-10', 1, NULL, NULL, NULL, NULL, 'Aguardando', 1, true, NOW(), NOW()),

-- DIME (Declaração do ICMS e do Movimento Econômico)
(28, 28, 14, 1, '2024-10-01', '2024-11-09', 3, '2024-11-07 11:30:00', 'DIME-028-10-2024', 'dime_rj_10_2024.xml', 'recibo_028.pdf', 'DIME-RJ', 1, true, NOW(), NOW()),

-- ISS (Declarações Municipais)
(29, 29, 15, 1, '2024-10-01', '2024-11-10', 3, '2024-11-09 15:00:00', 'ISS-029-10-2024', 'iss_sp_10_2024.xml', 'recibo_029.pdf', 'NFS-e prefeitura SP', 1, true, NOW(), NOW()),
(30, 30, 15, 1, '2024-11-01', '2024-12-10', 1, NULL, NULL, NULL, NULL, 'Em aberto', 1, true, NOW(), NOW()),

-- EFD-Reinf
(31, 31, 16, 1, '2024-10-01', '2024-11-15', 3, '2024-11-14 10:20:00', 'REINF-031-10-2024', 'reinf_10_2024.xml', 'recibo_031.pdf', 'Retenções na fonte', 1, true, NOW(), NOW()),
(32, 32, 16, 1, '2024-11-01', '2024-12-15', 2, NULL, NULL, NULL, NULL, 'Gerando eventos', 1, true, NOW(), NOW()),

-- Mais obrigações diversas
(33, 33, 1, 1, '2024-09-01', '2024-10-20', 3, '2024-10-18 13:00:00', 'SPED-033-09-2024', 'sped_fiscal_09_2024.txt', 'recibo_033.pdf', 'Mês anterior entregue', 1, true, NOW(), NOW()),
(34, 34, 2, 1, '2024-09-01', '2024-10-10', 3, '2024-10-09 16:30:00', 'CONTRIB-034-09-2024', 'contrib_09_2024.txt', 'recibo_034.pdf', 'Entregue', 1, true, NOW(), NOW()),
(35, 35, 4, 1, '2024-09-01', '2024-10-15', 3, '2024-10-13 11:15:00', 'DCTF-035-09-2024', 'dctf_09_2024.dec', 'recibo_035.pdf', 'OK', 1, true, NOW(), NOW()),
(36, 36, 5, 1, '2024-09-01', '2024-10-15', 3, '2024-10-14 09:40:00', 'DCTFWEB-036-09-2024', 'dctfweb_09_2024.xml', 'recibo_036.pdf', 'Transmitido', 1, true, NOW(), NOW()),
(37, 37, 6, 1, '2024-09-01', '2024-10-07', 3, '2024-10-06 14:50:00', 'ESOCIAL-037-09-2024', 'esocial_09_2024.xml', 'recibo_037.pdf', 'Folha OK', 1, true, NOW(), NOW()),
(38, 38, 8, 1, '2024-09-01', '2024-10-20', 3, '2024-10-19 10:00:00', 'DARF-038-09-2024', NULL, 'darf_038.pdf', 'Pago', 1, true, NOW(), NOW()),
(39, 5, 9, 1, '2024-09-01', '2024-10-20', 3, '2024-10-18 12:30:00', 'DAS-039-09-2024', NULL, 'das_039.pdf', 'DAS pago', 1, true, NOW(), NOW()),
(40, 40, 10, 1, '2024-09-01', '2024-10-07', 3, '2024-10-05 15:20:00', 'GFIP-040-09-2024', 'gfip_09_2024.xml', 'recibo_040.pdf', 'GFIP OK', 1, true, NOW(), NOW()),

-- Obrigações atrasadas (para exemplo)
(41, 41, 1, 1, '2024-08-01', '2024-09-20', 4, NULL, NULL, NULL, NULL, 'ATRASADA - providenciar entrega', 1, true, NOW(), NOW()),
(42, 42, 2, 1, '2024-08-01', '2024-09-10', 4, NULL, NULL, NULL, NULL, 'ATRASADA - urgente', 1, true, NOW(), NOW()),

-- Obrigações futuras
(43, 43, 1, 1, '2024-12-01', '2025-01-20', 1, NULL, NULL, NULL, NULL, 'Dezembro 2024', 1, true, NOW(), NOW()),
(44, 44, 2, 1, '2024-12-01', '2025-01-10', 1, NULL, NULL, NULL, NULL, 'Dezembro 2024', 1, true, NOW(), NOW()),
(45, 45, 6, 1, '2024-12-01', '2025-01-07', 1, NULL, NULL, NULL, NULL, 'Folha dezembro', 1, true, NOW(), NOW()),
(46, 46, 10, 1, '2024-12-01', '2025-01-07', 1, NULL, NULL, NULL, NULL, 'GFIP dezembro', 1, true, NOW(), NOW()),

-- Mais obrigações anuais
(47, 47, 3, 4, '2024-12-31', '2025-05-31', 1, NULL, NULL, NULL, NULL, 'ECD 2024 - futuro', 1, true, NOW(), NOW()),
(48, 48, 7, 4, '2024-12-31', '2025-02-28', 1, NULL, NULL, NULL, NULL, 'DIRF 2024 - futuro', 1, true, NOW(), NOW()),
(49, 49, 11, 4, '2024-12-31', '2025-03-31', 1, NULL, NULL, NULL, NULL, 'DEFIS 2024 - futuro', 1, true, NOW(), NOW()),
(50, 50, 16, 1, '2024-10-01', '2024-11-15', 2, NULL, NULL, NULL, NULL, 'EFD-Reinf em processamento', 1, true, NOW(), NOW());

-- Atualizar a sequência
SELECT setval('obrigacoes_fiscais_id_seq', 50, true);
