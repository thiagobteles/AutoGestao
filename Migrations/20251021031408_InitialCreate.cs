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
                name: "empresas",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    razao_social = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    cnpj = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    celular = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    cep = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    endereco = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    complemento = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    observacoes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    empresa_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_empresas", x => x.id);
                    table.ForeignKey(
                        name: "FK_empresas_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cep = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    endereco = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    complemento = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    observacoes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    foto_cliente = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    documento_rg = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    empresa_id = table.Column<long>(type: "bigint", nullable: true),
                    tipo_pessoa = table.Column<int>(type: "integer", nullable: false),
                    nome = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    data_nascimento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    rg = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    cnpj = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    celular = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_clientes", x => x.id);
                    table.ForeignKey(
                        name: "FK_clientes_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "leads",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    celular = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    tipo_retorno_contato = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    contexto = table.Column<string>(type: "text", maxLength: -1, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    empresa_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_leads", x => x.id);
                    table.ForeignKey(
                        name: "FK_leads_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "report_templates",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tipo_entidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    template_json = table.Column<string>(type: "text", nullable: false),
                    is_padrao = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    empresa_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_report_templates", x => x.id);
                    table.ForeignKey(
                        name: "FK_report_templates_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    perfil = table.Column<int>(type: "integer", nullable: false),
                    ultimo_login = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    senha_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    confirmar_senha = table.Column<string>(type: "text", nullable: true),
                    observacoes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_usuarios_empresas_id_empresa",
                        column: x => x.id_empresa,
                        principalTable: "empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "veiculo_cores",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    empresa_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_veiculo_cores", x => x.id);
                    table.ForeignKey(
                        name: "FK_veiculo_cores_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "veiculo_filiais",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    empresa_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_veiculo_filiais", x => x.id);
                    table.ForeignKey(
                        name: "FK_veiculo_filiais_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "veiculo_localizacoes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    empresa_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_veiculo_localizacoes", x => x.id);
                    table.ForeignKey(
                        name: "FK_veiculo_localizacoes_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "veiculo_marcas",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    empresa_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_veiculo_marcas", x => x.id);
                    table.ForeignKey(
                        name: "FK_veiculo_marcas_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    usuario_nome = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    usuario_email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    entidade_nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entidade_display_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    entidade_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    tipo_operacao = table.Column<int>(type: "integer", nullable: false),
                    tabela_nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    valores_antigos = table.Column<string>(type: "text", nullable: true),
                    valores_novos = table.Column<string>(type: "text", nullable: true),
                    campos_alterados = table.Column<string>(type: "text", nullable: true),
                    ip_cliente = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    url_requisicao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    metodo_http = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    sucesso = table.Column<bool>(type: "boolean", nullable: false),
                    mensagem_erro = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    duracao_ms = table.Column<long>(type: "bigint", nullable: true),
                    data_hora = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_logs", x => x.id);
                    table.ForeignKey(
                        name: "FK_audit_logs_empresas_id_empresa",
                        column: x => x.id_empresa,
                        principalTable: "empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_audit_logs_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "veiculo_marca_modelos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    id_veiculo_marca = table.Column<long>(type: "bigint", nullable: true),
                    VeiculoMarcaId = table.Column<long>(type: "bigint", nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_veiculo_marca_modelos", x => x.id);
                    table.ForeignKey(
                        name: "FK_veiculo_marca_modelos_empresas_id_empresa",
                        column: x => x.id_empresa,
                        principalTable: "empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_veiculo_marca_modelos_veiculo_marcas_VeiculoMarcaId",
                        column: x => x.VeiculoMarcaId,
                        principalTable: "veiculo_marcas",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_veiculo_marca_modelos_veiculo_marcas_id_veiculo_marca",
                        column: x => x.id_veiculo_marca,
                        principalTable: "veiculo_marcas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "veiculos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    placa = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    situacao = table.Column<int>(type: "integer", nullable: false),
                    chassi = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: true),
                    renavam = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    id_veiculo_marca = table.Column<long>(type: "bigint", nullable: false),
                    id_veiculo_marca_modelo = table.Column<long>(type: "bigint", nullable: false),
                    ano_fabricacao = table.Column<int>(type: "integer", nullable: false),
                    ano_modelo = table.Column<int>(type: "integer", nullable: false),
                    id_veiculo_cor = table.Column<long>(type: "bigint", nullable: true),
                    motorizacao = table.Column<string>(type: "text", nullable: true),
                    capacidade_porta_malas = table.Column<int>(type: "integer", nullable: true),
                    combustivel = table.Column<int>(type: "integer", nullable: false),
                    cambio = table.Column<int>(type: "integer", nullable: false),
                    tipo_veiculo = table.Column<int>(type: "integer", nullable: false),
                    especie = table.Column<int>(type: "integer", nullable: false),
                    numero_portas = table.Column<int>(type: "integer", nullable: false),
                    pericia_cautelar = table.Column<int>(type: "integer", nullable: false),
                    origem_veiculo = table.Column<int>(type: "integer", nullable: false),
                    preco_compra = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    id_cliente = table.Column<long>(type: "bigint", nullable: false),
                    data_entrada = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    km_entrada = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    data_saida = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    km_saida = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    preco_venda = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    id_veiculo_filial = table.Column<long>(type: "bigint", nullable: true),
                    id_veiculo_localizacao = table.Column<long>(type: "bigint", nullable: true),
                    observacoes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    opcionais = table.Column<string>(type: "text", nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_veiculos", x => x.id);
                    table.ForeignKey(
                        name: "FK_veiculos_clientes_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_veiculos_empresas_id_empresa",
                        column: x => x.id_empresa,
                        principalTable: "empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_veiculos_veiculo_cores_id_veiculo_cor",
                        column: x => x.id_veiculo_cor,
                        principalTable: "veiculo_cores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_veiculos_veiculo_filiais_id_veiculo_filial",
                        column: x => x.id_veiculo_filial,
                        principalTable: "veiculo_filiais",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_veiculos_veiculo_localizacoes_id_veiculo_localizacao",
                        column: x => x.id_veiculo_localizacao,
                        principalTable: "veiculo_localizacoes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_veiculos_veiculo_marca_modelos_id_veiculo_marca_modelo",
                        column: x => x.id_veiculo_marca_modelo,
                        principalTable: "veiculo_marca_modelos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_veiculos_veiculo_marcas_id_veiculo_marca",
                        column: x => x.id_veiculo_marca,
                        principalTable: "veiculo_marcas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "veiculo_documentos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tipo_documento = table.Column<int>(type: "integer", nullable: false),
                    documento = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    observacoes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    data_upload = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    id_veiculo = table.Column<long>(type: "bigint", nullable: false),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_veiculo_documentos", x => x.id);
                    table.ForeignKey(
                        name: "FK_veiculo_documentos_empresas_id_empresa",
                        column: x => x.id_empresa,
                        principalTable: "empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_veiculo_documentos_veiculos_id_veiculo",
                        column: x => x.id_veiculo,
                        principalTable: "veiculos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "veiculo_fotos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome_arquivo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    caminho_arquivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    foto = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    data_upload = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    principal = table.Column<bool>(type: "boolean", nullable: false),
                    id_veiculo = table.Column<long>(type: "bigint", nullable: false),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_veiculo_fotos", x => x.id);
                    table.ForeignKey(
                        name: "FK_veiculo_fotos_empresas_id_empresa",
                        column: x => x.id_empresa,
                        principalTable: "empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_veiculo_fotos_veiculos_id_veiculo",
                        column: x => x.id_veiculo,
                        principalTable: "veiculos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_data_hora",
                table: "audit_logs",
                column: "data_hora");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_entidade_nome_entidade_id",
                table: "audit_logs",
                columns: new[] { "entidade_nome", "entidade_id" });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_id_empresa",
                table: "audit_logs",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_usuario_id",
                table: "audit_logs",
                column: "usuario_id");

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
                name: "IX_clientes_empresa_id",
                table: "clientes",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_empresas_empresa_id",
                table: "empresas",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_leads_empresa_id",
                table: "leads",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_report_templates_empresa_id",
                table: "report_templates",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_report_templates_tipo_entidade",
                table: "report_templates",
                column: "tipo_entidade");

            migrationBuilder.CreateIndex(
                name: "IX_report_templates_tipo_entidade_is_padrao",
                table: "report_templates",
                columns: new[] { "tipo_entidade", "is_padrao" });

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_id_empresa",
                table: "usuarios",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_cores_descricao",
                table: "veiculo_cores",
                column: "descricao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_cores_empresa_id",
                table: "veiculo_cores",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_cores_id_empresa",
                table: "veiculo_cores",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_documentos_id_empresa",
                table: "veiculo_documentos",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_documentos_id_veiculo",
                table: "veiculo_documentos",
                column: "id_veiculo");

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_filiais_descricao",
                table: "veiculo_filiais",
                column: "descricao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_filiais_empresa_id",
                table: "veiculo_filiais",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_filiais_id_empresa",
                table: "veiculo_filiais",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_fotos_id_empresa",
                table: "veiculo_fotos",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_fotos_id_veiculo",
                table: "veiculo_fotos",
                column: "id_veiculo");

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_localizacoes_descricao",
                table: "veiculo_localizacoes",
                column: "descricao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_localizacoes_empresa_id",
                table: "veiculo_localizacoes",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_localizacoes_id_empresa",
                table: "veiculo_localizacoes",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_marca_modelos_id_empresa",
                table: "veiculo_marca_modelos",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_marca_modelos_id_veiculo_marca",
                table: "veiculo_marca_modelos",
                column: "id_veiculo_marca");

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_marca_modelos_VeiculoMarcaId",
                table: "veiculo_marca_modelos",
                column: "VeiculoMarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_marcas_descricao",
                table: "veiculo_marcas",
                column: "descricao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_marcas_empresa_id",
                table: "veiculo_marcas",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_marcas_id_empresa",
                table: "veiculo_marcas",
                column: "id_empresa");

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
                name: "IX_veiculos_id_cliente",
                table: "veiculos",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_veiculos_id_empresa",
                table: "veiculos",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_veiculos_id_veiculo_cor",
                table: "veiculos",
                column: "id_veiculo_cor");

            migrationBuilder.CreateIndex(
                name: "IX_veiculos_id_veiculo_filial",
                table: "veiculos",
                column: "id_veiculo_filial");

            migrationBuilder.CreateIndex(
                name: "IX_veiculos_id_veiculo_localizacao",
                table: "veiculos",
                column: "id_veiculo_localizacao");

            migrationBuilder.CreateIndex(
                name: "IX_veiculos_id_veiculo_marca",
                table: "veiculos",
                column: "id_veiculo_marca");

            migrationBuilder.CreateIndex(
                name: "IX_veiculos_id_veiculo_marca_modelo",
                table: "veiculos",
                column: "id_veiculo_marca_modelo");

            migrationBuilder.CreateIndex(
                name: "IX_veiculos_placa",
                table: "veiculos",
                column: "placa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_veiculos_renavam",
                table: "veiculos",
                column: "renavam",
                unique: true,
                filter: "renavam IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "leads");

            migrationBuilder.DropTable(
                name: "report_templates");

            migrationBuilder.DropTable(
                name: "veiculo_documentos");

            migrationBuilder.DropTable(
                name: "veiculo_fotos");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "veiculos");

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

            migrationBuilder.DropTable(
                name: "empresas");
        }
    }
}
