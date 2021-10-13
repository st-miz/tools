using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using log4net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using MsgPack;
using MsgPack.Serialization;

namespace MobileCall.Core.Cache
{
    /// <summary>
    /// ファイルキャッシュクラス
    /// </summary>
    /// <remarks>
    /// システム内でほぼ不変に近いものなどはDBやAPIから取得した後<br />
    /// 自身の中にファイルとしてキャッシュする<br />
    /// 揮発性のあるメモリでは運用上の調査作業などが困難なため<br />
    /// また、アプリ再起動後やDB, API取得エラー時でもファイルの中身の値を正として<br />
    /// 他システムの障害に引きずられないためにファイルの中身の値を利用しつづけるものとする
    /// </remarks>
    public class FileCache : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // IIS上ホストしたAppだとSystem.Reflection.Assembly.GetCallingAssembly().LocationがApp配置先のパスを示してくれないためコメントアウト.
        //private static readonly string CacheDir = Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location) + "\\cache";
        private string CacheDir = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\cache";
        private string privateDirNm = "";

        private static readonly TimeSpan AccessTimeout = new TimeSpan(0, 0, 0, 15, 0);

        private volatile bool disposed = false;

        public FileCache() { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="privateDirName"></param>
        /// <remarks>
        /// 個別に管理したい場合などの時使用する<br />
        /// 通常のcacheディレクトリから1段階層をさげてcacheさせたい時<br />
        /// このコンストラクタを使用してください<br />
        /// privateDirName毎にフォルダを作成してcacheを保持します
        /// </remarks>
        public FileCache(string privateDirName)
        {
            if (!privateDirName.StartsWith("\\"))
            {
                //cacheディレクトリのパス文字列妥当性を
                //コンストラクタの時点で保証するための処理
                privateDirName = privateDirName.Insert(0, "\\");
            }
            this.privateDirNm = privateDirName;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cacheRootDir"></param>
        /// <param name="privateDirName"></param>
        /// <remarks>
        /// 継続的に管理したい場合などの時使用する<br />
        /// 通常の(app_root)/cacheディレクトリではなく特定のディレクトリに集約させる<br />
        /// appのバージョンアップなどで"/app/v1.0.0/bin/cache" から "/app/v1.1.0/bin/cache"<br />
        /// などのようにリリース時に揮発してしまう事を防ぎたい時に使用します
        /// </remarks>
        public FileCache(string cacheRootDir, string privateDirName) : this(privateDirName)
        {
            if (cacheRootDir == null)
                cacheRootDir = "";
            if (cacheRootDir.EndsWith("\\"))
            {
                //cacheディレクトリのパス文字列妥当性を
                //コンストラクタの時点で保証するための処理
                cacheRootDir = cacheRootDir.Substring(0, cacheRootDir.Length - 1);
            }
            this.CacheDir = cacheRootDir;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //

            disposed = true;
        }

        /// <summary>
        /// キャッシュを一括でクリアする
        /// </summary>
        /// <remarks>
        /// キャッシュ用のディレクトリ配下のファイルはすべて削除<br />
        /// </remarks>
        public void Clean()
        {
            try
            {
                Directory.Delete(CacheDir, true);
            }
            catch { }
        }

        /// <summary>
        /// キャッシュを取得する
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="t">valueで指定する型</param>
        /// <returns></returns>
        public object Get(string key, Type t)
        {
            object o = ReadBase(key, FileShare.ReadWrite, false, t);

            if (o == null)
                return null;

            return o;
        }

        /// <summary>
        /// 特定のPrefixを含むキャッシュファイルの一覧を取得
        /// </summary>
        /// <param name="keyPrefix">キーの接頭辞</param>
        /// <returns></returns>
        public string[] GetKeys(string keyPrefix)
        {
            string[] paths = Directory.GetFiles(CacheDir + privateDirNm, keyPrefix + "*", SearchOption.AllDirectories);
            List<string> result = new List<string>(512);
            foreach (string path in paths)
            {
                string fileWithoutExt = Path.GetFileNameWithoutExtension(path);
                if (fileWithoutExt.IndexOf(keyPrefix) >= 0)
                {
                    result.Add(fileWithoutExt);
                }
            }
            return result.ToArray<string>();
        }

        /// <summary>
        /// キャッシュファイルの最終更新日時を取得
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns>ファイルの最終更新日時</returns>
        public DateTime GetLastUpdateTime(string key)
        {
            DateTime d = DateTime.MinValue;
            string path = GetCacheFileFullPath(key);
            if (File.Exists(path))
            {
                FileInfo info = new FileInfo(path);
                d = info.LastWriteTime;
            }
            return d;
        }

        /// <summary>
        /// 有効期限切れか
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="expired">有効期限(秒)</param>
        /// <returns>有効期限切れであればtrue</returns>
        public bool Expired(string key, double expired)
        {
            bool b = false;
            if (Contains(key))
            {
                DateTime last = GetLastUpdateTime(key);
                double sec = DateTime.Now.Subtract(last).TotalSeconds;
                b = expired < sec;
            }
            return b;
        }

        public void Remove(string key)
        {
            string path = GetCacheFileFullPath(key);
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception e)
                {
                    log.Error("FileCache Remove Error." + e.StackTrace);
                }
            }
        }

