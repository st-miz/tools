set KEYWORD=xxxxxxxx
rem set KEYWORD=install.bat
rem set KEYWORD=uninstall.bat

FOR /F %%n IN ('findstr /m ERROR *.txt') DO ECHO %%n

pause