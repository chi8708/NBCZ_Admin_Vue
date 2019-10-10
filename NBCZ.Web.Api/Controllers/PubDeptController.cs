using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NBCZ.BLL;
using NBCZ.Model;
using WebApi.Jwt.Filters;
using System.Web.Http;

namespace NBCZ.Web.Api.Controllers
{

    [RoutePrefix("api/PubDept")]
    [JwtAuthentication]
    public class PubDeptController :BaseController
    {
        Pub_DepartmentBLL bll = new Pub_DepartmentBLL();
        V_PubDept_ParentBLL deptParentBLL = new V_PubDept_ParentBLL();
        [HttpPost]
        [Route("GetList")]
        public DataRes<List<Pub_Department>> GetList()
        {
            var depts = bll.GetList("StopFlag=0");

            return new DataRes<List<Pub_Department>>() { data = depts };
        }

        /// <summary>
        /// 获取子部门列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetChildList")]
        [Route("GetChildList/{code}")]
        public DataRes<List<V_PubDept_Parent>> GetChildList(string code= "D000001")
        {

            var depts = deptParentBLL.GetList(string.Format(" StopFlag=0 And DeptCode IN (Select DeptCode From f_SearchChildDept('{0}'))", code), " DeptCode ");

            return new DataRes<List<V_PubDept_Parent>>() { data = depts };
        }


        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        [Route("Add")]
        [HttpPost]
        public DataRes<bool> Add([FromBody]Pub_Department model)
        {
            DataRes<bool> res = new DataRes<bool>() { code = ResCode.Success, data = true };

            model.DeptCode =bll.GetCode();
            model.Lmdt = model.Lmdt = DateTime.Now;
            var user = User.GetNBCZUser();
            model.Lmid = string.Format("{0}-{1}",user.UserCode,user.UserName);
            model.StopFlag = false;
            var r = bll.Insert(model);
            return res;
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        [Route("Edit")]
        [HttpPost]
        public DataRes<bool> Edit([FromBody]Pub_Department model)
        {
            DataRes<bool> res = new DataRes<bool>() { code = ResCode.Success, data = true };

            model.Lmdt = DateTime.Now;
            var user = User.GetNBCZUser();
            model.Lmid = string.Format("{0}-{1}",user.UserCode,user.UserName);
            var r = bll.Update(model);
            if (!r)
            {
                res.code = ResCode.Error;
                res.data = false;
                res.msg = "保存失败";
            }

            return res;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [Route("Delete/{id}")]
        [HttpPost]
        public DataRes<bool> Delete(string id)
        {
            DataRes<bool> res = new DataRes<bool>() { code = ResCode.Success, data = true };

            var r = bll.ChangeSotpStatus(string.Format("DeptCode='{0}'",id));
            if (!r)
            {
                res.code = ResCode.Error;
                res.data = false;
                res.msg = "删除失败";
            }

            return res;
        }
    }
}