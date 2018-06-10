using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hyperion.Pf.Entity;
using Newtonsoft.Json;
using Pixstock.Service.Infra.Model;

namespace Pixstock.Service.Model
{
    [Table("svp_Label")]
    public class Label : Infra.Model.ILabel, IAuditableEntity
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }

        public string Comment { get; set; }
        
        public string MetaType { get; set; }

        [JsonIgnore]
        public List<Label2Content> Contents { get; set; }

        [JsonIgnore]
        public List<Label2Category> Categories { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}