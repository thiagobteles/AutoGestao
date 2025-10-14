-- ====================================================================
-- 1. CLIENTES (ALUNOS/INTERESSADOS)
-- ====================================================================
INSERT INTO clientes (id_empresa, id, tipo_pessoa, nome, cpf, cnpj, rg, email, telefone, celular, endereco, cidade, estado, cep, numero, complemento, bairro, ativo, observacoes, data_cadastro, data_alteracao) VALUES
(1, 1, 1, 'Maria Fernanda Costa', '12345678901', NULL, '123456789', 'maria.costa@email.com', '1133334444', '11987654321', 'Rua das Acácias, 123', 'São Paulo', 25, '01234567', '123', 'Apto 45', 'Jardins', true, 'Interessada em Psicanálise EAD', NOW(), NOW()),
(1, 2, 1, 'Ana Paula Rodrigues', '23456789012', NULL, '234567890', 'ana.rodrigues@email.com', '1133445566', '11988776655', 'Av. Paulista, 1500', 'São Paulo', 25, '01310100', '1500', 'Conj 82', 'Bela Vista', true, 'Aluna ativa - Psicanálise Clínica', NOW(), NOW()),
(1, 3, 1, 'Carlos Eduardo Silva', '34567890123', NULL, '345678901', 'carlos.silva@email.com', '1144556677', '11999887766', 'Rua Augusta, 2500', 'São Paulo', 25, '01412100', '2500', NULL, 'Consolação', true, 'Interessado em Terapia Cognitiva', NOW(), NOW()),
(1, 4, 1, 'Juliana Santos Oliveira', '45678901234', NULL, '456789012', 'juliana.oliveira@email.com', '1155667788', '11988998877', 'Rua Oscar Freire, 800', 'São Paulo', 25, '01426000', '800', 'Apto 301', 'Pinheiros', true, 'Psicóloga - busca especialização', NOW(), NOW()),
(1, 5, 1, 'Roberto Alves Mendes', '56789012345', NULL, '567890123', 'roberto.mendes@email.com', '1166778899', '11977889966', 'Av. Rebouças, 3000', 'São Paulo', 25, '05401300', '3000', 'Bloco B', 'Pinheiros', true, 'Médico psiquiatra', NOW(), NOW()),
(1, 6, 1, 'Fernanda Lima Souza', '67890123456', NULL, '678901234', 'fernanda.souza@email.com', '1177889900', '11966778855', 'Rua Haddock Lobo, 595', 'São Paulo', 25, '01414001', '595', 'Sala 12', 'Cerqueira César', true, 'Terapeuta - especialização', NOW(), NOW()),
(1, 7, 1, 'Paulo Henrique Santos', '78901234567', NULL, '789012345', 'paulo.santos@email.com', '1188990011', '11955667744', 'Rua da Consolação, 2000', 'São Paulo', 25, '01301100', '2000', NULL, 'Consolação', true, 'Estudante de Psicologia', NOW(), NOW()),
(1, 8, 1, 'Beatriz Almeida Costa', '89012345678', NULL, '890123456', 'beatriz.costa@email.com', '1199001122', '11944556633', 'Av. Angélica, 1200', 'São Paulo', 25, '01228200', '1200', 'Apto 501', 'Higienópolis', true, 'Pedagoga - curso complementar', NOW(), NOW()),
(1, 9, 1, 'Ricardo Ferreira Lima', '90123456789', NULL, '901234567', 'ricardo.lima@email.com', '1100112233', '11933445522', 'Rua Estados Unidos, 1500', 'São Paulo', 25, '01427001', '1500', 'Conj 45', 'Jardim América', true, 'Empresário - desenvolvimento pessoal', NOW(), NOW()),
(1, 10, 1, 'Camila Ribeiro Dias', '01234567890', NULL, '012345678', 'camila.dias@email.com', '1111223344', '11922334411', 'Rua Bela Cintra, 986', 'São Paulo', 25, '01415000', '986', NULL, 'Consolação', true, 'Psicanalista em formação', NOW(), NOW()),
(1, 11, 1, 'Marcos Vinicius Souza', '11234567890', NULL, '112345678', 'marcos.souza@email.com', '1122334455', '11911223300', 'Av. Brasil, 2500', 'São Paulo', 25, '01431000', '2500', 'Apto 102', 'Jardim Paulista', true, 'Coach - especialização clínica', NOW(), NOW()),
(1, 12, 1, 'Luciana Martins Rocha', '22345678901', NULL, '223456789', 'luciana.rocha@email.com', '1133445566', '11900112299', 'Rua Pamplona, 1200', 'São Paulo', 25, '01405001', '1200', 'Sala 88', 'Jardim Paulista', true, 'Assistente social', NOW(), NOW()),
(1, 13, 1, 'André Luiz Barbosa', '33456789012', NULL, '334567890', 'andre.barbosa@email.com', '1144556677', '11988776655', 'Rua Avanhandava, 126', 'São Paulo', 25, '01306000', '126', NULL, 'Bela Vista', true, 'Filósofo - interesse em psicanálise', NOW(), NOW()),
(1, 14, 1, 'Patricia Helena Santos', '44567890123', NULL, '445678901', 'patricia.santos@email.com', '1155667788', '11977665544', 'Alameda Santos, 1000', 'São Paulo', 25, '01418100', '1000', 'Conj 210', 'Cerqueira César', true, 'Neuropsicóloga', NOW(), NOW()),
(1, 15, 1, 'Thiago Henrique Costa', '55678901234', NULL, '556789012', 'thiago.costa@email.com', '1166778899', '11966554433', 'Rua Frei Caneca, 569', 'São Paulo', 25, '01307001', '569', 'Apto 71', 'Consolação', true, 'Professor universitário', NOW(), NOW()),
(1, 16, 1, 'Gabriela Oliveira Lima', '66789012345', NULL, '667890123', 'gabriela.lima@email.com', '1177889900', '11955443322', 'Av. 9 de Julho, 3300', 'São Paulo', 25, '01407200', '3300', 'Torre A', 'Jardim Paulista', true, 'Terapeuta holística', NOW(), NOW()),
(1, 17, 1, 'Felipe Augusto Moreira', '77890123456', NULL, '778901234', 'felipe.moreira@email.com', '1188990011', '11944332211', 'Rua Martinico Prado, 26', 'São Paulo', 25, '01224010', '26', NULL, 'Higienópolis', true, 'Advogado - autoconhecimento', NOW(), NOW()),
(1, 18, 1, 'Alessandra Silva Pinto', '88901234567', NULL, '889012345', 'alessandra.pinto@email.com', '1199001122', '11933221100', 'Rua Maria Antônia, 294', 'São Paulo', 25, '01222010', '294', 'Sala 5', 'Vila Buarque', true, 'Psicóloga clínica', NOW(), NOW()),
(1, 19, 1, 'Leonardo Dias Santos', '99012345678', NULL, '990123456', 'leonardo.santos@email.com', '1100112233', '11922110099', 'Alameda Jaú, 1700', 'São Paulo', 25, '01420002', '1700', 'Apto 203', 'Jardim Paulista', true, 'Psiquiatra infantil', NOW(), NOW()),
(1, 20, 1, 'Renata Cristina Alves', '00123456789', NULL, '001234567', 'renata.alves@email.com', '1111223344', '11911009988', 'Rua Aspicuelta, 422', 'São Paulo', 25, '05433010', '422', NULL, 'Vila Madalena', true, 'Artista plástica', NOW(), NOW()),
(1, 21, 1, 'Gustavo Henrique Lopes', '10123456789', NULL, '101234567', 'gustavo.lopes@email.com', '1122334455', '11900998877', 'Rua Girassol, 273', 'São Paulo', 25, '05433000', '273', 'Casa 2', 'Vila Madalena', true, 'Músico - terapia através da arte', NOW(), NOW()),
(1, 22, 1, 'Vanessa Rodrigues Melo', '20234567890', NULL, '202345678', 'vanessa.melo@email.com', '1133445566', '11988887766', 'Rua Harmonia, 1000', 'São Paulo', 25, '05435000', '1000', 'Apto 45', 'Pinheiros', true, 'Designer - desenvolvimento pessoal', NOW(), NOW()),
(1, 23, 1, 'Diego Fernandes Silva', '30345678901', NULL, '303456789', 'diego.silva@email.com', '1144556677', '11977776655', 'Rua Mourato Coelho, 1404', 'São Paulo', 25, '05417002', '1404', NULL, 'Pinheiros', true, 'Administrador de empresas', NOW(), NOW()),
(1, 24, 1, 'Aline Patricia Souza', '40456789012', NULL, '404567890', 'aline.souza@email.com', '1155667788', '11966665544', 'Rua Teodoro Sampaio, 2000', 'São Paulo', 25, '05406200', '2000', 'Sala 301', 'Pinheiros', true, 'Fisioterapeuta', NOW(), NOW()),
(1, 25, 1, 'Bruno César Martins', '50567890123', NULL, '505678901', 'bruno.martins@email.com', '1166778899', '11955554433', 'Rua dos Pinheiros, 498', 'São Paulo', 25, '05422001', '498', NULL, 'Pinheiros', true, 'Engenheiro - transição carreira', NOW(), NOW()),
(1, 26, 1, 'Cristiane Aparecida Rosa', '60678901234', NULL, '606789012', 'cristiane.rosa@email.com', '1177889900', '11944443322', 'Rua Cardeal Arcoverde, 2365', 'São Paulo', 25, '05408003', '2365', 'Conj 12', 'Pinheiros', true, 'Enfermeira - humanização', NOW(), NOW()),
(1, 27, 1, 'Rodrigo Almeida Santos', '70789012345', NULL, '707890123', 'rodrigo.santos@email.com', '1188990011', '11933332211', 'Av. Pedroso de Moraes, 1200', 'São Paulo', 25, '05419001', '1200', NULL, 'Pinheiros', true, 'Jornalista', NOW(), NOW()),
(1, 28, 1, 'Tatiana Ferreira Costa', '80890123456', NULL, '808901234', 'tatiana.costa@email.com', '1199001122', '11922221100', 'Rua Cunha Gago, 700', 'São Paulo', 25, '05421001', '700', 'Apto 82', 'Pinheiros', true, 'Nutricionista', NOW(), NOW()),
(1, 29, 1, 'Fábio Augusto Oliveira', '90901234567', NULL, '909012345', 'fabio.oliveira@email.com', '1100112233', '11911110099', 'Rua Purpurina, 84', 'São Paulo', 25, '05435020', '84', NULL, 'Vila Madalena', true, 'Publicitário', NOW(), NOW()),
(1, 30, 1, 'Isabela Cristina Rocha', '01012345678', NULL, '010123456', 'isabela.rocha@email.com', '1111223344', '11900009988', 'Rua Harmonia, 500', 'São Paulo', 25, '05435000', '500', 'Casa', 'Vila Madalena', true, 'Socióloga', NOW(), NOW());

