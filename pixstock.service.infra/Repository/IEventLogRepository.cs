using Pixstock.Service.Infra.Model;

namespace Pixstock.Service.Infra.Repository
{
    public interface IEventLogRepository : IRepositoryBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEventLog New();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEventLog Load(long id);
    }
}