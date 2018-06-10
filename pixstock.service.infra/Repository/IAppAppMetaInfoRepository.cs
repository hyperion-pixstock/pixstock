using Pixstock.Service.Infra.Model;

namespace Pixstock.Service.Infra.Repository
{
    public interface IAppAppMetaInfoRepository : IRepositoryBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IAppMetaInfo New();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IAppMetaInfo Load(long id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        IAppMetaInfo LoadByKey(string keyName);
    }
}