-- PostgreSQL のクライアント接続情報
select 
 datid
 ,datname
 ,pid
 ,usesysid
 ,usename
 ,application_name
 ,client_addr
 ,client_hostname
 ,client_port
 ,backend_start
 ,xact_start
 ,query_start
 ,state_change
 ,waiting
 ,state
 -- データ量が多いのでコメント
 -- ,query
from pg_stat_activity


