using System.Linq;
using Hyperion.Pf.Entity.Repository;
using Microsoft.EntityFrameworkCore;
using Pixstock.Service.Infra;
using Pixstock.Service.Infra.Model;
using Pixstock.Service.Infra.Repository;
using Pixstock.Service.Model;

namespace Pixstock.Service.Gateway.Repository
{
    public class WorkspaceRepository : PixstockAppRepositoryBase<Workspace, IWorkspace>, IWorkspaceRepository
    {
        public WorkspaceRepository(IAppDbContext context) : base((DbContext)context, "Workspace")
        {
        }

        /// <summary>
        /// Workpaceの読み込み
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IWorkspace Load(long id)
        {
            return _dbset.Where(x => x.Id == id).FirstOrDefault();
        }

        public IWorkspace New()
        {
            var entity = new Workspace();
            return this.Add(entity);
        }
    }
}