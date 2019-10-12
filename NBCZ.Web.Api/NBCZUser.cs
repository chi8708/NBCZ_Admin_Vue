using NBCZ.BLL;
using NBCZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace NBCZ.Web.Api
{
    public static class NBCZFactory
    {
        public static NBCZUser GetNBCZUser(this IPrincipal user)
        {
            if (!user.Identity.IsAuthenticated)
            {
                return null;
            }
            return new NBCZUser(user);
        }
    }

    public class NBCZUser
    {
        public NBCZUser(IPrincipal user)
        {
            this.User = user;
            this.Identity= (ClaimsIdentity)user.Identity;
        }

        public IPrincipal User { get; set; }
        public ClaimsIdentity Identity { get; set; }

        public int Id 
        {
            get
            {

                var id = Identity.FindFirst(p => p.Type == "Id").Value;
                return Convert.ToInt32(id);
            }
            
        }
        public string UserCode
        {
            get
            {

                var userCode = Identity.FindFirst(p => p.Type == "UserCode").Value;
                return userCode;               
            }
        }

        public  string UserName
        {
            get
            {
                var userName = Identity.FindFirst(p => p.Type == ClaimTypes.Name).Value;
                return userName;   
            }
        }

        public  string DeptCode
        {
            get
            {
                var deptCode = Identity.FindFirst(p => p.Type == "DeptCode").Value;
                return deptCode; 
            }
        }


        public  string MobilePhone
        {
            get
            {
                var mobile = Identity.FindFirst(p => p.Type == "MobilePhone").Value;
                return mobile;
            }
        }

        public  List<string> Access
        {
            get
            {
                //var userFunctions = new Pub_UserFunctionBLL().GetList(string.Format("UserCode='{0}'", this.UserCode)).Select(p => p.FunctionCode);
                //var roleFunctions = new Pub_RoleFunctionBLL().GetList(string.Format(" RoleCode IN(SELECT pur.RoleCode FROM Pub_UserRole AS pur WHERE pur.UserCode='{0}' )", this.UserCode)).Select(p => p.FunctionCode);
                //var functions = userFunctions.Concat(roleFunctions).Distinct();
                var functions = new Pub_FunctionBLL().GetUserAccess(this.UserCode);

                return functions;
            }
        }
    }

   
}