-- ====================================================================
-- 2. DESPESAS TIPOS
-- ====================================================================
INSERT INTO despesa_tipos (id_empresa, id, descricao, data_cadastro, data_alteracao, ativo) VALUES
(1, 1, 'Material Didático', NOW(), NOW(), true),
(1, 2, 'Plataforma EAD', NOW(), NOW(), true),
(1, 3, 'Marketing Digital', NOW(), NOW(), true),
(1, 4, 'Certificados', NOW(), NOW(), true),
(1, 5, 'Infraestrutura TI', NOW(), NOW(), true),
(1, 6, 'Palestrantes/Professores', NOW(), NOW(), true),
(1, 7, 'Material Gráfico', NOW(), NOW(), true),
(1, 8, 'Licenças Software', NOW(), NOW(), true),
(1, 9, 'Manutenção Site', NOW(), NOW(), true),
(1, 10, 'Eventos e Workshops', NOW(), NOW(), true);

-- ====================================================================
-- 3. VENDEDORES (CONSULTORES/COORDENADORES)
-- ====================================================================
INSERT INTO vendedores (id_empresa, id, nome, cpf, email, telefone, celular, percentual_comissao, meta, ativo, data_cadastro, data_alteracao) VALUES
(1, 1, 'Angela Sirino', '11111111111', 'angela.sirino@institutofd.com.br', '1133445566', '11988776655', 10.00, 100000.00, true, NOW(), NOW()),
(1, 2, 'Carla Mendes Silva', '22222222222', 'carla.mendes@institutofd.com.br', '1133445567', '11988776656', 7.50, 80000.00, true, NOW(), NOW()),
(1, 3, 'Ricardo Tavares Costa', '33333333333', 'ricardo.tavares@institutofd.com.br', '1133445568', '11988776657', 7.50, 80000.00, true, NOW(), NOW()),
(1, 4, 'Juliana Campos Rocha', '44444444444', 'juliana.campos@institutofd.com.br', '1133445569', '11988776658', 7.00, 75000.00, true, NOW(), NOW()),
(1, 5, 'Fernando Augusto Lima', '55555555555', 'fernando.lima@institutofd.com.br', '1133445570', '11988776659', 7.00, 75000.00, true, NOW(), NOW()),
(1, 6, 'Patricia Helena Santos', '66666666666', 'patricia.santos@institutofd.com.br', '1133445571', '11988776660', 6.50, 70000.00, true, NOW(), NOW()),
(1, 7, 'Marcos Vinicius Alves', '77777777777', 'marcos.alves@institutofd.com.br', '1133445572', '11988776661', 6.50, 70000.00, true, NOW(), NOW()),
(1, 8, 'Roberta Cristina Souza', '88888888888', 'roberta.souza@institutofd.com.br', '1133445573', '11988776662', 6.00, 65000.00, true, NOW(), NOW()),
(1, 9, 'André Luis Barbosa', '99999999999', 'andre.barbosa@institutofd.com.br', '1133445574', '11988776663', 6.00, 65000.00, true, NOW(), NOW()),
(1, 10, 'Simone Aparecida Rosa', '10101010101', 'simone.rosa@institutofd.com.br', '1133445575', '11988776664', 5.50, 60000.00, true, NOW(), NOW()),
(1, 11, 'Thiago Henrique Dias', '20202020202', 'thiago.dias@institutofd.com.br', '1133445576', '11988776665', 5.50, 60000.00, true, NOW(), NOW()),
(1, 12, 'Vanessa Rodrigues Melo', '30303030303', 'vanessa.melo@institutofd.com.br', '1133445577', '11988776666', 5.00, 55000.00, true, NOW(), NOW()),
(1, 13, 'Daniel Fernando Silva', '40404040404', 'daniel.silva@institutofd.com.br', '1133445578', '11988776667', 5.00, 55000.00, true, NOW(), NOW()),
(1, 14, 'Camila Fernanda Costa', '50505050505', 'camila.costa@institutofd.com.br', '1133445579', '11988776668', 5.00, 55000.00, true, NOW(), NOW()),
(1, 15, 'Lucas Gabriel Santos', '60606060606', 'lucas.santos@institutofd.com.br', '1133445580', '11988776669', 5.00, 50000.00, true, NOW(), NOW()),
(1, 16, 'Fernanda Cristina Lopes', '70707070707', 'fernanda.lopes@institutofd.com.br', '1133445581', '11988776670', 5.00, 50000.00, true, NOW(), NOW()),
(1, 17, 'Bruno César Martins', '80808080808', 'bruno.martins@institutofd.com.br', '1133445582', '11988776671', 4.50, 45000.00, true, NOW(), NOW()),
(1, 18, 'Aline Patricia Oliveira', '90909090909', 'aline.oliveira@institutofd.com.br', '1133445583', '11988776672', 4.50, 45000.00, true, NOW(), NOW()),
(1, 19, 'Rafael Augusto Souza', '11011011011', 'rafael.souza@institutofd.com.br', '1133445584', '11988776673', 4.50, 45000.00, true, NOW(), NOW()),
(1, 20, 'Larissa Beatriz Costa', '12012012012', 'larissa.costa@institutofd.com.br', '1133445585', '11988776674', 4.50, 45000.00, true, NOW(), NOW()),
(1, 21, 'Gustavo Henrique Rocha', '13013013013', 'gustavo.rocha@institutofd.com.br', '1133445586', '11988776675', 4.00, 40000.00, true, NOW(), NOW()),
(1, 22, 'Priscila Santos Lima', '14014014014', 'priscila.lima@institutofd.com.br', '1133445587', '11988776676', 4.00, 40000.00, true, NOW(), NOW()),
(1, 23, 'Rodrigo Almeida Santos', '15015015015', 'rodrigo.santos@institutofd.com.br', '1133445588', '11988776677', 4.00, 40000.00, true, NOW(), NOW()),
(1, 24, 'Tatiana Ferreira Silva', '16016016016', 'tatiana.silva@institutofd.com.br', '1133445589', '11988776678', 4.00, 40000.00, true, NOW(), NOW()),
(1, 25, 'Felipe Augusto Costa', '17017017017', 'felipe.costa@institutofd.com.br', '1133445590', '11988776679', 3.50, 35000.00, true, NOW(), NOW()),
(1, 26, 'Mariana Cristina Dias', '18018018018', 'mariana.dias@institutofd.com.br', '1133445591', '11988776680', 3.50, 35000.00, true, NOW(), NOW()),
(1, 27, 'Leonardo Santos Silva', '19019019019', 'leonardo.silva@institutofd.com.br', '1133445592', '11988776681', 3.50, 35000.00, true, NOW(), NOW()),
(1, 28, 'Gabriela Oliveira Lima', '20020020020', 'gabriela.lima@institutofd.com.br', '1133445593', '11988776682', 3.50, 35000.00, true, NOW(), NOW()),
(1, 29, 'Diego Fernandes Rocha', '21021021021', 'diego.rocha@institutofd.com.br', '1133445594', '11988776683', 3.00, 30000.00, true, NOW(), NOW()),
(1, 30, 'Cristiane Aparecida Souza', '22022022022', 'cristiane.souza@institutofd.com.br', '1133445595', '11988776684', 3.00, 30000.00, true, NOW(), NOW());

