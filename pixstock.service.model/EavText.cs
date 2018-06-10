using System;
using System.ComponentModel.DataAnnotations.Schema;
using Pixstock.Service.Infra.Model.Eav;
using Hyperion.Pf.Entity;

namespace Pixstock.Service.Model
{

    [Table("svp_Eav_Text")]
    public class EavText : IEavText
    {
        public string EntityTypeName { get; set; }

        public string CategoryName { get; set; }

        public long EntityId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}