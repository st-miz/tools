#!/bin/sh
echo -n INPUT_MINUTE_NUMBER:
read sleep_min
echo $sleep_min"分後に通知します..."

#sleep $(($sleep_min * 60));
#vardisplay="時間になりました"
#osascript -e "display notification ${vardisplay}"
#osascript -e 'display notification "時間になりました"'

# 非同期実行用のfunction
function sleepTimer(){
    (sleep $(($1 * 60)); osascript -e 'display notification "時間になりました"') &
}
sleepTimer $sleep_min
