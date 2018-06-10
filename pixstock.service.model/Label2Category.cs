using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Pixstock.Service.Infra.Model;

namespace Pixstock.Service.Model
{
    [Table("svp_P_Label2Category")]
    public class Label2Category
    {
        public long LabelId { get; set; }

        public Label Label { get; set; }

        public long CategoryId { get; set; }

        public Category Category { get; set; }

        public int Order { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// EF2.1からは、文字列化できる。
        /// </remarks>
        /// <returns></returns>
        public LabelCauseType LastCause { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// EF2.1からは、自動的にシリアライズ・デシリアライズできる。
        /// </remarks>
        /// <returns></returns>
        public string TraceRecord { get; set; }

        public void AddTraceRecord(LabelCauseType causeType, string detail = "")
        {
            var list = GetTraceRecordList();
            list.Add(new TraceRecordLogData
            {
                CauseType = causeType,
                CauseDetailText = detail,
                TraceDate = DateTime.Now
            });

            this.TraceRecord = JsonConvert.SerializeObject(list);
        }

        public List<TraceRecordLogData> GetTraceRecordList()
        {
            if (string.IsNullOrEmpty(this.TraceRecord))
            {
                return new List<TraceRecordLogData>();
            }
            else
            {
                return JsonConvert.DeserializeObject<List<TraceRecordLogData>>(TraceRecord);
            }
        }
    }
}