namespace Pixstock.Common.Model
{
    public interface IThumbnail
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        long Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string ThumbnailKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string MimeType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ThumbnailType ThumbnailType { get; set; }
    }
}