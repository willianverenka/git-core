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
);