using Microsoft.Extensions.Configuration;
using Npgsql;

class Program
{
    private static string connectionString;

    private static NpgsqlConnection connection;


    static void Main(string[] args)
    {
        string[] inputs = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10"];
        string input = string.Empty;


        InicializarConexao();

        do
        {
        Menu();
        input = Console.ReadLine();

        bool existemTabelas = ExistemTabelasNoBanco();

        if (!existemTabelas && input != "0" && input != "9" && input != "10")
        {
            Console.WriteLine("Não existem tabelas no banco. Gere as tabelas (opção 9 ou 10) antes de usar as opções 1-8.");
            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
            Console.Clear();
            continue;
        }

        switch (input)
        {
            case "0":
                Console.WriteLine("Saindo...");
                break;
            case "1":
                CriarUsuario();
                break;
            case "2":
                CriarRepositorio();
                break;
            case "3":
                CriarBranch();
                break;
            case "4":
                CriarCommit();
                break;
            case "5":
                CriarPullRequest();
                break;
            case "6":
                CriarIssue();
                break;
            case "7":
                VincularUsuarioRepositorio();
                break;
            case "8":
                AdicionarReviewerPullRequest();
                break;
            case "9":
                ResetarEstadoBanco();
                break;
            case "10":
                LimparBancoEInserirDadosFicticios();
                break;
            default:
                Console.WriteLine("Opção inválida!");
                break;
        }

        Console.WriteLine("\nPressione qualquer tecla para continuar...");
        Console.ReadKey();
        Console.Clear();
        }
        while (input != "0");

        connection.Close();
    }

