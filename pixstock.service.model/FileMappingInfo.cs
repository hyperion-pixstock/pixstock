using System;
using System.ComponentModel.DataAnnotations.Schema;
using Pixstock.Service.Infra.Model;
using Hyperion.Pf.Entity;
using Newtonsoft.Json;

namespace Pixstock.Service.Model
{
    [Table("svp_FileMappingInfo")]
    public class FileMappingInfo : Infra.Model.IFileMappingInfo, IAuditableEntity
    {
        public long Id { get; set; }

        public bool LostFileFlag { get; set; }

        public string MappingFilePath { get; set; }

        public string Mimetype { get; set; }

        public bool RecycleBoxFlag { get; set; }

        public string AclHash { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        [JsonIgnore]
        public Workspace Workspace { get; set; }

        public IWorkspace GetWorkspace() => this.Workspace;

        public void SetWorkspace(IWorkspace workspace) => this.Workspace = (Workspace)workspace;
    }
}