using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using NBCZ.Common;
using NBCZ.Model;

namespace NBCZ.Web.Api
{
    /// <summary>
    /// 异常捕获
    /// </summary>
    public class WebApiExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            DataRes<object> rto = new DataRes<object>();
            var response = HttpContext.Current.Response;
            var request = HttpContext.Current.Request;
            rto.code = ResCode.Error;
            rto.msg = "[WEB.API]服务器异常！异常信息：" + actionExecutedContext.Exception.Message;
            response.Write(JsonConvert.SerializeObject(rto));
            response.End();
            Task.Run(() => WriteLog(actionExecutedContext));
            base.OnException(actionExecutedContext);
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="message"></param>
        private void WriteLog(HttpActionExecutedContext actionExecutedContext)
        {
            try
            {
                var exception=actionExecutedContext.Exception.InnerException!=null?actionExecutedContext.Exception.InnerException.StackTrace:actionExecutedContext.Exception.Message;
                string message = string.Format("\r\n url:{0}\r\n exception:{1}\r\n contextdata:{2}\r\n ",
                    actionExecutedContext.Request.RequestUri.AbsolutePath, exception,
                    actionExecutedContext.Request.Content.ReadAsStringAsync().Result
                    );

                var log = LogFactory.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(message);
            }
            catch (Exception ex)
            {
                string message = string.Format("\r\n url:{0}\r\n exception:{1}",
                   actionExecutedContext.Request.RequestUri.AbsolutePath, ex.Message
                   );
                var log = LogFactory.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(message);

            }



        }
    }
}