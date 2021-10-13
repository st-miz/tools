-- PostgreSQL 
-- アクセスの多いtableを調べる事ができる
--
select 
    relname
    --, coalesce(seq_tup_read,0)+coalesce(idx_tup_fetch,0)+coalesce(n_tup_ins,0)+coalesce(n_tup_upd,0)+coalesce(n_tup_del,0) as total
    , coalesce(seq_tup_read,0)+coalesce(idx_tup_fetch,0) as select
    , coalesce(n_tup_ins,0) as insert, coalesce(n_tup_upd,0) as update
    , coalesce(n_tup_del,0) as delete 
from
    pg_stat_user_tables
order by total
desc
;