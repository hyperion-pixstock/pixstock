using System.ComponentModel.DataAnnotations.Schema;
using Pixstock.Service.Infra.Model;

namespace Pixstock.Service.Model
{
    [Table("svp_AppMetaInfo")]
    public class AppMetaInfo : IAppMetaInfo
    {
        public long Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}