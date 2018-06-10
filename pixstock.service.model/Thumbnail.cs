using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pixstock.Common.Model;
using Pixstock.Service.Infra.Model;

namespace Pixstock.Service.Model
{
    [Table("svp_Thumbnail")]
    public class Thumbnail : Infra.Model.IThumbnail
    {
        [Key]
        public long Id { get; set; }

        public string ThumbnailKey { get; set; }

        public byte[] BitmapBytes { get; set; }

        public string MimeType { get; set; }

        public ThumbnailType ThumbnailType { get; set; }
    }
}