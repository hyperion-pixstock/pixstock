using System.Collections.Generic;
using System.Linq;
using Hyperion.Pf.Entity.Repository;
using Microsoft.EntityFrameworkCore;
using Pixstock.Service.Infra;
using Pixstock.Service.Infra.Model;
using Pixstock.Service.Infra.Repository;
using Pixstock.Service.Model;

namespace Pixstock.Service.Gateway.Repository
{
    public class FileMappingInfoRepository : PixstockAppRepositoryBase<FileMappingInfo, IFileMappingInfo>, IFileMappingInfoRepository
    {
        public FileMappingInfoRepository(IAppDbContext context)
            : base((DbContext)context, "FileMappingInfo")
        {
        }

        public void Delete(IFileMappingInfo entity)
        {
            base.Delete((FileMappingInfo)entity);
        }

        public IFileMappingInfo Load(long id)
        {
            var set = _dbset
                .Include(prop => prop.Workspace);
            return set.Where(x => x.Id == id).FirstOrDefault();
        }

        public IFileMappingInfo LoadByAclHash(string aclHash)
        {
            return _dbset.Where(x => x.AclHash == aclHash).FirstOrDefault();
        }

        public IFileMappingInfo LoadByPath(string path)
        {
            return _dbset.Where(x => x.MappingFilePath == path).FirstOrDefault();
        }

        public IFileMappingInfo New()
        {
            var entity = new FileMappingInfo();
            return this.Add(entity);
        }

        public IEnumerable<IFileMappingInfo> FindPathWithStart(string startText)
        {
            return this.FindBy(p => p.MappingFilePath.StartsWith(startText));
        }
    }
}