using System;

namespace Pixstock.Service.Infra.Model
{
    /// <summary>
    /// ラベル関連付与のトレースログデータ
    /// </summary>
    public class TraceRecordLogData
    {
        public LabelCauseType CauseType { get; set; }

        /// <summary>
        /// Causeの詳細がある場合は、任意のテキストを記述する
        /// </summary>
        /// <returns></returns>
        public string CauseDetailText { get; set; }

        public DateTime TraceDate { get; set; }
    }
}