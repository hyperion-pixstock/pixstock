using System;
using System.Linq;

namespace Katalib.Nc.Standard.String
{
    public static class RandomAlphameric
    {
        static Random random = new Random();

        /// <summary>
        /// 英数字で構成されたランダムな文字列を取得します
        /// </summary>
        /// <param name="num">構成文字数</param>
        /// <returns></returns>
        public static string RandomAlphanumeric(int num)
        {
            if (num < 1) throw new ArgumentException("num");

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new string(
                Enumerable.Repeat(chars, num)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }

        /// <summary>
        /// 英字で構成されたランダムな文字列を取得します
        /// </summary>
        /// <param name="num">構成文字数</param>
        /// <returns></returns>
        public static string RandomAlpha(int num)
        {
            if (num < 1) throw new ArgumentException("num");

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var result = new string(
                Enumerable.Repeat(chars, num)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }
    }
}