-- ====================================================================
-- 4. TAREFAS
-- ====================================================================
INSERT INTO tarefas (id_empresa, id, titulo, descricao, status, prioridade, data_criacao, data_vencimento, data_conclusao, id_responsavel, data_cadastro, data_alteracao, ativo) VALUES
(1, 1, 'Atualizar material do curso de Psicanálise', 'Revisar e atualizar apostilas e vídeo-aulas do módulo de Freud para a nova turma', 1, 3, '2024-09-01 08:00:00', '2024-09-15 18:00:00', NULL, 1, NOW(), NOW(), true),
(1, 2, 'Configurar turma EAD de outubro', 'Criar turma na plataforma, cadastrar alunos e liberar acesso aos módulos iniciais', 1, 3, '2024-09-05 09:00:00', '2024-09-25 17:00:00', NULL, 2, NOW(), NOW(), true),
(1, 3, 'Emitir certificados turma agosto', 'Gerar e enviar certificados digitais para os 45 alunos concluintes da turma de agosto', 2, 2, '2024-09-03 10:00:00', '2024-09-10 18:00:00', NULL, 3, NOW(), NOW(), true),
(1, 4, 'Campanha de matrículas Q4', 'Planejar e executar campanha de marketing digital para captação de novos alunos no último trimestre', 1, 3, '2024-09-02 08:30:00', '2024-09-30 18:00:00', NULL, 4, NOW(), NOW(), true),
(1, 5, 'Workshop presencial - Clínica Psicanalítica', 'Organizar workshop presencial com Angela Sirino sobre técnicas clínicas em psicanálise', 1, 2, '2024-09-10 14:00:00', '2024-10-05 18:00:00', NULL, 5, NOW(), NOW(), true),
(1, 6, 'Manutenção plataforma EAD', 'Realizar atualização de segurança e otimização de performance da plataforma de ensino', 1, 3, '2024-09-01 08:00:00', '2024-09-08 18:00:00', NULL, 6, NOW(), NOW(), true),
(1, 7, 'Pesquisa de satisfação alunos', 'Elaborar e enviar questionário de avaliação do curso para alunos ativos e concluintes', 1, 2, '2024-09-07 09:00:00', '2024-09-20 18:00:00', NULL, 7, NOW(), NOW(), true),
(1, 8, 'Gravar novas videoaulas', 'Produzir 10 videoaulas sobre Lacan para o módulo avançado do curso', 1, 3, '2024-09-15 10:00:00', '2024-10-15 18:00:00', NULL, 1, NOW(), NOW(), true),
(1, 9, 'Renovar licenças de software', 'Renovar licenças da plataforma EAD, Zoom e ferramentas de design para o próximo ano', 1, 2, '2024-09-20 08:00:00', '2024-09-30 18:00:00', NULL, 8, NOW(), NOW(), true),
(1, 10, 'Atendimento a inadimplentes', 'Entrar em contato com 15 alunos inadimplentes e negociar pagamento das mensalidades atrasadas', 1, 3, '2024-09-04 08:00:00', '2024-09-18 18:00:00', NULL, 9, NOW(), NOW(), true);

