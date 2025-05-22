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
(5, 3, 'aprovado', 'Implementação excelente. Aprovado!', '2023-05-11 10:30:00');