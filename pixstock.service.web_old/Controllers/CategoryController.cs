using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using Pixstock.Base.AppIf.Sdk;
using Pixstock.Nc.Srv.Ext;
using Pixstock.Service.App.Builder;
using Pixstock.Service.App.Model;
using Pixstock.Service.Infra.Exception;
using Pixstock.Service.Infra.Model;
using Pixstock.Service.Infra.Repository;
using Pixstock.Service.Model;

namespace Pixstock.Service.App.Controllers {
    [Route ("aapi/[controller]")]
    public class CategoryController : Controller {
        private static Logger _logger = LogManager.GetCurrentClassLogger ();

        readonly ApiResponseBuilder mBuilder;

        readonly ICategoryRepository mCategoryRepository;

        readonly IContentRepository mContentRepository;

        readonly ExtentionManager mExtentionManager;

        public CategoryController (ApiResponseBuilder builder, ExtentionManager extentionManager, ICategoryRepository categoryRepository, IContentRepository contentRepository) {
            this.mBuilder = builder;
            this.mCategoryRepository = categoryRepository;
            this.mExtentionManager = extentionManager;
            this.mContentRepository = contentRepository;
        }

        /// <summary>
        /// カテゴリ情報取得
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <remarks>
        /// GET api/category/5
        /// </remarks>
        /// <returns></returns>
        [HttpGet ("{id}")]
        public ResponseAapi<ICategory> GetCategory (int id, [FromQuery] CategoryParam param) {
            var response = new ResponseAapi<ICategory> ();
            try {
                mBuilder.AttachCategoryEntity (id, response);
                var category = response.Value;

                // "la"
                if (param.lla_order == CategoryParam.LLA_ORDER_NAME_ASC) {
                    response.Link.Add ("la", category.GetContentList ().OrderBy (prop => prop.Name).Select (prop => prop.Id).ToArray ());
                } else if (param.lla_order == CategoryParam.LLA_ORDER_NAME_DESC) {
                    response.Link.Add ("la", category.GetContentList ().OrderByDescending (prop => prop.Name).Select (prop => prop.Id).ToArray ());
                } else {
                    response.Link.Add ("la", category.GetContentList ().Select (prop => prop.Id).ToArray ());
                }

                // "cc"
                var ccQuery = this.mCategoryRepository.FindChildren (category);
                response.Link.Add ("cc", ccQuery.Select (prop => prop.Id).ToArray ());

                // 拡張機能の呼び出し
                this.mExtentionManager.Execute (ExtentionCutpointType.API_GET_CATEGORY, category);
            } catch (Exception expr) {
                _logger.Error (expr.Message);
                throw new InterfaceOperationException ();
            }

            return response;
        }

        /// <summary>
        /// カテゴリ親階層カテゴリリンク情報取得API
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet ("{id}/pc")]
        public ResponseAapi<ICollection<ICategory>> GetCategoryLink_pc (int id) {
            return GetCategoryLink (id, "pc");
        }

