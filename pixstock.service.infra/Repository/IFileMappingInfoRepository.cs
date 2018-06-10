using System.Collections.Generic;
using Pixstock.Service.Infra.Model;

namespace Pixstock.Service.Infra.Repository
{
    public interface IFileMappingInfoRepository : IRepositoryBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IFileMappingInfo Load(long id);

        /// <summary>
        /// AclHashが一致するエンティティを取得します。
        /// </summary>
        /// <param name="aclHash"></param>
        /// <returns></returns>
        IFileMappingInfo LoadByAclHash(string aclHash);

        /// <summary>
        /// MappingFilePathが一致するエンティティを取得します。
        /// </summary>
        /// <remarks>
        /// MappingFilePathは、ディレクトリ空間からの相対パスです。
        /// </remarks>
        /// <param name="path"></param>
        /// <returns></returns>
        IFileMappingInfo LoadByPath(string path);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IFileMappingInfo New();

        void Delete(IFileMappingInfo entity);

        IEnumerable<IFileMappingInfo> FindPathWithStart(string startText);
    }
}