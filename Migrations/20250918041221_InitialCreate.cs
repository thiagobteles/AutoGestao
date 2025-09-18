using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AutoGestao.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tipo_cliente = table.Column<int>(type: "integer", nullable: false),
                    nome = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    cnpj = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    rg = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    data_nascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    celular = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    endereco = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    cep = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    complemento = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    observacoes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(2291))
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_clientes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "despesa_tipos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 549, DateTimeKind.Utc).AddTicks(272))
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_despesa_tipos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "fornecedores",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tipo_fornecedor = table.Column<int>(type: "integer", nullable: false),
                    nome = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    cnpj = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    rg = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    data_nascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    celular = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    endereco = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    estado = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    cep = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    complemento = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    observacoes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(3787))
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fornecedores", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "veiculo_cores",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_veiculo_cores", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "veiculo_filiais",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_veiculo_filiais", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "veiculo_localizacoes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_veiculo_localizacoes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "veiculo_marcas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_veiculo_marcas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vendedores",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    celular = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    percentual_comissao = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    meta = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(2951))
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vendedores", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "veiculo_marca_modelos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    veiculo_marca_id = table.Column<int>(type: "integer", nullable: true),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_veiculo_marca_modelos", x => x.id);
                    table.ForeignKey(
                        name: "fk_veiculo_marca_modelos_veiculo_marcas_veiculo_marca_id",
                        column: x => x.veiculo_marca_id,
                        principalTable: "veiculo_marcas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "tarefas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    prioridade = table.Column<int>(type: "integer", nullable: false),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_vencimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_conclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    responsavel_id = table.Column<int>(type: "integer", nullable: true),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 549, DateTimeKind.Utc).AddTicks(735))
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tarefas", x => x.id);
                    table.ForeignKey(
                        name: "fk_tarefas_vendedores_responsavel_id",
                        column: x => x.responsavel_id,
                        principalTable: "vendedores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "avaliacoes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ano_veiculo = table.Column<int>(type: "integer", nullable: false),
                    placa_veiculo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    valor_oferecido = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    observacoes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    data_avaliacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status_avaliacao = table.Column<int>(type: "integer", nullable: false),
                    cliente_id = table.Column<int>(type: "integer", nullable: true),
                    vendedor_responsavel_id = table.Column<int>(type: "integer", nullable: true),
                    veiculo_marca_id = table.Column<int>(type: "integer", nullable: true),
                    veiculo_marca_modelo_id = table.Column<int>(type: "integer", nullable: true),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(7838))
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_avaliacoes", x => x.id);
                    table.ForeignKey(
                        name: "fk_avaliacoes_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_avaliacoes_veiculo_marca_modelos_veiculo_marca_modelo_id",
                        column: x => x.veiculo_marca_modelo_id,
                        principalTable: "veiculo_marca_modelos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_avaliacoes_veiculo_marcas_veiculo_marca_id",
                        column: x => x.veiculo_marca_id,
                        principalTable: "veiculo_marcas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_avaliacoes_vendedores_vendedor_responsavel_id",
                        column: x => x.vendedor_responsavel_id,
                        principalTable: "vendedores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "veiculos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    combustivel = table.Column<int>(type: "integer", nullable: false),
                    cambio = table.Column<int>(type: "integer", nullable: false),
                    situacao = table.Column<int>(type: "integer", nullable: false),
                    status_veiculo = table.Column<int>(type: "integer", nullable: false),
                    tipo_veiculo = table.Column<int>(type: "integer", nullable: false),
                    especie = table.Column<int>(type: "integer", nullable: false),
                    portas = table.Column<int>(type: "integer", nullable: false),
                    pericia_cautelar = table.Column<int>(type: "integer", nullable: false),
                    motorizacao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ano_fabricacao = table.Column<int>(type: "integer", nullable: false),
                    ano_modelo = table.Column<int>(type: "integer", nullable: false),
                    placa = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    km_saida = table.Column<long>(type: "bigint", nullable: false),
                    chassi = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    renavam = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    quilometragem = table.Column<int>(type: "integer", nullable: true),
                    preco_compra = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    preco_venda = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    observacoes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    opcionais = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    data_saida = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(5032)),
                    proprietario_id = table.Column<int>(type: "integer", nullable: true),
                    veiculo_cor_id = table.Column<int>(type: "integer", nullable: true),
                    veiculo_filial_id = table.Column<int>(type: "integer", nullable: true),
                    veiculo_localizacao_id = table.Column<int>(type: "integer", nullable: true),
                    veiculo_marca_id = table.Column<int>(type: "integer", nullable: true),
                    veiculo_marca_modelo_id = table.Column<int>(type: "integer", nullable: true),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_veiculos", x => x.id);
                    table.ForeignKey(
                        name: "fk_veiculos_clientes_proprietario_id",
                        column: x => x.proprietario_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_veiculos_veiculo_cores_veiculo_cor_id",
                        column: x => x.veiculo_cor_id,
                        principalTable: "veiculo_cores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_veiculos_veiculo_filiais_veiculo_filial_id",
                        column: x => x.veiculo_filial_id,
                        principalTable: "veiculo_filiais",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_veiculos_veiculo_localizacoes_veiculo_localizacao_id",
                        column: x => x.veiculo_localizacao_id,
                        principalTable: "veiculo_localizacoes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_veiculos_veiculo_marca_modelos_veiculo_marca_modelo_id",
                        column: x => x.veiculo_marca_modelo_id,
                        principalTable: "veiculo_marca_modelos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_veiculos_veiculo_marcas_veiculo_marca_id",
                        column: x => x.veiculo_marca_id,
                        principalTable: "veiculo_marcas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "despesas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    numero_nf = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    data_despesa = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    veiculo_id = table.Column<int>(type: "integer", nullable: false),
                    despesa_tipo_id = table.Column<int>(type: "integer", nullable: false),
                    fornecedor_id = table.Column<int>(type: "integer", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(9658))
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_despesas", x => x.id);
                    table.ForeignKey(
                        name: "fk_despesas_despesa_tipos_despesa_tipo_id",
                        column: x => x.despesa_tipo_id,
                        principalTable: "despesa_tipos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_despesas_fornecedores_fornecedor_id",
                        column: x => x.fornecedor_id,
                        principalTable: "fornecedores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_despesas_veiculos_veiculo_id",
                        column: x => x.veiculo_id,
                        principalTable: "veiculos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "veiculo_documentos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tipo_documento = table.Column<int>(type: "integer", nullable: false),
                    nome_arquivo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    caminho_arquivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    observacoes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    data_upload = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    veiculo_id = table.Column<int>(type: "integer", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(6499))
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_veiculo_documentos", x => x.id);
                    table.ForeignKey(
                        name: "fk_veiculo_documentos_veiculos_veiculo_id",
                        column: x => x.veiculo_id,
                        principalTable: "veiculos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "veiculo_fotos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome_arquivo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    caminho_arquivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    data_upload = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    principal = table.Column<bool>(type: "boolean", nullable: false),
                    veiculo_id = table.Column<int>(type: "integer", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(6130))
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_veiculo_fotos", x => x.id);
                    table.ForeignKey(
                        name: "fk_veiculo_fotos_veiculos_veiculo_id",
                        column: x => x.veiculo_id,
                        principalTable: "veiculos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vendas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    valor_venda = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    valor_entrada = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    numero_parcelas = table.Column<int>(type: "integer", nullable: true),
                    forma_pagamento = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    observacoes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    data_venda = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    cliente_id = table.Column<int>(type: "integer", nullable: false),
                    veiculo_id = table.Column<int>(type: "integer", nullable: false),
                    vendedor_id = table.Column<int>(type: "integer", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(6910))
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vendas", x => x.id);
                    table.ForeignKey(
                        name: "fk_vendas_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_vendas_veiculos_veiculo_id",
                        column: x => x.veiculo_id,
                        principalTable: "veiculos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_vendas_vendedores_vendedor_id",
                        column: x => x.vendedor_id,
                        principalTable: "vendedores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "parcelas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    numero_parcela = table.Column<int>(type: "integer", nullable: false),
                    valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    data_vencimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_pagamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    valor_pago = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    observacoes = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    venda_id = table.Column<int>(type: "integer", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(7370))
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_parcelas", x => x.id);
                    table.ForeignKey(
                        name: "fk_parcelas_vendas_venda_id",
                        column: x => x.venda_id,
                        principalTable: "vendas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_avaliacoes_cliente_id",
                table: "avaliacoes",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_avaliacoes_data_avaliacao",
                table: "avaliacoes",
                column: "data_avaliacao");

            migrationBuilder.CreateIndex(
                name: "IX_avaliacoes_status_avaliacao",
                table: "avaliacoes",
                column: "status_avaliacao");

            migrationBuilder.CreateIndex(
                name: "ix_avaliacoes_veiculo_marca_id",
                table: "avaliacoes",
                column: "veiculo_marca_id");

            migrationBuilder.CreateIndex(
                name: "ix_avaliacoes_veiculo_marca_modelo_id",
                table: "avaliacoes",
                column: "veiculo_marca_modelo_id");

            migrationBuilder.CreateIndex(
                name: "ix_avaliacoes_vendedor_responsavel_id",
                table: "avaliacoes",
                column: "vendedor_responsavel_id");

            migrationBuilder.CreateIndex(
                name: "ix_cliente_email",
                table: "clientes",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "ix_cliente_nome",
                table: "clientes",
                column: "nome");

            migrationBuilder.CreateIndex(
                name: "IX_clientes_cnpj",
                table: "clientes",
                column: "cnpj",
                unique: true,
                filter: "cnpj IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_clientes_cpf",
                table: "clientes",
                column: "cpf",
                unique: true,
                filter: "cpf IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_despesa_tipos_descricao",
                table: "despesa_tipos",
                column: "descricao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_despesas_data_despesa",
                table: "despesas",
                column: "data_despesa");

            migrationBuilder.CreateIndex(
                name: "ix_despesas_despesa_tipo_id",
                table: "despesas",
                column: "despesa_tipo_id");

            migrationBuilder.CreateIndex(
                name: "ix_despesas_fornecedor_id",
                table: "despesas",
                column: "fornecedor_id");

            migrationBuilder.CreateIndex(
                name: "IX_despesas_status",
                table: "despesas",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_despesas_veiculo_id",
                table: "despesas",
                column: "veiculo_id");

            migrationBuilder.CreateIndex(
                name: "ix_fornecedor_nome",
                table: "fornecedores",
                column: "nome");

            migrationBuilder.CreateIndex(
                name: "IX_fornecedores_cnpj",
                table: "fornecedores",
                column: "cnpj",
                unique: true,
                filter: "cnpj IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_fornecedores_cpf",
                table: "fornecedores",
                column: "cpf",
                unique: true,
                filter: "cpf IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_parcela_vencimento_status",
                table: "parcelas",
                columns: new[] { "data_vencimento", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_parcelas_data_vencimento",
                table: "parcelas",
                column: "data_vencimento");

            migrationBuilder.CreateIndex(
                name: "IX_parcelas_status",
                table: "parcelas",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_parcelas_venda_id",
                table: "parcelas",
                column: "venda_id");

            migrationBuilder.CreateIndex(
                name: "IX_tarefas_data_vencimento",
                table: "tarefas",
                column: "data_vencimento");

            migrationBuilder.CreateIndex(
                name: "ix_tarefas_responsavel_id",
                table: "tarefas",
                column: "responsavel_id");

            migrationBuilder.CreateIndex(
                name: "IX_tarefas_status",
                table: "tarefas",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_cores_descricao",
                table: "veiculo_cores",
                column: "descricao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_veiculo_documentos_veiculo_id",
                table: "veiculo_documentos",
                column: "veiculo_id");

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_filiais_descricao",
                table: "veiculo_filiais",
                column: "descricao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_veiculo_fotos_veiculo_id",
                table: "veiculo_fotos",
                column: "veiculo_id");

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_localizacoes_descricao",
                table: "veiculo_localizacoes",
                column: "descricao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_veiculo_marca_modelos_veiculo_marca_id",
                table: "veiculo_marca_modelos",
                column: "veiculo_marca_id");

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_marcas_descricao",
                table: "veiculo_marcas",
                column: "descricao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_veiculo_situacao_marca",
                table: "veiculos",
                columns: new[] { "situacao", "veiculo_marca_id" });

            migrationBuilder.CreateIndex(
                name: "IX_veiculos_chassi",
                table: "veiculos",
                column: "chassi",
                unique: true,
                filter: "chassi IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_veiculos_codigo",
                table: "veiculos",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_veiculos_placa",
                table: "veiculos",
                column: "placa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_veiculos_proprietario_id",
                table: "veiculos",
                column: "proprietario_id");

            migrationBuilder.CreateIndex(
                name: "IX_veiculos_renavam",
                table: "veiculos",
                column: "renavam",
                unique: true,
                filter: "renavam IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_veiculos_situacao",
                table: "veiculos",
                column: "situacao");

            migrationBuilder.CreateIndex(
                name: "IX_veiculos_status_veiculo",
                table: "veiculos",
                column: "status_veiculo");

            migrationBuilder.CreateIndex(
                name: "ix_veiculos_veiculo_cor_id",
                table: "veiculos",
                column: "veiculo_cor_id");

            migrationBuilder.CreateIndex(
                name: "ix_veiculos_veiculo_filial_id",
                table: "veiculos",
                column: "veiculo_filial_id");

            migrationBuilder.CreateIndex(
                name: "ix_veiculos_veiculo_localizacao_id",
                table: "veiculos",
                column: "veiculo_localizacao_id");

            migrationBuilder.CreateIndex(
                name: "ix_veiculos_veiculo_marca_id",
                table: "veiculos",
                column: "veiculo_marca_id");

            migrationBuilder.CreateIndex(
                name: "ix_veiculos_veiculo_marca_modelo_id",
                table: "veiculos",
                column: "veiculo_marca_modelo_id");

            migrationBuilder.CreateIndex(
                name: "ix_venda_data_status",
                table: "vendas",
                columns: new[] { "data_venda", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_vendas_cliente_id",
                table: "vendas",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_vendas_data_venda",
                table: "vendas",
                column: "data_venda");

            migrationBuilder.CreateIndex(
                name: "IX_vendas_status",
                table: "vendas",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_vendas_veiculo_id",
                table: "vendas",
                column: "veiculo_id");

            migrationBuilder.CreateIndex(
                name: "ix_vendas_vendedor_id",
                table: "vendas",
                column: "vendedor_id");

            migrationBuilder.CreateIndex(
                name: "ix_vendedor_nome",
                table: "vendedores",
                column: "nome");

            migrationBuilder.CreateIndex(
                name: "IX_vendedores_cpf",
                table: "vendedores",
                column: "cpf",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "avaliacoes");

            migrationBuilder.DropTable(
                name: "despesas");

            migrationBuilder.DropTable(
                name: "parcelas");

            migrationBuilder.DropTable(
                name: "tarefas");

            migrationBuilder.DropTable(
                name: "veiculo_documentos");

            migrationBuilder.DropTable(
                name: "veiculo_fotos");

            migrationBuilder.DropTable(
                name: "despesa_tipos");

            migrationBuilder.DropTable(
                name: "fornecedores");

            migrationBuilder.DropTable(
                name: "vendas");

            migrationBuilder.DropTable(
                name: "veiculos");

            migrationBuilder.DropTable(
                name: "vendedores");

            migrationBuilder.DropTable(
                name: "clientes");

            migrationBuilder.DropTable(
                name: "veiculo_cores");

            migrationBuilder.DropTable(
                name: "veiculo_filiais");

            migrationBuilder.DropTable(
                name: "veiculo_localizacoes");

            migrationBuilder.DropTable(
                name: "veiculo_marca_modelos");

            migrationBuilder.DropTable(
                name: "veiculo_marcas");
        }
    }
}
