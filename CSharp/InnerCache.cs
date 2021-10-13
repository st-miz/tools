using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;

namespace Common
{
    /// <summary>
    /// .NETアプリケーション内部のcache機構
    /// </summary>
    /// <remarks>
    /// cacheサーバとの通信すら惜しい時に使用する.内部のcacheなので通信処理分だけこちらの方が高速<br/>
    /// .NET 4.5 以上を動作環境として推奨する
    /// </remarks>
    public class InnerCache
    {
        private const string REGION = null;
        /// <summary>
        /// キャッシュを保持しておくための条件になるディレクトリパス
        /// </summary>
        /// <remarks>キャッシュを意図的にリフレッシュかけたい場合はディレクトリを手動で削除</remarks>
        internal static readonly string CacheKeepDir =
            System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location) + "\\MobileCallServiceCacheKeepDirectory";
        //AppDomain.CurrentDomain.BaseDirectory + "\\MobileCallServiceCacheKeepDirectory";

        private static readonly TimeSpan DEFAULT_TIME = new TimeSpan(0, 5, 0);
        private static CacheItemPolicy DEFAULT_POLICY = new CacheItemPolicy()
        {
            //SlidingExpiration = new TimeSpan(0, 0, 30) //前回参照されてから一定時間経過
        };
        private static ObjectCache context = new MemoryCache("customeNameSpace", null);
        /// <summary>
        /// 排他制御用
        /// </summary>
        private static object _lock = new object();

        /// <summary>
        /// 唯一のインスタンス
        /// </summary>
        private static InnerCache _instance = null;

        // Singleton
        private InnerCache() { }

        /// <summary>
        /// キャッシュ用インスタンスを取得
        /// </summary>
        /// <returns>BusLocaCache</returns>
        public static InnerCache GetInstance()
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new InnerCache();
                }
                if (IsRefresh())
                {
                    // キャッシュ値をキープしつづける条件となるためのディレクトリを作成しておく
                    System.IO.Directory.CreateDirectory(CacheKeepDir);
                    _instance.Clear();
                }
            }
            return _instance;
        }

        /// <summary>
        /// cacheを追加する
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="time"></param>
        public void Put(string key, object val, TimeSpan time)
        {
            if (time == null) time = DEFAULT_TIME;
            DateTimeOffset now = DateTimeOffset.Now;
            //context.Set(key, val, _policy);
            CacheItem item = new CacheItem(key, val, REGION);
            lock (_lock)
            {
                context.Set(item, new CacheItemPolicy() { AbsoluteExpiration = now.Add(time) });
            }
        }

        /// <summary>
        /// cacheを追加する
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void Put(string key, object val)
        {
            Put(key, val, DEFAULT_TIME);
        }

        /// <summary>
        /// cacheを追加する
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="second"></param>
        public void Put(string key, object val, int second)
        {
            var ts = TimeSpan.FromSeconds((double)secound);
            Put(key, val, ts);
        }

        /// <summary>
        /// cacheを削除する
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            lock (_lock)
            {
                context.Remove(key, REGION);
            }
        }

        /// <summary>
        /// cacheを取得する
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key)
        {
            return context[key];
        }

        /// <summary>
        /// 指定したキーがキャッシュされているか
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns>キャッシュされていればtrue</returns>
        public bool Contains(string key)
        {
            bool b = context.Contains(key, REGION);
            return b;
        }

        /// <summary>
        /// キャッシュを初期化する
        /// </summary>
        public void Clear()
        {
            var cacheItems = context.ToList();
            lock (_lock)
            {
                foreach (KeyValuePair<String, Object> a in cacheItems)
                {
                    context.Remove(a.Key);
                }
            }
        }

        /// <summary>
        /// キャッシュしている要素数を取得
        /// </summary>
        /// <returns>キャッシュ要素数</returns>
        public int Count()
        {
            int a = 0;
            lock (_lock)
            {
                a = context.Count();
            }
            return a;
        }

        /// <summary>
        /// キャッシュデータを破棄し更新が必要か
        /// </summary>
        /// <returns>キャッシュをリフレッシュさせる場合はtrue</returns>
        private static bool IsRefresh()
        {
            return !System.IO.Directory.Exists(CacheKeepDir);
        }
    }
}