REM TCP port�̍ė��p�܂ł̎���(default:240sec)��Z������(60�b���x���ǂ�����...�k�߂�����ƒᑬ����Ȃǂ̏ꍇ���v�Ȃ̂��ǂ����̊m�F�����ĂȂ�)
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\Tcpip\Parameters" /v TcpTimedWaitDelay /t REG_DWORD /d 60 /f
REM TCP port�̍ő�ڑ���(default:5000)�𑝂₷
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\Tcpip\Parameters" /v MaxUserPort /t REG_DWORD /d 60000 /f

pause