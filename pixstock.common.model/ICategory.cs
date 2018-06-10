using System;
using Hyperion.Pf.Entity;

namespace Pixstock.Common.Model
{
    public interface ICategory : IEntity<long>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string Name { get; set; }

        int ReadableCount { get; set; }

        DateTime? LastReadDate { get; set; }

        DateTime? ReadableDate { get; set; }

        bool ReadableFlag { get; set; }

        string ArtworkThumbnailKey { get; set; }

        int StarRating { get; set; }

        bool AlbumFlag { get; set; }

        long? NextDisplayContentId { get; set; }

        string BookmarkValue { get; set; }
    }
}