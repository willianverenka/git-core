-- 1. listar os repositórios mais ativos (com mais commits)
select 
    r.id, 
    r.nome, 
    r.linguagem_principal, 
    count(c.id) as total_commits
from 
    repositorio r
left join 
    commit c on r.id = c.repositorio_id
group by 
    r.id, r.nome, r.linguagem_principal
order by 
    total_commits desc;

-- 2. encontrar os usuários mais colaborativos (que participam de mais repositórios)
select 
    u.id, 
    u.nome, 
    u.email, 
    count(ur.repositorio_id) as total_repositorios
from 
    usuario u
join 
    usuario_repositorio ur on u.id = ur.usuario_id
group by 
    u.id, u.nome, u.email
order by 
    total_repositorios desc;

-- 3. listar issues abertas com seus respectivos repositórios e criadores
select 
    i.id as issue_id, 
    i.titulo, 
    i.data_criacao, 
    r.nome as repositorio, 
    u.nome as criador
from 
    issue i
join 
    repositorio r on i.repositorio_id = r.id
join 
    usuario u on i.criador_id = u.id
where 
    i.status = 'aberto'
order by 
    i.data_criacao desc;

-- 4. encontrar pull requests com mais revisores e seus status
select 
    pr.id, 
    pr.titulo, 
    r.nome as repositorio,
    u.nome as criador,
    count(prr.usuario_id) as total_revisores,
    sum(case when prr.status_revisao = 'aprovado' then 1 else 0 end) as aprovacoes,
    sum(case when prr.status_revisao = 'solicitando alterações' then 1 else 0 end) as solicitacoes_alteracao
from 
    pull_request pr
join 
    repositorio r on pr.repositorio_id = r.id
join 
    usuario u on pr.criador_id = u.id
left join 
    pull_request_revisor prr on pr.id = prr.pull_request_id
group by 
    pr.id, pr.titulo, r.nome, u.nome
order by 
    total_revisores desc;

-- 5. identificar os usuários mais ativos por tipo de contribuição (commits, issues, pull requests)
select 
    u.nome as usuario,
    count(distinct c.id) as total_commits,
    count(distinct i.id) as total_issues,
    count(distinct pr.id) as total_pull_requests,
    (count(distinct c.id) + count(distinct i.id) + count(distinct pr.id)) as contribuicoes_totais
from 
    usuario u
left join 
    commit c on u.id = c.usuario_id
left join 
    issue i on u.id = i.criador_id
left join 
    pull_request pr on u.id = pr.criador_id
group by 
    u.id, u.nome
order by 
    contribuicoes_totais desc;

-- 6. identificar os usuários mais ativos em comentários por repositório
select 
    r.nome as repositorio,
    u.nome as usuario,
    count(c.id) as total_comentarios
from 
    comentario c
join 
    issue i on c.issue_id = i.id
join 
    repositorio r on i.repositorio_id = r.id
join 
    usuario u on c.usuario_id = u.id
group by 
    r.nome, u.nome
order by 
    total_comentarios desc, r.nome desc;

-- 7. listar branches com mais pull requests associados (como origem ou destino)
select 
    b.nome as branch,
    r.nome as repositorio,
    count(case when pr.branch_origem_id = b.id then 1 else null end) as prs_como_origem,
    count(case when pr.branch_destino_id = b.id then 1 else null end) as prs_como_destino,
    count(pr.id) as total_prs
from 
    branch b
join 
    repositorio r on b.repositorio_id = r.id
left join 
    pull_request pr on pr.branch_origem_id = b.id or pr.branch_destino_id = b.id
group by 
    b.id, b.nome, r.nome
order by 
    total_prs desc;

-- 8. listar os repositórios ordenados por número de branches
select 
    r.nome as repositorio,
    count(b.id) as total_branches
from 
    repositorio r
left join 
    branch b on r.id = b.repositorio_id
group by 
    r.id, r.nome
order by 
    total_branches desc;

-- 9. encontrar as issues com mais comentários
select 
    i.titulo as issue,
    r.nome as repositorio,
    u.nome as criador,
    count(c.id) as total_comentarios
from 
    issue i
join 
    repositorio r on i.repositorio_id = r.id
join 
    usuario u on i.criador_id = u.id
left join 
    comentario c on i.id = c.issue_id
group by 
    i.id, i.titulo, r.nome, u.nome
order by 
    total_comentarios desc
limit 10;

-- 10. identificar os repositórios com maior colaboração entre usuários diferentes
select 
    r.nome as repositorio,
    count(distinct c.usuario_id) as usuarios_distintos_commits,
    count(distinct i.criador_id) as usuarios_distintos_issues,
    count(distinct pr.criador_id) as usuarios_distintos_prs,
    count(distinct ur.usuario_id) as total_colaboradores,
    (count(distinct c.usuario_id) + count(distinct i.criador_id) + count(distinct pr.criador_id)) as indice_colaboracao
from 
    repositorio r
left join 
    commit c on r.id = c.repositorio_id
left join 
    issue i on r.id = i.repositorio_id
left join 
    pull_request pr on r.id = pr.repositorio_id
left join 
    usuario_repositorio ur on r.id = ur.repositorio_id
group by 
    r.id, r.nome
order by 
    indice_colaboracao desc;