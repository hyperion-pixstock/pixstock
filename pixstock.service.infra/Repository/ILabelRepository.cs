using System.Collections.Generic;
using System.Linq;
using Pixstock.Service.Infra.Model;

namespace Pixstock.Service.Infra.Repository
{
    public interface ILabelRepository : IRepositoryBase
    {
        ILabel New();

        ILabel Load(long id);

        ILabel LoadByName(string name);

        /// <summary>
        /// ラベル名に一致するラベル情報を取得する
        /// </summary>
        /// <param name="name">ラベル名</param>
        /// <param name="ownerType">一致するオーナータイプの検索文字列</param>
        /// <returns></returns>
        ILabel LoadByName(string name, string ownerType);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<ILabel> GetAll();
    }
}