-- ====================================================================
-- 5. FORNECEDORES
-- ====================================================================
INSERT INTO fornecedores (id_empresa, id, tipo_pessoa, nome, cpf, cnpj, rg, email, telefone, celular, endereco, cidade, estado, cep, numero, complemento, bairro, ativo, observacoes, data_cadastro, data_alteracao) VALUES
(1, 1, 2, 'Hotmart Plataforma Digital', NULL, '11111111000111', NULL, 'contato@hotmart.com', '1133331111', '11911111111', 'Av. Brigadeiro Faria Lima, 1461', 'São Paulo', 25, '01452001', '1461', 'Torre Norte', 'Jardim Paulistano', true, 'Plataforma principal de vendas e hospedagem EAD', NOW(), NOW()),
(1, 2, 2, 'Eduzz Tecnologia Ltda', NULL, '22222222000222', NULL, 'comercial@eduzz.com', '1133332222', '11922222222', 'Av. Paulista, 1842', 'São Paulo', 25, '01310200', '1842', 'Conj 72', 'Bela Vista', true, 'Plataforma alternativa de cursos online', NOW(), NOW()),
(1, 3, 2, 'Gráfica Express Digital', NULL, '33333333000333', NULL, 'vendas@graficaexpress.com.br', '1133333333', '11933333333', 'Rua do Triunfo, 387', 'São Paulo', 25, '01212010', '387', NULL, 'Santa Efigênia', true, 'Impressão de certificados e materiais', NOW(), NOW()),
(1, 4, 2, 'Zoom Video Communications', NULL, '44444444000444', NULL, 'brasil@zoom.us', '1133334444', '11944444444', 'Av. das Nações Unidas, 12551', 'São Paulo', 25, '04578000', '12551', 'Torre Norte', 'Brooklin', true, 'Licenças para aulas ao vivo', NOW(), NOW()),
(1, 5, 2, 'RD Station Marketing Digital', NULL, '55555555000555', NULL, 'contato@rdstation.com', '4733030390', '47933030390', 'Rua Tenente Silveira, 234', 'Florianópolis', 24, '88010300', '234', '8º andar', 'Centro', true, 'Ferramenta de automação marketing', NOW(), NOW()),
(1, 6, 2, 'Adobe Systems Brasil', NULL, '66666666000666', NULL, 'vendas@adobe.com.br', '1133336666', '11966666666', 'Av. Presidente Juscelino Kubitschek, 1830', 'São Paulo', 25, '04543900', '1830', 'Torre II', 'Itaim Bibi', true, 'Licenças Creative Cloud para design', NOW(), NOW()),
(1, 7, 2, 'Google Workspace Brasil', NULL, '77777777000777', NULL, 'workspace@google.com', '1133337777', '11977777777', 'Av. Brigadeiro Faria Lima, 3477', 'São Paulo', 25, '04538133', '3477', '18º andar', 'Itaim Bibi', true, 'E-mail corporativo e ferramentas', NOW(), NOW()),
(1, 8, 2, 'Amazon Web Services Brasil', NULL, '88888888000888', NULL, 'aws-brasil@amazon.com', '1133338888', '11988888888', 'Av. Presidente Juscelino Kubitschek, 2041', 'São Paulo', 25, '04543011', '2041', 'Torre B', 'Vila Olímpia', true, 'Hospedagem e infraestrutura cloud', NOW(), NOW()),
(1, 9, 2, 'Vimeo Tecnologia Ltda', NULL, '99999999000999', NULL, 'business@vimeo.com', '1133339999', '11999999999', 'Av. Paulista, 1374', 'São Paulo', 25, '01310100', '1374', 'Conj 1501', 'Bela Vista', true, 'Hospedagem profissional de vídeos', NOW(), NOW()),
(1, 10, 2, 'Resultados Digitais Ltda', NULL, '10101010000101', NULL, 'contato@resultadosdigitais.com.br', '4733030300', '47933030300', 'Rodovia SC 401, 4100', 'Florianópolis', 24, '88032005', '4100', 'Bloco B', 'Saco Grande', true, 'Plataforma de CRM e automação', NOW(), NOW()),
(1, 11, 2, 'Canva Design Brasil', NULL, '11101110000111', NULL, 'empresas@canva.com', '1133331010', '11910101010', 'Av. Faria Lima, 2232', 'São Paulo', 25, '01451000', '2232', '22º andar', 'Jardim Paulistano', true, 'Ferramenta de design gráfico', NOW(), NOW()),
(1, 12, 2, 'Certificados Online Ltda', NULL, '12121212000121', NULL, 'contato@certificadosonline.com.br', '1133331212', '11912121212', 'Rua Pamplona, 518', 'São Paulo', 25, '01405000', '518', 'Sala 102', 'Jardim Paulista', true, 'Emissão digital de certificados', NOW(), NOW()),
(1, 13, 2, 'StreamYard Broadcasting', NULL, '13131313000131', NULL, 'support@streamyard.com', '1133331313', '11913131313', 'Av. Paulista, 1636', 'São Paulo', 25, '01310200', '1636', 'Conj 1204', 'Bela Vista', true, 'Transmissões ao vivo profissionais', NOW(), NOW()),
(1, 14, 2, 'Mailchimp Brasil', NULL, '14141414000141', NULL, 'brasil@mailchimp.com', '1133331414', '11914141414', 'Rua Funchal, 418', 'São Paulo', 25, '04551060', '418', '35º andar', 'Vila Olímpia', true, 'E-mail marketing para alunos', NOW(), NOW()),
(1, 15, 2, 'Typeform Tecnologia', NULL, '15151515000151', NULL, 'business@typeform.com', '1133331515', '11915151515', 'Av. Brigadeiro Faria Lima, 2055', 'São Paulo', 25, '01452001', '2055', 'Conj 31', 'Jardim Paulistano', true, 'Formulários e pesquisas online', NOW(), NOW()),
(1, 16, 2, 'Moodle Brasil Serviços', NULL, '16161616000161', NULL, 'contato@moodlebrasil.com.br', '1133331616', '11916161616', 'Rua Vergueiro, 3185', 'São Paulo', 25, '04101300', '3185', 'Sala 801', 'Vila Mariana', true, 'Consultoria plataforma Moodle', NOW(), NOW()),
(1, 17, 2, 'PayPal Brasil Ltda', NULL, '17171717000171', NULL, 'comercial@paypal.com.br', '1133331717', '11917171717', 'Av. das Nações Unidas, 14171', 'São Paulo', 25, '04794000', '14171', 'Torre A', 'Vila Gertrudes', true, 'Gateway de pagamento internacional', NOW(), NOW()),
(1, 18, 2, 'PagSeguro UOL', NULL, '18181818000181', NULL, 'empresas@pagseguro.com.br', '1133331818', '11918181818', 'Av. Brigadeiro Faria Lima, 1384', 'São Paulo', 25, '01451001', '1384', '7º andar', 'Jardim Paulistano', true, 'Gateway de pagamento nacional', NOW(), NOW()),
(1, 19, 2, 'Sympla Eventos Online', NULL, '19191919000191', NULL, 'contato@sympla.com.br', '1133331919', '11919191919', 'Rua Funchal, 375', 'São Paulo', 25, '04551060', '375', 'Conj 51', 'Vila Olímpia', true, 'Plataforma para workshops presenciais', NOW(), NOW()),
(1, 20, 2, 'Biblioteca Digital Psicanalítica', NULL, '20202020000202', NULL, 'acervo@bibliopsicanalise.com.br', '1133332020', '11920202020', 'Rua da Consolação, 930', 'São Paulo', 25, '01302000', '930', 'Sala 15', 'Consolação', true, 'Acervo digital de obras psicanalíticas', NOW(), NOW());


