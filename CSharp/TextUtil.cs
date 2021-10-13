    /// <summary>
    /// TextのUtil
    /// </summary>
    public class Text
    {
        /// <summary>
        /// SJISのエンコーディング
        /// </summary>
        private static readonly Encoding SjisEnc = Encoding.GetEncoding("Shift_JIS");

        /// <summary>
        /// 文章から特定文字を置換する　
        /// replaceWordの配列数と、replaceTextの配列数が一致しないと置換を行わない
        /// </summary>
        /// <param name="text">置換する元の文章</param>
        /// <param name="replaceWord">置換元の文字</param>
        /// <param name="replaceText">置換文字</param>
        /// <returns>置換後の文字</returns>
        public static string ReplaceText(string text, string[] replaceWord, string[] replaceText)
        {
            //置換元と置換文字の数が一致しなければ、何もしない
            if (replaceWord.Length != replaceText.Length)
            {
                return text;
            }

            string replacedText = text;

            int len = replaceText.Length;
            for (int i = 0; i < len; i++)
            {
                replacedText = replacedText.Replace(replaceWord[i], replaceText[i]);
            }
            return replacedText;
        }

        /// <summary>
        /// UTF-8 正規形C へ変換する
        /// か゛->が と正規化され、同一も文字として変換される
        /// </summary>
        /// <param name="text">変換ターゲットの文字列</param>
        /// <returns>変換後文字列</returns>
        public static string ChangeNormalize(string text)
        {
            if (text == null)
            {
                return null;
            }

            return text.Normalize(System.Text.NormalizationForm.FormC);
        }

        /// <summary>
        /// 規定文字数以内に収めた文字列を取得する
        /// 規定文字列以上は末尾に「...」を追加する
        /// </summary>
        /// <param name="utf8">入力文字列</param>
        /// <param name="maxByteLength">最大文字列</param>
        /// <returns>規定文字数以内に収めた文字列</returns>
        public static string GetTrimSjisString(string utf8, int maxByteLength)
        {
            //SJISで表現できない文字を変換する
            string result = ChangeNoSjisString(utf8);

            //MaxLength以内かチェックする
            if (Util.Text.IsSJISCheckLength(result, maxByteLength) == true)
            {
                return result;
            }

            // ...の1byteを稼ぎたい.文字数貧乏...
            if (2 <= maxByteLength)
            {
                //規定文字数に収まるようにトリムする
                result = Util.Text.GetSJISTrimMaxLength(result, maxByteLength - 2);
                //点々を追加
                // result += "...";
                result += "…";
            }

            return result;
        }

        /// <summary>
        /// SJISで表現できない文字をを変換する
        /// </summary>
        /// <param name="utf8">テキスト(utf-8)</param>
        /// <returns></returns>
        public static string ChangeNoSjisString(string utf8)
        {
            return ChangeNoSjisString(utf8, null, true);
        }

        /// <summary>
        /// SJISで表現できない文字をを変換する
        /// </summary>
        /// <param name="utf8">テキスト(utf-8)</param>
        /// <param name="replace"></param>
        /// <param name="trimStartSpace"></param>
        /// <returns></returns>
        private static string ChangeNoSjisString(string utf8, string replace, bool trimStartSpace)
        {
            if (string.IsNullOrEmpty(utf8))
            {
                return string.Empty;
            }

            //制御コードの改行をカットするかの判断
            bool isCutCR = false;
            string changeNoSjisStringCutLR = ConfigurationManager.AppSettings["ChangeNoSjisStringCutLR"];
            if (changeNoSjisStringCutLR == "true")
            {
                isCutCR = true;
            }

            //制御コードを除去する
            utf8 = TrimControlWord(utf8, isCutCR);

            //IBM-UNICODEを置換しておく SJIS変換不可能
            //http://d.hatena.ne.jp/mokkouyou2001/20081219/1229689787
            utf8 = utf8.Replace("\u2014", "\u2015"); //―(全角ダッシュ)*
            utf8 = utf8.Replace("\u301C", "\uFF5E"); //～
            utf8 = utf8.Replace("\u2016", "\u2225"); //∥
            utf8 = utf8.Replace("\u2212", "\uFF0D"); //－(全角マイナス)
            utf8 = utf8.Replace("\u00A2", "\uFFE0"); //￠
            utf8 = utf8.Replace("\u00A3", "\uFFE1"); //￡
            utf8 = utf8.Replace("\u00AC", "\uFFE2"); //￢
            utf8 = utf8.Replace("\uFF65", "\u30FB"); //・（なかてん）半角を全角に
            utf8 = utf8.Replace("\u2022", "\u30FB"); //・（なかてん）半角を全角に
            utf8 = utf8.Replace("\u00B7", "\u30FB"); //・（なかてん）半角を全角に

            //クォーテーションを大文字に変更する
            //文字に'があると、SQLが壊れる
            //そのため、ReserveEXが、ReceptHeadなどに書けずに、予約が振り返らない事象が発生する
            //Web側でも「'」を「’」に変換している
            //しかし、一度DBにはいってしまったクォーテーションは、大文字にできない
            //そのため、ここで変換する
            //変換前  [着有線][迎え先]’いえい’[送り先]大分市大道町５丁目９(’ああ’)[装備]新生児シート[備考]2016'10'11なし'
            //変換後  [着有線][迎え先]’いえい’[送り先]大分市大道町５丁目９(’ああ’)[装備]新生児シート[備考]2016’10’11なし’
            utf8 = utf8.Replace("'", "’"); //１バイトクォーテーションを2バイトクォーテーションに変更

            //SJISに変換し、変換できないものを?にする
            replace = string.IsNullOrEmpty(replace) ? "?" : replace; // 置き換える文字列のデフォルトは"?"とする
            var encoderFallback = new EncoderReplacementFallback(replace);

            //var ms932 = Encoding.GetEncoding("Shift_JIS", encoderFallback, DecoderFallback.ReplacementFallback);
            var ms932 = Encoding.GetEncoding(932, encoderFallback, DecoderFallback.ReplacementFallback);

            var bytes = ms932.GetBytes(utf8);

            string result = ms932.GetString(bytes);

            //if (!string.IsNullOrEmpty(replace))
            //{
            //    result = result.Replace("?", replace);
            //}

            if (trimStartSpace)
            {
                result = result.Trim();
            }

            return result;
        }

        /// <summary>
        /// 制御コードを除去する
        /// </summary>
        /// <param name="utf8">除去対象</param>
        /// <param name="isCutLr">\l\rを除去するかどうか</param>
        /// <returns></returns>
        private static string TrimControlWord(string utf8, bool isCutLr)
        {
            //Tab -> space に変換
            utf8 = utf8.Replace("\x09", "\x20");

            //DEL -> ? に変換
            utf8 = utf8.Replace("\x7f", "\x3f");

            //制御コード除去 ３回実行するのは、\x0aと\x0dを避けるため
            Regex regex1 = new Regex(@"[\x00-\x09]");
            utf8 = regex1.Replace(utf8, "\x3f");

            Regex regex2 = new Regex(@"[\x0b-\x0c]");
            utf8 = regex2.Replace(utf8, "\x3f");

            Regex regex3 = new Regex(@"[\x0e-\x1f]");
            utf8 = regex3.Replace(utf8, "\x3f");

            //改行コードをカット
            if (isCutLr == true)
            {
                utf8 = utf8.Replace("\x0a", "");
                utf8 = utf8.Replace("\x0d", "");
            }

            return utf8;
        }

        /// <summary>
        /// メッセージをSJISで最大文字数以下にトリムし取得する
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="maxLength">最大文字バイト数</param>
        /// <returns>最大文字数以下にトリムしたメッセージ</returns>
        public static string GetSJISTrimMaxLength(string message, int maxLength)
        {
            bool isInLength = IsSJISCheckLength(message, maxLength);
            if (isInLength == true)
            {
                //文字が最大文字バイト数以下なので何もせず返却
                return message;
            }

            StringBuilder stringBuilder = new StringBuilder("", maxLength);
            int totalByteLength = 0;

            int messageLength = message.Length;
            for (int i = 0; i < messageLength; i++)
            {
                string word = message.Substring(i, 1);//１文字ずつ取得
                byte[] bytes = SjisEnc.GetBytes(word);

                //現在の文字数加算
                totalByteLength += bytes.Length;

                //最大文字数を超えれば終了
                if (maxLength < totalByteLength)
                {
                    return stringBuilder.ToString();
                }

                //１文字追加
                stringBuilder.Append(word);
            }
            return "";
        }

        /// <summary>
        /// Shift_JISにした時の長さをチェックする<br/>
        /// 指定の長さ以内であればtrue, オーバーしていればfalse
        /// </summary>
        /// <param name="message">チェックする文字列</param>
        /// <param name="maxLength">最大長</param>
        /// <returns>指定の長さ以内であればtrue, オーバーしていればfalse</returns>
        public static bool IsSJISCheckLength(string message, int maxLength)
        {
            if (string.IsNullOrEmpty(message))
            {
                return true;
            }

            byte[] b = SjisEnc.GetBytes(message);

            return b.Length <= maxLength;
        }

        /// <summary>
        /// Shift_JISにした時の長さをチェックする<br/>
        /// 指定の長さ以内であればtrue, オーバーしていればfalse
        /// </summary>
        /// <param name="message">チェックする文字列</param>
        /// <param name="minLength">最小長さ</param>
        /// <param name="maxLength">最大長さ</param>
        /// <returns>指定の長さ以内であればtrue, オーバーしていればfalse</returns>
        public static bool IsSJISCheckLength(string message, int minLength, int maxLength)
        {
            if (string.IsNullOrEmpty(message))
            {
                return true;
            }

            byte[] b = SjisEnc.GetBytes(message);

            //return minLength <= b.Length && b.Length <= maxLength;

            if (b.Length < minLength)
            {
                // 短すぎる
                return false;
            }
            else if (b.Length > maxLength)
            {
                // 長すぎる
                return false;
            }
            return true;
        }
    }