        public bool Contains(string key)
        {
            string path = GetCacheFileFullPath(key);
            return File.Exists(path);
        }

        //public void Put(string key, object o)
        //{
        //    Put(key, o);
        //}

        public void Put<T>(string key, object v)
        {
            WriteFile<T>(key, v);
        }

        private void WriteFile<T>(string key, object o)
        {
            if (o == null)
                return;

            string path = GetCacheFileFullPath(key);

            //try
            //{
            //    string json = Common.JsonSerialize(o);
            //    using (FileStream stream = GetStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            //    {
            //        using (Stream streamSync = Stream.Synchronized(stream))
            //        using (StreamWriter sw = new StreamWriter(stream, Encoding.UTF8))
            //        {
            //            sw.AutoFlush = false;
            //            sw.WriteLine(json);
            //            sw.Flush();
            //            sw.Close();
            //        }
            //    }
            //}

            //BinaryFormatter formatter = new BinaryFormatter();
            var serializer = MessagePackSerializer.Get<T>();
            try
            {
                using (FileStream stream = GetStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
                {
                    using (Stream streamSync = Stream.Synchronized(stream))
                    using (MemoryStream ms = new MemoryStream())
                    {
                        //formatter.Serialize(ms, o);
                        serializer.Pack(ms, o);
                        stream.Write(ms.GetBuffer(), 0, (int)ms.Length);
                        stream.Flush();
                        stream.Close();
                    }
                }
            }
            catch (FileNotFoundException fne)
            {
                log.Error("FileCacheWriteError." + fne.Message + "\r\n" + fne.StackTrace);
            }
            catch (Exception e)
            {
                log.Error("FileCacheWriteError." + e.Message + "\r\n" + e.StackTrace);
            }
            finally
            {
            }
        }

        private object ReadBase(string key, FileShare shareMode, bool delFlg, Type t)
        {
            object res = null;
            string path = GetCacheFileFullPath(key);

            //try
            //{
            //    if (!File.Exists(path)) return null;
            //    using (FileStream stream = GetStream(path, FileMode.Open, FileAccess.Read, shareMode))
            //    {
            //        if (stream.Length == 0) return res;
            //        using (Stream streamSync = Stream.Synchronized(stream))
            //        using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
            //        {
            //            string a = sr.ReadToEnd();
            //            res = Common.JsonDeserialize(a, t);
            //        }
            //        if (delFlg)
            //        {
            //            // 読込み後即削除（ロックしている間に削除）
            //            File.Delete(path);
            //        }
            //        stream.Close();
            //    }
            //    return res;
            //}

            //BinaryFormatter bf = new BinaryFormatter();
            try
            {
                if (!File.Exists(path))
                    return null;
                using (FileStream stream = GetStream(path, FileMode.Open, FileAccess.Read, shareMode))
                {
                    if (stream.Length == 0)
                        return res;

                    var serializer = MessagePackSerializer.Get(t);
                    using (Stream streamSync = Stream.Synchronized(stream))
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        // メモリストリームの位置を初期化して正しくデシリアライズされるようにする大事な１行
                        ms.Seek(0, SeekOrigin.Begin);
                        //res = bf.Deserialize(ms);
                        byte[] bs = new byte[ms.Length];
                        ms.Read(bs, 0, bs.Length);
                        res = serializer.UnpackSingleObject(bs);
                    }
                    if (delFlg)
                    {
                        // 読込み後即削除（ロックしている間に削除）
                        File.Delete(path);
                    }
                    stream.Close();
                }
                return res;
            }
            catch (FileNotFoundException fne)
            {
                log.Error(fne.Message + "\r\n" + fne.StackTrace);
            }
            catch (Exception e)
            {
                log.Error(e.Message + "\r\n" + e.StackTrace);
            }
            finally
            {

            }
            return res;
        }

        private FileStream GetStream(string path, FileMode mode, FileAccess access, FileShare share)
        {
            FileStream stream = null;
            TimeSpan interval = new TimeSpan(0, 0, 0, 0, 50);
            TimeSpan totalTime = new TimeSpan();
            while (stream == null)
            {
                try
                {
                    stream = File.Open(path, mode, access, share);
                }
                catch (IOException ex)
                {
                    Thread.Sleep(interval);
                    totalTime += interval;

                    if (AccessTimeout.Ticks != 0)
                    {
                        if (totalTime > AccessTimeout)
                        {
                            if (log.IsDebugEnabled)
                                log.Debug("FileCache stream error.path=" + path + ", " + ex.Message + "\r\n" + ex.StackTrace);
                            throw ex;
                        }
                    }
                }
            }
            return stream;
        }

        private string GetCacheFileFullPath(string key)
        {
            if (!Directory.Exists(CacheDir + privateDirNm))
            {
                Directory.CreateDirectory(CacheDir + privateDirNm);
            }
            return CacheDir + privateDirNm + "\\" + key + ".dat";
        }

    }
}
