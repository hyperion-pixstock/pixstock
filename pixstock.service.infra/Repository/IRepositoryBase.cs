namespace Pixstock.Service.Infra.Repository
{
    public interface IRepositoryBase
    {
        /// <summary>
        /// 永続化
        /// </summary>
        void Save();
    }
}