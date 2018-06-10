using Microsoft.AspNetCore.Mvc;
using Pixstock.Base.AppIf.Sdk;
using Pixstock.Service.Infra.Repository;

namespace Pixstock.Service.App.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("aapi/[controller]")]
    public class DevelopController : Controller
    {
        private readonly IWorkspaceRepository workspaceRepository;

        public DevelopController(IWorkspaceRepository workspaceRepository) {
            this.workspaceRepository = workspaceRepository;
        }

        [HttpGet("version")]
        public ResponseAapi<string> Get_Version() {
            var response = new ResponseAapi<string>();
            response.Value = "1.0.0";
            return response;
        }

        [HttpGet("register_workspace")]
        public ResponseAapi<string> Get_RegisterWorkspace() {
            var workspace = workspaceRepository.New();
            workspace.Name = "Private";
            workspace.PhysicalPath = "/home/atachi/PixstockSample";
            workspaceRepository.Save();

            var response = new ResponseAapi<string>();
            response.Value = "1.0.0";
            return response;
        }
    }
}