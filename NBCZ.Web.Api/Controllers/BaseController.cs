using NBCZ.Common;
using NBCZ.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApi.Jwt;

namespace NBCZ.Web.Api.Controllers
{
    public class BaseController : ApiController
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            var res = new DataRes<string>();
            try
            {
              
                var data = controllerContext.Request.Content.ReadAsStringAsync().Result;
                var token = controllerContext.Request.Headers.Authorization;
                LogHelper.WrtieRequestLog(LogLevel.Info, HttpContext.Current.Request.Url.AbsoluteUri +
                    "\r\ntoken:" + token + " \r\n data:" + data);

            }
            catch (Exception ex)
            {

                res.code = ResCode.Error;
                res.msg = "Base接口异常！异常信息：" + ex.Message;
                controllerContext.Request.CreateResponse(res);
                return;
            }

            finally 
            {
                base.Initialize(controllerContext);
            }

        }
     
    }
}
