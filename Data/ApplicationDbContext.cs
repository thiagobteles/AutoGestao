using FGT.Entidades;
using FGT.Entidades.Base;
using FGT.Entidades.Processamento;
using FGT.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FGT.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        // CRÍTICO: Propriedade usada pelo Query Filter Global para Multi-Tenancy
        // Deve ser setada pelo serviço antes de qualquer query
        public long CurrentEmpresaId { get; set; }

        #region DbSets

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioEmpresaCliente> UsuarioEmpresaClientes { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ReportTemplateEntity> ReportTemplates { get; set; }

        #endregion

        #region DbSets Específicos da contabilidade

        // 📊 Entidades Fiscais/Contabilidade
        public DbSet<EmpresaCliente> EmpresasClientes { get; set; }
        public DbSet<NotaFiscal> NotasFiscais { get; set; }
        public DbSet<NotaFiscalItem> NotasFiscaisItens { get; set; }
        public DbSet<CertificadoDigital> CertificadosDigitais { get; set; }
        public DbSet<CNAE> CNAEs { get; set; }
        public DbSet<AliquotaImposto> AliquotasImpostos { get; set; }
        public DbSet<ParametroFiscal> ParametrosFiscais { get; set; }
        public DbSet<ContadorResponsavel> ContadoresResponsaveis { get; set; }
        public DbSet<DadoBancario> DadosBancarios { get; set; }
        public DbSet<PlanoContas> PlanoContas { get; set; }
        public DbSet<LancamentoContabil> LancamentosContabeis { get; set; }
        public DbSet<ObrigacaoFiscal> ObrigacoesFiscais { get; set; }
        public DbSet<NegociacaoFiscal> NegociacoesFiscais { get; set; }

        #endregion DbSets Específicos da contabilidade

        #region DbSets de Processamento

        public DbSet<ImportacaoNegociacaoFiscal> ImportacoesNegociacoesFiscais { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<BaseEntidade>();

            // ===========================================
            // CONFIGURAÇÃO CRÍTICA: QUERY FILTER GLOBAL PARA MULTI-TENANCY
            // IMPORTANTE: Garante que TODAS as queries filtram por IdEmpresa automaticamente
            // Evita vazamento de dados entre tenants (empresas)
            // ===========================================
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Aplicar filtro apenas para entidades que herdam de BaseEntidade
                if (typeof(BaseEntidade).IsAssignableFrom(entityType.ClrType))
                {
                    // Entidades que NÃO devem ter filtro de tenant (são compartilhadas)
                    var entitiesToExclude = new[] { typeof(Usuario), typeof(Empresa) };

                    if (!entitiesToExclude.Contains(entityType.ClrType))
                    {
                        // Criar expressão lambda: e => e.IdEmpresa == CurrentEmpresaId
                        var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                        var property = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseEntidade.IdEmpresa));
                        var empresaIdProperty = System.Linq.Expressions.Expression.Property(
                            System.Linq.Expressions.Expression.Constant(this),
                            nameof(CurrentEmpresaId));
                        var comparison = System.Linq.Expressions.Expression.Equal(property, empresaIdProperty);
                        var lambda = System.Linq.Expressions.Expression.Lambda(comparison, parameter);

                        // Aplicar o filtro
                        modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                    }
                }
            }

            // ===========================================
            // CONFIGURAÇÃO GLOBAL: IGNORAR PROPRIEDADES DE AUDITORIA
            // IMPORTANTE: Deve vir ANTES de qualquer outra configuração
            // ===========================================
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Verifica se a entidade herda de BaseEntidade
                if (typeof(BaseEntidade).IsAssignableFrom(entityType.ClrType))
                {
                    // Configura Id como auto-incremento
                    modelBuilder.Entity(entityType.ClrType).Property("Id").ValueGeneratedOnAdd();

                    // Ignora as propriedades de navegação de auditoria para evitar conflitos
                    modelBuilder.Entity(entityType.ClrType).Ignore(nameof(BaseEntidade.CriadoPorUsuario));
                    modelBuilder.Entity(entityType.ClrType).Ignore(nameof(BaseEntidade.AlteradoPorUsuario));
                }
            }

            // ===========================================
            // IGNORAR PROPRIEDADES PROBLEMÁTICAS DO USUARIO
            // ===========================================
            modelBuilder.Entity<Usuario>().Ignore(u => u.EntidadesCriadas);
            modelBuilder.Entity<Usuario>().Ignore(u => u.EntidadesAlteradas);
            modelBuilder.Entity<Usuario>().Ignore(u => u.ConfirmarSenha);

            // Configuração global para snake_case
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName()?.ToSnakeCase();
                if (!string.IsNullOrEmpty(tableName))
                {
                    entity.SetTableName(tableName);
                }

                foreach (var property in entity.GetProperties())
                {
                    var columnName = property.GetColumnName().ToSnakeCase();
                    property.SetColumnName(columnName);
                }

                foreach (var key in entity.GetKeys())
                {
                    var keyName = key.GetName()?.ToSnakeCase();
                    if (!string.IsNullOrEmpty(keyName))
                    {
                        key.SetName(keyName);
                    }
                }

                foreach (var foreignKey in entity.GetForeignKeys())
                {
                    var tableNameInterno = entity.ClrType.Name.ToSnakeCase();
                    var originalName = foreignKey.GetConstraintName();

                    if (string.IsNullOrEmpty(originalName))
                    {
                        var fkProperty = foreignKey.Properties[0].GetColumnName().ToSnakeCase();
                        foreignKey.SetConstraintName($"fk_{tableNameInterno}_{fkProperty}");
                    }
                }
            }

            #region CONFIGURAÇÕES DAS ENTIDADES

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE CLIENTE
            // ===========================================
            modelBuilder.Entity<Cliente>().ToTable("clientes");
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.Property(e => e.TipoPessoa).IsRequired();
                entity.Property(e => e.Nome).HasMaxLength(250).IsRequired();
                entity.Property(e => e.Cpf).HasMaxLength(14);
                entity.Property(e => e.Cnpj).HasMaxLength(18);
                entity.Property(e => e.Rg).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(150);
                entity.Property(e => e.Telefone).HasMaxLength(20);
                entity.Property(e => e.Celular).HasMaxLength(20);
                entity.Property(e => e.Endereco).HasMaxLength(500);
                entity.Property(e => e.Cidade).HasMaxLength(100);
                entity.Property(e => e.Estado).IsRequired();
                entity.Property(e => e.CEP).HasMaxLength(10);
                entity.Property(e => e.Numero).HasMaxLength(20);
                entity.Property(e => e.Complemento).HasMaxLength(150);
                entity.Property(e => e.Bairro).HasMaxLength(100);
                entity.Property(e => e.Ativo).IsRequired();
                entity.Property(e => e.Observacoes).HasMaxLength(2000);
                entity.Property(e => e.FotoCliente).HasMaxLength(10000);
                entity.Property(e => e.DocumentoRG).HasMaxLength(10000);
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired();
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE USUARIO
            // ===========================================
            modelBuilder.Entity<Usuario>().ToTable("usuarios");
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.Property(e => e.Nome).HasMaxLength(250).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(150).IsRequired();
                entity.Property(e => e.SenhaHash).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Cpf).HasMaxLength(14);
                entity.Property(e => e.Telefone).HasMaxLength(20);
                entity.Property(e => e.Perfil).IsRequired();
                entity.Property(e => e.Ativo).IsRequired().HasDefaultValue(true);
                entity.Property(e => e.Observacoes).HasMaxLength(2000);
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired();
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE EMPRESA
            // ===========================================
            modelBuilder.Entity<Empresa>().ToTable("empresas");
            modelBuilder.Entity<Empresa>(entity =>
            {
                entity.Property(e => e.RazaoSocial).HasMaxLength(250).IsRequired();
                entity.Property(e => e.Cnpj).HasMaxLength(18);
                entity.Property(e => e.Email).HasMaxLength(150);
                entity.Property(e => e.Telefone).HasMaxLength(20);
                entity.Property(e => e.Celular).HasMaxLength(20);
                entity.Property(e => e.Endereco).HasMaxLength(500);
                entity.Property(e => e.Cidade).HasMaxLength(100);
                entity.Property(e => e.Estado).IsRequired();
                entity.Property(e => e.CEP).HasMaxLength(10);
                entity.Property(e => e.Numero).HasMaxLength(20);
                entity.Property(e => e.Complemento).HasMaxLength(150);
                entity.Property(e => e.Bairro).HasMaxLength(100);
                entity.Property(e => e.Ativo).IsRequired();
                entity.Property(e => e.Observacoes).HasMaxLength(2000);
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired();
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE AUDITLOG
            // ===========================================
            modelBuilder.Entity<AuditLog>().ToTable("audit_logs");
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.EntidadeNome).HasMaxLength(100).IsRequired();
                entity.Property(e => e.EntidadeId).HasMaxLength(50).IsRequired();
                entity.Property(e => e.TipoOperacao).IsRequired();
                entity.Property(e => e.TabelaNome).HasMaxLength(100);
                entity.Property(e => e.IpCliente).HasMaxLength(45);
                entity.Property(e => e.UrlRequisicao).HasMaxLength(500);
                entity.Property(e => e.MetodoHttp).HasMaxLength(10);
                entity.Property(e => e.MensagemErro).HasMaxLength(2000);
                entity.Property(e => e.Sucesso).IsRequired();
                entity.Property(e => e.DataHora).IsRequired();
            });


            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE ReportTemplateEntity
            // ===========================================
            modelBuilder.Entity<ReportTemplateEntity>().ToTable("report_templates");
            modelBuilder.Entity<ReportTemplateEntity>(entity =>
            {
                entity.Property(e => e.Nome).HasMaxLength(200).IsRequired();
                entity.Property(e => e.TipoEntidade).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Descricao).HasMaxLength(500);
                entity.Property(e => e.TemplateJson).IsRequired();
                entity.Property(e => e.Padrao).HasDefaultValue(false);
                entity.Property(e => e.Ativo).HasDefaultValue(true);

                // Índices
                entity.HasIndex(e => e.TipoEntidade);
                entity.HasIndex(e => new { e.TipoEntidade, e.Padrao });
            });

            #endregion CONFIGURAÇÕES DAS ENTIDADES

            #region CONFIGURAÇÕES DE RELACIONAMENTOS

            // ===========================================
            // RELACIONAMENTOS DO USUARIO
            // ===========================================
            modelBuilder.Entity<Usuario>().HasOne(u => u.Empresa).WithMany().HasForeignKey(u => u.IdEmpresa).OnDelete(DeleteBehavior.SetNull);

            // ===========================================
            // RELACIONAMENTOS DO AUDITLOG
            // ===========================================
            modelBuilder.Entity<AuditLog>().HasOne(v => v.Empresa).WithMany().HasForeignKey(v => v.IdEmpresa).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<AuditLog>().HasOne(a => a.Usuario).WithMany(u => u.AuditLogs).HasForeignKey(a => a.UsuarioId).OnDelete(DeleteBehavior.SetNull);

            #endregion CONFIGURAÇÕES DE RELACIONAMENTOS

            #region CONFIGURAÇÕES DE ÍNDICES

            // ===========================================
            // ÍNDICES ÚNICOS - CLIENTE
            // ===========================================
            modelBuilder.Entity<Cliente>().HasIndex(c => c.Cpf).IsUnique().HasFilter("cpf IS NOT NULL");
            modelBuilder.Entity<Cliente>().HasIndex(c => c.Cnpj).IsUnique().HasFilter("cnpj IS NOT NULL");

            // ===========================================
            // ÍNDICES PARA PERFORMANCE - CLIENTE
            // ===========================================
            modelBuilder.Entity<Cliente>().HasIndex(c => c.Nome).HasDatabaseName("ix_cliente_nome");

            // ===========================================
            // ÍNDICES PARA EMPRESA
            // ===========================================
            modelBuilder.Entity<Usuario>().HasIndex(u => u.IdEmpresa);
            modelBuilder.Entity<AuditLog>().HasIndex(a => a.IdEmpresa);

            // ===========================================
            // ÍNDICES PARA PERFORMANCE - AUDITLOG
            // ===========================================
            modelBuilder.Entity<AuditLog>().HasIndex(a => a.DataHora);
            modelBuilder.Entity<AuditLog>().HasIndex(a => a.UsuarioId);
            modelBuilder.Entity<AuditLog>().HasIndex(a => new { a.EntidadeNome, a.EntidadeId });

            #endregion CONFIGURAÇÕES DE ÍNDICES

            ConfiguracoesEspecificas(modelBuilder);
        }

        private static void ConfiguracoesEspecificas(ModelBuilder modelBuilder)
        {
            #region CONFIGURAÇÕES ESPECÍFICAS DA CONTABILIDADE

            // ===========================================
            // RELACIONAMENTOS DAS ENTIDADES FISCAIS/CONTÁBEIS
            // ===========================================

            // LancamentoContabil - Múltiplos relacionamentos com PlanoContas
            modelBuilder.Entity<LancamentoContabil>()
                .HasOne(l => l.ContaDebito)
                .WithMany()
                .HasForeignKey(l => l.ContaDebitoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LancamentoContabil>()
                .HasOne(l => l.ContaCredito)
                .WithMany()
                .HasForeignKey(l => l.ContaCreditoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LancamentoContabil>()
                .HasOne(l => l.EmpresaCliente)
                .WithMany(e => e.Lancamentos)
                .HasForeignKey(l => l.EmpresaClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // PlanoContas - Auto-relacionamento para hierarquia
            modelBuilder.Entity<PlanoContas>()
                .HasOne(p => p.ContaPai)
                .WithMany(p => p.ContasFilhas)
                .HasForeignKey(p => p.ContaPaiId)
                .OnDelete(DeleteBehavior.Restrict);

            // EmpresaCliente - Relacionamentos
            modelBuilder.Entity<EmpresaCliente>()
                .HasOne(e => e.CNAEPrincipal)
                .WithMany()
                .HasForeignKey(e => e.CNAEPrincipalId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EmpresaCliente>()
                .HasOne(e => e.ContadorResponsavel)
                .WithMany(c => c.EmpresasClientes)
                .HasForeignKey(e => e.ContadorResponsavelId)
                .OnDelete(DeleteBehavior.SetNull);

            #endregion
        }
    }
}