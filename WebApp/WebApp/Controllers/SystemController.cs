using Business.OrganizationManage;
using Entity.OrganizationManage;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util.Model;
using Web.Code;
using static Enum.CommonEnum;

namespace WebApp.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        private UserBLL userBLL = new UserBLL();

        #region 用户管理   
        /// <summary>
        /// 用户密码重置
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<TData<OperatorInfo>> ResetPassword(Org_User Entity)
        {
            TData<OperatorInfo> obj = new TData<OperatorInfo>();
            TData<Org_User> userObj = await userBLL.ResetPassword(Entity);
            //if (userObj.Tag == 1)
            //{
            //    await new UserBLL().UpdateUser(userObj.Data);
            //    await Operator.Instance.AddCurrent(userObj.Data.ApiToken);
            //    obj.Data = await Operator.Instance.Current(userObj.Data.ApiToken);
            //}
            obj.Tag = userObj.Tag;
            obj.Message = userObj.Message;
            return obj;
        }
        #endregion
    }
}
