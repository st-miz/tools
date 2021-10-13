/**
 * cacheする機構
 * sessionStorageに入れる形式は以下のobject形式とする
 * {
 * var: <object>
 * expired : Date(cacheに登録時間)
 * }
 */
var clientCache = (function (){
    var _cacheEnabled = ("sessionStorage" in window) && window["sessionStorage"] !== null;
    var CACHE_EXPIRED_TIME = 30;
    var CLIENT_CACHE_PREFIX = "in_cache";
    return {
        set: function(k, v){
            if (!_cacheEnabled) return;

            var tempCache = {val: v, expired: Date.now()};
            try{
                sessionStorage.setItem(CLIENT_CACHE_PREFIX + k, JSON.stringify(tempCache));
                //console.log("-- cache set. ->" + JSON.stringify(tempCache));
            } catch(ex){}
        },
        get: function(k, expiredTime){
            if (!_cacheEnabled) return null;
            if(expiredTime == null){
                expiredTime = CACHE_EXPIRED_TIME;
            }

            var txt = sessionStorage.getItem(CLIENT_CACHE_PREFIX + k);
            if (txt == null){
                return null;
            }
            try {
                var cacheVal = JSON.parse(txt);
                if (cacheVal == null) {
                    return null;
                }
                var subSec = (Date.now() - cacheVal.expired) / 1000;
                // 期限切れは無いものとみなす
                if (subSec > expiredTime){
                    //console.log("-- cache remove. sec ->" + subSec);
                    sessionStorage.removeItem(CLIENT_CACHE_PREFIX + k);
                    return null;
                }
                // cache専用のexpiredは削除して返す
                //console.log("-- cache hit !!! " + JSON.stringify(cacheVal.val));
                return cacheVal.val;
            } catch (ex) {
                return null;
            }
        },
        remove:function(k){
            sessionStorage.removeItem(CLIENT_CACHE_PREFIX + k);
        },
        clear: function(){
            var keys = [];
            for (var i = 0, len = sessionStorage.length; i < len; i++) {
                keys.push(sessionStorage.key(i));
            }
            for (var i = 0, keyLen = keys.length; i < keyLen; i++) {
                var delKey = keys[i];
                //console.log("-- cache clear. key ->" + delKey);
                sessionStorage.removeItem(delKey);
            }
        }
    }
})();