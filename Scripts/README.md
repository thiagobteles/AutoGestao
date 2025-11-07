# Scripts de Carga de Dados - Sistema de Contabilidade

## 📋 Descrição

Scripts SQL para popular o banco de dados PostgreSQL com dados de demonstração para o **Sistema de Contabilidade ContaExpert**.

## 🚀 Como Executar

### Opção 1: Script Master (Recomendado)

Execute o script master que roda todos os inserts automaticamente:

```bash
psql -U seu_usuario -d nome_do_banco -f 00_EXECUTAR_TODOS.sql
```

### Opção 2: Scripts Individuais

Execute os scripts na ordem numérica:

```bash
psql -U seu_usuario -d nome_do_banco -f 01_Insert_CNAEs.sql
psql -U seu_usuario -d nome_do_banco -f 02_Insert_Contadores.sql
psql -U seu_usuario -d nome_do_banco -f 03_Insert_EmpresasClientes.sql
# ... e assim por diante
```

### Opção 3: Pelo pgAdmin

1. Abra o pgAdmin
2. Conecte ao banco de dados
3. Clique com botão direito no banco → **Query Tool**
4. Abra o arquivo `00_EXECUTAR_TODOS.sql`
5. Execute (F5)

## 📊 Conteúdo dos Scripts

| Script | Tabela | Registros | Descrição |
|--------|--------|-----------|-----------|
| `01_Insert_CNAEs.sql` | cnaes | 50 | CNAEs mais comuns no Brasil |
| `02_Insert_Contadores.sql` | contadores_responsaveis | 50 | Contadores com CRC de todo Brasil |
| `03_Insert_EmpresasClientes.sql` | empresas_clientes | 50 | Empresas diversificadas |
| `04_Insert_CertificadosDigitais.sql` | certificados_digitais | 50 | Certificados A1 e A3 |
| `05_Insert_ParametrosFiscais.sql` | parametros_fiscais | 50 | Configurações fiscais |
| `06_Insert_DadosBancarios.sql` | dados_bancarios | 50 | Contas bancárias |
| `07_Insert_AliquotasImpostos.sql` | aliquotas_impostos | 50 | Alíquotas por região |
| `08_Insert_PlanoContas.sql` | plano_contas | 50 | Estrutura contábil |
| `09_Insert_ObrigacoesFiscais.sql` | obrigacoes_fiscais | 50 | SPED, DCTF, eSocial, etc |
| `10_Insert_Clientes.sql` | clientes | 50 | Clientes PF e PJ |
| `11_Insert_NotasFiscais.sql` | notas_fiscais | 50 | NFe, NFCe e NFSe |
| `12_Insert_LancamentosContabeis.sql` | lancamentos_contabeis | 50 | Lançamentos com partidas dobradas |

**TOTAL: 600 registros**

## ⚠️ Importante

- Os scripts devem ser executados **NA ORDEM** (01, 02, 03...)
- Certifique-se de que as migrations foram aplicadas antes
- Os dados são fictícios mas realistas
- CNPJs e CPFs são apenas formatados, não validados pela Receita

## 🎯 Dados Incluídos

### Características dos Dados

- ✅ **50 CNAEs** reais mais utilizados no Brasil
- ✅ **50 Contadores** com CRCs de todos os estados
- ✅ **50 Empresas** de diversos segmentos e regiões
- ✅ **50 Certificados Digitais** A1 e A3 com validades diversas
- ✅ **50 Configurações Fiscais** completas por empresa
- ✅ **50 Contas Bancárias** dos principais bancos brasileiros
- ✅ **50 Alíquotas** de impostos (ICMS, ISS, PIS, COFINS, etc)
- ✅ **50 Contas Contábeis** estrutura hierárquica completa
- ✅ **50 Obrigações Fiscais** (entregues, pendentes e atrasadas)
- ✅ **50 Clientes** (25 PF e 25 PJ) com dados completos
- ✅ **50 Notas Fiscais** (NFe, NFCe, NFSe) emitidas e canceladas
- ✅ **50 Lançamentos Contábeis** com partidas dobradas vinculadas

### Distribuição Geográfica

Empresas distribuídas por todo o Brasil:
- São Paulo, Rio de Janeiro, Minas Gerais
- Rio Grande do Sul, Paraná, Santa Catarina
- Bahia, Ceará, Pernambuco
- E todos os demais estados

### Regimes Tributários

- Lucro Real
- Lucro Presumido
- Simples Nacional

## 🔧 Troubleshooting

### Erro: "permission denied"
```sql
-- Verifique permissões do usuário
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO seu_usuario;
```

### Erro: "violates foreign key constraint"
```sql
-- Execute os scripts na ordem correta (01, 02, 03...)
-- O script 00_EXECUTAR_TODOS.sql já resolve isso
```

### Limpar dados e reexecutar
```sql
-- CUIDADO: Isso apaga todos os dados!
TRUNCATE TABLE lancamentos_contabeis CASCADE;
TRUNCATE TABLE notas_fiscais CASCADE;
TRUNCATE TABLE clientes CASCADE;
TRUNCATE TABLE obrigacoes_fiscais CASCADE;
TRUNCATE TABLE plano_contas CASCADE;
TRUNCATE TABLE aliquotas_impostos CASCADE;
TRUNCATE TABLE dados_bancarios CASCADE;
TRUNCATE TABLE parametros_fiscais CASCADE;
TRUNCATE TABLE certificados_digitais CASCADE;
TRUNCATE TABLE empresas_clientes CASCADE;
TRUNCATE TABLE contadores_responsaveis CASCADE;
TRUNCATE TABLE cnaes CASCADE;

-- Depois reexecute:
psql -U seu_usuario -d nome_do_banco -f 00_EXECUTAR_TODOS.sql
```

## 📞 Suporte

Em caso de dúvidas ou problemas, verifique:
1. Se as migrations foram aplicadas corretamente
2. Se o banco de dados está acessível
3. Se o usuário tem permissões adequadas

## 🎉 Resultado

Após executar os scripts, o sistema estará com:
- Dados realistas e profissionais
- Empresas de diversos segmentos
- Obrigações fiscais com diferentes status
- Estrutura contábil completa
- **Sistema pronto para apresentação!**

---

**Desenvolvido para o Sistema de Contabilidade ContaExpert**
**Versão dos scripts: 1.0.0**
