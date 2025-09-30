using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AutoGestao.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
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
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_empresas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tipo_cliente = table.Column<int>(type: "integer", nullable: false),
                    nome = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    data_nascimento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    rg = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    cnpj = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    celular = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    cep = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    endereco = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    complemento = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    observacoes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
                    empresa_id = table.Column<long>(type: "bigint", nullable: true)
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
                name: "despesa_tipos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
                    empresa_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_despesa_tipos", x => x.id);
                    table.ForeignKey(
                        name: "FK_despesa_tipos_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "fornecedores",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tipo_fornecedor = table.Column<int>(type: "integer", nullable: false),
                    nome = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    cnpj = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    rg = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    data_nascimento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
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
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
                    empresa_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fornecedores", x => x.id);
                    table.ForeignKey(
                        name: "FK_fornecedores_empresas_empresa_id",
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
                    senha_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    confirmar_senha = table.Column<string>(type: "text", nullable: true),
                    cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    perfil = table.Column<int>(type: "integer", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ultimo_login = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    observacoes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false)
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
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
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
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
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
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
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
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
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
                name: "vendedores",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    celular = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    percentual_comissao = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    meta = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
                    empresa_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vendedores", x => x.id);
                    table.ForeignKey(
                        name: "FK_vendedores_empresas_empresa_id",
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
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false)
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
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false)
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
                        name: "FK_veiculo_marca_modelos_veiculo_marcas_id_veiculo_marca",
                        column: x => x.id_veiculo_marca,
                        principalTable: "veiculo_marcas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "tarefas",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    prioridade = table.Column<int>(type: "integer", nullable: false),
                    data_criacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_vencimento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    data_conclusao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    id_responsavel = table.Column<long>(type: "bigint", nullable: true),
                    id_responsavel_usuario = table.Column<long>(type: "bigint", nullable: true),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tarefas", x => x.id);
                    table.ForeignKey(
                        name: "FK_tarefas_empresas_id_empresa",
                        column: x => x.id_empresa,
                        principalTable: "empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_tarefas_usuarios_id_responsavel_usuario",
                        column: x => x.id_responsavel_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_tarefas_vendedores_id_responsavel",
                        column: x => x.id_responsavel,
                        principalTable: "vendedores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "avaliacoes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ano_veiculo = table.Column<int>(type: "integer", nullable: false),
                    placa_veiculo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    valor_oferecido = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    observacoes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    data_avaliacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    status_avaliacao = table.Column<int>(type: "integer", nullable: false),
                    id_cliente = table.Column<long>(type: "bigint", nullable: true),
                    id_vendedor_responsavel = table.Column<long>(type: "bigint", nullable: true),
                    id_veiculo_marca = table.Column<long>(type: "bigint", nullable: true),
                    id_veiculo_marca_modelo = table.Column<long>(type: "bigint", nullable: true),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_avaliacoes", x => x.id);
                    table.ForeignKey(
                        name: "FK_avaliacoes_clientes_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_avaliacoes_empresas_id_empresa",
                        column: x => x.id_empresa,
                        principalTable: "empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_avaliacoes_veiculo_marca_modelos_id_veiculo_marca_modelo",
                        column: x => x.id_veiculo_marca_modelo,
                        principalTable: "veiculo_marca_modelos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_avaliacoes_veiculo_marcas_id_veiculo_marca",
                        column: x => x.id_veiculo_marca,
                        principalTable: "veiculo_marcas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_avaliacoes_vendedores_id_vendedor_responsavel",
                        column: x => x.id_vendedor_responsavel,
                        principalTable: "vendedores",
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
                    chassi = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: true),
                    renavam = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    id_veiculo_marca = table.Column<long>(type: "bigint", nullable: false),
                    id_veiculo_marca_modelo = table.Column<long>(type: "bigint", nullable: false),
                    ano_fabricacao = table.Column<int>(type: "integer", nullable: false),
                    ano_modelo = table.Column<int>(type: "integer", nullable: false),
                    id_veiculo_cor = table.Column<long>(type: "bigint", nullable: true),
                    motorizacao = table.Column<string>(type: "text", nullable: true),
                    quilometragem = table.Column<int>(type: "integer", nullable: true),
                    combustivel = table.Column<int>(type: "integer", nullable: false),
                    cambio = table.Column<int>(type: "integer", nullable: false),
                    tipo_veiculo = table.Column<int>(type: "integer", nullable: false),
                    especie = table.Column<int>(type: "integer", nullable: false),
                    numero_portas = table.Column<int>(type: "integer", nullable: false),
                    capacidade_porta_malas = table.Column<string>(type: "text", nullable: false),
                    pericia_cautelar = table.Column<int>(type: "integer", nullable: false),
                    origem_veiculo = table.Column<int>(type: "integer", nullable: false),
                    id_cliente = table.Column<long>(type: "bigint", nullable: false),
                    data_entrada = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    preco_compra = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    preco_venda = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    km_saida = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    data_saida = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    situacao = table.Column<int>(type: "integer", nullable: false),
                    id_veiculo_filial = table.Column<long>(type: "bigint", nullable: true),
                    id_veiculo_localizacao = table.Column<long>(type: "bigint", nullable: true),
                    observacoes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    opcionais = table.Column<string>(type: "text", nullable: true),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false)
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
                name: "despesas",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    numero_nf = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    data_despesa = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    id_veiculo = table.Column<long>(type: "bigint", nullable: false),
                    id_despesa_tipo = table.Column<long>(type: "bigint", nullable: false),
                    id_fornecedor = table.Column<long>(type: "bigint", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_despesas", x => x.id);
                    table.ForeignKey(
                        name: "FK_despesas_despesa_tipos_id_despesa_tipo",
                        column: x => x.id_despesa_tipo,
                        principalTable: "despesa_tipos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_despesas_empresas_id_empresa",
                        column: x => x.id_empresa,
                        principalTable: "empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_despesas_fornecedores_id_fornecedor",
                        column: x => x.id_fornecedor,
                        principalTable: "fornecedores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_despesas_veiculos_id_veiculo",
                        column: x => x.id_veiculo,
                        principalTable: "veiculos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "veiculo_documentos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tipo_documento = table.Column<int>(type: "integer", nullable: false),
                    nome_arquivo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    caminho_arquivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    observacoes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    data_upload = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    id_veiculo = table.Column<long>(type: "bigint", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false)
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
                    descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    data_upload = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    principal = table.Column<bool>(type: "boolean", nullable: false),
                    id_veiculo = table.Column<long>(type: "bigint", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "vendas",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    valor_venda = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    valor_entrada = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    numero_parcelas = table.Column<int>(type: "integer", nullable: true),
                    forma_pagamento = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    observacoes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    data_venda = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    id_cliente = table.Column<long>(type: "bigint", nullable: false),
                    id_veiculo = table.Column<long>(type: "bigint", nullable: false),
                    id_vendedor = table.Column<long>(type: "bigint", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vendas", x => x.id);
                    table.ForeignKey(
                        name: "FK_vendas_clientes_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_vendas_empresas_id_empresa",
                        column: x => x.id_empresa,
                        principalTable: "empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_vendas_veiculos_id_veiculo",
                        column: x => x.id_veiculo,
                        principalTable: "veiculos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_vendas_vendedores_id_vendedor",
                        column: x => x.id_vendedor,
                        principalTable: "vendedores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "parcelas",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    numero_parcela = table.Column<int>(type: "integer", nullable: false),
                    valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    data_vencimento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_pagamento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    valor_pago = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    observacoes = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    id_venda = table.Column<long>(type: "bigint", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_parcelas", x => x.id);
                    table.ForeignKey(
                        name: "FK_parcelas_empresas_id_empresa",
                        column: x => x.id_empresa,
                        principalTable: "empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_parcelas_vendas_id_venda",
                        column: x => x.id_venda,
                        principalTable: "vendas",
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
                name: "IX_avaliacoes_id_cliente",
                table: "avaliacoes",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_avaliacoes_id_empresa",
                table: "avaliacoes",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_avaliacoes_id_veiculo_marca",
                table: "avaliacoes",
                column: "id_veiculo_marca");

            migrationBuilder.CreateIndex(
                name: "IX_avaliacoes_id_veiculo_marca_modelo",
                table: "avaliacoes",
                column: "id_veiculo_marca_modelo");

            migrationBuilder.CreateIndex(
                name: "IX_avaliacoes_id_vendedor_responsavel",
                table: "avaliacoes",
                column: "id_vendedor_responsavel");

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
                name: "IX_despesa_tipos_descricao",
                table: "despesa_tipos",
                column: "descricao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_despesa_tipos_empresa_id",
                table: "despesa_tipos",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_despesa_tipos_id_empresa",
                table: "despesa_tipos",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_despesas_data_despesa",
                table: "despesas",
                column: "data_despesa");

            migrationBuilder.CreateIndex(
                name: "IX_despesas_id_despesa_tipo",
                table: "despesas",
                column: "id_despesa_tipo");

            migrationBuilder.CreateIndex(
                name: "IX_despesas_id_empresa",
                table: "despesas",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_despesas_id_fornecedor",
                table: "despesas",
                column: "id_fornecedor");

            migrationBuilder.CreateIndex(
                name: "IX_despesas_id_veiculo",
                table: "despesas",
                column: "id_veiculo");

            migrationBuilder.CreateIndex(
                name: "IX_despesas_status",
                table: "despesas",
                column: "status");

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
                name: "IX_fornecedores_empresa_id",
                table: "fornecedores",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_parcelas_data_vencimento",
                table: "parcelas",
                column: "data_vencimento");

            migrationBuilder.CreateIndex(
                name: "IX_parcelas_id_empresa",
                table: "parcelas",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_parcelas_id_venda",
                table: "parcelas",
                column: "id_venda");

            migrationBuilder.CreateIndex(
                name: "IX_parcelas_status",
                table: "parcelas",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_tarefas_data_vencimento",
                table: "tarefas",
                column: "data_vencimento");

            migrationBuilder.CreateIndex(
                name: "IX_tarefas_id_empresa",
                table: "tarefas",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_tarefas_id_responsavel",
                table: "tarefas",
                column: "id_responsavel");

            migrationBuilder.CreateIndex(
                name: "IX_tarefas_id_responsavel_usuario",
                table: "tarefas",
                column: "id_responsavel_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_tarefas_status",
                table: "tarefas",
                column: "status");

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

            migrationBuilder.CreateIndex(
                name: "IX_vendas_data_venda",
                table: "vendas",
                column: "data_venda");

            migrationBuilder.CreateIndex(
                name: "IX_vendas_id_cliente",
                table: "vendas",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_vendas_id_empresa",
                table: "vendas",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_vendas_id_veiculo",
                table: "vendas",
                column: "id_veiculo");

            migrationBuilder.CreateIndex(
                name: "IX_vendas_id_vendedor",
                table: "vendas",
                column: "id_vendedor");

            migrationBuilder.CreateIndex(
                name: "IX_vendas_status",
                table: "vendas",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_vendedores_cpf",
                table: "vendedores",
                column: "cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vendedores_empresa_id",
                table: "vendedores",
                column: "empresa_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs");

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
                name: "usuarios");

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

            migrationBuilder.DropTable(
                name: "empresas");
        }
    }
}
