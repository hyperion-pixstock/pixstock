using System.ComponentModel.DataAnnotations.Schema;

namespace Pixstock.Service.Model
{
    [Table("svp_P_Label2Content")]
    public class Label2Content
    {
        public long LabelId { get; set; }

        public Label Label { get; set; }

        public long ContentId { get; set; }

        public Content Content { get; set; }
    }
}