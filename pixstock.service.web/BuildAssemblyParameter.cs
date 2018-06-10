using System.Collections.Generic;
using Pixstock.Nc.Common;

namespace Pixstock.Service.App
{
    public class BuildAssemblyParameter : IBuildAssemblyParameter
    {
        private readonly Dictionary<string, string> _Params = new Dictionary<string, string>();

        public Dictionary<string, string> Params => _Params;

        public BuildAssemblyParameter()
        {
            BuildParams();
        }

        public BuildAssemblyParameter(AppSettings appSettings)
        {
            this.Params.Add("ApplicationDirectoryPath", appSettings.ApplicationDirectoryBasePath);
            if (!string.IsNullOrEmpty(appSettings.InitializeSqlAppDb))
            {
                this.Params.Add("InitializeSqlAppDb", appSettings.InitializeSqlAppDb);
            }
            if (appSettings.AbsoluteApplicationDirectoryBase)
            {
                this.Params.Add("AbsoluteApplicationDirectoryBase", "true");
            }
            else
            {
                this.Params.Add("AbsoluteApplicationDirectoryBase", "false");
            }
        }

        private void BuildParams()
        {
            if (!_Params.ContainsKey("ApplicationDirectoryPath"))
            {
                this.Params.Add("ApplicationDirectoryPath", @"Pixstock.Srv");
            }
        }
    }
}