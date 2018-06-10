using System;
using Hyperion.Pf.Entity;

namespace Pixstock.Common.Model
{
    public interface IWorkspace : IEntity<long>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string PhysicalPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string VirtualPath {get;set;}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        DateTime? LastFullBuildDate { get; set; }
    }
}