using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Common
{
    /// <summary>
    /// 緯度経度を表すクラス
    /// </summary>
    [Serializable]
    public class LatLng
    {
        /// <summary>
        /// 進数
        /// </summary>
        public enum AdicNumbers
        {
            /// <summary>
            /// 1度を1とする
            /// </summary>
            Degree = 1,
            /// <summary>
            /// 1/1000秒を1とする
            /// </summary>
            MilliSec = 2,
            /// <summary>
            /// 1/1000分を1とする
            /// </summary>
            MilliMin = 3,
            /// <summary>
            /// 1/256秒を1とする
            /// </summary>
            MapDK = 4,
        }
        /// <summary>
        /// 測地系
        /// </summary>
        public enum Datums
        {
            /// <summary>
            /// 世界測地系
            /// </summary>
            WGS84 = 1,
            /// <summary>
            /// 日本測地系
            /// </summary>
            Tokyo = 2
        }

        /// <summary>
        /// 日本列島の離島
        /// </summary>
        /// <remarks>石垣島や硫黄島、青ヶ島など</remarks>
        private enum RemoteIsland
        {
            /// <summary>
            /// 離島じゃない
            /// </summary>
            None = 0,
            /// <summary>
            /// 石垣島、竹富島
            /// </summary>
            IshigakiJima = 1,
            /// <summary>
            /// 硫黄島
            /// </summary>
            IoJima = 2,
            /// <summary>
            /// 北大東島、南大東島
            /// </summary>
            DaitoJima = 3,
            /// <summary>
            /// 多良間島(たらましま)、水納島(みんなしま)
            /// </summary>
            TaramaJima = 4,
            /// <summary>
            /// 与那国島
            /// </summary>
            YonaguniJima = 5,
            /// <summary>
            /// 青ヶ島
            /// </summary>
            Aogashinma = 6,
            /// <summary>
            /// 宮古諸島
            /// </summary>
            MiyakoJima = 7
            // TODO:必要に応じて離島の値を追加していくこと.(石垣島、硫黄島、青ヶ島、北大東島・南大東島、多良間島・水納島、与那国島、宮古諸島)
        }

        /// <summary>
        /// 空の緯度経度を表す
        /// </summary>
        public static LatLng Empty = new LatLng(0, 0);
        /// <summary>
        /// このクラスのインスタンスを生成します。
        /// </summary>
        /// <param name="lat">緯度</param>
        /// <param name="lng">経度</param>
        /// <param name="adic">緯度経度値の進数</param>
        public static LatLng GetInstance(decimal lat, decimal lng, AdicNumbers adic)
        {
            return GetInstance(lat, lng, adic, Datums.Tokyo);
        }
        /// <summary>
        /// このクラスのインスタンスを生成します。
        /// </summary>
        /// <param name="lat">緯度</param>
        /// <param name="lng">経度</param>
        /// <param name="adic">緯度経度値の進数</param>
        /// <param name="datum">測地系</param>
        public static LatLng GetInstance(decimal lat, decimal lng, AdicNumbers adic, Datums datum)
        {
            LatLng instance = new LatLng(lat, lng);
            instance.AdicNumber = adic;
            instance.Datum = datum;

            return instance;
        }
        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="lat">緯度。</param>
        /// <param name="lng">経度。</param>
        private LatLng(decimal lat, decimal lng)
        {
            this.Latitude = lat;
            this.Longitude = lng;
            this.AdicNumber = AdicNumbers.MilliMin;
            this.Datum = Datums.Tokyo;
        }

        /// <summary>
        /// 緯度。
        /// </summary>
        public decimal Latitude { get; private set; }

        /// <summary>
        /// 経度。
        /// </summary>
        public decimal Longitude { get; private set; }

        /// <summary>
        /// 進数
        /// </summary>
        public AdicNumbers AdicNumber { get; private set; }

        /// <summary>
        /// 測地系
        /// </summary>
        public Datums Datum { get; private set; }

        /// <summary>
        /// 指定された進数での緯度を取得します。
        /// </summary>
        /// <returns>指定された進数での緯度</returns>
        public decimal LatitudeAs(AdicNumbers adic)
        {
            if (this.AdicNumber == adic)
            {
                return this.Latitude;
            }

            return ConvertAdic(this.Latitude, this.AdicNumber, adic);
        }

        /// <summary>
        /// 指定された進数での経度を取得します。
        /// </summary>
        /// <returns>指定された進数での経度</returns>
        public decimal LongitudeAs(AdicNumbers adic)
        {
            if (this.AdicNumber == adic)
            {
                return this.Longitude;
            }

            return ConvertAdic(this.Longitude, this.AdicNumber, adic);
        }

        /// <summary>
        /// ラジアン緯度。
        /// １°は、(PI / 180)ラジアン。
        /// </summary>
        private double LatitudeAsRadian
        {
            get
            {
                if (this.Latitude == 0) return 0;

                return (Math.PI / 180) * Convert.ToDouble(this.LatitudeAs(AdicNumbers.Degree));
            }
        }
        /// <summary>
        /// ラジアン経度。
        /// １°は、(PI / 180)ラジアン。
        /// </summary>
        private double LongitudeAsRadian
        {
            get
            {
                if (this.Longitude == 0) return 0;

                return (Math.PI / 180) * Convert.ToDouble(this.LongitudeAs(AdicNumbers.Degree));
            }
        }
        /// <summary>
        /// 世界測地系と日本測地系を変換します。
        /// </summary>
        /// <param name="latlng">世界測地系の緯度経度</param>
        /// <returns>日本測地系の緯度経度</returns>
        public static LatLng ConvertDatum(LatLng latlng, Datums to)
        {
            if (latlng.Datum == to)
            {
                return LatLng.GetInstance(latlng.Latitude, latlng.Longitude, latlng.AdicNumber, to);
            }

            LatLng ret = null;

            if (to == Datums.Tokyo)
            {
                var lat1 = latlng.LatitudeAs(AdicNumbers.Degree);
                var lng1 = latlng.LongitudeAs(AdicNumbers.Degree);
                decimal lat = lat1 + lat1 * 0.000106961m - lng1 * 0.000017467m - 0.004602017m;
                decimal lng = lng1 + lat1 * 0.000046047m + lng1 * 0.000083049m - 0.010041046m;

                RemoteIsland island = IsRemoteIsland(lat, lng);
                ConvertRemoteIslandOffsetToTokyo(to, AdicNumbers.Degree, island, ref lat, ref lng);

                ret = LatLng.GetInstance(lat, lng, AdicNumbers.Degree, to);
            }
            else
            {
                var lat1 = latlng.LatitudeAs(AdicNumbers.Degree);
                var lng1 = latlng.LongitudeAs(AdicNumbers.Degree);

                RemoteIsland island = IsRemoteIsland(lat1, lng1);

                decimal lat = lat1 - lat1 * 0.00010695m + lng1 * 0.000017464m + 0.0046017m;
                decimal lng = lng1 - lat1 * 0.000046038m - lng1 * 0.000083043m + 0.010040m;

                ConvertRemoteIslandOffsetToWGS84(to, AdicNumbers.Degree, island, ref lat, ref lng);

                ret = LatLng.GetInstance(lat, lng, AdicNumbers.Degree, to);
            }

            return ConvertAdic(ret, latlng.AdicNumber);
        }
        /// <summary>
        /// 数値緯度経度を度分秒に変換します。
        /// </summary>
        /// <param name="latlngNum">数値緯度経度</param>
        /// <param name="adic">数値緯度経度の進数</param>
        /// <param name="degree">度</param>
        /// <param name="minutes">分</param>
        /// <param name="seconds">秒</param>
        public static void ConvertToDMS(decimal latlngNum, AdicNumbers adic, ref int degree, ref int minutes, ref float seconds)
        {
            switch (adic)
            {
                case AdicNumbers.Degree:
                    decimal resultDegree = Math.Floor((decimal)latlngNum);
                    decimal x = (3600000m * ((decimal)latlngNum - resultDegree)) / 1000m;
                    decimal resultMin = Math.Floor(x / 60m);
                    decimal resultSec = x - (resultMin * 60m);

                    degree = Convert.ToInt32(resultDegree);
                    minutes = Convert.ToInt32(resultMin);
                    seconds = Convert.ToSingle(resultSec);

                    break;
                case AdicNumbers.MilliSec:
                    degree = Convert.ToInt32((int)latlngNum / (60 * 60 * 1000));
                    minutes = Convert.ToInt32(((int)latlngNum % (60 * 60 * 1000)) / (1000 * 60));
                    seconds = ((int)latlngNum % (60 * 60 * 1000)) % (1000 * 60) / 1000f;

                    break;
                case AdicNumbers.MilliMin:
                    degree = Convert.ToInt32((int)latlngNum / (60 * 1000));
                    minutes = Convert.ToInt32(((int)latlngNum % (60 * 1000)) / 1000);
                    seconds = ((int)latlngNum % 1000) * 0.06f;

                    break;
                default:
                    throw new NotSupportedException("");
            }
        }
        /// <summary>
        /// 緯度経度の進数を変換します。
        /// </summary>
        /// <param name="latlng">変換前</param>
        /// <param name="to">変換後の進数</param>
        /// <return>変換された緯度経度</return>
        public static LatLng ConvertAdic(LatLng latlng, AdicNumbers to)
        {
            decimal lat = ConvertAdic(latlng.Latitude, latlng.AdicNumber, to);
            decimal lng = ConvertAdic(latlng.Longitude, latlng.AdicNumber, to);

            return GetInstance(lat, lng, to, latlng.Datum);
        }
        /// <summary>
        /// 数値緯度経度の進数を変換します。
        /// </summary>
        /// <param name="latlngNum">数値緯度経度</param>
        /// <param name="from">数値緯度経度の進数</param>
        /// <param name="to">変換後の進数</param>
        /// <returns>変換された数値緯度経度</returns>
        private static decimal ConvertAdic(decimal latlngNum, AdicNumbers from, AdicNumbers to)
        {
            if (from == to) return latlngNum;

            // 度分秒に変換
            int degree = 0;
            int minutes = 0;
            float seconds = 0f;

            ConvertToDMS(latlngNum, from, ref degree, ref minutes, ref seconds);

            switch (to)
            {
                case AdicNumbers.Degree:
                    return Convert.ToDecimal(degree + minutes / 60d + seconds / 3600d);
                case AdicNumbers.MilliSec:
                    return Convert.ToInt32(((degree * 3600) + (minutes * 60) + seconds) * 1000);
                case AdicNumbers.MilliMin:
                    return Convert.ToInt32(((degree * 60) + minutes + (seconds / 60)) * 1000);
                case AdicNumbers.MapDK:
                    return Convert.ToInt32(((degree * 60 * 60) + (minutes * 60) + seconds) * 256);

                default:
                    throw new NotSupportedException();
            }
        }
        /// <summary>
        /// 別の位置との距離をメートル単位で取得します。
        /// 計算論理には球面三角法を使用します。
        /// </summary>
        /// <param name="pos">距離を測る位置</param>
        /// <returns>指定された位置との距離。メートル単位</returns>
        public double GetDistance(LatLng pos)
        {
            double distance;

            LatLng otherPos = ConvertAdic(pos, this.AdicNumber);
            if (pos.Datum != this.Datum) otherPos = ConvertDatum(otherPos, this.Datum);

            // 同位置
            if (this.Latitude == otherPos.Latitude &&
                this.Longitude == otherPos.Longitude)
            {
                return 0;
            }

            // 球面三角法を利用して、距離を求める
            double d = Math.Sin(this.LatitudeAsRadian) * Math.Sin(otherPos.LatitudeAsRadian) +
                       Math.Cos(this.LatitudeAsRadian) * Math.Cos(otherPos.LatitudeAsRadian) *
                       Math.Cos(this.LongitudeAsRadian - otherPos.LongitudeAsRadian);
            distance = Math.Acos(d) * (180 / Math.PI) * 60 * 1.852;

            return distance * 1000;
        }
        /// <summary>
        /// 指定された方位に指定距離移動した緯度経度を取得します。
        /// </summary>
        /// <param name="direction">方位</param>
        /// <param name="distance">距離(メートル単位)</param>
        /// <returns>求まる緯度経度</returns>
        public LatLng GetPointMoved(double direction, double distance)
        {
            const double EPS = 0.00000000005;

            // 符号 ＋ or -
            int latitudeSign = (this.Latitude < 0 ? -1 : 1);
            int longitudeSign = (this.Longitude < 0 ? 1 : -1);

            // 基準地をラジアンに変換
            double latitude1Radian = latitudeSign * LatitudeAsRadian;
            double longitude1Radian = longitudeSign * LongitudeAsRadian;

            // 距離をmからnmに変換してからラジアンに変換
            double distanceRadian = (distance / 1000) / (180 * 60 / Math.PI) / 1.852;

            // 方位をラジアンに変換
            double directionRadian = direction * (Math.PI / 180);

            // 目的地の緯度を算出
            double latitude2Radian = Math.Asin(
                Math.Sin(latitude1Radian) * Math.Cos(distanceRadian) +
                Math.Cos(latitude1Radian) * Math.Sin(distanceRadian) * Math.Cos(directionRadian));

            // 目的地の経度を算出
            double longitude2Radian;
            if (Math.Abs(Math.Cos(latitude2Radian)) < EPS)
            {
                longitude2Radian = 0;
            }
            else
            {
                double dlon = Math.Atan2(
                    Math.Sin(directionRadian) * Math.Sin(distanceRadian) * Math.Cos(latitude1Radian),
                    Math.Cos(distanceRadian) - Math.Sin(latitude1Radian) * Math.Sin(latitude2Radian));

                double x = longitude1Radian - dlon + Math.PI;
                double y = 2 * Math.PI;

                longitude2Radian = (x - y * Math.Floor(x / y)) - Math.PI;
            }

            // 目的地を度の百分率に変換
            double latitude2 = Math.Abs(latitude2Radian * (180 / Math.PI));
            double longitude2 = Math.Abs(longitude2Radian * (180 / Math.PI));

            LatLng ret = GetInstance((decimal)latitude2, (decimal)longitude2, AdicNumbers.Degree, this.Datum);

            ret = ConvertAdic(ret, this.AdicNumber);

            return ret;
        }
        /// <summary>
        /// 別の位置の方位を取得します。
        /// </summary>
        /// <param name="otherPoint">別の位置</param>
        /// <returns>別の位置の方位</returns>
        public double GetDirection(LatLng otherPoint)
        {
            double direction;

            LatLng otherPos = ConvertAdic(otherPoint, this.AdicNumber);
            if (otherPoint.Datum != this.Datum) otherPos = ConvertDatum(otherPos, this.Datum);


            // 同位置
            if (this.Latitude == otherPos.Latitude &&
                this.Longitude == otherPos.Longitude)
            {
                return 0;
            }

            double d = Math.Sin(this.LatitudeAsRadian) * Math.Sin(otherPos.LatitudeAsRadian) +
                       Math.Cos(this.LatitudeAsRadian) * Math.Cos(otherPos.LatitudeAsRadian) *
                       Math.Cos(this.LongitudeAsRadian - otherPos.LongitudeAsRadian);
            d = Math.Acos(d);

            if ((Math.Sin(d) * Math.Cos(this.LatitudeAsRadian)) == 0)
            {
                return 0;
            }

            double argAcos =
                (Math.Sin(otherPos.LatitudeAsRadian) - Math.Sin(this.LatitudeAsRadian) * Math.Cos(d)) /
                (Math.Sin(d) * Math.Cos(this.LatitudeAsRadian));

            if (Math.Sin(this.LongitudeAsRadian - otherPos.LongitudeAsRadian) < 0)
            {
                if (Math.Abs(argAcos) > 1)
                {
                    argAcos /= Math.Abs(argAcos);
                }
                direction = Math.Acos(argAcos);
            }
            else
            {
                if (Math.Abs(argAcos) > 1)
                {
                    argAcos /= Math.Abs(argAcos);
                }
                direction = 2 * Math.PI - Math.Acos(argAcos);
            }

            return direction * (180 / Math.PI);
        }

        /// <summary>
        /// 離島でないかチェック、離島の場合、日本測地では補正が必要
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        private static RemoteIsland IsRemoteIsland(decimal lat, decimal lon)
        {
            RemoteIsland result = RemoteIsland.None;
            if (lat >= 24.30130555555555389m && lon >= 124.06452777777778351m && lat <= 24.6179722222222m && lon <= 124.352027777778m)
            {
                result = RemoteIsland.IshigakiJima;
            }
            else if (lat >= 24.74166667m && lon >= 141.275m && lat <= 24.81666667m && lon <= 141.3875m)
            {
                result = RemoteIsland.IoJima;
            }
            else if (lat >= 25.8049786111m && lon >= 131.2052305556m && lat <= 25.9633119444m && lon <= 131.3427305556m)
            {
                result = RemoteIsland.DaitoJima;
            }
            else if (lat >= 24.6307911111m && lon >= 124.6672794444m && lat <= 24.7641244444m && lon <= 124.7297794444m)
            {
                result = RemoteIsland.TaramaJima;
            }
            else if (lat >= 24.4263300000m && lon >= 122.9269680556m && lat <= 24.4763300000m && lon <= 123.0519680556m)
            {
                result = RemoteIsland.YonaguniJima;
            }
            else if (lat >= 32.4340111111m && lon >= 139.7500386111m && lat <= 32.4756777778m && lon <= 139.7875386111m)
            {
                result = RemoteIsland.Aogashinma;
            }
            else if (lat >= 24.6994891667m && lon >= 125.1256694444m && lat <= 24.9411558333m && lon <= 125.4756694444m)
            {
                result = RemoteIsland.MiyakoJima;
            }
            return result;
        }

        /// <summary>
        /// 日本測地の離島補正を実施する（世界測地-->日本測地）
        /// </summary>
        /// <param name="to"></param>
        /// <param name="adic"></param>
        /// <param name="remoteIsland"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        /// <remarks>離島補正は日本測地のみ実施すること</remarks>
        private static void ConvertRemoteIslandOffsetToTokyo(Datums to, AdicNumbers adic, RemoteIsland remoteIsland, ref decimal lat, ref decimal lon)
        {
            // http://vldb.gsi.go.jp/sokuchi/coordinates/localtrans.html
            // https://www.jstage.jst.go.jp/article/sokuchi1954/49/3/49_3_181/_pdf

            if (to != Datums.Tokyo) return;
            if (remoteIsland == RemoteIsland.None) return;

            // 必要に応じて処理を追加すること
            if (adic == AdicNumbers.Degree)
            {
                switch (remoteIsland)
                {
                    case RemoteIsland.IshigakiJima:
                        lat = lat - 0.00001451111111066m;
                        lon = lon + 0.00020742973677787969m;
                        break;
                    case RemoteIsland.IoJima:
                        break;
                    case RemoteIsland.DaitoJima:
                        lat = lat + 0.000012211m;
                        lon = lon + 0.00017559m;
                        break;
                    case RemoteIsland.TaramaJima:
                        lat = lat - 0.0000122203m;
                        lon = lon + 0.0001988605m;
                        break;
                    case RemoteIsland.YonaguniJima:
                        lat = lat - 0.0000234166m;
                        lon = lon + 0.0001989007m;
                        break;
                    case RemoteIsland.Aogashinma:
                        break;
                    case RemoteIsland.MiyakoJima:
                        lat = lat - 0.0000087136m;
                        lon = lon + 0.0001976584m;
                        break;
                    default:
                        break;
                }
            }
            // もちろん世界測地は離島補正無し
            //else if (this.Datum == Datums.WGS84){}
        }

        /// <summary>
        /// 日本測地の離島補正を実施する（世界測地-->日本測地）
        /// </summary>
        /// <param name="to"></param>
        /// <param name="adic"></param>
        /// <param name="remoteIsland"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        /// <remarks>離島補正は日本測地のみ実施すること</remarks>
        private static void ConvertRemoteIslandOffsetToWGS84(Datums to, AdicNumbers adic, RemoteIsland remoteIsland, ref decimal lat, ref decimal lon)
        {
            // http://vldb.gsi.go.jp/sokuchi/coordinates/localtrans.html
            // https://www.jstage.jst.go.jp/article/sokuchi1954/49/3/49_3_181/_pdf

            // 世界測地にしたい時だけこのメソッドは仕事をする
            if (to != Datums.WGS84) return;
            if (remoteIsland == RemoteIsland.None) return;

            // 必要に応じて処理を追加すること
            if (adic == AdicNumbers.Degree)
            {
                switch (remoteIsland)
                {
                    case RemoteIsland.IshigakiJima:
                        lat = lat + 0.00001451111111066m;
                        lon = lon - 0.00020742973677787969m;
                        break;
                    case RemoteIsland.IoJima:
                        break;
                    case RemoteIsland.DaitoJima:
                        lat = lat - 0.000012211m;
                        lon = lon - 0.00017559m;
                        break;
                    case RemoteIsland.TaramaJima:
                        lat = lat + 0.0000122203m;
                        lon = lon - 0.0001988605m;
                        break;
                    case RemoteIsland.YonaguniJima:
                        lat = lat + 0.0000234166m;
                        lon = lon - 0.0001989007m;
                        break;
                    case RemoteIsland.Aogashinma:
                        break;
                    case RemoteIsland.MiyakoJima:
                        lat = lat + 0.0000087136m;
                        lon = lon - 0.0001976584m;
                        break;
                    default:
                        break;
                }
            }
            // もちろん世界測地は離島補正無し
            //else if (this.Datum == Datums.WGS84){}
        }

        /// <summary>
        /// (緯度,経度)の形式の文字列を返します。
        /// </summary>
        public override string ToString()
        {
            return string.Format("({0},{1})", this.Latitude, this.Longitude);
        }
        /// <summary>
        /// Equalsのオーバーライド
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == this) return true;
            if (!(obj is LatLng)) return false;

            LatLng other = ((LatLng)obj);

            if ((other.Latitude == this.Latitude) &&
                (other.Longitude == this.Longitude))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
