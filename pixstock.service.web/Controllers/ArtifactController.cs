using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using NLog;
using Pixstock.Base.AppIf.Sdk;
using Pixstock.Service.App.Builder;
using Pixstock.Service.Infra.Exception;
using Pixstock.Service.Infra.Model;
using Pixstock.Service.Infra.Repository;

namespace Pixstock.Service.App.Controllers
{
    [Route("aapi/[controller]")]
    public class ArtifactController : Controller
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        readonly ApiResponseBuilder mBuilder;

        readonly IContentRepository mContentRepository;

        readonly IFileMappingInfoRepository mFileMappingInfoRepository;

        public ArtifactController(
            ApiResponseBuilder builder,
            IContentRepository contentRepository,
            IFileMappingInfoRepository fileMappingInfoRepository
            )
        {
            this.mBuilder = builder;
            this.mContentRepository = contentRepository;
            this.mFileMappingInfoRepository = fileMappingInfoRepository;
        }

        /// <summary>
        /// コンテント詳細情報取得API
        /// </summary>
        /// <remarks>
        /// GET aapi/artifact/{id}
        /// </remarks>
        /// <param name="id">コンテントID</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ResponseAapi<IContent> GetContent(int id)
        {
            var response = new ResponseAapi<IContent>();
            try
            {
                mBuilder.AttachContentEntity(id, response);
                var content = response.Value;

                // リンクデータ
                // "category"
                if (content.GetCategory() != null)
                    response.Link.Add("category", content.GetCategory().Id);
            }
            catch (Exception expr)
            {
                _logger.Error(expr.Message);
                throw new InterfaceOperationException();
            }
            return response;
        }

        /// <summary>
        /// コンテント情報のリンクデータ(所属カテゴリ情報)を取得します
        /// </summary>
        /// <param name="id">コンテント情報のキー</param>
        /// <returns>所属カテゴリ情報</returns>
        [HttpGet("{id}/category")]
        public ResponseAapi<ICategory> GetContentLink_Category(int id)
        {
            var response = new ResponseAapi<ICategory>();
            try
            {
                var content = mContentRepository.Load(id);
                mBuilder.AttachCategoryEntity(content.GetCategory().Id, response);
            }
            catch (Exception expr)
            {
                _logger.Error(expr.Message);
                throw new InterfaceOperationException();
            }
            return response;
        }

        /// <summary>
        /// プレビューファイルを取得します
        /// </summary>
        /// <param name="id">コンテントID</param>
        /// <returns>コンテントのプレビューファイル</returns>
        [HttpGet("{id}/preview")]
        public IActionResult FetchPreviewFile(int id)
        {
            _logger.Debug("IN");
            var content = mContentRepository.Load(id);
            if (content == null) throw new InterfaceOperationException("コンテント情報が見つかりません");

            var fmi = content.GetFileMappingInfo();
            if (fmi == null) throw new InterfaceOperationException("ファイルマッピング情報が見つかりません1");

            var efmi = mFileMappingInfoRepository.Load(fmi.Id);
            if (efmi == null) throw new InterfaceOperationException("ファイルマッピング情報が見つかりません2");

            // NOTE: リソースの有効期限等を決定する
            DateTimeOffset now = DateTime.Now;
            var etag = new EntityTagHeaderValue("\"" + Guid.NewGuid().ToString() + "\"");
            string filePath = Path.Combine(efmi.GetWorkspace().PhysicalPath, efmi.MappingFilePath);
            var file = PhysicalFile(
                Path.Combine(efmi.GetWorkspace().PhysicalPath, efmi.MappingFilePath)
                , efmi.Mimetype, now, etag);

            _logger.Debug("OUT");
            return file;
        }
    }
}
