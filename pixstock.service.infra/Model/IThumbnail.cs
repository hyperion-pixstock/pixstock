namespace Pixstock.Service.Infra.Model
{
    public interface IThumbnail : Pixstock.Common.Model.IThumbnail
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        byte[] BitmapBytes { get; set; }
    }
}