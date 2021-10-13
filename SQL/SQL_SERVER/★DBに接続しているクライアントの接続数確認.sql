select
  hostname
  ,program_name
  ,hostprocess
  ,COUNT(*) as cnt
from
  master..sysprocesses
where hostname!=''
--and hostname like 'GIO%'
group by
  hostname
  ,program_name
  ,hostprocess
order by
 hostname
 ,COUNT(*) desc
