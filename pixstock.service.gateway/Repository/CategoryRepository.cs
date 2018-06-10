using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Hyperion.Pf.Entity.Repository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pixstock.Service.Infra;
using Pixstock.Service.Infra.Model;
using Pixstock.Service.Infra.Model.Eav;
using Pixstock.Service.Infra.Repository;
using Pixstock.Service.Model;

namespace Pixstock.Service.Gateway.Repository {
    public class CategoryRepository : PixstockAppRepositoryBase<Category, ICategory>, ICategoryRepository {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context"></param>
        public CategoryRepository (IAppDbContext context) : base ((DbContext) context, "Category") { }

        /// <summary>
        /// エンティティの読み込み(静的メソッド)
        /// </summary>
        /// <remarks>
        /// エンティティの読み込みをワンライナーで記述できます。
        /// </remarks>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ICategory Load (IAppDbContext context, long id) {
            var repo = new CategoryRepository (context);
            return repo.Load (id);
        }

        public IQueryable<ICategory> FindCategory (ILabel label) {
            var set = _dbset
                .Include (prop => prop.ParentCategory)
                .Include (prop => prop.Labels)
                .Include (prop => prop.Contents);
            return set.Where (x => x.Labels.Where (x2 => x2.LabelId == label.Id).Count () > 0);
        }

        public IQueryable<ICategory> FindCategory (List<ILabel> label) {
            var set = _dbset
                .Include (prop => prop.ParentCategory)
                .Include (prop => prop.Labels)
                .Include (prop => prop.Contents);
            IQueryable<Category> a = set;
            foreach (var prop in label) {
                a = a.Where (x => x.Labels.Where (x2 => x2.LabelId == prop.Id).Count () > 0);
            }
            return a;
        }

        public IQueryable<ICategory> FindCategoryOr (List<ILabel> label) {
            Boolean isFirst = false;
            StringBuilder sb = new StringBuilder ();
            foreach (var prop in label) {
                if (isFirst) sb.Append (" OR ");
                sb.Append ("svp_P_Label2Category.LabelId = " + prop.Id);
                isFirst = true;
            }

            var set = _dbset.FromSql ($"SELECT * FROM svp_Category WHERE Id IN (SELECT CategoryId FROM svp_P_Label2Category WHERE {sb.ToString()})")
                .Include (prop => prop.ParentCategory)
                .Include (prop => prop.Labels)
                .Include (prop => prop.Contents);
            return set;
        }

        public IQueryable<ICategory> FindChildren (long parentCategoryId) {
            var set = _dbset
                .Include (prop => prop.ParentCategory)
                .Include (prop => prop.Labels)
                .Include (prop => prop.Contents);
            return set.Where (x => x.ParentCategory.Id == parentCategoryId);
        }

        public IQueryable<ICategory> FindChildren (ICategory parentCategory) {
            var set = _dbset
                .Include (prop => prop.ParentCategory)
                .Include (prop => prop.Labels)
                .Include (prop => prop.Contents);
            return set.Where (x => x.ParentCategory.Id == parentCategory.Id);
        }

        /// <summary>
        /// Categoryの読み込み
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ICategory Load (long id) {
            var set = _dbset
                .Include (prop => prop.ParentCategory)
                .Include (prop => prop.Labels)
                .ThenInclude (label => label.Label)
                .Include (prop => prop.Contents);
            return set.Where (x => x.Id == id).FirstOrDefault ();
        }

        public ICategory LoadByName (string categoryName) {
            var set = _dbset
                .Include (prop => prop.ParentCategory)
                .Include (prop => prop.Labels)
                .ThenInclude (label => label.Label)
                .Include (prop => prop.Contents);
            var entity = set.Where (x => x.Name == categoryName).FirstOrDefault ();
            return entity;
        }

        public ICategory LoadByName (string categoryName, ICategory parentCategory) {
            var set = _dbset
                .Include (prop => prop.ParentCategory)
                .Include (prop => prop.Labels)
                .ThenInclude (label => label.Label)
                .Include (prop => prop.Contents);
            var entity = set.Where (x => x.Name == categoryName && x.ParentCategory.Id == parentCategory.Id).FirstOrDefault ();
            return entity;
        }

        public ICategory LoadRootCategory () {
            return Load (1L);
        }

        public ICategory New () {
            var entity = new Category ();
            return this.Add (entity);
        }

        public void UpdatePopulateFromJson (long id, string json) {
            var category = Load (id);
            JsonConvert.PopulateObject (json, category);
        }
    }
}

public static class ExpressionCombiner {
    public static Expression<Func<T, bool>> OrTheseFiltersTogether<T> (this IEnumerable<Expression<Func<T, bool>> > filters) {
        Expression<Func<T, bool>> firstFilter = filters.FirstOrDefault ();
        if (firstFilter == null) {
            Expression<Func<T, bool>> alwaysTrue = x => true;
            return alwaysTrue;
        }

        var body = firstFilter.Body;
        var param = firstFilter.Parameters.ToArray ();
        foreach (var nextFilter in filters.Skip (1)) {
            var nextBody = Expression.Invoke (nextFilter, param);
            body = Expression.OrElse (body, nextBody);
        }
        Expression<Func<T, bool>> result = Expression.Lambda<Func<T, bool>> (body, param);
        return result;
    }

    public static Expression<Func<T, bool>> OrElse<T> (
        params Expression<Func<T, bool>>[] expressions) {
        return OrElse (expressions.AsEnumerable ());
    }

    public static Expression<Func<T, bool>> OrElse<T> (
        this IEnumerable<Expression<Func<T, bool>> > expressions) {
        if (!expressions.Any ()) {
            throw new ArgumentException ($"parameter [{nameof(expressions)}] is empty.", nameof (expressions));
        }

        var lambda = expressions.First ();
        var body = lambda.Body;
        var parameter = lambda.Parameters[0];

        foreach (var expression in expressions.Skip (1)) {
            var visitor = new ParameterReplaceVisitor (expression.Parameters[0], parameter);
            body = Expression.OrElse (body, visitor.Visit (expression.Body));
        }

        lambda = Expression.Lambda<Func<T, bool>> (body, parameter);
        return lambda;
    }
}

internal class ParameterReplaceVisitor:
    ExpressionVisitor {
        private readonly ParameterExpression _originalParameter;
        private readonly ParameterExpression _newParameter;

        public ParameterReplaceVisitor (
            ParameterExpression originalParameter,
            ParameterExpression newParameter) {
            this._originalParameter = originalParameter;
            this._newParameter = newParameter;
        }

        protected override Expression VisitParameter (ParameterExpression node) {
            if (node == this._originalParameter) {
                return this._newParameter;
            }

            return base.VisitParameter (node);
        }
    }