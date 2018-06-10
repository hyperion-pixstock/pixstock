using System;
using Hyperion.Pf.Entity;

namespace Pixstock.Common.Model
{
    public interface IFileMappingInfo : IEntity<long>
    {
        Boolean LostFileFlag { get; set; }

        string MappingFilePath { get; set; }

        string Mimetype { get; set; }

        Boolean RecycleBoxFlag { get; set; }

        string AclHash { get; set; }

        string CreatedBy { get; set; }

        DateTime CreatedDate { get; set; }

        string UpdatedBy { get; set; }

        DateTime UpdatedDate { get; set; }
    }
}