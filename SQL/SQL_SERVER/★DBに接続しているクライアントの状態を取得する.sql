USE master
select
hostname
,program_name
,hostprocess
,open_tran
, DB_NAME(sysdatabases.dbid) AS DB_NAME
,sysdatabases.dbid
,COUNT(*) as cnt
,cmd
,memusage
,cpu
,physical_io
from
master..sysprocesses
left join master..sysdatabases
ON sysprocesses.dbid = sysdatabases.dbid
where hostname!=''
group by
hostname
,program_name
,hostprocess
,memusage
,open_tran
,sysdatabases.dbid
,cmd
,cpu
,physical_io
ORDER BY hostname,hostprocess,cmd, cnt
;