        /// <summary>
        /// カテゴリサブカテゴリリンク情報取得API
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet ("{id}/cc")]
        public ResponseAapi<ICollection<ICategory>> GetCategoryLink_cc (int id) {
            return GetCategoryLink (id, "cc");
        }

        /// <summary>
        /// カテゴリコンテントリンク情報取得API
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet ("{id}/la")]
        public ResponseAapi<ICollection<IContent>> GetCategoryLink_la (int id) {
            _logger.Info ("REQUEST - {0}", id);

            var categoryList = new List<IContent> ();
            var response = new ResponseAapi<ICollection<IContent>> ();

            var category = this.mCategoryRepository.Load (id);
            categoryList.AddRange (
                category.GetContentList ().OrderBy (prop => prop.Name).Select (prop => prop).Take (100000)
            );
            response.Value = categoryList;
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        ///    GET api/category/{id}/albc/{link_id}
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="link_id"></param>
        /// <returns></returns>
        [HttpGet ("{id}/albc/{link_id}")]
        public ResponseAapi<Category> GetCategoryLink_albc (int id, int link_id) {
            _logger.Info ("REQUEST - {0}/albc/{1}", id, link_id);

            var response = new ResponseAapi<Category> ();
            response.Value = new Category { Id = link_id, Name = "リンクカテゴリ " + link_id };
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        ///    GET api/category/{id}/cc/{link_id}
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="link_id"></param>
        /// <returns></returns>
        [HttpGet ("{id}/cc/{link_id}")]
        public ResponseAapi<ICategory> GetCategoryLink_cc (int id, int link_id) {
            _logger.Info ("REQUEST - {0}/cc/{1}", id, link_id);
            var response = new ResponseAapi<ICategory> ();

            var linkedCategory = this.mCategoryRepository.FindChildren (id).Where (prop => prop.Id == link_id).SingleOrDefault ();
            if (linkedCategory != null) {
                mBuilder.AttachCategoryEntity (link_id, response);

                var sub = this.mCategoryRepository.FindChildren (linkedCategory).FirstOrDefault ();
                if (sub != null) {
                    response.Link.Add ("cc_available", true);
                }
            }

            return response;
        }

        /// <summary>
        /// カテゴリ情報とリンクしているアーティファクト情報を取得
        /// </summary>
        /// <remarks>
        ///    GET api/category/{id}/la/{link_id}
        ///    
        ///    コンテント情報取得と同じ情報量を返します。
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="la_id"></param>
        /// <returns></returns>
        [HttpGet ("{id}/la/{la_id}")]
        public ResponseAapi<IContent> GetCategoryLink_la (int id, int la_id) {
            _logger.Info ("REQUEST - {0}/la/{1}", id, la_id);

            var response = new ResponseAapi<IContent> ();

            var content = mContentRepository.Load (la_id);
            if (content != null) {
                if (content.GetCategory ().Id != id)
                    throw new InterfaceOperationException ();
                response.Value = content;
            } else {
                throw new InterfaceOperationException ();
            }

            return response;
        }

        // POST api/values
        [HttpPost]
        public void Post ([FromBody] string value) { }

        /// <summary>
        /// カテゴリ表示増加API
        /// </summary>
        /// <remarks>
        ///   PUT api/category/{id}/read
        /// </remarks>
        /// <param name="id">カテゴリ情報のキー</param>
        /// <param name="value">使用しない</param>
        [HttpPut ("{id}/read")]
        public void PutReadableCategory (int id) {
            _logger.Info ("REQUEST - {0}/read", id);

            var category = mCategoryRepository.Load (id);
            if (category != null) {
                if (!category.ReadableFlag) {
                    category.ReadableFlag = true;
                    category.ReadableDate = DateTime.Now;
                }
                category.LastReadDate = DateTime.Now;
                category.ReadableCount = category.ReadableCount + 1;
                mCategoryRepository.Save ();
            } else {
                throw new InterfaceOperationException ();
            }
        }

        /// <summary>
        /// カテゴリ情報更新API
        /// </summary>
        /// <remarks>
        ///   PUT api/category/{id}
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="value"></param>
        [HttpPut ("{id}")]
        public void PutCategoryProp (int id, [FromBody] string value) {
            _logger.Info ("REQUEST - {0} Body={1}", id, value);

            mCategoryRepository.UpdatePopulateFromJson (id, value);
            mCategoryRepository.Save ();
        }

        // DELETE api/values/5
        [HttpDelete ("{id}")]
        public void Delete (int id) { }

        /// <summary>
        /// カテゴリ情報リンク取得
        /// </summary>
        /// <param name="id"></param>
        /// <param name="link_type">リンクタイプを指定します。</param>
        /// <remarks>
        /// GET api/category/{id}/{link_type}
        /// link_type = 
        /// "pc" : 親階層のカテゴリ情報を取得します
        /// "cc" : 子階層のカテゴリ情報リストを取得します。
        /// </remarks>
        /// <returns></returns>
        private ResponseAapi<ICollection<ICategory>> GetCategoryLink (int id, string link_type) {
            var categoryList = new List<ICategory> ();

            var response = new ResponseAapi<ICollection<ICategory>> ();

            if (link_type == "pc") {
                var category = this.mCategoryRepository.Load (id);
                var parentCategory = this.mCategoryRepository.Load (category.GetParentCategory ().Id);
                if (parentCategory != null) categoryList.Add (parentCategory);
            } else if (link_type == "cc") {
                var category = this.mCategoryRepository.Load (id);
                categoryList.AddRange (
                    this.mCategoryRepository.FindChildren (category).Take (1000000)
                );
            }

            response.Value = categoryList;
            return response;
        }

    }
}