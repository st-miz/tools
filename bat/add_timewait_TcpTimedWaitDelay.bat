REM TCP portの再利用までの時間(default:240sec)を短くする(60秒程度が良いかと...縮めすぎると低速回線などの場合大丈夫なのかどうかの確認が取れてない)
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\Tcpip\Parameters" /v TcpTimedWaitDelay /t REG_DWORD /d 60 /f
REM TCP portの最大接続数(default:5000)を増やす
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\Tcpip\Parameters" /v MaxUserPort /t REG_DWORD /d 60000 /f

pause