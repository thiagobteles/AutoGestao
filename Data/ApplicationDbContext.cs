using AutoGestao.Entidades;
using AutoGestao.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Vendedor> Vendedores { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<VeiculoFoto> VeiculoFotos { get; set; }
        public DbSet<VeiculoDocumento> VeiculoDocumentos { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<Parcela> Parcelas { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }
        public DbSet<Despesa> Despesas { get; set; }
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
                entity.Property(e => e.TipoCliente).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Nome).HasMaxLength(200).IsRequired();
                entity.Property(e => e.CPF).HasMaxLength(14);
                entity.Property(e => e.CNPJ).HasMaxLength(18);
                entity.Property(e => e.RG).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Telefone).HasMaxLength(20);
                entity.Property(e => e.Celular).HasMaxLength(20);
                entity.Property(e => e.Endereco).HasMaxLength(500);
                entity.Property(e => e.Cidade).HasMaxLength(100);
                entity.Property(e => e.Estado).HasMaxLength(2);
                entity.Property(e => e.CEP).HasMaxLength(10);
                entity.Property(e => e.Numero).HasMaxLength(20);
                entity.Property(e => e.Complemento).HasMaxLength(100);
                entity.Property(e => e.Bairro).HasMaxLength(100);
                entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Observacoes).HasMaxLength(1000);
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE VENDEDOR
            // ===========================================
            modelBuilder.Entity<Vendedor>(entity =>
            {
                entity.Property(e => e.Nome).HasMaxLength(200).IsRequired();
                entity.Property(e => e.CPF).HasMaxLength(14).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Telefone).HasMaxLength(20);
                entity.Property(e => e.PercentualComissao).HasColumnType("decimal(5,2)");
                entity.Property(e => e.Meta).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE VEICULO
            // ===========================================
            modelBuilder.Entity<Veiculo>(entity =>
            {
                entity.Property(e => e.Codigo).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Modelo).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Motorizacao).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Placa).HasMaxLength(10).IsRequired();
                entity.Property(e => e.Chassi).HasMaxLength(50);
                entity.Property(e => e.Renavam).HasMaxLength(20);
                entity.Property(e => e.PrecoCompra).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PrecoVenda).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Observacoes).HasMaxLength(1000);
                entity.Property(e => e.Opcionais).HasMaxLength(1000);
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE VEICULO FOTO
            // ===========================================
            modelBuilder.Entity<VeiculoFoto>(entity =>
            {
                entity.Property(e => e.NomeArquivo).HasMaxLength(255).IsRequired();
                entity.Property(e => e.CaminhoArquivo).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Descricao).HasMaxLength(200);
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE VEICULO DOCUMENTO
            // ===========================================
            modelBuilder.Entity<VeiculoDocumento>(entity =>
            {
                entity.Property(e => e.TipoDocumento).HasMaxLength(50).IsRequired();
                entity.Property(e => e.NomeArquivo).HasMaxLength(255).IsRequired();
                entity.Property(e => e.CaminhoArquivo).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Observacoes).HasMaxLength(500).IsRequired();
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE VENDA
            // ===========================================
            modelBuilder.Entity<Venda>(entity =>
            {
                entity.Property(e => e.ValorVenda).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.ValorEntrada).HasColumnType("decimal(18,2)");
                entity.Property(e => e.FormaPagamento).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Observacoes).HasMaxLength(1000);
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE PARCELA
            // ===========================================
            modelBuilder.Entity<Parcela>(entity =>
            {
                entity.Property(e => e.Valor).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.ValorPago).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE AVALIACAO
            // ===========================================
            modelBuilder.Entity<Avaliacao>(entity =>
            {
                entity.Property(e => e.MarcaVeiculo).HasMaxLength(50).IsRequired();
                entity.Property(e => e.ModeloVeiculo).HasMaxLength(100).IsRequired();
                entity.Property(e => e.PlacaVeiculo).HasMaxLength(10);
                entity.Property(e => e.ValorOferecido).HasColumnType("decimal(18,2)");
                entity.Property(e => e.StatusAvaliacao).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Observacoes).HasMaxLength(1000);
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE DESPESA
            // ===========================================
            modelBuilder.Entity<Despesa>(entity =>
            {
                entity.Property(e => e.TipoDespesa).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Descricao).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Valor).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.Fornecedor).HasMaxLength(200);
                entity.Property(e => e.NumeroNF).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
            });

            // ===========================================
            // CONFIGURAÇÕES DA ENTIDADE TAREFA
            // ===========================================
            modelBuilder.Entity<Tarefa>(entity =>
            {
                entity.Property(e => e.Titulo).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Descricao).HasMaxLength(1000);
                entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Prioridade).HasMaxLength(20).IsRequired();
            });

            // ===========================================
            // CONFIGURAÇÕES DE RELACIONAMENTOS
            // ===========================================

            // Relacionamentos da Venda
            modelBuilder.Entity<Venda>()
                .HasOne(v => v.Cliente)
                .WithMany(c => c.Vendas)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Venda>()
                .HasOne(v => v.Veiculo)
                .WithMany(ve => ve.Vendas)
                .HasForeignKey(v => v.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Venda>()
                .HasOne(v => v.Vendedor)
                .WithMany(ve => ve.Vendas)
                .HasForeignKey(v => v.VendedorId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Relacionamentos da Parcela
            modelBuilder.Entity<Parcela>()
                .HasOne(p => p.Venda)
                .WithMany(v => v.Parcelas)
                .HasForeignKey(p => p.VendaId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Relacionamentos do Veículo
            modelBuilder.Entity<Veiculo>()
                .HasOne(v => v.Proprietario)
                .WithMany(c => c.Veiculos)
                .HasForeignKey(v => v.ProprietarioId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Relacionamentos das Fotos do Veículo
            modelBuilder.Entity<VeiculoFoto>()
                .HasOne(vf => vf.Veiculo)
                .WithMany(v => v.Fotos)
                .HasForeignKey(vf => vf.VeiculoId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Relacionamentos dos Documentos do Veículo
            modelBuilder.Entity<VeiculoDocumento>()
                .HasOne(vd => vd.Veiculo)
                .WithMany(v => v.Documentos)
                .HasForeignKey(vd => vd.VeiculoId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Relacionamentos das Despesas
            modelBuilder.Entity<Despesa>()
                .HasOne(d => d.Veiculo)
                .WithMany(v => v.Despesas)
                .HasForeignKey(d => d.VeiculoId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Relacionamentos das Avaliações
            modelBuilder.Entity<Avaliacao>()
                .HasOne(a => a.Cliente)
                .WithMany(c => c.Avaliacoes)
                .HasForeignKey(a => a.ClienteId)
                .OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<Avaliacao>()
                .HasOne(a => a.VendedorResponsavel)
                .WithMany(v => v.Avaliacoes)
                .HasForeignKey(a => a.VendedorResponsavelId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Relacionamentos das Tarefas
            modelBuilder.Entity<Tarefa>()
                .HasOne(t => t.Responsavel)
                .WithMany(v => v.Tarefas)
                .HasForeignKey(t => t.ResponsavelId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // ===========================================
            // CONFIGURAÇÕES DE ÍNDICES
            // ===========================================

            // Índices únicos
            modelBuilder.Entity<Cliente>().HasIndex(c => c.CPF).IsUnique().HasFilter("cpf IS NOT NULL");
            modelBuilder.Entity<Cliente>().HasIndex(c => c.CNPJ).IsUnique().HasFilter("cnpj IS NOT NULL");

            modelBuilder.Entity<Vendedor>().HasIndex(v => v.CPF).IsUnique();

            modelBuilder.Entity<Veiculo>().HasIndex(v => v.Placa).IsUnique();
            modelBuilder.Entity<Veiculo>().HasIndex(v => v.Chassi).IsUnique().HasFilter("chassi IS NOT NULL");
            modelBuilder.Entity<Veiculo>().HasIndex(v => v.Renavam).IsUnique().HasFilter("renavam IS NOT NULL");

            // Índices para performance
            modelBuilder.Entity<Venda>().HasIndex(v => v.DataVenda);
            modelBuilder.Entity<Venda>().HasIndex(v => v.Status);

            modelBuilder.Entity<Parcela>().HasIndex(p => p.DataVencimento);
            modelBuilder.Entity<Parcela>().HasIndex(p => p.Status);

            modelBuilder.Entity<Veiculo>().HasIndex(v => v.Situacao);
            modelBuilder.Entity<Veiculo>().HasIndex(v => v.StatusVeiculo);

            modelBuilder.Entity<Tarefa>().HasIndex(t => t.Status);
            modelBuilder.Entity<Tarefa>().HasIndex(t => t.DataVencimento);
        }
    }
}