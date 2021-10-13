// C# util系の処理


public class Util {
        //カナチェック
        public static bool IsKatakana(string kana)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(kana, @"^[\p{IsKatakana}\u31F0-\u31FF\u3099-\u309C\uFF65-\uFF9F]+$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Emailチェック
        public static bool IsMail(string mail)
        {
            //Emailチェック
            System.Text.RegularExpressions.Regex regex =
            new System.Text.RegularExpressions.Regex("^([^@])+@([^@])*[.]+([^@])*$");

            System.Text.RegularExpressions.Match result = regex.Match(mail);

            return result.Success;

        }

        //電話番号チェック
        public static bool IsTel(string Tel)
        {
            if (Tel.Length == 10)
            {
                //固定電話
                System.Text.RegularExpressions.Regex regex =
                new System.Text.RegularExpressions.Regex(@"^0(?:[1-9]|[1-9]\d{8})$");

                System.Text.RegularExpressions.Match result = regex.Match(Tel);
                return result.Success;
            }
            else if (Tel.Length == 11)
            {
                //携帯・PHS・IP
                System.Text.RegularExpressions.Regex regex =
                new System.Text.RegularExpressions.Regex(@"(090|080|070|050)\d{8}");

                System.Text.RegularExpressions.Match result = regex.Match(Tel);
                return result.Success;
            }
            return false;
        }

        /// <summary>
        /// 誕生日のチェック
        /// 日を0にすると、日についてチェックしない
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="day">日</param>
        /// <returns></returns>
        public static bool IsValidBirthDay(int year, int month, int day)
        {
            string dateFormat = "{0:D4}-{1:D2}-{2:D2}T00:00:00+09:00";
            string dateString = null;
            if (day == 0)
            {
                //日のチェックをしない
                dateString = String.Format(dateFormat, year, month, 1);
            }
            else
            {
                //日のチェックを行う
                dateString = String.Format(dateFormat, year, month, day);
            }

            try
            {
                //parseしてみる
                DateTimeOffset dateTimeOffset = TimeUtil.ParseIso8601String(dateString);

                return true;
            }
            catch (Exception)
            {
                //不正な日付 2月30日など
                //型 'System.FormatException' のハンドルされていない例外が mscorlib.dll で発生しました
                //追加情報:文字列で表される DateTime がカレンダー System.Globalization.GregorianCalendar でサポートされていません。
            }

            return false;
        }

        /// <summary>
        /// BCCを計算する
        /// </summary>
        /// <param name="bytes">対象のバイト配列</param>
        /// <returns>BCC計算結果</returns>
        private byte CalcBcc(byte[] bytes)
        {
            byte bcc = 0x00;

            for (int i = 0; i < bytes.Length; i++)
            {
                bcc ^= bytes[i];
            }

            return bcc;
        }

    }