    private static bool ExistemTabelasNoBanco()
    {
        try
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = @"
            select count (*) from pg_tables where schemaname = 'public'
            ";

                long numTabelas = (long)cmd.ExecuteScalar();
                Console.WriteLine($"Tabelas existentes: {numTabelas}");

                if (numTabelas != 0) return true;

                return false;

            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao verificar tabelas: {ex.Message}");
        }
    }

    private static bool LimparBanco(bool confirmaAcao = true)
    {
        if (confirmaAcao)
        {
            string input = string.Empty;
            do
            {
                System.Console.WriteLine("Deseja limpar o banco de dados? S/N");
                input = Console.ReadLine();
                if (input?.ToUpper() == "N") return false;
            }
            while (input?.ToUpper() is not "S" or "N");
        }

        try
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = @"
                    do $$ declare
                        r record;
                    begin
                        for r in (select tablename from pg_tables where schemaname = 'public') loop
                            execute 'drop table if exists ' || quote_ident(r.tablename) || ' cascade';
                        end loop;
                    end $$;
                ";

                cmd.ExecuteScalar();
                System.Console.WriteLine("Banco resetado com sucesso!");
                return true;
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Erro ao limpar banco: {ex.Message}");
            return false;
        }




    }

    private static void ResetarEstadoBanco()
    {
        try
        {
            bool existemTabelas = ExistemTabelasNoBanco();

            if (existemTabelas)
                LimparBanco(false);

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = @"
                create table usuario (
                    id serial primary key,
                    nome varchar not null,
                    email varchar not null unique,
                    data_cadastro timestamp not null default now(),
                    biografia text,
                    foto_perfil varchar
                );

                create table repositorio (
                    id serial primary key,
                    nome varchar not null,
                    descricao text,
                    data_criacao timestamp not null default now(),
                    visibilidade varchar not null,
                    linguagem_principal varchar
                );

                create table branch (
                    id serial primary key,
                    nome varchar not null,
                    descricao text,
                    data_criacao timestamp not null default now(),
                    repositorio_id integer not null references repositorio(id)
                );

                create table commit (
                    id serial primary key,
                    mensagem text not null,
                    data_commit timestamp not null default now(),
                    hash varchar not null,
                    usuario_id integer not null references usuario(id),
                    repositorio_id integer not null references repositorio(id)
                );

                create table issue (
                    id serial primary key,
                    titulo varchar not null,
                    descricao text,
                    data_criacao timestamp not null default now(),
                    status varchar not null,
                    repositorio_id integer not null references repositorio(id),
                    criador_id integer not null references usuario(id)
                );

                create table comentario (
                    id serial primary key,
                    conteudo text not null,
                    data_criacao timestamp not null default now(),
                    usuario_id integer not null references usuario(id),
                    issue_id integer not null references issue(id)
                );

                create table pull_request (
                    id serial primary key,
                    titulo varchar not null,
                    descricao text,
                    data_criacao timestamp not null default now(),
                    status varchar not null,
                    branch_origem_id integer not null references branch(id),
                    branch_destino_id integer not null references branch(id),
                    repositorio_id integer not null references repositorio(id),
                    criador_id integer not null references usuario(id)
                );

                create table usuario_repositorio (
                    usuario_id integer not null references usuario(id),
                    repositorio_id integer not null references repositorio(id),
                    tipo_permissao varchar not null,
                    data_adicao timestamp not null default now(),
                    primary key (usuario_id, repositorio_id)
                );

                create table pull_request_revisor (
                    pull_request_id integer not null references pull_request(id),
                    usuario_id integer not null references usuario(id),
                    status_revisao varchar,
                    comentario_revisao text,
                    data_revisao timestamp,
                    primary key (pull_request_id, usuario_id)
                );";

                cmd.ExecuteNonQuery();
            }

            System.Console.WriteLine("Tabelas criadas com sucesso!");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Erro ao criar tabelas: {ex.Message}");
        }
    }

    private static void LimparBancoEInserirDadosFicticios()
    {
        ResetarEstadoBanco();

        try
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = @"
                    insert into usuario (id, nome, email, data_cadastro, biografia, foto_perfil)
                    values
                    (1, 'Ana Silva', 'ana.silva@email.com', '2023-01-15 10:30:00', 'Desenvolvedora full-stack com 5 anos de experiência', 'ana_perfil.jpg'),
                    (2, 'Carlos Oliveira', 'carlos.oliveira@email.com', '2023-02-20 14:45:00', 'Especialista em segurança e DevOps', 'carlos_perfil.jpg'),
                    (3, 'Mariana Costa', 'mariana.costa@email.com', '2023-03-10 09:15:00', 'Desenvolvedora backend com foco em APIs', 'mariana_perfil.jpg'),
                    (4, 'Rafael Santos', 'rafael.santos@email.com', '2023-04-05 16:20:00', 'UX/UI Designer e desenvolvedor frontend', 'rafael_perfil.jpg'),
                    (5, 'Juliana Lima', 'juliana.lima@email.com', '2023-05-12 11:10:00', 'Engenheira de dados e entusiasta de IA', 'juliana_perfil.jpg');

                    insert into repositorio (id, nome, descricao, data_criacao, visibilidade, linguagem_principal)
                    values
                    (1, 'api-rest-exemplo', 'API REST de exemplo com documentação completa', '2023-02-01 08:30:00', 'público', 'C#'),
                    (2, 'sistema-gestao', 'Sistema de gestão empresarial modular', '2023-03-15 13:45:00', 'privado', 'C#'),
                    (3, 'app-mobile', 'Aplicativo mobile multiplataforma', '2023-04-20 10:20:00', 'público', 'C#'),
                    (4, 'biblioteca-componentes', 'Biblioteca de componentes reutilizáveis', '2023-05-25 15:30:00', 'público', 'C#'),
                    (5, 'dashboard-analytics', 'Dashboard para análise de dados em tempo real', '2023-06-10 09:15:00', 'privado', 'C#');

                    insert into branch (id, nome, descricao, data_criacao, repositorio_id)
                    values
                    (1, 'main', 'Branch principal', '2023-02-01 08:30:00', 1),
                    (2, 'develop', 'Branch de desenvolvimento', '2023-02-01 08:35:00', 1),
                    (3, 'feature/login', 'Implementação do sistema de login', '2023-02-10 14:20:00', 1),
                    (4, 'main', 'Branch principal', '2023-03-15 13:45:00', 2),
                    (5, 'develop', 'Branch de desenvolvimento', '2023-03-15 13:50:00', 2),
                    (6, 'feature/relatorios', 'Módulo de relatórios', '2023-03-25 10:15:00', 2),
                    (7, 'main', 'Branch principal', '2023-04-20 10:20:00', 3),
                    (8, 'develop', 'Branch de desenvolvimento', '2023-04-20 10:25:00', 3),
                    (9, 'feature/notificacoes', 'Sistema de notificações push', '2023-05-05 16:40:00', 3),
                    (10, 'main', 'Branch principal', '2023-05-25 15:30:00', 4),
                    (11, 'develop', 'Branch de desenvolvimento', '2023-05-25 15:35:00', 4),
                    (12, 'main', 'Branch principal', '2023-06-10 09:15:00', 5),
                    (13, 'develop', 'Branch de desenvolvimento', '2023-06-10 09:20:00', 5);

                    insert into commit (id, mensagem, data_commit, hash, usuario_id, repositorio_id)
                    values
                    (1, 'Inicialização do projeto', '2023-02-01 09:30:00', 'a1b2c3d4e5f6g7h8i9j0', 1, 1),
                    (2, 'Implementação da estrutura base da API', '2023-02-05 14:20:00', 'b2c3d4e5f6g7h8i9j0k1', 1, 1),
                    (3, 'Adição de endpoints de autenticação', '2023-02-12 11:45:00', 'c3d4e5f6g7h8i9j0k1l2', 3, 1),
                    (4, 'Configuração inicial do projeto', '2023-03-15 14:30:00', 'd4e5f6g7h8i9j0k1l2m3', 2, 2),
                    (5, 'Implementação do módulo de usuários', '2023-03-20 10:15:00', 'e5f6g7h8i9j0k1l2m3n4', 2, 2),
                    (6, 'Criação da estrutura do app', '2023-04-20 11:30:00', 'f6g7h8i9j0k1l2m3n4o5', 4, 3),
                    (7, 'Implementação da tela de login', '2023-04-25 15:45:00', 'g7h8i9j0k1l2m3n4o5p6', 4, 3),
                    (8, 'Configuração inicial da biblioteca', '2023-05-25 16:20:00', 'h8i9j0k1l2m3n4o5p6q7', 5, 4),
                    (9, 'Adição dos primeiros componentes', '2023-05-30 09:45:00', 'i9j0k1l2m3n4o5p6q7r8', 5, 4),
                    (10, 'Estrutura inicial do dashboard', '2023-06-10 10:30:00', 'j0k1l2m3n4o5p6q7r8s9', 1, 5);

                    insert into issue (id, titulo, descricao, data_criacao, status, repositorio_id, criador_id)
                    values
                    (1, 'Erro na autenticação OAuth', 'Usuários não conseguem fazer login usando Google', '2023-02-10 13:30:00', 'aberto', 1, 3),
                    (2, 'Melhorar documentação da API', 'Adicionar exemplos de uso para todos os endpoints', '2023-02-15 09:45:00', 'fechado', 1, 1),
                    (3, 'Otimização de consultas ao banco', 'As consultas estão lentas em volumes grandes de dados', '2023-03-25 14:20:00', 'em progresso', 2, 2),
                    (4, 'Bug no módulo de relatórios', 'Relatório mensal não exibe dados corretamente', '2023-04-02 11:15:00', 'aberto', 2, 5),
                    (5, 'Crash na tela de perfil', 'App fecha ao tentar editar foto de perfil', '2023-05-05 10:30:00', 'fechado', 3, 4),
                    (6, 'Adicionar tema escuro', 'Implementar opção de tema escuro no app', '2023-05-10 16:45:00', 'aberto', 3, 3),
                    (7, 'Componente de tabela não responsivo', 'A tabela não se adapta bem em telas pequenas', '2023-06-01 09:20:00', 'em progresso', 4, 5),
                    (8, 'Melhorar performance dos gráficos', 'Gráficos com muitos dados ficam lentos', '2023-06-15 14:30:00', 'aberto', 5, 1);

                    insert into comentario (id, conteudo, data_criacao, usuario_id, issue_id)
                    values
                    (1, 'Consegui reproduzir o erro. Parece ser um problema na configuração do cliente OAuth.', '2023-02-10 15:20:00', 2, 1),
                    (2, 'Estou trabalhando na correção, deve ser resolvido até amanhã.', '2023-02-11 09:30:00', 3, 1),
                    (3, 'Documentação atualizada com exemplos para todos os endpoints.', '2023-02-16 14:45:00', 1, 2),
                    (4, 'Ótimo trabalho! Muito mais claro agora.', '2023-02-16 16:30:00', 3, 2),
                    (5, 'Identifiquei o problema. Estamos usando consultas não otimizadas para grandes volumes.', '2023-03-26 10:15:00', 2, 3),
                    (6, 'Consegui reproduzir o bug. O problema está na formatação de datas.', '2023-04-03 09:45:00', 2, 4),
                    (7, 'Estou trabalhando na correção. Deve ser resolvido hoje.', '2023-04-03 13:20:00', 5, 4),
                    (8, 'Erro corrigido na versão 1.2.3 do app.', '2023-05-06 11:30:00', 4, 5),
                    (9, 'Já comecei a implementação do tema escuro. Preciso de feedback sobre as cores.', '2023-05-12 14:45:00', 3, 6),
                    (10, 'Estou trabalhando na responsividade da tabela. Será incluído na próxima versão.', '2023-06-02 10:30:00', 5, 7);

                    insert into pull_request (id, titulo, descricao, data_criacao, status, branch_origem_id, branch_destino_id, repositorio_id, criador_id)
                    values
                    (1, 'Implementação do sistema de login', 'Adiciona endpoints de autenticação e autorização', '2023-02-15 16:30:00', 'aberto', 3, 2, 1, 3),
                    (2, 'Atualização da documentação', 'Melhoria na documentação da API com exemplos', '2023-02-17 10:45:00', 'mesclado', 2, 1, 1, 1),
                    (3, 'Módulo de relatórios', 'Implementação do módulo de relatórios gerenciais', '2023-03-30 14:20:00', 'mesclado', 6, 5, 2, 2),
                    (4, 'Correção de bugs no módulo de relatórios', 'Corrige problemas de formatação de datas', '2023-04-05 11:30:00', 'aberto', 6, 5, 2, 5),
                    (5, 'Sistema de notificações push', 'Implementação de notificações push para eventos', '2023-05-10 09:45:00', 'mesclado', 9, 8, 3, 4);

                    insert into usuario_repositorio (usuario_id, repositorio_id, tipo_permissao, data_adicao)
                    values
                    (1, 1, 'proprietário', '2023-02-01 08:30:00'),
                    (2, 1, 'colaborador', '2023-02-02 10:15:00'),
                    (3, 1, 'colaborador', '2023-02-03 14:30:00'),
                    (2, 2, 'proprietário', '2023-03-15 13:45:00'),
                    (1, 2, 'colaborador', '2023-03-16 09:20:00'),
                    (5, 2, 'colaborador', '2023-03-17 15:40:00'),
                    (4, 3, 'proprietário', '2023-04-20 10:20:00'),
                    (3, 3, 'colaborador', '2023-04-21 11:30:00'),
                    (5, 4, 'proprietário', '2023-05-25 15:30:00'),
                    (4, 4, 'colaborador', '2023-05-26 10:15:00'),
                    (1, 5, 'proprietário', '2023-06-10 09:15:00'),
                    (2, 5, 'colaborador', '2023-06-11 14:30:00');

                    insert into pull_request_revisor (pull_request_id, usuario_id, status_revisao, comentario_revisao, data_revisao)
                    values
                    (1, 2, 'aprovado', 'Código bem estruturado e testado. Aprovado!', '2023-02-16 11:30:00'),
                    (1, 1, 'solicitando alterações', 'Precisa melhorar a cobertura de testes.', '2023-02-16 14:45:00'),
                    (2, 3, 'aprovado', 'Documentação clara e completa.', '2023-02-18 09:30:00'),
                    (3, 1, 'aprovado', 'Implementação bem feita. Aprovado!', '2023-03-31 10:15:00'),
                    (3, 5, 'aprovado', 'Código limpo e bem documentado.', '2023-03-31 13:45:00'),
                    (4, 2, 'solicitando alterações', 'Faltam testes para alguns cenários.', '2023-04-06 15:20:00'),
                    (5, 3, 'aprovado', 'Implementação excelente. Aprovado!', '2023-05-11 10:30:00');";

                cmd.ExecuteNonQuery();
                Console.WriteLine($"Dados inseridos");
            }

            SincronizaSequencias();
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Erro ao inserir dados: {ex.Message}");
        }
    }


    /* como os dados ficticios sao inseridos com chave primaria manual, o nextval dessas chaves nao sao setados corretamente,
    isso ocasiona um erro de chave primaria duplicada na hora de inserir qualquer dado nessas tabelas pelo menu;
    se as tabelas estiverem resetadas, sem dados ficticios ou outras insercoes manuais, nao ha a necessidade de chamar o metodo.
    */
    private static void SincronizaSequencias()
    {
        try
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = @"
                select setval('issue_id_seq', (select max(id) from issue));
                select setval('branch_id_seq', (select max(id) from branch));
                select setval('comentario_id_seq', (select max(id) from comentario));
                select setval('commit_id_seq', (select max(id) from commit));
                select setval('pull_request_id_seq', (select max(id) from pull_request));
                select setval('repositorio_id_seq', (select max(id) from repositorio));
                select setval('usuario_id_seq', (select max(id) from usuario));
                ";

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Erro ao sincronizar sequencias: {ex.Message}");
        }
    }

    static void Menu()
    {
        Console.WriteLine("=== SISTEMA DE GERENCIAMENTO DE REPOSITÓRIOS ===");
        Console.WriteLine("0. Sair");
        Console.WriteLine("1. Criar usuário");
        Console.WriteLine("2. Criar repositório");
        Console.WriteLine("3. Criar branch");
        Console.WriteLine("4. Criar commit");
        Console.WriteLine("5. Criar pull request");
        Console.WriteLine("6. Criar issue");
        Console.WriteLine("7. Vincular usuario a repositorio");
        Console.WriteLine("8. Adicionar reviewer a pull request");
        Console.WriteLine("9. Criar tabelas/resetar banco");
        Console.WriteLine("10. Resetar banco e inserir dados ficticios");
        Console.Write("\nEscolha uma opção: ");
    }

    static void InicializarConexao()
    {
        try
        {

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).ToString())
            .AddJsonFile("appsettings.json")
            .Build();

            connectionString = configuration.GetConnectionString("ConnectionString");

            connection = new NpgsqlConnection(connectionString);
            connection.Open();
            Console.WriteLine("Conexão com o banco de dados estabelecida com sucesso!");
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao estabelecer conexao com o banco. Certifique que a connection string esta setada corretamente no appsettings.json {ex.Message}");
        }
    }


    static void CriarUsuario()
    {
        Console.WriteLine("\n=== CRIAR NOVO USUÁRIO ===");

        Console.Write("Nome: ");
        string nome = Console.ReadLine();

        Console.Write("Email: ");
        string email = Console.ReadLine();

        Console.Write("Biografia (opcional): ");
        string biografia = Console.ReadLine();

        Console.Write("URL da foto de perfil (opcional): ");
        string fotoPerfil = Console.ReadLine();

        DateTime dataCadastro = DateTime.Now;

        try
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = @"
                    insert into usuario (nome, email, data_cadastro, biografia, foto_perfil)
                    values (@nome, @email, @data_cadastro, @biografia, @foto_perfil)
                    returning id";

                cmd.Parameters.AddWithValue("nome", nome);
                cmd.Parameters.AddWithValue("email", email);
                cmd.Parameters.AddWithValue("data_cadastro", dataCadastro);
                cmd.Parameters.AddWithValue("biografia", string.IsNullOrEmpty(biografia) ? DBNull.Value : biografia);
                cmd.Parameters.AddWithValue("foto_perfil", string.IsNullOrEmpty(fotoPerfil) ? DBNull.Value : fotoPerfil);

                int id = (int)cmd.ExecuteScalar();
                Console.WriteLine($"Usuário criado com sucesso! ID: {id}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar usuário: {ex.Message}");
        }
    }


    static void CriarRepositorio()
    {
        Console.WriteLine("\n=== CRIAR NOVO REPOSITÓRIO ===");

        Console.Write("Nome: ");
        string nome = Console.ReadLine();

        Console.Write("Descrição: ");
        string descricao = Console.ReadLine();

        Console.Write("Visibilidade (public/private): ");
        string visibilidade = Console.ReadLine().ToLower();
        if (visibilidade != "public" && visibilidade != "private")
        {
            visibilidade = "public";
        }

        Console.Write("Linguagem principal: ");
        string linguagemPrincipal = Console.ReadLine();

        DateTime dataCriacao = DateTime.Now;

        try
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = @"
                    insert into repositorio (nome, descricao, data_criacao, visibilidade, linguagem_principal)
                    values (@nome, @descricao, @data_criacao, @visibilidade, @linguagem_principal)
                    returning id";

                cmd.Parameters.AddWithValue("nome", nome);
                cmd.Parameters.AddWithValue("descricao", descricao);
                cmd.Parameters.AddWithValue("data_criacao", dataCriacao);
                cmd.Parameters.AddWithValue("visibilidade", visibilidade);
                cmd.Parameters.AddWithValue("linguagem_principal", string.IsNullOrEmpty(linguagemPrincipal) ? DBNull.Value : linguagemPrincipal);

                int id = (int)cmd.ExecuteScalar();
                Console.WriteLine($"Repositório criado com sucesso! ID: {id}");


                CriarBranchPadrao(id);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar repositório: {ex.Message}");
        }
    }


    static void CriarBranchPadrao(int repositorioId)
    {
        try
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = @"
                    insert into branch (nome, descricao, data_criacao, repositorio_id)
                    values ('main', 'Branch principal', @data_criacao, @repositorio_id)
                    returning id";

                cmd.Parameters.AddWithValue("data_criacao", DateTime.Now);
                cmd.Parameters.AddWithValue("repositorio_id", repositorioId);

                int id = (int)cmd.ExecuteScalar();
                Console.WriteLine($"Branch 'main' criada automaticamente! ID: {id}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar branch padrão: {ex.Message}");
        }
    }


    static void CriarBranch()
    {
        Console.WriteLine("\n=== CRIAR NOVA BRANCH ===");


        ListarRepositorios();

        Console.Write("ID do Repositório: ");
        if (!int.TryParse(Console.ReadLine(), out int repositorioId))
        {
            Console.WriteLine("ID inválido!");
            return;
        }


        if (!RepositorioExiste(repositorioId))
        {
            Console.WriteLine("Repositório não encontrado!");
            return;
        }

        Console.Write("Nome da Branch: ");
        string nome = Console.ReadLine();

        Console.Write("Descrição: ");
        string descricao = Console.ReadLine();

        DateTime dataCriacao = DateTime.Now;

        try
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = @"
                    insert into branch (nome, descricao, data_criacao, repositorio_id)
                    values (@nome, @descricao, @data_criacao, @repositorio_id)
                    returning id";

                cmd.Parameters.AddWithValue("nome", nome);
                cmd.Parameters.AddWithValue("descricao", descricao);
                cmd.Parameters.AddWithValue("data_criacao", dataCriacao);
                cmd.Parameters.AddWithValue("repositorio_id", repositorioId);

                int id = (int)cmd.ExecuteScalar();
                Console.WriteLine($"Branch criada com sucesso! ID: {id}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar branch: {ex.Message}");
        }
    }


    static void CriarCommit()
    {
        Console.WriteLine("\n=== CRIAR NOVO COMMIT ===");


        ListarUsuarios();

        Console.Write("ID do Usuário: ");
        if (!int.TryParse(Console.ReadLine(), out int usuarioId))
        {
            Console.WriteLine("ID inválido!");
            return;
        }


        if (!UsuarioExiste(usuarioId))
        {
            Console.WriteLine("Usuário não encontrado!");
            return;
        }


        ListarRepositorios();

        Console.Write("ID do Repositório: ");
        if (!int.TryParse(Console.ReadLine(), out int repositorioId))
        {
            Console.WriteLine("ID inválido!");
            return;
        }


        if (!RepositorioExiste(repositorioId))
        {
            Console.WriteLine("Repositório não encontrado!");
            return;
        }

        Console.Write("Mensagem do Commit: ");
        string mensagem = Console.ReadLine();

        DateTime dataCommit = DateTime.Now;


        string hash = GerarHashCommit();

        try
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = @"
                    insert into commit (mensagem, data_commit, hash, usuario_id, repositorio_id)
                    values (@mensagem, @data_commit, @hash, @usuario_id, @repositorio_id)
                    returning id";

                cmd.Parameters.AddWithValue("mensagem", mensagem);
                cmd.Parameters.AddWithValue("data_commit", dataCommit);
                cmd.Parameters.AddWithValue("hash", hash);
                cmd.Parameters.AddWithValue("usuario_id", usuarioId);
                cmd.Parameters.AddWithValue("repositorio_id", repositorioId);

                int id = (int)cmd.ExecuteScalar();
                Console.WriteLine($"Commit criado com sucesso! ID: {id}");
                Console.WriteLine($"Hash: {hash}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar commit: {ex.Message}");
        }
    }


    static void CriarPullRequest()
    {
        Console.WriteLine("\n=== CRIAR NOVO PULL REQUEST ===");


        ListarUsuarios();

        Console.Write("ID do Criador: ");
        if (!int.TryParse(Console.ReadLine(), out int criadorId))
        {
            Console.WriteLine("ID inválido!");
            return;
        }


        if (!UsuarioExiste(criadorId))
        {
            Console.WriteLine("Usuário não encontrado!");
            return;
        }


        ListarRepositorios();

        Console.Write("ID do Repositório: ");
        if (!int.TryParse(Console.ReadLine(), out int repositorioId))
        {
            Console.WriteLine("ID inválido!");
            return;
        }


        if (!RepositorioExiste(repositorioId))
        {
            Console.WriteLine("Repositório não encontrado!");
            return;
        }


        ListarBranchesPorRepositorio(repositorioId);

        Console.Write("ID da Branch de Origem: ");
        if (!int.TryParse(Console.ReadLine(), out int branchOrigemId))
        {
            Console.WriteLine("ID inválido!");
            return;
        }

        Console.Write("ID da Branch de Destino: ");
        if (!int.TryParse(Console.ReadLine(), out int branchDestinoId))
        {
            Console.WriteLine("ID inválido!");
            return;
        }

        Console.Write("Título: ");
        string titulo = Console.ReadLine();

        Console.Write("Descrição: ");
        string descricao = Console.ReadLine();

        DateTime dataCriacao = DateTime.Now;
        string status = "open";

        try
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = @"
                    insert into pull_request (titulo, descricao, data_criacao, status, branch_origem_id, branch_destino_id, repositorio_id, criador_id)
                    values (@titulo, @descricao, @data_criacao, @status, @branch_origem_id, @branch_destino_id, @repositorio_id, @criador_id)
                    returning id";

                cmd.Parameters.AddWithValue("titulo", titulo);
                cmd.Parameters.AddWithValue("descricao", descricao);
                cmd.Parameters.AddWithValue("data_criacao", dataCriacao);
                cmd.Parameters.AddWithValue("status", status);
                cmd.Parameters.AddWithValue("branch_origem_id", branchOrigemId);
                cmd.Parameters.AddWithValue("branch_destino_id", branchDestinoId);
                cmd.Parameters.AddWithValue("repositorio_id", repositorioId);
                cmd.Parameters.AddWithValue("criador_id", criadorId);

                int id = (int)cmd.ExecuteScalar();
                Console.WriteLine($"Pull Request criado com sucesso! ID: {id}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar pull request: {ex.Message}");
        }
    }


    static void CriarIssue()
    {
        Console.WriteLine("\n=== CRIAR NOVA ISSUE ===");


        ListarUsuarios();

        Console.Write("ID do Criador: ");
        if (!int.TryParse(Console.ReadLine(), out int criadorId))
        {
            Console.WriteLine("ID inválido!");
            return;
        }


        if (!UsuarioExiste(criadorId))
        {
            Console.WriteLine("Usuário não encontrado!");
            return;
        }


        ListarRepositorios();

        Console.Write("ID do Repositório: ");
        if (!int.TryParse(Console.ReadLine(), out int repositorioId))
        {
            Console.WriteLine("ID inválido!");
            return;
        }


        if (!RepositorioExiste(repositorioId))
        {
            Console.WriteLine("Repositório não encontrado!");
            return;
        }

        Console.Write("Título: ");
        string titulo = Console.ReadLine();

        Console.Write("Descrição: ");
        string descricao = Console.ReadLine();

        DateTime dataCriacao = DateTime.Now;
        string status = "open";

        try
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = @"
                    insert into issue (titulo, descricao, data_criacao, status, repositorio_id, criador_id)
                    values (@titulo, @descricao, @data_criacao, @status, @repositorio_id, @criador_id)
                    returning id";

                cmd.Parameters.AddWithValue("titulo", titulo);
                cmd.Parameters.AddWithValue("descricao", descricao);
                cmd.Parameters.AddWithValue("data_criacao", dataCriacao);
                cmd.Parameters.AddWithValue("status", status);
                cmd.Parameters.AddWithValue("repositorio_id", repositorioId);
                cmd.Parameters.AddWithValue("criador_id", criadorId);

                int id = (int)cmd.ExecuteScalar();
                Console.WriteLine($"Issue criada com sucesso! ID: {id}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar issue: {ex.Message}");
        }
    }


    static void VincularUsuarioRepositorio()
    {
        Console.WriteLine("\n=== VINCULAR USUÁRIO A REPOSITÓRIO ===");


        ListarUsuarios();

        Console.Write("ID do Usuário: ");
        if (!int.TryParse(Console.ReadLine(), out int usuarioId))
        {
            Console.WriteLine("ID inválido!");
            return;
        }


        if (!UsuarioExiste(usuarioId))
        {
            Console.WriteLine("Usuário não encontrado!");
            return;
        }


        ListarRepositorios();

        Console.Write("ID do Repositório: ");
        if (!int.TryParse(Console.ReadLine(), out int repositorioId))
        {
            Console.WriteLine("ID inválido!");
            return;
        }


        if (!RepositorioExiste(repositorioId))
        {
            Console.WriteLine("Repositório não encontrado!");
            return;
        }

        Console.Write("Tipo de Permissão (read, write, admin): ");
        string tipoPermissao = Console.ReadLine().ToLower();
        if (tipoPermissao != "read" && tipoPermissao != "write" && tipoPermissao != "admin")
        {
            tipoPermissao = "read";
        }

        DateTime dataAdicao = DateTime.Now;

        try
        {

            if (UsuarioJaVinculadoAoRepositorio(usuarioId, repositorioId))
            {
                Console.WriteLine("Este usuário já está vinculado a este repositório!");
                return;
            }

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = @"
                    insert into usuario_repositorio (usuario_id, repositorio_id, tipo_permissao, data_adicao)
                    values (@usuario_id, @repositorio_id, @tipo_permissao, @data_adicao)";

                cmd.Parameters.AddWithValue("usuario_id", usuarioId);
                cmd.Parameters.AddWithValue("repositorio_id", repositorioId);
                cmd.Parameters.AddWithValue("tipo_permissao", tipoPermissao);
                cmd.Parameters.AddWithValue("data_adicao", dataAdicao);

                cmd.ExecuteNonQuery();
                Console.WriteLine("Usuário vinculado ao repositório com sucesso!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao vincular usuário ao repositório: {ex.Message}");
        }
    }


    static void AdicionarReviewerPullRequest()
    {
        Console.WriteLine("\n=== ADICIONAR REVIEWER A PULL REQUEST ===");


        ListarPullRequests();

        Console.Write("ID do Pull Request: ");
        if (!int.TryParse(Console.ReadLine(), out int pullRequestId))
        {
            Console.WriteLine("ID inválido!");
            return;
        }


        if (!PullRequestExiste(pullRequestId))
        {
            Console.WriteLine("Pull Request não encontrado!");
            return;
        }


        ListarUsuarios();

        Console.Write("ID do Usuário Revisor: ");
        if (!int.TryParse(Console.ReadLine(), out int usuarioId))
        {
            Console.WriteLine("ID inválido!");
            return;
        }


        if (!UsuarioExiste(usuarioId))
        {
            Console.WriteLine("Usuário não encontrado!");
            return;
        }

        string statusRevisao = "pending";

        Console.Write("Comentário da Revisão (opcional): ");
        string comentarioRevisao = Console.ReadLine();

        DateTime dataRevisao = DateTime.Now;

        try
        {

            if (UsuarioJaRevisorDoPullRequest(pullRequestId, usuarioId))
            {
                Console.WriteLine("Este usuário já é revisor deste Pull Request!");
                return;
            }

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = @"
                    insert into pull_request_revisor (pull_request_id, usuario_id, status_revisao, comentario_revisao, data_revisao)
                    values (@pull_request_id, @usuario_id, @status_revisao, @comentario_revisao, @data_revisao)";

                cmd.Parameters.AddWithValue("pull_request_id", pullRequestId);
                cmd.Parameters.AddWithValue("usuario_id", usuarioId);
                cmd.Parameters.AddWithValue("status_revisao", statusRevisao);
                cmd.Parameters.AddWithValue("comentario_revisao", string.IsNullOrEmpty(comentarioRevisao) ? DBNull.Value : comentarioRevisao);
                cmd.Parameters.AddWithValue("data_revisao", dataRevisao);

                cmd.ExecuteNonQuery();
                Console.WriteLine("Revisor adicionado ao Pull Request com sucesso!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao adicionar revisor ao Pull Request: {ex.Message}");
        }
    }




    static void ListarUsuarios()
    {
        Console.WriteLine("\n=== USUÁRIOS DISPONÍVEIS ===");

        try
        {
            using (var cmd = new NpgsqlCommand("select id, nome, email from usuario ORDER BY id", connection))
            using (var reader = cmd.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    Console.WriteLine("Nenhum usuário cadastrado!");
                    return;
                }

                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader.GetInt32(0)} | Nome: {reader.GetString(1)} | Email: {reader.GetString(2)}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao listar usuários: {ex.Message}");
        }
    }


    static void ListarRepositorios()
    {
        Console.WriteLine("\n=== REPOSITÓRIOS DISPONÍVEIS ===");

        try
        {
            using (var cmd = new NpgsqlCommand("select id, nome, visibilidade from repositorio ORDER BY id", connection))
            using (var reader = cmd.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    Console.WriteLine("Nenhum repositório cadastrado!");
                    return;
                }

                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader.GetInt32(0)} | Nome: {reader.GetString(1)} | Visibilidade: {reader.GetString(2)}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao listar repositórios: {ex.Message}");
        }
    }


    static void ListarBranchesPorRepositorio(int repositorioId)
    {
        Console.WriteLine($"\n=== BRANCHES DO REPOSITÓRIO (ID: {repositorioId}) ===");

        try
        {
            using (var cmd = new NpgsqlCommand("select id, nome from branch WHERE repositorio_id = @repositorio_id ORDER BY id", connection))
            {
                cmd.Parameters.AddWithValue("repositorio_id", repositorioId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("Nenhuma branch encontrada para este repositório!");
                        return;
                    }

                    while (reader.Read())
                    {
                        Console.WriteLine($"ID: {reader.GetInt32(0)} | Nome: {reader.GetString(1)}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao listar branches: {ex.Message}");
        }
    }


    static void ListarPullRequests()
    {
        Console.WriteLine("\n=== PULL REQUESTS DISPONÍVEIS ===");

        try
        {
            using (var cmd = new NpgsqlCommand(@"
                select pr.id, pr.titulo, r.nome as repositorio, u.nome as criador
                from pull_request pr
                JOIN repositorio r ON pr.repositorio_id = r.id
                JOIN usuario u ON pr.criador_id = u.id
                ORDER BY pr.id", connection))
            using (var reader = cmd.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    Console.WriteLine("Nenhum Pull Request cadastrado!");
                    return;
                }

                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader.GetInt32(0)} | Título: {reader.GetString(1)} | Repositório: {reader.GetString(2)} | Criador: {reader.GetString(3)}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao listar Pull Requests: {ex.Message}");
        }
    }


    static bool UsuarioExiste(int id)
    {
        try
        {
            using (var cmd = new NpgsqlCommand("select COUNT(*) from usuario WHERE id = @id", connection))
            {
                cmd.Parameters.AddWithValue("id", id);
                return (long)cmd.ExecuteScalar() > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao verificar usuário: {ex.Message}");
            return false;
        }
    }


    static bool RepositorioExiste(int id)
    {
        try
        {
            using (var cmd = new NpgsqlCommand("select COUNT(*) from repositorio WHERE id = @id", connection))
            {
                cmd.Parameters.AddWithValue("id", id);
                return (long)cmd.ExecuteScalar() > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao verificar repositório: {ex.Message}");
            return false;
        }
    }


    static bool PullRequestExiste(int id)
    {
        try
        {
            using (var cmd = new NpgsqlCommand("select COUNT(*) from pull_request WHERE id = @id", connection))
            {
                cmd.Parameters.AddWithValue("id", id);
                return (long)cmd.ExecuteScalar() > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao verificar Pull Request: {ex.Message}");
            return false;
        }
    }


    static bool UsuarioJaVinculadoAoRepositorio(int usuarioId, int repositorioId)
    {
        try
        {
            using (var cmd = new NpgsqlCommand("select COUNT(*) from usuario_repositorio WHERE usuario_id = @usuario_id AND repositorio_id = @repositorio_id", connection))
            {
                cmd.Parameters.AddWithValue("usuario_id", usuarioId);
                cmd.Parameters.AddWithValue("repositorio_id", repositorioId);
                return (long)cmd.ExecuteScalar() > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao verificar vínculo: {ex.Message}");
            return false;
        }
    }


    static bool UsuarioJaRevisorDoPullRequest(int pullRequestId, int usuarioId)
    {
        try
        {
            using (var cmd = new NpgsqlCommand("select COUNT(*) from pull_request_revisor WHERE pull_request_id = @pull_request_id AND usuario_id = @usuario_id", connection))
            {
                cmd.Parameters.AddWithValue("pull_request_id", pullRequestId);
                cmd.Parameters.AddWithValue("usuario_id", usuarioId);
                return (long)cmd.ExecuteScalar() > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao verificar revisor: {ex.Message}");
            return false;
        }
    }


    static string GerarHashCommit()
    {

        Random random = new Random();
        const string chars = "0123456789abcdef";
        return new string(Enumerable.Repeat(chars, 40)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}