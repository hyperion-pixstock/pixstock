using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NLog;

namespace Pixstock.Service.App
{
    /// <summary>
    /// 
    /// </summary>
    public class ErrorExceptionResolver
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        
        public async Task Invoke(HttpContext context, Func<Task> next)
        {
            try
            {
                await next();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.Warn("例外をキャッチしました。例外ハンドラからレスポンスします。");
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            // TODO: 例外別のステータスコードを返します
            //if (exception is MyNotFoundException) code = HttpStatusCode.NotFound;
            //else if (exception is MyUnauthorizedException) code = HttpStatusCode.Unauthorized;
            //else if (exception is MyException) code = HttpStatusCode.BadRequest;

            var result = JsonConvert.SerializeObject(new { error = exception.Message });
            _logger.Warn("ErrorMessage=" + exception.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}