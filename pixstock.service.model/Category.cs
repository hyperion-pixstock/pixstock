using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pixstock.Service.Infra.Model;
using Newtonsoft.Json;
using Hyperion.Pf.Entity;
using NLog;
using Microsoft.EntityFrameworkCore;

namespace Pixstock.Service.Model
{
    [Table("svp_Category")]
    public class Category : Infra.Model.ICategory, IAuditableEntity, ISaveEntity
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Category()
        {
            this.Labels = new List<Label2Category>();
        }

        [Key]
        public long Id { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public Category ParentCategory { get; set; }

        [JsonIgnore]
        public List<Content> Contents { get; set; }

        [JsonIgnore]
        public List<Label2Category> Labels { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }
        public int ReadableCount { get; set; }
        public DateTime? LastReadDate { get; set; }
        public DateTime? ReadableDate { get; set; }
        public bool ReadableFlag { get; set; }
        public string ArtworkThumbnailKey { get; set; }
        public int StarRating { get; set; }
        public bool AlbumFlag { get; set; }
        public long? NextDisplayContentId { get; set; }
        public string BookmarkValue { get; set; }

        public void SetParentCategory(ICategory category) => this.ParentCategory = (Category)category;

        public ICategory GetParentCategory() => this.ParentCategory;

        public List<IContent> GetContentList() => this.Contents.Select(p => (IContent)p).ToList();

        public List<ILabel> GetLabelList() => this.Labels.OrderBy(prop => prop.Order).Select(prop => (ILabel)prop.Label).ToList();

        public void AddLabelRelation(ILabel label, LabelCauseType cause, string causeDetail = "")
        {
            // 注意: labelは、現在のCategoryと同じDBコンテキストを設定してください。

            // 追加しようとしているラベル(label)が、このカテゴリにすでに設定済みの場合は、
            // 新たに追加せずに、トレースログに追記する。
            var r = from u in this.Labels
                    where u.LabelId == label.Id
                    select u;
            var existsLabel2Category = r.FirstOrDefault();
            if (existsLabel2Category == null)
            {
                var e2 = new Label2Category
                {
                    Category = this,
                    Label = (Label)label,
                    Order = 100, // デフォルト値を設定する
                    LastCause = cause
                };
                e2.AddTraceRecord(cause, causeDetail);
                this.Labels.Add(e2);
            }
            else
            {
                // labelがすでに登録済みの場合は、TraceRecordを追加してデータを更新する
                existsLabel2Category.AddTraceRecord(cause, causeDetail);
            }
        }

        public void OnSave(DbContext context)
        {
            // _logger.Debug("Categoryの保存 = {}",this.Id);
        }
    }
}
