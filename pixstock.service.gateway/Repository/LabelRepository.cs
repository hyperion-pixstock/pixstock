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
    public class LabelRepository : PixstockAppRepositoryBase<Label, ILabel>, ILabelRepository
    {
        public LabelRepository(IAppDbContext context)
            : base((DbContext)context, "Label")
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
        public static ILabel Load(IAppDbContext context, long id)
        {
            var repo = new LabelRepository(context);
            return repo.Load(id);
        }

        /// <summary>
        /// エンティティの読み込み
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ILabel Load(long id)
        {
            var set = _dbset
                .Include(prop => prop.Categories)
                    .ThenInclude(category => category.Category)
                .Include(prop => prop.Contents)
                    .ThenInclude(content => content.Content);
            var entity = set.Where(x => x.Id == id).FirstOrDefault();
            return entity;
        }

        public ILabel LoadByName(string name)
        {
            var set = _dbset
                .Include(prop => prop.Categories)
                    .ThenInclude(category => category.Category)
                .Include(prop => prop.Contents)
                    .ThenInclude(content => content.Content);
            var entity = set.Where(x => x.Name == name).FirstOrDefault();
            return entity;
        }

        public ILabel LoadByName(string name, string ownerType)
        {
            var set = _dbset
                .Include(prop => prop.Categories)
                    .ThenInclude(category => category.Category)
                .Include(prop => prop.Contents)
                    .ThenInclude(content => content.Content);
            var entity = set.Where(x => x.Name == name /*&& x.OwnerType == ownerType*/).FirstOrDefault();
            return entity;
        }

        public ILabel New()
        {
            var entity = new Label();
            return this.Add(entity);
        }

        IEnumerable<ILabel> ILabelRepository.GetAll()
        {
            return base.GetAll().Cast<ILabel>();
        }
    }
}