SELECT es.session_id
    ,es.host_name
    ,es.program_name
    --,es.client_interface_name
    ,es.login_name
    ,es.nt_user_name
    ,es.login_time
    --,es.endpoint_id
	--,es.context_info
	,es.row_count
    ,es.cpu_time
    ,es.total_scheduled_time
    ,es.total_elapsed_time
    ,es.memory_usage
    ,es.logical_reads
    -- セッション中に、セッションの要求によって実行された読み書き数
    ,es.reads
    ,es.writes
    -- この接続で最後に発生した読み書きのタイムスタンプ
    ,ec.last_read
    ,ec.last_write
    -- この接続で発生したパケット読み書きの数
    ,ec.num_reads
    ,ec.num_writes
--    ,st.text
    ,ec.client_net_address
    ,ec.client_tcp_port
    ,ec.connect_time
    --,ec.local_net_addressa
    --,ec.local_tcp_port
--    ,p.cpu as syspro_cpu -- es.cpu_timeと同じような気がする
    ,p.physical_io as syspro_io
--    ,p.login_time as syspro_loin_time
    ,p.open_tran as syspro_open_tran
    --,p.hostname as p_host_nm
    --,p.program_name as p_program_nm
FROM sys.dm_exec_sessions es
    LEFT JOIN sys.dm_exec_connections ec
        ON es.session_id = ec.session_id
    LEFT JOIN sys.dm_exec_requests er
        ON es.session_id = er.session_id
    LEFT JOIN  sysprocesses p
        ON es.session_id = p.spid
--    OUTER APPLY sys.dm_exec_sql_text (er.sql_handle) st
WHERE es.session_id > 50    -- < 50 system sessions
	-- local machineなどは対象から除去する
	AND es.program_name != 'SQLAgent - Job invocation engine'
	AND es.program_name != 'SQLAgent - Generic Refresher'
	AND es.program_name != 'Microsoft SQL Server Management Studio'
	AND es.program_name != 'Microsoft SQL Server Management Studio - クエリ'
	AND es.program_name != 'Report Server'
ORDER BY 
 es.cpu_time DESC
-- es.host_name ,es.cpu_time DESC
