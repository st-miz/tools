SELECT TOP 20 SUBSTRING(qt.text, (qs.statement_start_offset/2)+1, 
        ((CASE qs.statement_end_offset
          WHEN -1 THEN DATALENGTH(qt.text)
         ELSE qs.statement_end_offset
         END - qs.statement_start_offset)/2)+1)
,qs.execution_count
,qs.total_logical_reads, qs.last_logical_reads
,qs.min_logical_reads, qs.max_logical_reads
,qs.total_elapsed_time, qs.last_elapsed_time
,qs.min_elapsed_time, qs.max_elapsed_time
,qs.last_execution_time
,qp.query_plan
,qs.total_worker_time / qs.execution_count / 1000.0 AS cpu_time -- 平均実行時間（ミリ秒）]
,qs.total_physical_reads / qs.execution_count as AvgPhysicalIOCount
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt
CROSS APPLY sys.dm_exec_query_plan(qs.plan_handle) qp
WHERE qt.encrypted=0
ORDER BY qs.total_logical_reads DESC
