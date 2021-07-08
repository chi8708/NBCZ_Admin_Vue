using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NBCZ.Web.Api.Request
{
    public class EditPasswordReq
    {
        /// <summary>
        /// 原密码
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        public string Password { get; set; }
    }
}