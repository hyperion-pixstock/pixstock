using System;

namespace Pixstock.Common.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ThumbnailInfoAttribute : System.Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public String ThumbnailDirectoryName;

        /// <summary>
        /// サムネイルのサイズ(横幅)
        /// </summary>
        /// <remarks>
        /// この値が「0」の場合、最大高さがHeightになるようにアスペクト比を維持しながら縮小する。
        /// </remarks>
        public int Width;

        /// <summary>
        /// サムネイルのサイズ(高さ)
        /// </summary>
        /// <remarks>
        /// この値が「0」の場合、最大横幅がWidthになるようにアスペクト比を維持しながら縮小する。
        /// </remarks>
        public int Height;

        public ThumbnailInfoAttribute(String str)
        {
            this.ThumbnailDirectoryName = str;
        }
    }

}