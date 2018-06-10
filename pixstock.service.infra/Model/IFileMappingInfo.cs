using System;

namespace Pixstock.Service.Infra.Model
{
    public interface IFileMappingInfo : Pixstock.Common.Model.IFileMappingInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="workspace"></param>
        void SetWorkspace(IWorkspace workspace);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IWorkspace GetWorkspace();
    }
}