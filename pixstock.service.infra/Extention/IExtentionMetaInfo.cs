using System;
using System.Collections.Generic;

namespace Pixstock.Service.Infra.Extention
{
    /// <summary>
    /// 拡張機能のエントリポイント
    /// </summary>
    public interface IExtentionMetaInfo
    {
        /// <summary>
        /// 拡張機能名を取得します
        /// </summary>
        /// <returns></returns>
        string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<Type> Cutpoints();
    }
}