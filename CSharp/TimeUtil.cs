    /// <summary>
    /// 時間関係のUtil
    /// </summary>
    public class TimeUtil
    {
        /// <summary>
        /// ISO8601表記
        /// </summary>
        private const string FormatIso8601 = "yyyy-MM-ddTHH:mm:sszzz";

        /// <summary>
        /// ユーザーに見せる時間形式
        /// </summary>
        private const string FormatUser = "yyyy/MM/dd HH:mm";

        /// <summary>
        /// 2000/01/01からの通算ミリ秒を取得する
        /// </summary>
        /// <returns></returns>
        public static long GetMillisecondFrom2000(DateTimeOffset compareDateTimeOffset)
        {
            //UTCへ変換
            DateTimeOffset utcCompareDateTimeOffset = compareDateTimeOffset.ToUniversalTime();

            //2000年を作成
            DateTimeOffset dateTimeOffset2000Year = new DateTimeOffset(2000, 1, 1, 0, 0, 0, new TimeSpan(0, 0, 0));

            //2000年を一応UTCへ変換
            DateTimeOffset utcDateTimeOffset2000Year = dateTimeOffset2000Year.ToUniversalTime();

            //2000年からの通算ミリ秒計算
            long subMilliSecond = (utcCompareDateTimeOffset.Ticks - utcDateTimeOffset2000Year.Ticks) / TimeSpan.TicksPerMillisecond;

            return subMilliSecond;
        }

        /// <summary>
        /// DBのTimestampにアクセスするためにはこのDateTimeを使用すること
        /// </summary>
        /// <param name="dateTimeOffset"></param>
        /// <returns></returns>
        public static DateTime GetChangeDbDateFromUtcNow()
        {
            //UTCに変換
            DateTimeOffset utcDateTimeOffset = DateTimeOffset.UtcNow;

            //DateTimeへ変換
            DateTime dateTime = new DateTime(utcDateTimeOffset.Ticks, DateTimeKind.Unspecified);

            return dateTime;
        }

        /// <summary>
        /// DBのTimestampにアクセスするためにはこのDateTimeを使用すること
        /// </summary>
        /// <param name="dateTimeOffset"></param>
        /// <returns></returns>
        public static DateTime GetChangeDbDate(DateTimeOffset targetDateTimeOffset)
        {
            //UTCに変換
            DateTimeOffset utcDateTimeOffset = targetDateTimeOffset.ToOffset(new TimeSpan(0, 0, 0));

            //DateTimeへ変換
            DateTime dateTime = new DateTime(utcDateTimeOffset.Ticks, DateTimeKind.Unspecified);

            return dateTime;
        }

        /// <summary>
        /// メールなどユーザーに見せる時間形式を取得する
        /// </summary>
        /// <param name="dateTimeOffset"></param>
        /// <returns></returns>
        public static string GetDateTimeUserFormatString(DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.ToString(FormatUser);
        }

        /// <summary>
        /// ISO8601形式のstringを取得する
        /// </summary>
        /// <param name="DateTimeOffset"></param>
        /// <returns></returns>
        public static string GetDateTimeIso8601FormatString(DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.ToString(FormatIso8601);
        }

        /// <summary>
        /// 文字列からDateTimeOffsetへ変換する
        /// </summary>
        /// <param name="DateTimeOffset">DateTime</param>
        /// <returns>DateTimeOffset</returns>
        public static bool TryParseIso8601String(string dateTimeOffsetString, out DateTimeOffset value)
        {
            //文字列からDateTimeOffsetへ変換
            System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
            if (DateTimeOffset.TryParseExact(dateTimeOffsetString, FormatIso8601, provider, System.Globalization.DateTimeStyles.None, out value))
            {
                //日本時間へ変換する
                value = Util.Time.GetTranslateToJapanDateTime(value);
                return true;
            }
            value = default(DateTimeOffset);
            return false;
        }

        /// <summary>
        /// 文字列からDateTimeOffsetへ変換する
        /// </summary>
        /// <param name="DateTimeOffset">DateTime</param>
        /// <returns>DateTimeOffset</returns>
        public static DateTimeOffset ParseIso8601String(string dateTimeOffsetString)
        {
            //文字列からDateTimeOffsetへ変換
            System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
            DateTimeOffset parseDateTimeOffset = DateTimeOffset.ParseExact(dateTimeOffsetString, FormatIso8601, provider);

            //日本時間へ変換する
            parseDateTimeOffset = Util.Time.GetTranslateToJapanDateTime(parseDateTimeOffset);
            return parseDateTimeOffset;
        }

        /// <summary>
        /// DateTimeOffsetを日本時間へ変換する
        /// </summary>
        /// <param name="DateTimeOffset">変換元DateTimeOffset</param>
        /// <returns>日本時間に変換されたDateTimeOffset</returns>
        public static DateTimeOffset GetTranslateToJapanDateTime(DateTimeOffset dateTimeOffset)
        {
            //UTC時刻へ変換
            DateTimeOffset utcDateTimeOffset = dateTimeOffset.ToUniversalTime();

            //UTC時刻を日本時間へ変換
            TimeZoneInfo jstZoneInfo = System.TimeZoneInfo.FindSystemTimeZoneById(Util.WebConfig.GetTimeZoneName());
            DateTimeOffset jpDateTimeOffset = System.TimeZoneInfo.ConvertTime(utcDateTimeOffset, jstZoneInfo);

            return jpDateTimeOffset;
        }

        //入力されたDateTimeOffsetを日本時間へ変換し、ISO8601形式のstringを取得する
        public static string GetJapanDateTimeIso8601String(DateTimeOffset dateTimeOffset)
        {
            //日本時間へ変換
            DateTimeOffset jpDateTimeOffset = GetTranslateToJapanDateTime(dateTimeOffset);

            //ISO8601形式へ変換
            return GetDateTimeIso8601FormatString(jpDateTimeOffset);
        }

        /// <summary>
        /// 文字列で日本時間を取得する
        /// </summary>
        /// <returns></returns>
        public static string GetJapanNowDateTimeIso8601String()
        {
            //日本時間を取
            DateTimeOffset jpDateTimeOffset = GetJapanDateTime();

            return GetDateTimeIso8601FormatString(jpDateTimeOffset);
        }

        /// <summary>
        /// 日本時間を取得する
        /// </summary>
        /// <returns></returns>
        public static DateTimeOffset GetJapanDateTime()
        {
            //時間をログ出力。以下は日本時間を出力するための処理
            //UTC時間を取得
            DateTimeOffset utcDateTimeOffset = DateTimeOffset.UtcNow;

            //日本のTimeZoneを取得
            TimeZoneInfo jstZoneInfo = System.TimeZoneInfo.FindSystemTimeZoneById(Util.WebConfig.GetTimeZoneName());

            //UTC時刻を日本時間へ変換
            DateTimeOffset jpDateTimeOffset = TimeZoneInfo.ConvertTime(utcDateTimeOffset, jstZoneInfo);

            return jpDateTimeOffset;
        }

        /// <summary>
        /// DBに時間系はnullを設定できないため、nullの役割をするTimeStampを返却する
        /// </summary>
        /// <param name="DateTimeOffset"></param>
        /// <returns></returns>
        public static DateTimeOffset GetNullTime()
        {
            return DateTimeOffset.MinValue;
        }

        /// <summary>
        /// Timestamp型のチェック
        /// </summary>
        /// <param name="timestamp">ISO8601形式のTimestamp型string</param>
        /// <returns></returns>
        public static bool IsTimestamp(string timestamp)
        {
            try
            {
                //変換できれば、日付と判断
                DateTimeOffset dateTimeOffset = Util.Time.ParseIso8601String(timestamp);
                return true;
            }
            catch (Exception)
            {
                //エラーが発生すればTimeStamp型ではないと判断
                return false;
            }
        }

        /// <summary>
        /// 与えられたDateTimeから、分、秒、ミリ秒を0にしたDateTimeOffsetを返却する
        /// </summary>
        /// <param name="DateTimeOffset">DateTimeOffset</param>
        /// <returns></returns>
        public static DateTimeOffset GetCompareHourStartDateTime(DateTimeOffset dateTimeOffset)
        {
            return new DateTimeOffset(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, dateTimeOffset.Hour, 0, 0, dateTimeOffset.Offset);
        }

        /// <summary>
        /// 与えられたDateTimeOffsetから、+1時、分、秒、ミリ秒を0にしたDateTimeOffsetを返却する
        /// </summary>
        /// <param name="DateTimeOffset">DateTimeOffset</param>
        /// <returns></returns>
        public static DateTimeOffset GetCompareHourEndDateTime(DateTimeOffset dateTimeOffset)
        {
            DateTimeOffset newDateTimeOffset = new DateTimeOffset(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, dateTimeOffset.Hour, 0, 0, dateTimeOffset.Offset);

            return newDateTimeOffset.AddHours(1);
        }

        /// <summary>
        /// 与えられたDateTimeOffsetから、2桁の時間を返却する
        /// 例)2014-07-24 09:00:00 -> 09を返却
        ///
        /// </summary>
        /// <param name="DateTimeOffset">DateTimeOffset</param>
        /// <returns>2桁の時間</returns>
        public static string Get2DigitsHour(DateTimeOffset dateTimeOffset)
        {
            string hourString = null;
            hourString = dateTimeOffset.Hour.ToString();
            if (hourString.Length == 1)
            {
                hourString = "0" + hourString;
            }
            return hourString;
        }
    }
