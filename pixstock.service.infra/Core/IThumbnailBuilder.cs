namespace Pixstock.Service.Infra.Core
{
    public interface IThumbnailBuilder
    {
        /// <summary>
        /// サムネイル作成
        /// </summary>
        /// <param name="thumbnailhash">既存のサムネイルを、baseImageFilePathで生成しなおしたい場合、
        /// 既存のサムネイル情報を示すキーを指定します。それ以外は、NULLを指定します。</param>
        /// <param name="baseImageFilePath">サムネイル生成元の画像ファイルパス</param>
        /// <returns></returns>
        string BuildThumbnail(string thumbnailhash, string baseImageFilePath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thumbnailhash"></param>
        /// <returns></returns>
        bool RemoveThumbnail(string thumbnailhash);
    }
}