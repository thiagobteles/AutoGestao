using AutoGestao.Entidades;
using AutoGestao.Entidades.Relatorio;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador;
using AutoGestao.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        // CRÍTICO: Propriedade usada pelo Query Filter Global para Multi-Tenancy
        // Deve ser setada pelo serviço antes de qualquer query
        public long CurrentEmpresaId { get; set; }

        #region DbSets

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<VeiculoCor> VeiculoCores { get; set; }
        public DbSet<VeiculoFilial> VeiculoFiliais { get; set; }
        public DbSet<VeiculoLocalizacao> VeiculoLocalizacoes { get; set; }
        public DbSet<VeiculoMarca> VeiculoMarcas { get; set; }
        public DbSet<VeiculoMarcaModelo> VeiculoMarcaModelos { get; set; }
        public DbSet<VeiculoFoto> VeiculoFotos { get; set; }
        public DbSet<VeiculoDocumento> VeiculoDocumentos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ReportTemplateEntity> ReportTemplates { get; set; }
        public DbSet<Lead> Leads { get; set; }

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
                        var fkProperty = foreignKey.Properties.First().GetColumnName().ToSnakeCase();
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
            // CONFIGURAÇÕES DA ENTIDADE VEÍCULO
            // ===========================================
            modelBuilder.Entity<Veiculo>().ToTable("veiculos");
            modelBuilder.Entity<Veiculo>(entity =>
            {
                entity.Property(e => e.Codigo).HasMaxLength(50);
                entity.Property(e => e.Placa).HasMaxLength(10).IsRequired();
                entity.Property(e => e.Chassi).HasMaxLength(17);
                entity.Property(e => e.Renavam).HasMaxLength(11);
                entity.Property(e => e.AnoFabricacao).IsRequired();
                entity.Property(e => e.AnoModelo).IsRequired();
                entity.Property(e => e.KmSaida).HasColumnType("decimal(18,2)");
                entity.Property(e => e.KmEntrada).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Combustivel).IsRequired();
                entity.Property(e => e.Cambio).IsRequired();
                entity.Property(e => e.NumeroPortas).IsRequired();
                entity.Property(e => e.CapacidadePortaMalas).IsRequired(false);
                entity.Property(e => e.PrecoCompra).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PrecoVenda).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Observacoes).HasMaxLength(2000);
                entity.Property(e => e.DataEntrada).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired();
            });

            // ===========================================
            // CONFIGURAÇÕES DAS ENTIDADES AUXILIARES
            // ===========================================
            modelBuilder.Entity<VeiculoCor>().ToTable("veiculo_cores");
            modelBuilder.Entity<VeiculoCor>(entity =>
            {
                entity.Property(e => e.Descricao).HasMaxLength(50).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired();
            });

            modelBuilder.Entity<VeiculoFilial>().ToTable("veiculo_filiais");
            modelBuilder.Entity<VeiculoFilial>(entity =>
            {
                entity.Property(e => e.Descricao).HasMaxLength(100).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired();
            });

            modelBuilder.Entity<VeiculoLocalizacao>().ToTable("veiculo_localizacoes");
            modelBuilder.Entity<VeiculoLocalizacao>(entity =>
            {
                entity.Property(e => e.Descricao).HasMaxLength(100).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired();
            });

            modelBuilder.Entity<VeiculoMarca>().ToTable("veiculo_marcas");
            modelBuilder.Entity<VeiculoMarca>(entity =>
            {
                entity.Property(e => e.Descricao).HasMaxLength(100).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired();
            });

            modelBuilder.Entity<VeiculoMarcaModelo>().ToTable("veiculo_marca_modelos");
            modelBuilder.Entity<VeiculoMarcaModelo>(entity =>
            {
                entity.Property(e => e.Descricao).HasMaxLength(100).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired();
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE VEICULO FOTO
            // ===========================================
            modelBuilder.Entity<VeiculoFoto>().ToTable("veiculo_fotos");
            modelBuilder.Entity<VeiculoFoto>(entity =>
            {
                entity.Property(e => e.NomeArquivo).HasMaxLength(255).IsRequired();
                entity.Property(e => e.CaminhoArquivo).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Descricao).HasMaxLength(200);
                entity.Property(e => e.DataUpload).IsRequired();
                entity.Property(e => e.Principal).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired();
                entity.Property(e => e.Foto).HasMaxLength(10000).IsRequired();
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE VEICULO DOCUMENTO
            // ===========================================
            modelBuilder.Entity<VeiculoDocumento>().ToTable("veiculo_documentos");
            modelBuilder.Entity<VeiculoDocumento>(entity =>
            {
                entity.Property(e => e.TipoDocumento).IsRequired();
                entity.Property(e => e.Documento).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Observacoes).HasMaxLength(500).IsRequired();
                entity.Property(e => e.DataUpload).IsRequired();
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
            // CONFIGURAÇÕES DA ENTIDADE LEAD
            // ===========================================
            modelBuilder.Entity<Lead>().ToTable("leads");
            modelBuilder.Entity<Lead>(entity =>
            {
                entity.Property(e => e.Nome).HasMaxLength(250).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(150);
                entity.Property(e => e.Celular).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Contexto).HasMaxLength(-1);
                entity.Property(e => e.TipoRetornoContato).IsRequired().HasDefaultValue(EnumTipoRetornoContato.Whatsapp);
                entity.Property(e => e.Status).IsRequired().HasDefaultValue(EnumStatusLead.Pendente);
                entity.Property(e => e.Ativo).IsRequired().HasDefaultValue(true);
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
            // RELACIONAMENTOS DO VEÍCULO
            // ===========================================
            modelBuilder.Entity<Veiculo>().HasOne(v => v.Empresa).WithMany().HasForeignKey(v => v.IdEmpresa).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Veiculo>().HasOne(v => v.Cliente).WithMany(c => c.Veiculos).HasForeignKey(v => v.IdCliente).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Veiculo>().HasOne(v => v.VeiculoCor).WithMany().HasForeignKey(v => v.IdVeiculoCor).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Veiculo>().HasOne(v => v.VeiculoFilial).WithMany().HasForeignKey(v => v.IdVeiculoFilial).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Veiculo>().HasOne(v => v.VeiculoLocalizacao).WithMany().HasForeignKey(v => v.IdVeiculoLocalizacao).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Veiculo>().HasOne(v => v.VeiculoMarca).WithMany().HasForeignKey(v => v.IdVeiculoMarca).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Veiculo>().HasOne(v => v.VeiculoMarcaModelo).WithMany().HasForeignKey(v => v.IdVeiculoMarcaModelo).OnDelete(DeleteBehavior.SetNull);

            // ===========================================
            // RELACIONAMENTO ENTRE MARCA E MODELO
            // ===========================================
            modelBuilder.Entity<VeiculoMarcaModelo>().HasOne(v => v.Empresa).WithMany().HasForeignKey(v => v.IdEmpresa).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<VeiculoMarcaModelo>().HasOne(m => m.VeiculoMarca).WithMany().HasForeignKey(m => m.IdVeiculoMarca).OnDelete(DeleteBehavior.SetNull);

            // ===========================================
            // RELACIONAMENTOS DAS FOTOS DO VEÍCULO
            // ===========================================
            modelBuilder.Entity<VeiculoFoto>().HasOne(v => v.Empresa).WithMany().HasForeignKey(v => v.IdEmpresa).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<VeiculoFoto>().HasOne(vf => vf.Veiculo).WithMany(v => v.Fotos).HasForeignKey(vf => vf.IdVeiculo).OnDelete(DeleteBehavior.Cascade);

            // ===========================================
            // RELACIONAMENTOS DOS DOCUMENTOS DO VEÍCULO
            // ===========================================
            modelBuilder.Entity<VeiculoDocumento>().HasOne(v => v.Empresa).WithMany().HasForeignKey(v => v.IdEmpresa).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<VeiculoDocumento>().HasOne(vd => vd.Veiculo).WithMany(v => v.Documentos).HasForeignKey(vd => vd.IdVeiculo).OnDelete(DeleteBehavior.Cascade);

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
            // ÍNDICES ÚNICOS - VEÍCULO
            // ===========================================
            modelBuilder.Entity<Veiculo>().HasIndex(v => v.Codigo).IsUnique();
            modelBuilder.Entity<Veiculo>().HasIndex(v => v.Placa).IsUnique();
            modelBuilder.Entity<Veiculo>().HasIndex(v => v.Chassi).IsUnique().HasFilter("chassi IS NOT NULL");
            modelBuilder.Entity<Veiculo>().HasIndex(v => v.Renavam).IsUnique().HasFilter("renavam IS NOT NULL");

            // ===========================================
            // ÍNDICES ÚNICOS - ENTIDADES AUXILIARES
            // ===========================================
            modelBuilder.Entity<VeiculoCor>().HasIndex(c => c.Descricao).IsUnique();
            modelBuilder.Entity<VeiculoFilial>().HasIndex(f => f.Descricao).IsUnique();
            modelBuilder.Entity<VeiculoLocalizacao>().HasIndex(l => l.Descricao).IsUnique();
            modelBuilder.Entity<VeiculoMarca>().HasIndex(m => m.Descricao).IsUnique();

            // ===========================================
            // ÍNDICES PARA PERFORMANCE - CLIENTE
            // ===========================================
            modelBuilder.Entity<Cliente>().HasIndex(c => c.Nome).HasDatabaseName("ix_cliente_nome");

            // ===========================================
            // ÍNDICES PARA EMPRESA
            // ===========================================
            modelBuilder.Entity<Veiculo>().HasIndex(v => v.IdEmpresa);
            modelBuilder.Entity<Usuario>().HasIndex(u => u.IdEmpresa);
            modelBuilder.Entity<VeiculoCor>().HasIndex(c => c.IdEmpresa);
            modelBuilder.Entity<VeiculoFilial>().HasIndex(f => f.IdEmpresa);
            modelBuilder.Entity<VeiculoLocalizacao>().HasIndex(l => l.IdEmpresa);
            modelBuilder.Entity<VeiculoMarca>().HasIndex(m => m.IdEmpresa);
            modelBuilder.Entity<AuditLog>().HasIndex(a => a.IdEmpresa);

            // ===========================================
            // ÍNDICES PARA PERFORMANCE - AUDITLOG
            // ===========================================
            modelBuilder.Entity<AuditLog>().HasIndex(a => a.DataHora);
            modelBuilder.Entity<AuditLog>().HasIndex(a => a.UsuarioId);
            modelBuilder.Entity<AuditLog>().HasIndex(a => new { a.EntidadeNome, a.EntidadeId });

            #endregion CONFIGURAÇÕES DE ÍNDICES
        }
    }
}