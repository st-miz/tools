public class HExDecimal{
        /// <summary>
        /// 62進数定数
        /// </summary>
        private const int Hex62Max = 62;

        /// <summary>
        /// 36進数定数
        /// </summary>
        private const int Hex36Max = 36;

        /// <summary>
        /// 62進数を扱ううえで見間違いやすい文字
        /// </summary>
        private static readonly char[] DifficultChar = new char[] { 'I', 'P', 'O', 'S' };

        /// <summary>
        /// 36進数から10進数へ変換する（基本的にはアルファベットは大文字で変換するが間違いやすい文字のみ小文字とする）
        /// </summary>
        /// <remarks>
        /// 間違いやすい文字（例えば、"I", "1"は"数字のイチ"なのか"英語のアイ"なのかわかりづらい）の場合は<br/>
        /// 明示的に小文字として扱う
        /// </remarks>
        /// <param name="hex36">36進数</param>
        /// <returns>10進数</returns>
        public static long To36EasyDecimal(string hex36)
        {
            int len = hex36.Length;
            long result = 0;
            long power = 0;
            // 1桁目から計算するためループは逆からたどる
            for (int i = len - 1; i >= 0; i--)
            {
                long a = 0;
                char c0 = hex36[i];
                if ('0' <= c0 && c0 <= '9')
                {
                    a = c0 - '0';
                }
                else if ('a' <= c0 && c0 <= 'z')
                {
                    // 間違いやすい文字であれば小文字変換しているので大文字へ手直ししたうえで計算を行う
                    if (DifficultChar.Any(p => Char.ToUpper(c0) == p))
                    {
                        a = c0 - 'a' + 10; // offset a(97) - A(65)
                    }
                    else
                    {
                        throw new ArgumentException("36進数に変換できない値です." + c0);
                    }
                }
                else if ('A' <= c0 && c0 <= 'Z')
                {
                    a = c0 - 'A' + 10;
                }
                else
                {
                    throw new ArgumentException("36進数に変換できない値です." + c0);
                }
                // (36 ^ n) * number
                result += (int)Math.Pow(Hex36Max, power) * a;
                power++;
            }
            return result;
        }
        
        /// <summary>
        /// 10進数から36進数へ変換する（基本的にはアルファベットは大文字で変換するが間違いやすい文字のみ小文字とする）
        /// </summary>
        /// <remarks>
        /// 間違いやすい文字（例えば、"I", "1"は"数字のイチ"なのか"英語のアイ"なのかわかりづらい）の場合は<br/>
        /// 明示的に小文字として扱う
        /// </remarks>
        /// <param name="hex10">10進数</param>
        /// <returns>62進数</returns>
        static public string To36EasyHex(long hex10)
        {
            StringBuilder sb = new StringBuilder();
            long i = 1;
            bool hasNext = true;
            while (hasNext)
            {
                long more = hex10 % Hex36Max;
                hex10 = hex10 / Hex36Max;
                if (hex10 <= 0)
                {
                    hasNext = false;
                }
                // ASCII Code C# Offset
                // 65 -> A(LARGE A)
                // 97 -> a(small a)

                if (more < 10)
                {
                    // そのまま数字を追加
                    if (!hasNext && more == 0)
                    {
                        // 最大桁に"0"は付加しない
                        // ignore
                    }
                    else
                    {
                        sb.Append(more);
                    }
                }
                else if (more < 63)
                {
                    char c = (char)(more + 55); // offset 65 - 10
                    if (DifficultChar.Any(p => p == c))
                    {
                        // 間違いやすい文字は小文字にする
                        c = Char.ToLower(c);
                    }
                    sb.Append(c);
                }
                else
                {
                    throw new ArgumentException("36進数に変換できない値です");
                }
                i++;
            }
            string s = new String(sb.ToString().Reverse().ToArray());
            return s;
        }
        
}