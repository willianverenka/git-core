# GitCore

## Sobre o Projeto

Este projeto implementa um sistema de controle de versão que armazena e manipula informações sobre usuários, repositórios, branches, commits, issues, pull requests e comentários. O banco de dados utilizado foi o Supabase, baseado no postgres, e a parte da aplicação de população e validação de dados foi feita em dotnet com C#.

### Participantes

Willian Verenka RA 22.124.081-5

João Vitor Sitta Giopatto RA 22.123.054-3

## Preparando o projeto
Certifique-se de que tem o [dotnet 9 SDK instalado.](https://dotnet.microsoft.com/pt-br/download/dotnet/9.0)

Caso esteja tudo certo, `dotnet --list-sdks` deve mostrar a versão correta do SDK.

Clone o projeto pelo terminal ou IDE de preferência. Pelo terminal:

`git clone https://github.com/willianverenka/git-core.git`

## Conectando com o seu banco

Navegue até a raíz do projeto:
`cd git-core/`

1. Abra o arquivo "appsettings.json" em seu editor de texto de preferência
2. Navegue até o supabase e entre na página da sua database.
3. Clique no botão do topo "Connect"
4. Troque o tipo para .NET e copie o valor mostrado no campo "DefaultConnection"
   ![image](https://github.com/user-attachments/assets/eab03e5a-df93-471b-9070-e11426821669)

5. Substitua o conteúdo do campo "Connection String" em seu editor e salve o arquivo.
![image](https://github.com/user-attachments/assets/4210401c-f625-40b6-a74d-8a05555ada99)

## Executando a aplicação

Navegue até o source code:

`cd src/`

Execute o projeto:

`dotnet run`

Caso a conexão com seu banco esteja correta, o menu aparecerá. Como primeiro passo, você deve utilizar a opção 9 (cria o schema no seu banco) ou a opção 10 (cria o schema e insere dados ficticios). Após concluir qualquer uma dessas ações, será possível criar entidades normalmente com as opções 1-8.

Quando finalizado, você pode visualizar as queries no arquivo "queries.sql", que está uma subpasta acima: `cd ..` e executar na sua plataforma para visualizar os dados. A recomendação é que a opção de dados ficticios sejam gerados antes de rodar as queries para garantir que todas as relações entre as tabelas foram preenchidas corretamente.

**ATENÇÃO:** As duas opções que geram as tabelas APAGARÃO as entidades previamente registradas. No início da execução sempre há um reset para garantir a integridade das entidades que serão geradas posteriormente.

## Atendimento aos Requisitos

O sistema implementa nove entidades, superando o mínimo exigido de cinco e respeitando o limite de dez. Todas as entidades possuem mais que os três atributos mínimos necessários.

O projeto apresenta dois relacionamentos N:M com atributos próprios: a relação entre usuários e repositórios (usuario_repositorio) que armazena permissões e datas de adição; e a relação entre pull requests e revisores (pull_request_revisor) que mantém status, comentários e datas das revisões.

## Diagramas

```mermaid
erDiagram
    usuario {
    int id PK
    string nome
    string email
    datetime data_cadastro
    string biografia
    string foto_perfil
    }

    repositorio {
    int id PK
    string nome
    string descricao
    datetime data_criacao
    string visibilidade
    string linguagem_principal
    }

    branch {
    int id PK
    string nome
    string descricao
    datetime data_criacao
    int repositorio_id FK
    }

    commit {
    int id PK
    string mensagem
    datetime data_commit
    string hash
    int usuario_id FK
    int repositorio_id FK
    }

    issue {
    int id PK
    string titulo
    string descricao
    datetime data_criacao
    string status
    int repositorio_id FK
    int criador_id FK
    }

    comentario {
    int id PK
    string conteudo
    datetime data_criacao
    int usuario_id FK
    int issue_id FK
    }

    pull_request {
    int id PK
    string titulo
    string descricao
    datetime data_criacao
    string status
    int branch_origem_id FK
    int branch_destino_id FK
    int repositorio_id FK
    int criador_id FK
    }

    usuario_repositorio {
    int usuario_id FK
    int repositorio_id FK
    string tipo_permissao
    datetime data_adicao
    }

    pull_request_revisor {
    int pull_request_id FK
    int usuario_id FK
    string status_revisao
    string comentario_revisao
    datetime data_revisao
    }

    usuario ||--o{ commit : "realiza"
    usuario ||--o{ issue : "cria"
    usuario ||--o{ pull_request : "cria"
    usuario ||--o{ comentario : "faz"
    usuario }|--|| usuario_repositorio : "participa"
    usuario }|--|| pull_request_revisor : "revisa"
    repositorio }|--|| usuario_repositorio : "tem colaboradores"
    repositorio ||--o{ commit : "contém"
    repositorio ||--o{ issue : "contém"
    repositorio ||--o{ pull_request : "contém"
    repositorio ||--o{ branch : "possui"
    branch }|--|| repositorio : "pertence a"
    branch ||--o{ pull_request : "é origem de"
    branch ||--o{ pull_request : "é destino de"
    issue ||--o{ comentario : "recebe"
    pull_request }|--|| pull_request_revisor : "é revisado por"
