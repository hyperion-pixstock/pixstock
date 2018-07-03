using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Pixstock.Base.AppIf.Sdk;
using Pixstock.Service.App.Builder;
using Pixstock.Service.Infra.Exception;
using Pixstock.Service.Infra.Model;
using Pixstock.Service.Infra.Repository;
using Pixstock.Service.Model;

namespace Pixstock.Service.App.Controllers
{
    [Route("aapi/[controller]")]
    public class LabelController : Controller
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        readonly ApiResponseBuilder mBuilder;

        readonly ILabelRepository mLabelRepository;

        public LabelController(ApiResponseBuilder builder, ILabelRepository labelRepository)
        {
            this.mBuilder = builder;
            this.mLabelRepository = labelRepository;
        }

        /// <summary>
        /// ラベル情報一覧を取得
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public ResponseAapi<ICollection<ILabel>> GetLabel([FromQuery]RequestParamGetLabel requestParam)
        {
            // TODO: オフセット値を使用したデータ取得
            _logger.Info("Parameter Offset:{}", requestParam.Offset);

            List<ILabel> labels = new List<ILabel>();
            var response = new ResponseAapi<ICollection<ILabel>>();
            try
            {
                foreach (var prop in mLabelRepository.GetAll())
                {
                    labels.Add(prop);
                }
                response.Value = labels;
            }
            catch (Exception expr)
            {
                _logger.Error(expr.Message);
                throw new InterfaceOperationException();
            }

            return response;
        }

        /// <summary>
        /// ラベルリンクデータ情報取得API(カテゴリ情報)
        /// </summary>
        /// <returns></returns>
        [HttpGet("{query}/category")]
        public ResponseAapi<ICollection<ICategory>> GetLabelLinkCategory(string query)
        {
            // TODO: queryは、整数のみ（他の入力形式は、将来実装)
            var response = new ResponseAapi<ICollection<ICategory>>();

            try
            {
                // 現設計では、queryで指定できるラベル情報は1つのみであるため、
                // そのラベルを読み込んで、ラベル情報に関連付けされているカテゴリ情報の一覧を返す。
                long labelId = long.Parse(query);
                var label = mLabelRepository.Load(labelId) as Label;
                if (label != null)
                    response.Value = label.Categories.Select(prop => (ICategory)prop.Category).ToList();
                else
                    throw new ApplicationException(string.Format("指定したラベル情報(ID:{0})の読み込みに失敗しました", labelId));
            }
            catch (Exception expr)
            {
                _logger.Error(expr.Message);
                throw new InterfaceOperationException();
            }

            return response;
        }

        /// <summary>
        /// ラベルリンクデータ情報取得API(コンテント情報)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("{query}/content")]
        public ResponseAapi<ICollection<IContent>> GetLabelLinkContent(string query)
        {
            throw new NotImplementedException();
        }

        public class RequestParamGetLabel
        {
            public int Offset { get; set; }
        }
    }
}