using AutoGestao.Entidades;
using AutoGestao.Entidades.Relatorio;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador;
using AutoGestao.Extensions;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        #region DbSets

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Vendedor> Vendedores { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<VeiculoCor> VeiculoCores { get; set; }
        public DbSet<VeiculoFilial> VeiculoFiliais { get; set; }
        public DbSet<VeiculoLocalizacao> VeiculoLocalizacoes { get; set; }
        public DbSet<VeiculoMarca> VeiculoMarcas { get; set; }
        public DbSet<VeiculoMarcaModelo> VeiculoMarcaModelos { get; set; }
        public DbSet<VeiculoFoto> VeiculoFotos { get; set; }
        public DbSet<VeiculoDocumento> VeiculoDocumentos { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<Parcela> Parcelas { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }
        public DbSet<Despesa> Despesas { get; set; }
        public DbSet<DespesaTipo> DespesaTipos { get; set; }
        public DbSet<Tarefa> Tarefas { get; set; }
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
            // CONFIGURAÇÕES DA ENTIDADE VENDEDOR
            // ===========================================
            modelBuilder.Entity<Vendedor>().ToTable("vendedores");
            modelBuilder.Entity<Vendedor>(entity =>
            {
                entity.Property(e => e.TipoPessoa).IsRequired();
                entity.Property(e => e.Nome).HasMaxLength(250).IsRequired();
                entity.Property(e => e.Cpf).HasMaxLength(14).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(150);
                entity.Property(e => e.Cnpj).HasMaxLength(18);
                entity.Property(e => e.Rg).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(150);
                entity.Property(e => e.Telefone).HasMaxLength(20);
                entity.Property(e => e.Celular).HasMaxLength(20);
                entity.Property(e => e.PercentualComissao).HasColumnType("decimal(5,2)");
                entity.Property(e => e.Meta).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Ativo).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired();
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE FORNECEDOR
            // ===========================================
            modelBuilder.Entity<Fornecedor>().ToTable("fornecedores");
            modelBuilder.Entity<Fornecedor>(entity =>
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
                entity.Property(e => e.Estado).HasMaxLength(2);
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
            // CONFIGURAÇÕES DA ENTIDADE VENDA
            // ===========================================
            modelBuilder.Entity<Venda>().ToTable("vendas");
            modelBuilder.Entity<Venda>(entity =>
            {
                entity.Property(e => e.ValorVenda).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.ValorEntrada).HasColumnType("decimal(18,2)");
                entity.Property(e => e.FormaPagamento).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Observacoes).HasMaxLength(2000);
                entity.Property(e => e.DataVenda).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired();
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE PARCELA
            // ===========================================
            modelBuilder.Entity<Parcela>().ToTable("parcelas");
            modelBuilder.Entity<Parcela>(entity =>
            {
                entity.Property(e => e.NumeroParcela).IsRequired();
                entity.Property(e => e.Valor).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.ValorPago).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DataVencimento).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Observacoes).HasMaxLength(250);
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired();
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
                entity.Property(e => e.DataAlteracao).IsRequired();
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
                entity.Property(e => e.DataAlteracao).IsRequired();
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE DESPESA TIPO
            // ===========================================
            modelBuilder.Entity<DespesaTipo>().ToTable("despesa_tipos");
            modelBuilder.Entity<DespesaTipo>(entity =>
            {
                entity.Property(e => e.Descricao).HasMaxLength(100).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired();
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
                entity.Property(e => e.TipoRetornoContato).IsRequired();
                entity.Property(e => e.Contexto).HasMaxLength(-1);
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
                entity.Property(e => e.EntidadeNome).HasMaxLength(100).IsRequired();
                entity.Property(e => e.EntidadeDisplayName).HasMaxLength(100);
                entity.Property(e => e.EntidadeId).HasMaxLength(50).IsRequired();
                entity.Property(e => e.TipoOperacao).IsRequired();
                entity.Property(e => e.TabelaNome).HasMaxLength(100);
                entity.Property(e => e.UsuarioNome).HasMaxLength(250);
                entity.Property(e => e.UsuarioEmail).HasMaxLength(150);
                entity.Property(e => e.IpCliente).HasMaxLength(45);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.UrlRequisicao).HasMaxLength(500);
                entity.Property(e => e.MetodoHttp).HasMaxLength(10);
                entity.Property(e => e.MensagemErro).HasMaxLength(2000);
                entity.Property(e => e.Sucesso).IsRequired();
                entity.Property(e => e.DataHora).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired();
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
                entity.Property(e => e.IsPadrao).HasDefaultValue(false);
                entity.Property(e => e.Ativo).HasDefaultValue(true);

                // Índices
                entity.HasIndex(e => e.TipoEntidade);
                entity.HasIndex(e => new { e.TipoEntidade, e.IsPadrao });
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
            // RELACIONAMENTOS DA VENDA
            // ===========================================
            modelBuilder.Entity<Venda>().HasOne(v => v.Empresa).WithMany().HasForeignKey(v => v.IdEmpresa).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Venda>().HasOne(v => v.Cliente).WithMany(c => c.Vendas).HasForeignKey(v => v.IdCliente).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Venda>().HasOne(v => v.Veiculo).WithMany(ve => ve.Vendas).HasForeignKey(v => v.IdVeiculo).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Venda>().HasOne(v => v.Vendedor).WithMany(ve => ve.Vendas).HasForeignKey(v => v.IdVendedor).OnDelete(DeleteBehavior.Restrict);

            // ===========================================
            // RELACIONAMENTOS DA PARCELA
            // ===========================================
            modelBuilder.Entity<Parcela>().HasOne(v => v.Empresa).WithMany().HasForeignKey(v => v.IdEmpresa).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Parcela>().HasOne(p => p.Venda).WithMany(v => v.Parcelas).HasForeignKey(p => p.IdVenda).OnDelete(DeleteBehavior.Cascade);

            // ===========================================
            // RELACIONAMENTOS DAS DESPESAS
            // ===========================================
            modelBuilder.Entity<Despesa>().HasOne(v => v.Empresa).WithMany().HasForeignKey(v => v.IdEmpresa).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Despesa>().HasOne(d => d.Veiculo).WithMany(v => v.Despesas).HasForeignKey(d => d.IdVeiculo).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Despesa>().HasOne(d => d.DespesaTipo).WithMany().HasForeignKey(d => d.IdDespesaTipo).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Despesa>().HasOne(d => d.Fornecedor).WithMany().HasForeignKey(d => d.IdFornecedor).OnDelete(DeleteBehavior.Restrict);

            // ===========================================
            // RELACIONAMENTOS DAS AVALIAÇÕES
            // ===========================================
            modelBuilder.Entity<Avaliacao>().HasOne(v => v.Empresa).WithMany().HasForeignKey(v => v.IdEmpresa).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Avaliacao>().HasOne(a => a.Cliente).WithMany(c => c.Avaliacoes).HasForeignKey(a => a.IdCliente).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Avaliacao>().HasOne(a => a.VendedorResponsavel).WithMany(v => v.Avaliacoes).HasForeignKey(a => a.IdVendedorResponsavel).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Avaliacao>().HasOne(a => a.VeiculoMarca).WithMany().HasForeignKey(a => a.IdVeiculoMarca).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Avaliacao>().HasOne(a => a.VeiculoMarcaModelo).WithMany().HasForeignKey(a => a.IdVeiculoMarcaModelo).OnDelete(DeleteBehavior.SetNull);

            // ===========================================
            // RELACIONAMENTOS DAS TAREFAS
            // ===========================================
            modelBuilder.Entity<Tarefa>().HasOne(v => v.Empresa).WithMany().HasForeignKey(v => v.IdEmpresa).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Tarefa>().HasOne(t => t.Responsavel).WithMany(v => v.Tarefas).HasForeignKey(t => t.IdResponsavel).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Tarefa>().HasOne(t => t.ResponsavelUsuario).WithMany(u => u.TarefasResponsavel).HasForeignKey(t => t.IdResponsavelUsuario).OnDelete(DeleteBehavior.SetNull);

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
            // ÍNDICES ÚNICOS - VENDEDOR
            // ===========================================
            modelBuilder.Entity<Vendedor>().HasIndex(v => v.Cpf).IsUnique();

            // ===========================================
            // ÍNDICES ÚNICOS - FORNECEDOR
            // ===========================================
            modelBuilder.Entity<Fornecedor>().HasIndex(f => f.Cpf).IsUnique().HasFilter("cpf IS NOT NULL");
            modelBuilder.Entity<Fornecedor>().HasIndex(f => f.Cnpj).IsUnique().HasFilter("cnpj IS NOT NULL");

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
            modelBuilder.Entity<DespesaTipo>().HasIndex(dt => dt.Descricao).IsUnique();

            // ===========================================
            // ÍNDICES PARA PERFORMANCE - VENDA
            // ===========================================
            modelBuilder.Entity<Venda>().HasIndex(v => v.DataVenda);
            modelBuilder.Entity<Venda>().HasIndex(v => v.Status);
            modelBuilder.Entity<Venda>().HasIndex(v => v.IdCliente);
            modelBuilder.Entity<Venda>().HasIndex(v => v.IdVendedor);

            // ===========================================
            // ÍNDICES PARA PERFORMANCE - PARCELA
            // ===========================================
            modelBuilder.Entity<Parcela>().HasIndex(p => p.DataVencimento);
            modelBuilder.Entity<Parcela>().HasIndex(p => p.Status);

            // ===========================================
            // ÍNDICES PARA PERFORMANCE - DESPESA
            // ===========================================
            modelBuilder.Entity<Despesa>().HasIndex(d => d.DataDespesa);
            modelBuilder.Entity<Despesa>().HasIndex(d => d.Status);

            // ===========================================
            // ÍNDICES PARA PERFORMANCE - TAREFA
            // ===========================================
            modelBuilder.Entity<Tarefa>().HasIndex(t => t.Status);
            modelBuilder.Entity<Tarefa>().HasIndex(t => t.DataVencimento);
            modelBuilder.Entity<Tarefa>().HasIndex(t => t.IdResponsavel);
            modelBuilder.Entity<Tarefa>().HasIndex(t => t.IdResponsavelUsuario);

            // ===========================================
            // ÍNDICES PARA PERFORMANCE - CLIENTE
            // ===========================================
            modelBuilder.Entity<Cliente>().HasIndex(c => c.Nome).HasDatabaseName("ix_cliente_nome");

            // ===========================================
            // ÍNDICES PARA PERFORMANCE - FORNECEDOR
            // ===========================================
            modelBuilder.Entity<Fornecedor>().HasIndex(f => f.Nome).HasDatabaseName("ix_fornecedor_nome");

            // ===========================================
            // ÍNDICES PARA EMPRESA
            // ===========================================
            modelBuilder.Entity<Venda>().HasIndex(v => v.IdEmpresa);
            modelBuilder.Entity<Parcela>().HasIndex(p => p.IdEmpresa);
            modelBuilder.Entity<Veiculo>().HasIndex(v => v.IdEmpresa);
            modelBuilder.Entity<Tarefa>().HasIndex(t => t.IdEmpresa);
            modelBuilder.Entity<Avaliacao>().HasIndex(a => a.IdEmpresa);
            modelBuilder.Entity<Despesa>().HasIndex(d => d.IdEmpresa);
            modelBuilder.Entity<Usuario>().HasIndex(u => u.IdEmpresa);
            modelBuilder.Entity<VeiculoCor>().HasIndex(c => c.IdEmpresa);
            modelBuilder.Entity<VeiculoFilial>().HasIndex(f => f.IdEmpresa);
            modelBuilder.Entity<VeiculoLocalizacao>().HasIndex(l => l.IdEmpresa);
            modelBuilder.Entity<VeiculoMarca>().HasIndex(m => m.IdEmpresa);
            modelBuilder.Entity<DespesaTipo>().HasIndex(dt => dt.IdEmpresa);
            modelBuilder.Entity<AuditLog>().HasIndex(a => a.IdEmpresa);

            // ===========================================
            // ÍNDICES PARA PERFORMANCE - AUDITLOG
            // ===========================================
            modelBuilder.Entity<AuditLog>().HasIndex(a => a.DataHora);
            modelBuilder.Entity<AuditLog>().HasIndex(a => a.UsuarioId);
            modelBuilder.Entity<AuditLog>().HasIndex(a => new { a.EntidadeNome, a.EntidadeId });

            #endregion CONFIGURAÇÕES DE ÍNDICES

            #region RELACIONAMENTOS DE AUDITORIA

            // ===========================================
            // RELACIONAMENTOS DE AUDITORIA INDIVIDUAIS
            // Opcional: descomentar se desejar ter relacionamentos explícitos
            // ===========================================
            // modelBuilder.Entity<Cliente>().HasOne<Usuario>().WithMany().HasForeignKey(e => e.CriadoPorUsuarioId).OnDelete(DeleteBehavior.SetNull);
            // modelBuilder.Entity<Cliente>().HasOne<Usuario>().WithMany().HasForeignKey(e => e.AlteradoPorUsuarioId).OnDelete(DeleteBehavior.SetNull);

            #endregion
        }
    }
}