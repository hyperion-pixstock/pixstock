using System;
using System.Collections.Generic;

namespace Pixstock.Base.AppIf.Sdk
{
    /// <summary>
    /// アプリケーションAPIのレスポンスクラス
    /// </summary>
    public class ResponseAapi<T>
    {
        public ResponseAapi()
        {
            this.Rel = new Dictionary<string, string>();
            this.Link = new Dictionary<string, object>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Error { get; set; }

        /// <summary>
        /// サービスから取得する情報を取得、または設定します。
        /// </summary>
        /// <returns></returns>
        public T Value { get; set; }

        /// <summary>
        /// 関連リソースを取得、または設定します。
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> Rel { get; set; }

        /// <summary>
        /// リンクリソースを取得、または設定します。
        /// </summary>
        /// <remarks>
        /// リンクリソースはキーでのみ返します。
        /// </remarks>
        /// <returns></returns>
        public Dictionary<string, object> Link { get; set; }
    }
}
