using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Pixstock.Common.Model;

namespace pixstock.apl.app.Models {
    public class Category : ICategory {
        public long Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 付与されているラベル一覧を取得します
        /// </summary>
        /// <remarks>
        /// 外部参照データのため、データが読み込まれていない場合はこのフィールドはnullを返します。
        /// </remarks>
        /// <returns></returns>
        public List<Label> Labels { get; set; }

        /**
         * リンクしているサブカテゴリ情報一覧
         * このプロパティは、フロントエンドへのシリアライズ対象外です。
         */
        [JsonIgnore]
        public List<Category> LinkSubCategoryList { get; set; }

        /**
         * リンクしているコンテント情報一覧
         * このプロパティは、フロントエンドへのシリアライズ対象外です。
         */
        [JsonIgnore]
        public List<Content> LinkContentList { get; set; }

        /**
         * リンクしているサブカテゴリが存在するか示すフラグです。
         */
        public bool HasLinkSubCategoryFlag { get; set; }
        public int ReadableCount { get; set; }
        public DateTime? LastReadDate { get; set; }
        public DateTime? ReadableDate { get; set; }
        public bool ReadableFlag { get; set; }
        public string ArtworkThumbnailKey { get; set; }
        public int StarRating { get; set; }
        public bool AlbumFlag { get; set; }
        public long? NextDisplayContentId { get; set; }
        public string BookmarkValue { get; set; }
    }
}