-- ====================================================================
-- LEADS - 50 REGISTROS
-- ====================================================================
INSERT INTO leads (id_empresa, id, nome, celular, email, tipo_retorno_contato, status, contexto, ativo, data_cadastro, data_alteracao) VALUES
(1, 1, 'Amanda Silva Ferreira', '11987654321', 'amanda.ferreira@email.com', 1, 1, 'Interessada no curso de Psicanálise EAD. Encontrou pelo Instagram da Angela Sirino.', true, NOW(), NOW()),
(1, 2, 'Bruno Henrique Costa', '11988776655', 'bruno.costa@email.com', 2, 2, 'Ligou para saber valores e condições de pagamento. Psicólogo em busca de especialização.', true, NOW(), NOW()),
(1, 3, 'Carla Patricia Oliveira', '11999887766', 'carla.oliveira@email.com', 1, 3, 'Muito interessada! Quer começar em outubro. Trabalha na área de RH.', true, NOW(), NOW()),
(1, 4, 'Daniel Augusto Santos', '11966778855', 'daniel.santos@email.com', 1, 1, 'Preencheu formulário do site pedindo mais informações sobre o curso clínico.', true, NOW(), NOW()),
(1, 5, 'Elaine Cristina Lima', '11955667744', 'elaine.lima@email.com', 2, 4, 'Ligou mas disse que não tem disponibilidade no momento. Pediu contato em 6 meses.', true, NOW(), NOW()),
(1, 6, 'Felipe Eduardo Rocha', '11944556633', 'felipe.rocha@email.com', 1, 5, 'Convertido! Matriculou-se no curso de Psicanálise Clínica - turma de outubro.', true, NOW(), NOW()),
(1, 7, 'Gabriela Mendes Souza', '11933445522', 'gabriela.souza@email.com', 1, 1, 'Mandou mensagem no WhatsApp. Estudante de Psicologia no último ano.', true, NOW(), NOW()),
(1, 8, 'Henrique Alves Dias', '11922334411', 'henrique.dias@email.com', 2, 2, 'Atendido por telefone. Pediu ementa detalhada do curso para avaliar.', true, NOW(), NOW()),
(1, 9, 'Isabela Rodrigues Melo', '11911223300', 'isabela.melo@email.com', 1, 3, 'Muito empolgada com o curso! Quer saber se pode parcelar em mais vezes.', true, NOW(), NOW()),
(1, 10, 'João Carlos Barbosa', '11900112299', 'joao.barbosa@email.com', 1, 1, 'Conheceu o instituto através de indicação. Médico psiquiatra.', true, NOW(), NOW()),
(1, 11, 'Karen Fernanda Santos', '11988776655', 'karen.santos@email.com', 2, 5, 'Convertida! Já iniciou os estudos na plataforma EAD.', true, NOW(), NOW()),
(1, 12, 'Leonardo Silva Costa', '11977665544', 'leonardo.costa@email.com', 1, 2, 'Respondeu WhatsApp. Quer agendar uma conversa com a coordenação.', true, NOW(), NOW()),
(1, 13, 'Mariana Cristina Lima', '11966554433', 'mariana.lima@email.com', 1, 1, 'Enviou e-mail perguntando sobre certificação e reconhecimento do curso.', true, NOW(), NOW()),
(1, 14, 'Nicolas Henrique Rocha', '11955443322', 'nicolas.rocha@email.com', 2, 4, 'Desistiu por questões financeiras. Pode retomar contato no futuro.', true, NOW(), NOW()),
(1, 15, 'Olivia Patricia Dias', '11944332211', 'olivia.dias@email.com', 1, 3, 'Animada com o curso! Profissional da área de Recursos Humanos.', true, NOW(), NOW()),
(1, 16, 'Paulo Roberto Silva', '11933221100', 'paulo.silva@email.com', 1, 1, 'Deixou dados no formulário. Interessado em terapia cognitiva.', true, NOW(), NOW()),
(1, 17, 'Queila Fernandes Souza', '11922110099', 'queila.souza@email.com', 2, 5, 'Matriculada! Começou no curso de Psicanálise EAD em setembro.', true, NOW(), NOW()),
(1, 18, 'Rafael Augusto Melo', '11911009988', 'rafael.melo@email.com', 1, 2, 'Contatado via WhatsApp. Solicitou grade curricular completa.', true, NOW(), NOW()),
(1, 19, 'Sabrina Helena Costa', '11900998877', 'sabrina.costa@email.com', 1, 1, 'Nova lead do Google Ads. Clicou no anúncio sobre psicanálise clínica.', true, NOW(), NOW()),
(1, 20, 'Thiago Luiz Santos', '11988887766', 'thiago.santos@email.com', 2, 3, 'Ligou interessado. Quer parcelar em 12x. Aguardando proposta.', true, NOW(), NOW()),
(1, 21, 'Ursula Maria Oliveira', '11977776655', 'ursula.oliveira@email.com', 1, 1, 'Mensagem pelo Instagram. Pedagoga buscando especialização.', true, NOW(), NOW()),
(1, 22, 'Vinicius Eduardo Lima', '11966665544', 'vinicius.lima@email.com', 1, 4, 'Não demonstrou interesse após apresentação dos valores. Perdeu o contato.', true, NOW(), NOW()),
(1, 23, 'Wanda Cristina Rocha', '11955554433', 'wanda.rocha@email.com', 2, 5, 'Convertida em aluna! Matriculada e ativa no curso EAD.', true, NOW(), NOW()),
(1, 24, 'Xavier Henrique Dias', '11944443322', 'xavier.dias@email.com', 1, 2, 'Respondeu mensagem. Quer conhecer depoimentos de ex-alunos.', true, NOW(), NOW()),
(1, 25, 'Yasmin Fernanda Costa', '11933332211', 'yasmin.costa@email.com', 1, 3, 'Muito interessada! Assistente social querendo aprofundar conhecimentos.', true, NOW(), NOW()),
(1, 26, 'Zilda Patricia Santos', '11922221100', 'zilda.santos@email.com', 2, 1, 'Ligou para tirar dúvidas. Aguardando retorno com informações solicitadas.', true, NOW(), NOW()),
(1, 27, 'Arthur Luis Silva', '11911110099', 'arthur.silva@email.com', 1, 5, 'Aluno matriculado! Iniciou curso na turma de agosto.', true, NOW(), NOW()),
(1, 28, 'Beatriz Helena Souza', '11900009988', 'beatriz.souza@email.com', 1, 1, 'Contato via site. Interessada em workshop presencial com Angela Sirino.', true, NOW(), NOW()),
(1, 29, 'César Augusto Melo', '11989898989', 'cesar.melo@email.com', 2, 2, 'Atendido por telefone. Pediu prazo de 3 dias para decidir sobre matrícula.', true, NOW(), NOW()),
(1, 30, 'Débora Cristina Lima', '11978787878', 'debora.lima@email.com', 1, 3, 'Empolgada! Filósofa querendo estudar psicanálise. Precisa de desconto.', true, NOW(), NOW()),
(1, 31, 'Eduardo Fernandes Rocha', '11967676767', 'eduardo.rocha@email.com', 1, 1, 'Nova lead pelo Facebook. Clicou em anúncio sobre formação em psicanálise.', true, NOW(), NOW()),
(1, 32, 'Flávia Patricia Costa', '11956565656', 'flavia.costa@email.com', 2, 4, 'Desistiu. Preferiu outro instituto com mensalidade mais baixa.', true, NOW(), NOW()),
(1, 33, 'Guilherme Henrique Santos', '11945454545', 'guilherme.santos@email.com', 1, 5, 'Convertido! Aluno ativo desde julho. Muito satisfeito com o curso.', true, NOW(), NOW()),
(1, 34, 'Helena Cristina Dias', '11934343434', 'helena.dias@email.com', 1, 2, 'Contatada via WhatsApp. Quer saber sobre possibilidade de bolsa ou desconto.', true, NOW(), NOW()),
(1, 35, 'Igor Eduardo Silva', '11923232323', 'igor.silva@email.com', 2, 1, 'Ligou interessado. Deixou dados para receber material do curso por e-mail.', true, NOW(), NOW()),
(1, 36, 'Júlia Fernanda Souza', '11912121212', 'julia.souza@email.com', 1, 3, 'Muito animada! Coach em transição de carreira. Quer se especializar.', true, NOW(), NOW()),
(1, 37, 'Kevin Luis Oliveira', '11901010101', 'kevin.oliveira@email.com', 1, 1, 'Preencheu formulário. Engenheiro buscando autoconhecimento e nova carreira.', true, NOW(), NOW()),
(1, 38, 'Larissa Helena Melo', '11990909090', 'larissa.melo@email.com', 2, 5, 'Matriculada! Iniciou na turma de setembro. Enfermeira buscando humanização.', true, NOW(), NOW()),
(1, 39, 'Márcio Augusto Costa', '11989898988', 'marcio.costa@email.com', 1, 2, 'Respondeu contato. Solicitou conversa com ex-alunos antes de decidir.', true, NOW(), NOW()),
(1, 40, 'Natália Cristina Santos', '11978787877', 'natalia.santos@email.com', 1, 1, 'Lead do Instagram. Nutricionista interessada em atendimento clínico diferenciado.', true, NOW(), NOW()),
(1, 41, 'Oscar Fernando Lima', '11967676766', 'oscar.lima@email.com', 2, 4, 'Não retornou após primeira ligação. Tentativas de contato sem sucesso.', true, NOW(), NOW()),
(1, 42, 'Priscila Helena Rocha', '11956565655', 'priscila.rocha@email.com', 1, 3, 'Interessada! Jornalista querendo entender comportamento humano profundamente.', true, NOW(), NOW()),
(1, 43, 'Quirino Eduardo Silva', '11945454544', 'quirino.silva@email.com', 1, 5, 'Convertido! Aluno matriculado e muito participativo nas aulas ao vivo.', true, NOW(), NOW()),
(1, 44, 'Renata Cristina Costa', '11934343433', 'renata.costa@email.com', 2, 2, 'Atendida. Pediu para pensar no final de semana e retornar na segunda.', true, NOW(), NOW()),
(1, 45, 'Samuel Henrique Dias', '11923232322', 'samuel.dias@email.com', 1, 1, 'Contato via WhatsApp Business. Publicitário em busca de desenvolvimento pessoal.', true, NOW(), NOW()),
(1, 46, 'Talita Fernanda Souza', '11912121211', 'talita.souza@email.com', 1, 3, 'Muito empolgada! Socióloga querendo aprofundar estudos sobre inconsciente.', true, NOW(), NOW()),
(1, 47, 'Ulisses Luis Oliveira', '11901010100', 'ulisses.oliveira@email.com', 2, 1, 'Ligou pedindo informações. Aguardando envio de proposta comercial detalhada.', true, NOW(), NOW()),
(1, 48, 'Valéria Patricia Melo', '11990909089', 'valeria.melo@email.com', 1, 5, 'Aluna ativa! Convertida em agosto. Fisioterapeuta adorando o curso.', true, NOW(), NOW()),
(1, 49, 'Wagner Augusto Santos', '11989898987', 'wagner.santos@email.com', 1, 2, 'Contatado. Administrador de empresas buscando autoconhecimento para liderança.', true, NOW(), NOW()),
(1, 50, 'Yara Cristina Lima', '11978787876', 'yara.lima@email.com', 2, 3, 'Ligação atendida. Muito interessada! Designer querendo atuar com terapia pela arte.', true, NOW(), NOW());


-- ====================================================================
-- AJUSTES
-- ====================================================================
UPDATE vendedores SET tipo_pessoa = 1;
UPDATE leads SET status = 2 WHERE status > 4;