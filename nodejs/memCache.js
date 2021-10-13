let _cache = require('memory-cache');
//_cache.debug(true); //debug code;

const CACHE_TIME = 30 * 1000;
exports.put = function(key, val){
  //console.log("node-memory-cache put key = " + key);
  _cache.put(key, val, CACHE_TIME, function(key, value) {});
}
exports.putByTime = function(key, val, time){
  //console.log("node-memory-cache putByTime key = " + key);
  _cache.put(key, val, time, function(key, value) {});
}
exports.get = function(key){
  //console.log("node-memory-cache get key = " + key);
  return _cache.get(key);
}
exports.del = function(key){
  //console.log("node-memory-cache del key = " + key);
  return _cache.del(key);
}
exports.clear = function(){
  //console.log("node-memory-cache clear" + key);
  return _cache.clear();
}