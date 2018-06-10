using System.Linq;
using Hyperion.Pf.Entity.Repository;
using Microsoft.EntityFrameworkCore;
using Pixstock.Service.Infra;
using Pixstock.Service.Infra.Model;
using Pixstock.Service.Infra.Repository;
using Pixstock.Service.Model;

namespace Pixstock.Service.Gateway.Repository
{
    public class ThumbnailRepository : GenericRepository<Thumbnail>, IThumbnailRepository
    {
        public ThumbnailRepository(IThumbnailDbContext context) : base((DbContext)context)
        {
        }

        public void Delete(IThumbnail entity)
        {
            this.Delete((Thumbnail)entity);
        }

        public IQueryable<IThumbnail> FindByKey(string key)
        {
            return _dbset.Where(x => x.ThumbnailKey == key);
        }

        public IThumbnail Load(long id)
        {
            return _dbset.Where(x => x.Id == id).FirstOrDefault();
        }

        public IThumbnail New()
        {
            var entity = new Thumbnail();
            return this.Add(entity);
        }
    }
}