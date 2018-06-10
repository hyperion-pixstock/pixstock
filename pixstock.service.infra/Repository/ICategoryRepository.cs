using System.Collections.Generic;
using System.Linq;
using Pixstock.Service.Infra.Model;

namespace Pixstock.Service.Infra.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICategoryRepository : IRepositoryBase, IEavTextRepository<ICategory>, IEavBoolRepository<ICategory>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ICategory New();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ICategory Load(long id);

        /// <summary>
        /// カテゴリ名に一致するエントリを検索
        /// </summary>
        /// <remarks>
        /// 制限事項として、このメソッドは同名のカテゴリが複数存在する場合ははじめの１つのみ返す。
        /// </remarks>
        /// <param name="categoryName">カテゴリ名</param>
        /// <returns></returns>
        ICategory LoadByName(string categoryName);

        /// <summary>
        /// カテゴリ名に一致するエントリを検索(親ディレクトリ指定版)
        /// </summary>
        /// <remarks>
        /// 制限事項として、このメソッドは同名のカテゴリが複数存在する場合ははじめの１つのみ返す。
        /// </remarks>
        /// <param name="categoryName">カテゴリ名</param>
        /// <param name="parentCategory">親ディレクトリ</param>
        /// <returns></returns>
        ICategory LoadByName(string categoryName, ICategory parentCategory);

        /// <summary>
        /// 指定した親カテゴリを持つカテゴリリストを取得する
        /// </summary>
        /// <remarks>
        /// 呼び出し元では、戻り値を使って子階層カテゴリを更に絞り込んでください。
        /// </remarks>
        /// <param name="parentCategory"></param>
        /// <returns>子階層カテゴリのクエリ</returns>
        IQueryable<ICategory> FindChildren(ICategory parentCategory);

        /// <summary>
        /// 指定した親カテゴリを持つカテゴリリストを取得する
        /// </summary>
        /// <param name="parentCategoryId">親階層カテゴリのキー</param>
        /// <returns></returns>
        IQueryable<ICategory> FindChildren(long parentCategoryId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        IQueryable<ICategory> FindCategory(ILabel label);

        /// <summary>
        /// すべてのラベルを持つカテゴリを取得する
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        IQueryable<ICategory> FindCategory(List<ILabel> label);

        /// <summary>
        /// いずれかのラベルを持つカテゴリを取得する
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        IQueryable<ICategory> FindCategoryOr(List<ILabel> label);

        /// <summary>
        /// ルートカテゴリを取得
        /// </summary>
        /// <returns></returns>
        ICategory LoadRootCategory();

        /// <summary>
        /// JSON文字列を使って既存エンティティのプロパティを更新する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="json"></param>
        void UpdatePopulateFromJson (long id, string json);
    }
}