using Pixstock.Common.Model.Attributes;

namespace Pixstock.Common.Model
{
    public enum ThumbnailType
    {
        NON_SETTING = 0,

        /// <summary>
        /// ListIcon用サムネイル
        /// </summary>
        [ThumbnailInfo("ListIcon", Width = 250)]
        LISTICON = 1,
    }
}