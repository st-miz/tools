rem -----------
rem "Schedule"という部分をサービス名にすること。サービス名であり表示名ではないので注意すること！！
rem -----------
rem サービス起動を"手動"に設定する
sc config Schedule start= demand

rem サービス起動を"自動"に設定する
sc config Schedule start= auto

rem サービス起動を"無効"に設定する
sc config Schedule start= disabled

pause


