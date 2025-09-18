using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        // DbSets principais
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

                // Converter nomes de chaves para snake_case
                foreach (var key in entity.GetKeys())
                {
                    key.SetName(key.GetName()?.ToSnakeCase());
                }

                // Converter nomes de índices para snake_case
                foreach (var index in entity.GetIndexes())
                {
                    index.SetDatabaseName(index.GetDatabaseName()?.ToSnakeCase());
                }

                // Converter nomes de foreign keys para snake_case
                foreach (var foreignKey in entity.GetForeignKeys())
                {
                    foreignKey.SetConstraintName(foreignKey.GetConstraintName()?.ToSnakeCase());
                }
            }

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE CLIENTE
            // ===========================================
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
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE VENDEDOR
            // ===========================================
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

            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE FORNECEDOR
            // ===========================================
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
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE VEICULO
            // ===========================================
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
                entity.Property(e => e.Motorizacao).HasMaxLength(50).IsRequired();
                entity.Property(e => e.AnoFabricacao).IsRequired();
                entity.Property(e => e.AnoModelo).IsRequired();
                entity.Property(e => e.Placa).HasMaxLength(10).IsRequired();
                entity.Property(e => e.KmSaida).IsRequired();
                entity.Property(e => e.Chassi).HasMaxLength(50);
                entity.Property(e => e.Renavam).HasMaxLength(20);
                entity.Property(e => e.PrecoCompra).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PrecoVenda).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Observacoes).HasMaxLength(2000);
                entity.Property(e => e.Opcionais).HasMaxLength(2000);
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired().HasDefaultValue(DateTime.UtcNow);
            });

            // ===========================================
            // CONFIGURAÇÕES DAS ENTIDADES AUXILIARES DE VEÍCULO
            // ===========================================
            modelBuilder.Entity<VeiculoCor>(entity =>
            {
                entity.Property(e => e.Descricao).HasMaxLength(50).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
            });

            modelBuilder.Entity<VeiculoFilial>(entity =>
            {
                entity.Property(e => e.Descricao).HasMaxLength(100).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
            });

            modelBuilder.Entity<VeiculoLocalizacao>(entity =>
            {
                entity.Property(e => e.Descricao).HasMaxLength(100).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
            });

            modelBuilder.Entity<VeiculoMarca>(entity =>
            {
                entity.Property(e => e.Descricao).HasMaxLength(100).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
            });

            modelBuilder.Entity<VeiculoMarcaModelo>(entity =>
            {
                entity.Property(e => e.Descricao).HasMaxLength(100).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE VEICULO FOTO
            // ===========================================
            modelBuilder.Entity<VeiculoFoto>(entity =>
            {
                entity.Property(e => e.NomeArquivo).HasMaxLength(255).IsRequired();
                entity.Property(e => e.CaminhoArquivo).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Descricao).HasMaxLength(200);
                entity.Property(e => e.DataUpload).IsRequired();
                entity.Property(e => e.Principal).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired().HasDefaultValue(DateTime.UtcNow);
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE VEICULO DOCUMENTO
            // ===========================================
            modelBuilder.Entity<VeiculoDocumento>(entity =>
            {
                entity.Property(e => e.TipoDocumento).IsRequired();
                entity.Property(e => e.NomeArquivo).HasMaxLength(255).IsRequired();
                entity.Property(e => e.CaminhoArquivo).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Observacoes).HasMaxLength(500).IsRequired();
                entity.Property(e => e.DataUpload).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired().HasDefaultValue(DateTime.UtcNow);
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE VENDA
            // ===========================================
            modelBuilder.Entity<Venda>(entity =>
            {
                entity.Property(e => e.ValorVenda).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.ValorEntrada).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Observacoes).HasMaxLength(2000);
                entity.Property(e => e.DataVenda).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired().HasDefaultValue(DateTime.UtcNow);
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE PARCELA
            // ===========================================
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
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE AVALIACAO
            // ===========================================
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
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE DESPESA
            // ===========================================
            modelBuilder.Entity<Despesa>(entity =>
            {
                entity.Property(e => e.Descricao).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Valor).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.NumeroNF).HasMaxLength(50);
                entity.Property(e => e.DataDespesa).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired().HasDefaultValue(DateTime.UtcNow);
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE DESPESA TIPO
            // ===========================================
            modelBuilder.Entity<DespesaTipo>(entity =>
            {
                entity.Property(e => e.Descricao).HasMaxLength(100).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired().HasDefaultValue(DateTime.UtcNow);
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE TAREFA
            // ===========================================
            modelBuilder.Entity<Tarefa>(entity =>
            {
                entity.Property(e => e.Titulo).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Descricao).HasMaxLength(2000);
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Prioridade).IsRequired();
                entity.Property(e => e.DataCriacao).IsRequired();
                entity.Property(e => e.DataCadastro).IsRequired();
                entity.Property(e => e.DataAlteracao).IsRequired().HasDefaultValue(DateTime.UtcNow);
            });

            // ===========================================
            // CONFIGURAÇÕES DE RELACIONAMENTOS
            // ===========================================
            // Relacionamentos do Veículo
            modelBuilder.Entity<Veiculo>().HasOne(v => v.Proprietario).WithMany(c => c.Veiculos).HasForeignKey(v => v.ProprietarioId).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Veiculo>().HasOne(v => v.Cor).WithMany().HasForeignKey(v => v.VeiculoCorId).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Veiculo>().HasOne(v => v.Filial).WithMany().HasForeignKey(v => v.VeiculoFilialId).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Veiculo>().HasOne(v => v.Localizacao).WithMany().HasForeignKey(v => v.VeiculoLocalizacaoId).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Veiculo>().HasOne(v => v.Marca).WithMany().HasForeignKey(v => v.VeiculoMarcaId).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Veiculo>().HasOne(v => v.Modelo).WithMany().HasForeignKey(v => v.VeiculoMarcaModeloId).OnDelete(DeleteBehavior.SetNull);
            
            // Relacionamento entre Marca e Modelo
            modelBuilder.Entity<VeiculoMarcaModelo>().HasOne(m => m.Marca).WithMany().HasForeignKey(m => m.VeiculoMarcaId).OnDelete(DeleteBehavior.SetNull);
            
            // Relacionamentos da Venda
            modelBuilder.Entity<Venda>().HasOne(v => v.Cliente).WithMany(c => c.Vendas).HasForeignKey(v => v.ClienteId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Venda>().HasOne(v => v.Veiculo).WithMany(ve => ve.Vendas).HasForeignKey(v => v.VeiculoId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Venda>().HasOne(v => v.Vendedor).WithMany(ve => ve.Vendas).HasForeignKey(v => v.VendedorId).OnDelete(DeleteBehavior.Restrict);
            
            // Relacionamentos da Parcela
            modelBuilder.Entity<Parcela>().HasOne(p => p.Venda).WithMany(v => v.Parcelas).HasForeignKey(p => p.VendaId).OnDelete(DeleteBehavior.Cascade);
            
            // Relacionamentos das Fotos do Veículo
            modelBuilder.Entity<VeiculoFoto>().HasOne(vf => vf.Veiculo).WithMany(v => v.Fotos).HasForeignKey(vf => vf.VeiculoId).OnDelete(DeleteBehavior.Cascade);
            
            // Relacionamentos dos Documentos do Veículo
            modelBuilder.Entity<VeiculoDocumento>().HasOne(vd => vd.Veiculo).WithMany(v => v.Documentos).HasForeignKey(vd => vd.VeiculoId).OnDelete(DeleteBehavior.Cascade);
           
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
            
            // ===========================================
            // CONFIGURAÇÕES DE ÍNDICES
            // ===========================================

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

            // Índices compostos para consultas comuns
            modelBuilder.Entity<Veiculo>()
                .HasIndex(v => new { v.Situacao, v.VeiculoMarcaId })
                .HasDatabaseName("ix_veiculo_situacao_marca");

            modelBuilder.Entity<Venda>()
                .HasIndex(v => new { v.DataVenda, v.Status })
                .HasDatabaseName("ix_venda_data_status");
            
            modelBuilder.Entity<Parcela>()
                .HasIndex(p => new { p.DataVencimento, p.Status })
                .HasDatabaseName("ix_parcela_vencimento_status");
            
            // Índices para campos de busca textual
            modelBuilder.Entity<Cliente>().HasIndex(c => c.Nome).HasDatabaseName("ix_cliente_nome");
            modelBuilder.Entity<Cliente>().HasIndex(c => c.Email).HasDatabaseName("ix_cliente_email");
            modelBuilder.Entity<Vendedor>().HasIndex(v => v.Nome).HasDatabaseName("ix_vendedor_nome");
            modelBuilder.Entity<Fornecedor>().HasIndex(f => f.Nome).HasDatabaseName("ix_fornecedor_nome");
        }
    }
}