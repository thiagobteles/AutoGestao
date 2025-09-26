using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Extensions;
using AutoGestao.Interfaces;
using AutoGestao.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AutoGestao.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Vendedor> Vendedores { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<VeiculoFoto> VeiculoFotos { get; set; }
        public DbSet<VeiculoDocumento> VeiculoDocumentos { get; set; }
        public DbSet<VeiculoCor> VeiculoCores { get; set; }
        public DbSet<VeiculoFilial> VeiculoFiliais { get; set; }
        public DbSet<VeiculoLocalizacao> VeiculoLocalizacoes { get; set; }
        public DbSet<VeiculoMarca> VeiculoMarcas { get; set; }
        public DbSet<VeiculoMarcaModelo> VeiculoMarcaModelos { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<Parcela> Parcelas { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }
        public DbSet<Despesa> Despesas { get; set; }
        public DbSet<DespesaTipo> DespesaTipos { get; set; }
        public DbSet<Tarefa> Tarefas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Adicionar o interceptador de auditoria
            var serviceProvider = optionsBuilder.Options.Extensions
                .OfType<CoreOptionsExtension>()
                .FirstOrDefault()?.ApplicationServiceProvider;

            if (serviceProvider != null)
            {
                var auditService = serviceProvider.GetService<IAuditService>();
                if (auditService != null)
                {
                    optionsBuilder.AddInterceptors(new AuditInterceptor(auditService));
                }
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========================================
            // IGNORAR BASEENTIDADE - NÃO MAPEAR COMO TABELA
            // ========================================
            // BaseEntidade é apenas classe base para herança, não uma entidade de banco
            modelBuilder.Ignore<BaseEntidade>();

            // Configurações para PostgreSQL - usar snake_case para nomes de tabelas
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Converter nomes de tabelas para snake_case
                entity.SetTableName(entity.GetTableName()?.ToSnakeCase());

                // Converter nomes de colunas para snake_case
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.GetColumnName().ToSnakeCase());
                }

                // Gerar nomes únicos para as chaves primárias
                foreach (var key in entity.GetKeys())
                {
                    if (key.IsPrimaryKey())
                    {
                        var tableName = entity.GetTableName()?.ToSnakeCase() ?? entity.ClrType.Name.ToSnakeCase();
                        key.SetName($"pk_{tableName}");
                    }
                }

                // Configurar índices
                foreach (var index in entity.GetIndexes())
                {
                    var tableName = entity.GetTableName()?.ToSnakeCase() ?? entity.ClrType.Name.ToSnakeCase();
                    var originalName = index.GetDatabaseName();

                    if (string.IsNullOrEmpty(originalName))
                    {
                        var columnNames = string.Join("_", index.Properties.Select(p => p.GetColumnName().ToSnakeCase()));
                        index.SetDatabaseName($"ix_{tableName}_{columnNames}");
                    }
                }

                // Configurar foreign keys
                foreach (var foreignKey in entity.GetForeignKeys())
                {
                    var tableName = entity.GetTableName()?.ToSnakeCase() ?? entity.ClrType.Name.ToSnakeCase();
                    var originalName = foreignKey.GetConstraintName();

                    if (string.IsNullOrEmpty(originalName))
                    {
                        var fkProperty = foreignKey.Properties.First().GetColumnName().ToSnakeCase();
                        foreignKey.SetConstraintName($"fk_{tableName}_{fkProperty}");
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
                // Configurações de propriedades
                entity.Property(e => e.TipoCliente).IsRequired();
                entity.Property(e => e.Nome).HasMaxLength(250).IsRequired();
                entity.Property(e => e.CPF).HasMaxLength(14);
                entity.Property(e => e.CNPJ).HasMaxLength(18);
                entity.Property(e => e.RG).HasMaxLength(20);
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
                entity.Property(e => e.DataAlteracao).IsRequired().HasDefaultValue(DateTime.UtcNow);

                entity.Ignore(e => e.CriadoPorUsuario);
                entity.Ignore(e => e.AlteradoPorUsuario);
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE VENDEDOR
            // ===========================================
            modelBuilder.Entity<Vendedor>().ToTable("vendedores");
            modelBuilder.Entity<Vendedor>(entity =>
            {
                entity.Property(e => e.Nome).HasMaxLength(250).IsRequired();
                entity.Property(e => e.CPF).HasMaxLength(14).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(150);
                entity.Property(e => e.Telefone).HasMaxLength(20);
                entity.Property(e => e.Celular).HasMaxLength(20);
                entity.Property(e => e.PercentualComissao).HasColumnType("decimal(5,2)");
                entity.Property(e => e.Meta).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Ativo).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired().HasDefaultValue(DateTime.UtcNow);

                entity.Ignore(e => e.CriadoPorUsuario);
                entity.Ignore(e => e.AlteradoPorUsuario);
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE FORNECEDOR
            // ===========================================
            modelBuilder.Entity<Fornecedor>().ToTable("fornecedores");
            modelBuilder.Entity<Fornecedor>(entity =>
            {
                entity.Property(e => e.TipoFornecedor).IsRequired();
                entity.Property(e => e.Nome).HasMaxLength(250).IsRequired();
                entity.Property(e => e.CPF).HasMaxLength(14);
                entity.Property(e => e.CNPJ).HasMaxLength(18);
                entity.Property(e => e.RG).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(150);
                entity.Property(e => e.Telefone).HasMaxLength(20);
                entity.Property(e => e.Celular).HasMaxLength(20);
                entity.Property(e => e.Endereco).HasMaxLength(500);
                entity.Property(e => e.Cidade).HasMaxLength(100);
                entity.Property(e => e.Estado).HasMaxLength(2);
                entity.Property(e => e.CEP).HasMaxLength(10);
                entity.Property(e => e.Numero).HasMaxLength(20);
                entity.Property(e => e.Complemento).HasMaxLength(150);
                entity.Property(e => e.Bairro).HasMaxLength(100);
                entity.Property(e => e.Ativo).IsRequired();
                entity.Property(e => e.Observacoes).HasMaxLength(2000);
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired().HasDefaultValue(DateTime.UtcNow);

                entity.Ignore(e => e.CriadoPorUsuario);
                entity.Ignore(e => e.AlteradoPorUsuario);
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE VEICULO
            // ===========================================
            modelBuilder.Entity<Veiculo>().ToTable("veiculos");
            modelBuilder.Entity<Veiculo>(entity =>
            {
                entity.Property(e => e.Codigo).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Combustivel).IsRequired();
                entity.Property(e => e.Cambio).IsRequired();
                entity.Property(e => e.Situacao).IsRequired();
                entity.Property(e => e.StatusVeiculo).IsRequired();
                entity.Property(e => e.TipoVeiculo).IsRequired();
                entity.Property(e => e.Especie).IsRequired();
                entity.Property(e => e.Portas).IsRequired();
                entity.Property(e => e.PericiaCautelar).IsRequired();
                entity.Property(e => e.OrigemVeiculo).IsRequired();
                entity.Property(e => e.Motorizacao).HasMaxLength(50).IsRequired(false);
                entity.Property(e => e.AnoFabricacao).IsRequired(false);
                entity.Property(e => e.AnoModelo).IsRequired(false);
                entity.Property(e => e.Placa).HasMaxLength(10).IsRequired(false);
                entity.Property(e => e.KmSaida).IsRequired(false);
                entity.Property(e => e.Chassi).HasMaxLength(50).IsRequired(false);
                entity.Property(e => e.Renavam).HasMaxLength(20).IsRequired(false);
                entity.Property(e => e.PrecoCompra).HasColumnType("decimal(18,2)").IsRequired(false);
                entity.Property(e => e.PrecoVenda).HasColumnType("decimal(18,2)").IsRequired(false);
                entity.Property(e => e.Observacoes).HasMaxLength(2000).IsRequired(false);
                entity.Property(e => e.Opcionais).HasMaxLength(2000).IsRequired(false);
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired().HasDefaultValue(DateTime.UtcNow);

                entity.Ignore(e => e.CriadoPorUsuario);
                entity.Ignore(e => e.AlteradoPorUsuario);
            });

            // ===========================================
            // CONFIGURAÇÕES DAS ENTIDADES AUXILIARES DE VEÍCULO
            // ===========================================
            modelBuilder.Entity<VeiculoCor>().ToTable("veiculo_cores");
            modelBuilder.Entity<VeiculoCor>(entity =>
            {
                entity.Property(e => e.Descricao).HasMaxLength(50).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();

                entity.Ignore(e => e.CriadoPorUsuario);
                entity.Ignore(e => e.AlteradoPorUsuario);
            });

            modelBuilder.Entity<VeiculoFilial>().ToTable("veiculo_filiais");
            modelBuilder.Entity<VeiculoFilial>(entity =>
            {
                entity.Property(e => e.Descricao).HasMaxLength(100).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();

                entity.Ignore(e => e.CriadoPorUsuario);
                entity.Ignore(e => e.AlteradoPorUsuario);
            });

            modelBuilder.Entity<VeiculoLocalizacao>().ToTable("veiculo_localizacoes");
            modelBuilder.Entity<VeiculoLocalizacao>(entity =>
            {
                entity.Property(e => e.Descricao).HasMaxLength(100).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();

                entity.Ignore(e => e.CriadoPorUsuario);
                entity.Ignore(e => e.AlteradoPorUsuario);
            });

            modelBuilder.Entity<VeiculoMarca>().ToTable("veiculo_marcas");
            modelBuilder.Entity<VeiculoMarca>(entity =>
            {
                entity.Property(e => e.Descricao).HasMaxLength(100).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();

                entity.Ignore(e => e.CriadoPorUsuario);
                entity.Ignore(e => e.AlteradoPorUsuario);
            });

            modelBuilder.Entity<VeiculoMarcaModelo>().ToTable("veiculo_marca_modelos");
            modelBuilder.Entity<VeiculoMarcaModelo>(entity =>
            {
                entity.Property(e => e.Descricao).HasMaxLength(100).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();

                entity.Ignore(e => e.CriadoPorUsuario);
                entity.Ignore(e => e.AlteradoPorUsuario);
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
                entity.Property(e => e.DataAlteracao).IsRequired().HasDefaultValue(DateTime.UtcNow);

                entity.Ignore(e => e.CriadoPorUsuario);
                entity.Ignore(e => e.AlteradoPorUsuario);
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE VEICULO DOCUMENTO
            // ===========================================
            modelBuilder.Entity<VeiculoDocumento>().ToTable("veiculo_documentos");
            modelBuilder.Entity<VeiculoDocumento>(entity =>
            {
                entity.Property(e => e.TipoDocumento).IsRequired();
                entity.Property(e => e.NomeArquivo).HasMaxLength(255).IsRequired();
                entity.Property(e => e.CaminhoArquivo).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Observacoes).HasMaxLength(500).IsRequired();
                entity.Property(e => e.DataUpload).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired().HasDefaultValue(DateTime.UtcNow);

                entity.Ignore(e => e.CriadoPorUsuario);
                entity.Ignore(e => e.AlteradoPorUsuario);
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE VENDA
            // ===========================================
            modelBuilder.Entity<Venda>().ToTable("vendas");
            modelBuilder.Entity<Venda>(entity =>
            {
                entity.Property(e => e.ValorVenda).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.ValorEntrada).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Observacoes).HasMaxLength(2000);
                entity.Property(e => e.DataVenda).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired().HasDefaultValue(DateTime.UtcNow);

                entity.Ignore(e => e.CriadoPorUsuario);
                entity.Ignore(e => e.AlteradoPorUsuario);
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE PARCELA
            // ===========================================
            modelBuilder.Entity<Parcela>().ToTable("parcelas");
            modelBuilder.Entity<Parcela>(entity =>
            {
                entity.Property(e => e.NumeroParcela).IsRequired();
                entity.Property(e => e.Valor).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.DataVencimento).IsRequired();
                entity.Property(e => e.ValorPago).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Observacoes).HasMaxLength(250);
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired().HasDefaultValue(DateTime.UtcNow);

                entity.Ignore(e => e.CriadoPorUsuario);
                entity.Ignore(e => e.AlteradoPorUsuario);
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE AVALIACAO
            // ===========================================
            modelBuilder.Entity<Avaliacao>().ToTable("avaliacoes");
            modelBuilder.Entity<Avaliacao>(entity =>
            {
                entity.Property(e => e.AnoVeiculo).IsRequired();
                entity.Property(e => e.PlacaVeiculo).HasMaxLength(10);
                entity.Property(e => e.ValorOferecido).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Observacoes).HasMaxLength(2000);
                entity.Property(e => e.DataAvaliacao).IsRequired();
                entity.Property(e => e.StatusAvaliacao).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired().HasDefaultValue(DateTime.UtcNow);

                entity.Ignore(e => e.CriadoPorUsuario);
                entity.Ignore(e => e.AlteradoPorUsuario);
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE DESPESA
            // ===========================================
            modelBuilder.Entity<Despesa>().ToTable("despesas");
            modelBuilder.Entity<Despesa>(entity =>
            {
                entity.Property(e => e.Descricao).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Valor).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.NumeroNF).HasMaxLength(50);
                entity.Property(e => e.DataDespesa).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired().HasDefaultValue(DateTime.UtcNow);

                entity.Ignore(e => e.CriadoPorUsuario);
                entity.Ignore(e => e.AlteradoPorUsuario);
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE DESPESA TIPO
            // ===========================================
            modelBuilder.Entity<DespesaTipo>().ToTable("despesa_tipos");
            modelBuilder.Entity<DespesaTipo>(entity =>
            {
                entity.Property(e => e.Descricao).HasMaxLength(100).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired().HasDefaultValue(DateTime.UtcNow);

                entity.Ignore(e => e.CriadoPorUsuario);
                entity.Ignore(e => e.AlteradoPorUsuario);
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE TAREFA
            // ===========================================
            modelBuilder.Entity<Tarefa>().ToTable("tarefas");
            modelBuilder.Entity<Tarefa>(entity =>
            {
                entity.Property(e => e.Titulo).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Descricao).HasMaxLength(2000);
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Prioridade).IsRequired();
                entity.Property(e => e.DataCriacao).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired().HasDefaultValue(DateTime.UtcNow);

                entity.Ignore(e => e.CriadoPorUsuario);
                entity.Ignore(e => e.AlteradoPorUsuario);
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
                entity.Property(e => e.CPF).HasMaxLength(14);
                entity.Property(e => e.Telefone).HasMaxLength(20);
                entity.Property(e => e.Perfil).IsRequired();
                entity.Property(e => e.Ativo).IsRequired().HasDefaultValue(true);
                entity.Property(e => e.Observacoes).HasMaxLength(2000);
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired().HasDefaultValue(DateTime.UtcNow);

                entity.Ignore(e => e.CriadoPorUsuario);
                entity.Ignore(e => e.AlteradoPorUsuario);
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE AUDITLOG
            // ===========================================
            modelBuilder.Entity<AuditLog>().ToTable("audit_logs");
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.Property(e => e.EntidadeNome).HasMaxLength(100).IsRequired();
                entity.Property(e => e.EntidadeDisplayName).HasMaxLength(100);
                entity.Property(e => e.EntidadeId).HasMaxLength(50).IsRequired();
                entity.Property(e => e.TipoOperacao).IsRequired();
                entity.Property(e => e.TabelaNome).HasMaxLength(100);
                entity.Property(e => e.UsuarioNome).HasMaxLength(250);
                entity.Property(e => e.UsuarioEmail).HasMaxLength(150);
                entity.Property(e => e.IpCliente).HasMaxLength(45);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.UrlRequisicao).HasMaxLength(1000);
                entity.Property(e => e.MetodoHttp).HasMaxLength(10);
                entity.Property(e => e.MensagemErro).HasMaxLength(2000);
                entity.Property(e => e.DataHora).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired().HasDefaultValue(DateTime.UtcNow);

                entity.Ignore(e => e.CriadoPorUsuario);
                entity.Ignore(e => e.AlteradoPorUsuario);

                // Índices para performance
                entity.HasIndex(e => e.DataHora);
                entity.HasIndex(e => e.UsuarioId);
                entity.HasIndex(e => e.EntidadeNome);
                entity.HasIndex(e => e.TipoOperacao);
                entity.HasIndex(e => new { e.EntidadeNome, e.EntidadeId });

                // Relacionamentos
                entity.HasOne(e => e.Usuario)
                      .WithMany(u => u.AuditLogs)
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            #endregion CONFIGURAÇÕES DAS ENTIDADES

            #region CONFIGURAÇÕES DE RELACIONAMENTOS

            // ===========================================
            // CONFIGURAÇÕES DE RELACIONAMENTOS
            // ===========================================
            // Relacionamentos do Veículo
            modelBuilder.Entity<Veiculo>().HasOne(v => v.Proprietario).WithMany(c => c.Veiculos).HasForeignKey(v => v.ProprietarioId).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Veiculo>().HasOne(v => v.VeiculoCor).WithMany().HasForeignKey(v => v.VeiculoCorId).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Veiculo>().HasOne(v => v.VeiculoFilial).WithMany().HasForeignKey(v => v.VeiculoFilialId).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Veiculo>().HasOne(v => v.VeiculoLocalizacao).WithMany().HasForeignKey(v => v.VeiculoLocalizacaoId).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Veiculo>().HasOne(v => v.VeiculoMarca).WithMany().HasForeignKey(v => v.VeiculoMarcaId).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Veiculo>().HasOne(v => v.VeiculoMarcaModelo).WithMany().HasForeignKey(v => v.VeiculoMarcaModeloId).OnDelete(DeleteBehavior.SetNull);

            // Relacionamento entre Marca e Modelo
            modelBuilder.Entity<VeiculoMarcaModelo>().HasOne(m => m.VeiculoMarca).WithMany().HasForeignKey(m => m.VeiculoMarcaId).OnDelete(DeleteBehavior.SetNull);

            // Relacionamentos das Fotos do Veículo
            modelBuilder.Entity<VeiculoFoto>().HasOne(vf => vf.Veiculo).WithMany(v => v.Fotos).HasForeignKey(vf => vf.VeiculoId).OnDelete(DeleteBehavior.Cascade);

            // Relacionamentos dos Documentos do Veículo
            modelBuilder.Entity<VeiculoDocumento>().HasOne(vd => vd.Veiculo).WithMany(v => v.Documentos).HasForeignKey(vd => vd.VeiculoId).OnDelete(DeleteBehavior.Cascade);

            // Relacionamentos da Venda
            modelBuilder.Entity<Venda>().HasOne(v => v.Cliente).WithMany(c => c.Vendas).HasForeignKey(v => v.ClienteId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Venda>().HasOne(v => v.Veiculo).WithMany(ve => ve.Vendas).HasForeignKey(v => v.VeiculoId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Venda>().HasOne(v => v.Vendedor).WithMany(ve => ve.Vendas).HasForeignKey(v => v.VendedorId).OnDelete(DeleteBehavior.Restrict);

            // Relacionamentos da Parcela
            modelBuilder.Entity<Parcela>().HasOne(p => p.Venda).WithMany(v => v.Parcelas).HasForeignKey(p => p.VendaId).OnDelete(DeleteBehavior.Cascade);

            // Relacionamentos das Despesas
            modelBuilder.Entity<Despesa>().HasOne(d => d.Veiculo).WithMany(v => v.Despesas).HasForeignKey(d => d.VeiculoId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Despesa>().HasOne(d => d.DespesaTipo).WithMany().HasForeignKey(d => d.DespesaTipoId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Despesa>().HasOne(d => d.Fornecedor).WithMany().HasForeignKey(d => d.FornecedorId).OnDelete(DeleteBehavior.Restrict);

            // Relacionamentos das Avaliações
            modelBuilder.Entity<Avaliacao>().HasOne(a => a.Cliente).WithMany(c => c.Avaliacoes).HasForeignKey(a => a.ClienteId).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Avaliacao>().HasOne(a => a.VendedorResponsavel).WithMany(v => v.Avaliacoes).HasForeignKey(a => a.VendedorResponsavelId).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Avaliacao>().HasOne(a => a.VeiculoMarca).WithMany().HasForeignKey(a => a.VeiculoMarcaId).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Avaliacao>().HasOne(a => a.VeiculoMarcaModelo).WithMany().HasForeignKey(a => a.VeiculoMarcaModeloId).OnDelete(DeleteBehavior.SetNull);

            // Relacionamentos das Tarefas
            modelBuilder.Entity<Tarefa>().HasOne(t => t.Responsavel).WithMany(v => v.Tarefas).HasForeignKey(t => t.ResponsavelId).OnDelete(DeleteBehavior.SetNull);

            #endregion CONFIGURAÇÕES DE RELACIONAMENTOS

            #region RELACIONAMENTOS DE AUDITORIA (OPCIONAL)

            // ========================================
            // RELACIONAMENTOS DE AUDITORIA INDIVIDUAIS
            // ========================================
            modelBuilder.Entity<Cliente>().HasOne<Usuario>().WithMany().HasForeignKey(e => e.CriadoPorUsuarioId).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Cliente>().HasOne<Usuario>().WithMany().HasForeignKey(e => e.AlteradoPorUsuarioId).OnDelete(DeleteBehavior.SetNull);

            #endregion

            #region CONFIGURAÇÕES DE ÍNDICES

            // Índices únicos - Cliente
            modelBuilder.Entity<Cliente>().HasIndex(c => c.CPF).IsUnique().HasFilter("cpf IS NOT NULL");
            modelBuilder.Entity<Cliente>().HasIndex(c => c.CNPJ).IsUnique().HasFilter("cnpj IS NOT NULL");

            // Índices únicos - Vendedor
            modelBuilder.Entity<Vendedor>().HasIndex(v => v.CPF).IsUnique();

            // Índices únicos - Fornecedor
            modelBuilder.Entity<Fornecedor>().HasIndex(f => f.CPF).IsUnique().HasFilter("cpf IS NOT NULL");
            modelBuilder.Entity<Fornecedor>().HasIndex(f => f.CNPJ).IsUnique().HasFilter("cnpj IS NOT NULL");

            // Índices únicos - Veículo
            modelBuilder.Entity<Veiculo>().HasIndex(v => v.Codigo).IsUnique();
            modelBuilder.Entity<Veiculo>().HasIndex(v => v.Placa).IsUnique();
            modelBuilder.Entity<Veiculo>().HasIndex(v => v.Chassi).IsUnique().HasFilter("chassi IS NOT NULL");
            modelBuilder.Entity<Veiculo>().HasIndex(v => v.Renavam).IsUnique().HasFilter("renavam IS NOT NULL");

            // Índices únicos - Entidades auxiliares
            modelBuilder.Entity<VeiculoCor>().HasIndex(c => c.Descricao).IsUnique();
            modelBuilder.Entity<VeiculoFilial>().HasIndex(f => f.Descricao).IsUnique();
            modelBuilder.Entity<VeiculoLocalizacao>().HasIndex(l => l.Descricao).IsUnique();
            modelBuilder.Entity<VeiculoMarca>().HasIndex(m => m.Descricao).IsUnique();
            modelBuilder.Entity<DespesaTipo>().HasIndex(dt => dt.Descricao).IsUnique();

            // Índices para performance - Venda
            modelBuilder.Entity<Venda>().HasIndex(v => v.DataVenda);
            modelBuilder.Entity<Venda>().HasIndex(v => v.Status);
            modelBuilder.Entity<Venda>().HasIndex(v => v.ClienteId);
            modelBuilder.Entity<Venda>().HasIndex(v => v.VendedorId);

            // Índices para performance - Parcela
            modelBuilder.Entity<Parcela>().HasIndex(p => p.DataVencimento);
            modelBuilder.Entity<Parcela>().HasIndex(p => p.Status);
            modelBuilder.Entity<Parcela>().HasIndex(p => p.VendaId);

            // Índices para performance - Veículo
            modelBuilder.Entity<Veiculo>().HasIndex(v => v.Situacao);
            modelBuilder.Entity<Veiculo>().HasIndex(v => v.StatusVeiculo);
            modelBuilder.Entity<Veiculo>().HasIndex(v => v.VeiculoMarcaId);
            modelBuilder.Entity<Veiculo>().HasIndex(v => v.VeiculoMarcaModeloId);
            modelBuilder.Entity<Veiculo>().HasIndex(v => v.ProprietarioId);

            // Índices para performance - Tarefa
            modelBuilder.Entity<Tarefa>().HasIndex(t => t.Status);
            modelBuilder.Entity<Tarefa>().HasIndex(t => t.DataVencimento);
            modelBuilder.Entity<Tarefa>().HasIndex(t => t.ResponsavelId);

            // Índices para performance - Avaliação
            modelBuilder.Entity<Avaliacao>().HasIndex(a => a.DataAvaliacao);
            modelBuilder.Entity<Avaliacao>().HasIndex(a => a.StatusAvaliacao);
            modelBuilder.Entity<Avaliacao>().HasIndex(a => a.ClienteId);
            modelBuilder.Entity<Avaliacao>().HasIndex(a => a.VendedorResponsavelId);

            // Índices para performance - Despesa
            modelBuilder.Entity<Despesa>().HasIndex(d => d.DataDespesa);
            modelBuilder.Entity<Despesa>().HasIndex(d => d.Status);
            modelBuilder.Entity<Despesa>().HasIndex(d => d.VeiculoId);
            modelBuilder.Entity<Despesa>().HasIndex(d => d.FornecedorId);
            // Índices - Usuário
            modelBuilder.Entity<Usuario>().HasIndex(e => e.Email).IsUnique();
            modelBuilder.Entity<Usuario>().HasIndex(e => e.CPF).IsUnique();

            // Índices compostos para consultas comuns
            modelBuilder.Entity<Veiculo>().HasIndex(v => new { v.Situacao, v.VeiculoMarcaId }).HasDatabaseName("ix_veiculo_situacao_marca");
            modelBuilder.Entity<Venda>().HasIndex(v => new { v.DataVenda, v.Status }).HasDatabaseName("ix_venda_data_status");
            modelBuilder.Entity<Parcela>().HasIndex(p => new { p.DataVencimento, p.Status }).HasDatabaseName("ix_parcela_vencimento_status");

            // Índices para campos de busca textual
            modelBuilder.Entity<Cliente>().HasIndex(c => c.Nome).HasDatabaseName("ix_cliente_nome");
            modelBuilder.Entity<Cliente>().HasIndex(c => c.Email).HasDatabaseName("ix_cliente_email");
            modelBuilder.Entity<Vendedor>().HasIndex(v => v.Nome).HasDatabaseName("ix_vendedor_nome");
            modelBuilder.Entity<Fornecedor>().HasIndex(f => f.Nome).HasDatabaseName("ix_fornecedor_nome");

            #endregion CONFIGURAÇÕES DE ÍNDICES

            // Propriedade não mapeada
            modelBuilder.Entity<Usuario>().Ignore(e => e.ConfirmarSenha);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Atualizar campos de auditoria automáticamente
            var entries = ChangeTracker.Entries<IAuditable>();
            var httpContextAccessor = this.GetService<IHttpContextAccessor>();
            var usuarioId = GetCurrentUserId(httpContextAccessor);

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.DataCadastro = DateTime.UtcNow;
                        entry.Entity.DataAlteracao = DateTime.UtcNow;
                        entry.Entity.CriadoPorUsuarioId = usuarioId;
                        entry.Entity.AlteradoPorUsuarioId = usuarioId;
                        break;

                    case EntityState.Modified:
                        entry.Entity.DataAlteracao = DateTime.UtcNow;
                        entry.Entity.AlteradoPorUsuarioId = usuarioId;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        private static int? GetCurrentUserId(IHttpContextAccessor? httpContextAccessor)
        {
            var httpContext = httpContextAccessor?.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out int userId))
                {
                    return userId;
                }
            }
            return null;
        }
    }
}