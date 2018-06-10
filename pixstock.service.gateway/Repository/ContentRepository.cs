using System.Linq;
using Hyperion.Pf.Entity.Repository;
using Microsoft.EntityFrameworkCore;
using Pixstock.Service.Infra;
using Pixstock.Service.Infra.Model;
using Pixstock.Service.Infra.Repository;
using Pixstock.Service.Model;

namespace Pixstock.Service.Gateway.Repository
{
    public class ContentRepository : PixstockAppRepositoryBase<Content, IContent>, IContentRepository
    {
        public ContentRepository(IAppDbContext context)
            : base((DbContext)context, "Content")
        {
        }

        /// <summary>
        /// エンティティの読み込み(静的メソッド)
        /// </summary>
        /// <remarks>
        /// エンティティの読み込みをワンライナーで記述できます。
        /// </remarks>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IContent Load(IAppDbContext context, long id)
        {
            var repo = new ContentRepository(context);
            return repo.Load(id);
        }

        /// <summary>
        /// Contentの読み込み
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IContent Load(long id)
        {
            var set = _dbset
                .Include(prop => prop.Category)
                .Include(prop => prop.FileMappingInfo);
            return set.Where(x => x.Id == id).FirstOrDefault();
        }

        public IContent Load(IFileMappingInfo fileMappingInfo)
        {
            var set = _dbset
                .Include(prop => prop.Category)
                .Include(prop => prop.FileMappingInfo);
            return set.Where(x => x.FileMappingInfo.Id == fileMappingInfo.Id).FirstOrDefault();
        }

        public IContent New()
        {
            var entity = new Content();
            return this.Add(entity);
        }
    }
}