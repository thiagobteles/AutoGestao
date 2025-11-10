using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FGT.Migrations
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
                name: "aliquotas_impostos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tipo_imposto = table.Column<int>(type: "integer", nullable: false),
                    aliquota_percentual = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    regime_tributario = table.Column<int>(type: "integer", nullable: true),
                    estado = table.Column<int>(type: "integer", nullable: true),
                    cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    data_vigencia_inicial = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_vigencia_final = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    observacoes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("pk_aliquotas_impostos", x => x.id);
                    table.ForeignKey(
                        name: "FK_aliquotas_impostos_empresas_empresa_id",
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
                name: "cnaes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    aliquota_iss = table.Column<decimal>(type: "numeric", nullable: true),
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
                    table.PrimaryKey("pk_cnaes", x => x.id);
                    table.ForeignKey(
                        name: "FK_cnaes_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "contadores_responsaveis",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    crc = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    estado_crc = table.Column<int>(type: "integer", nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    celular = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    escritorio = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    cnpjescritorio = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    observacoes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("pk_contadores_responsaveis", x => x.id);
                    table.ForeignKey(
                        name: "FK_contadores_responsaveis_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "plano_contas",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tipo_conta = table.Column<int>(type: "integer", nullable: false),
                    natureza = table.Column<int>(type: "integer", nullable: false),
                    conta_pai_id = table.Column<long>(type: "bigint", nullable: true),
                    nivel = table.Column<int>(type: "integer", nullable: false),
                    conta_analitica = table.Column<bool>(type: "boolean", nullable: false),
                    aceita_lancamento = table.Column<bool>(type: "boolean", nullable: false),
                    exibir_na_dre = table.Column<bool>(type: "boolean", nullable: false),
                    exibir_no_balancete = table.Column<bool>(type: "boolean", nullable: false),
                    observacoes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("pk_plano_contas", x => x.id);
                    table.ForeignKey(
                        name: "FK_plano_contas_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_plano_contas_plano_contas_conta_pai_id",
                        column: x => x.conta_pai_id,
                        principalTable: "plano_contas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "report_templates",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    tipo_entidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    template_json = table.Column<string>(type: "text", nullable: false),
                    padrao = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
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
                name: "empresas_clientes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    razao_social = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    nome_fantasia = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    cnpj = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    inscricao_estadual = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    inscricao_municipal = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    regime_tributario = table.Column<int>(type: "integer", nullable: false),
                    cnaeprincipal_id = table.Column<long>(type: "bigint", nullable: true),
                    contador_responsavel_id = table.Column<long>(type: "bigint", nullable: true),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    cep = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    endereco = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    numero = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    complemento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    observacoes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("pk_empresas_clientes", x => x.id);
                    table.ForeignKey(
                        name: "FK_empresas_clientes_cnaes_cnaeprincipal_id",
                        column: x => x.cnaeprincipal_id,
                        principalTable: "cnaes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_empresas_clientes_contadores_responsaveis_contador_responsa~",
                        column: x => x.contador_responsavel_id,
                        principalTable: "contadores_responsaveis",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_empresas_clientes_empresas_empresa_id",
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
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
                    usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    entidade_nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entidade_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    tipo_operacao = table.Column<int>(type: "integer", nullable: false),
                    tabela_nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    valores_antigos = table.Column<string>(type: "text", nullable: true),
                    valores_novos = table.Column<string>(type: "text", nullable: true),
                    campos_alterados = table.Column<string>(type: "text", nullable: true),
                    ip_cliente = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    url_requisicao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    metodo_http = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    sucesso = table.Column<bool>(type: "boolean", nullable: false),
                    mensagem_erro = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    duracao_ms = table.Column<long>(type: "bigint", nullable: true),
                    data_hora = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
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
                name: "certificados_digitais",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    empresa_cliente_id = table.Column<long>(type: "bigint", nullable: false),
                    titular = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    data_validade = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    senha = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    arquivo_certificado = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("pk_certificados_digitais", x => x.id);
                    table.ForeignKey(
                        name: "FK_certificados_digitais_empresas_clientes_empresa_cliente_id",
                        column: x => x.empresa_cliente_id,
                        principalTable: "empresas_clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_certificados_digitais_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "dados_bancarios",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    empresa_cliente_id = table.Column<long>(type: "bigint", nullable: false),
                    nome_banco = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    codigo_banco = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    agencia = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    digito_agencia = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    numero_conta = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    digito_conta = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    tipo_conta = table.Column<int>(type: "integer", nullable: false),
                    chave_pix = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    conta_principal = table.Column<bool>(type: "boolean", nullable: false),
                    observacoes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("pk_dados_bancarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_dados_bancarios_empresas_clientes_empresa_cliente_id",
                        column: x => x.empresa_cliente_id,
                        principalTable: "empresas_clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dados_bancarios_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "notas_fiscais",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    numero = table.Column<int>(type: "integer", nullable: false),
                    serie = table.Column<int>(type: "integer", nullable: false),
                    chave_acesso = table.Column<string>(type: "character varying(44)", maxLength: 44, nullable: true),
                    modelo = table.Column<int>(type: "integer", nullable: false),
                    tipo = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    empresa_cliente_id = table.Column<long>(type: "bigint", nullable: false),
                    data_emissao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    data_saida_entrada = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    valor_total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    valor_produtos = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    valor_servicos = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    valor_icms = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    valor_ipi = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    valor_pis = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    valor_cofins = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    arquivo_xml = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    arquivo_pdf = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    observacoes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("pk_notas_fiscais", x => x.id);
                    table.ForeignKey(
                        name: "FK_notas_fiscais_empresas_clientes_empresa_cliente_id",
                        column: x => x.empresa_cliente_id,
                        principalTable: "empresas_clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_notas_fiscais_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "obrigacoes_fiscais",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    empresa_cliente_id = table.Column<long>(type: "bigint", nullable: false),
                    tipo_obrigacao = table.Column<int>(type: "integer", nullable: false),
                    periodicidade = table.Column<int>(type: "integer", nullable: false),
                    competencia = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_vencimento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    data_entrega = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    numero_recibo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    arquivo_enviado = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    arquivo_recibo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    observacoes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("pk_obrigacoes_fiscais", x => x.id);
                    table.ForeignKey(
                        name: "FK_obrigacoes_fiscais_empresas_clientes_empresa_cliente_id",
                        column: x => x.empresa_cliente_id,
                        principalTable: "empresas_clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_obrigacoes_fiscais_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "parametros_fiscais",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    empresa_cliente_id = table.Column<long>(type: "bigint", nullable: false),
                    ambiente_nfe = table.Column<int>(type: "integer", nullable: false),
                    serie_nfe = table.Column<int>(type: "integer", nullable: false),
                    proximo_numero_nfe = table.Column<int>(type: "integer", nullable: false),
                    serie_nfse = table.Column<int>(type: "integer", nullable: false),
                    proximo_numero_nfse = table.Column<int>(type: "integer", nullable: false),
                    cfop_padrao_venda = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true),
                    cfop_padrao_compra = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true),
                    cfop_padrao_servico = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true),
                    cst_icms_padrao = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    cst_pis_padrao = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    cst_cofins_padrao = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    mensagem_padrao_nfe = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    mensagem_padrao_nfse = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    enviar_email_automatico = table.Column<bool>(type: "boolean", nullable: false),
                    email_padrao_copia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("pk_parametros_fiscais", x => x.id);
                    table.ForeignKey(
                        name: "FK_parametros_fiscais_empresas_clientes_empresa_cliente_id",
                        column: x => x.empresa_cliente_id,
                        principalTable: "empresas_clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_parametros_fiscais_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "lancamentos_contabeis",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    data_lancamento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    tipo_lancamento = table.Column<int>(type: "integer", nullable: false),
                    empresa_cliente_id = table.Column<long>(type: "bigint", nullable: false),
                    conta_debito_id = table.Column<long>(type: "bigint", nullable: false),
                    conta_credito_id = table.Column<long>(type: "bigint", nullable: false),
                    valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    historico = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    complemento = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    numero_documento = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    nota_fiscal_id = table.Column<long>(type: "bigint", nullable: true),
                    conciliado = table.Column<bool>(type: "boolean", nullable: false),
                    data_conciliacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PlanoContasId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("pk_lancamentos_contabeis", x => x.id);
                    table.ForeignKey(
                        name: "FK_lancamentos_contabeis_empresas_clientes_empresa_cliente_id",
                        column: x => x.empresa_cliente_id,
                        principalTable: "empresas_clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_lancamentos_contabeis_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_lancamentos_contabeis_notas_fiscais_nota_fiscal_id",
                        column: x => x.nota_fiscal_id,
                        principalTable: "notas_fiscais",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_lancamentos_contabeis_plano_contas_PlanoContasId",
                        column: x => x.PlanoContasId,
                        principalTable: "plano_contas",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_lancamentos_contabeis_plano_contas_conta_credito_id",
                        column: x => x.conta_credito_id,
                        principalTable: "plano_contas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_lancamentos_contabeis_plano_contas_conta_debito_id",
                        column: x => x.conta_debito_id,
                        principalTable: "plano_contas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notas_fiscais_itens",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nota_fiscal_id = table.Column<long>(type: "bigint", nullable: false),
                    descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ncm = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    cfop = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    quantidade = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    valor_unitario = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    valor_total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
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
                    table.PrimaryKey("pk_notas_fiscais_itens", x => x.id);
                    table.ForeignKey(
                        name: "FK_notas_fiscais_itens_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_notas_fiscais_itens_notas_fiscais_nota_fiscal_id",
                        column: x => x.nota_fiscal_id,
                        principalTable: "notas_fiscais",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_aliquotas_impostos_empresa_id",
                table: "aliquotas_impostos",
                column: "empresa_id");

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
                name: "IX_certificados_digitais_empresa_cliente_id",
                table: "certificados_digitais",
                column: "empresa_cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_certificados_digitais_empresa_id",
                table: "certificados_digitais",
                column: "empresa_id");

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
                name: "IX_cnaes_empresa_id",
                table: "cnaes",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_contadores_responsaveis_empresa_id",
                table: "contadores_responsaveis",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_dados_bancarios_empresa_cliente_id",
                table: "dados_bancarios",
                column: "empresa_cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_dados_bancarios_empresa_id",
                table: "dados_bancarios",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_empresas_empresa_id",
                table: "empresas",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_empresas_clientes_cnaeprincipal_id",
                table: "empresas_clientes",
                column: "cnaeprincipal_id");

            migrationBuilder.CreateIndex(
                name: "IX_empresas_clientes_contador_responsavel_id",
                table: "empresas_clientes",
                column: "contador_responsavel_id");

            migrationBuilder.CreateIndex(
                name: "IX_empresas_clientes_empresa_id",
                table: "empresas_clientes",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_lancamentos_contabeis_conta_credito_id",
                table: "lancamentos_contabeis",
                column: "conta_credito_id");

            migrationBuilder.CreateIndex(
                name: "IX_lancamentos_contabeis_conta_debito_id",
                table: "lancamentos_contabeis",
                column: "conta_debito_id");

            migrationBuilder.CreateIndex(
                name: "IX_lancamentos_contabeis_empresa_cliente_id",
                table: "lancamentos_contabeis",
                column: "empresa_cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_lancamentos_contabeis_empresa_id",
                table: "lancamentos_contabeis",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_lancamentos_contabeis_nota_fiscal_id",
                table: "lancamentos_contabeis",
                column: "nota_fiscal_id");

            migrationBuilder.CreateIndex(
                name: "IX_lancamentos_contabeis_PlanoContasId",
                table: "lancamentos_contabeis",
                column: "PlanoContasId");

            migrationBuilder.CreateIndex(
                name: "IX_notas_fiscais_empresa_cliente_id",
                table: "notas_fiscais",
                column: "empresa_cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_notas_fiscais_empresa_id",
                table: "notas_fiscais",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_notas_fiscais_itens_empresa_id",
                table: "notas_fiscais_itens",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_notas_fiscais_itens_nota_fiscal_id",
                table: "notas_fiscais_itens",
                column: "nota_fiscal_id");

            migrationBuilder.CreateIndex(
                name: "IX_obrigacoes_fiscais_empresa_cliente_id",
                table: "obrigacoes_fiscais",
                column: "empresa_cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_obrigacoes_fiscais_empresa_id",
                table: "obrigacoes_fiscais",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_parametros_fiscais_empresa_cliente_id",
                table: "parametros_fiscais",
                column: "empresa_cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_parametros_fiscais_empresa_id",
                table: "parametros_fiscais",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_plano_contas_conta_pai_id",
                table: "plano_contas",
                column: "conta_pai_id");

            migrationBuilder.CreateIndex(
                name: "IX_plano_contas_empresa_id",
                table: "plano_contas",
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
                name: "IX_report_templates_tipo_entidade_padrao",
                table: "report_templates",
                columns: new[] { "tipo_entidade", "padrao" });

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_id_empresa",
                table: "usuarios",
                column: "id_empresa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "aliquotas_impostos");

            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "certificados_digitais");

            migrationBuilder.DropTable(
                name: "clientes");

            migrationBuilder.DropTable(
                name: "dados_bancarios");

            migrationBuilder.DropTable(
                name: "lancamentos_contabeis");

            migrationBuilder.DropTable(
                name: "notas_fiscais_itens");

            migrationBuilder.DropTable(
                name: "obrigacoes_fiscais");

            migrationBuilder.DropTable(
                name: "parametros_fiscais");

            migrationBuilder.DropTable(
                name: "report_templates");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "plano_contas");

            migrationBuilder.DropTable(
                name: "notas_fiscais");

            migrationBuilder.DropTable(
                name: "empresas_clientes");

            migrationBuilder.DropTable(
                name: "cnaes");

            migrationBuilder.DropTable(
                name: "contadores_responsaveis");

            migrationBuilder.DropTable(
                name: "empresas");
        }
    }
}
