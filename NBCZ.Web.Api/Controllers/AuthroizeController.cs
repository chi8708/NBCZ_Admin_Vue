using NBCZ.BLL;
using NBCZ.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Jwt;


namespace NBCZ.Web.Api
{
    /// <summary>
    /// 认证
    /// </summary>
    [Route("api/Authroize")]
    public class AuthroizeController : ApiController
    {
             /// <summary>
        /// 登录获取token
        /// </summary>
        /// <param name="loginViewModel">登录实体信息</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult Post([FromBody]LoginViewModel loginViewModel)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var users = new Pub_UserBLL().GetList( 
                string.Format("StopFlag=0 AND UserName='{0}' AND UserPwd='{1}'",loginViewModel.Name,loginViewModel.Password), limits: 1);
            
            if (users.Count>0)
            {
                var user = users.First();
                //var userFunctions = new  Pub_UserFunctionBLL().GetList(string.Format("UserCode='{0}'",user.UserCode)).Select(p=>p.FunctionCode);
                //var roleFunctions = new Pub_RoleFunctionBLL().GetList(string.Format(" RoleCode IN(SELECT pur.RoleCode FROM Pub_UserRole AS pur WHERE pur.UserCode='{0}' )", user.UserCode)).Select(p => p.FunctionCode);
                //var functions = userFunctions.Concat(roleFunctions).Distinct();
                //var functionsStr = string.Join(",", functions);

                var token= JwtManager.GenerateToken(user);
            
                return Ok(new ResponseObj<dynamic>()
                {
                    Code = 1,
                    Message = "认证成功",
                    Data =new
                    {
                        Token=token.Item1 ,
                        Expires = TypeUtil.ConvertDateTimeInt((DateTime)token.Item2)
                    }
                });
            }

            return Ok(new ResponseObj<dynamic>()
            {
                Code = 0,
                Message = "用户名密码错误！"
            });
            //return BadRequest();
        }

        public class ResponseObj<T>
        {
            /// <summary>
            /// 0请求数据错误 1 成功 -1失败
            /// </summary>
            public int Code { get; set; }

            public string Message { get; set; }

            public T Data { get; set; }
        }

    }
}
