using System;

namespace Katalib.Nc.Standard
{
    public class StringUtil
    {
        /// <summary>
		/// 指定した長さより長い部分を切り捨て、「...」を末尾に追加した文字列を取得します。
		/// </summary>
		/// <param name="s">処理を行いたい文字列</param>
		/// <param name="maximumLength">切り捨て文字列数</param>
		/// <returns></returns>
		public static string Truncate(string s, int maximumLength)
        {
            return Truncate(s, maximumLength, "...");
        }


        /// <summary>
        /// 指定した長さより長い部分を切り捨て、任意の文字を末尾に追加した文字列を取得します。
        /// </summary>
        /// <param name="s">処理を行いたい文字列</param>
        /// <param name="maximumLength">切り捨て文字列数</param>
        /// <param name="suffix">末尾に追加する文字</param>
        /// <returns></returns>
        public static string Truncate(string s, int maximumLength, string suffix)
        {
            if (suffix == null)
                throw new ArgumentNullException("suffix");

            if (maximumLength <= 0)
                throw new ArgumentException("Maximum length must be greater than zero.", "maximumLength");

            int subStringLength = maximumLength - suffix.Length;

            if (subStringLength <= 0)
                throw new ArgumentException("Length of suffix string is greater or equal to maximumLength");

            if (s != null && s.Length > maximumLength)
            {
                string truncatedString = s.Substring(0, subStringLength);
                // incase the last character is a space
                truncatedString = truncatedString.Trim();
                truncatedString += suffix;

                return truncatedString;
            }
            else
            {
                return s;
            }
        }
    }
}
