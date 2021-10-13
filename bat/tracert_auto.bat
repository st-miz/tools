@echo off
REM log�o�͐�(�t�H���_�͎��O�ɍ쐬�ς݂��O��)
set LOG_FILE="C:\temp\tracert.log"
REM �g���[�X����Ώ�]
set TRACERT_IP=192.168.1.1
REM �P�ʁF�b
set INTERVAL_SEC=600
echo �I������ꍇ��Ctrl + c�ŏI�����Ă�������

:LOOP
echo ----- %date% %time% tracert!! -----
echo ----- %date% %time% tracert!! ----- >> %LOG_FILE%
tracert  -d %TRACERT_IP%  >> %LOG_FILE%
echo --------------------------------
echo -------------------------------- >> %LOG_FILE%
timeout /T %INTERVAL_SEC%
goto LOOP

:EXIT
pause