using Business.OrganizationManage;
using Entity.OrganizationManage;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Util.Model;
using Web.Code;
using static Enum.CommonEnum;
using WebApp.Filter;
using Util.Extension;

namespace WebApp.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[AuthorizeFilter]
    public class UserController : ControllerBase
    {
        private readonly UserBLL userBLL = new UserBLL();

        #region 获取数据
        /// <summary>
        /// 当前登录用户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthorizeFilter]
        public async Task<TData<OperatorInfo>> GetCurrentUser()
        {
            TData<OperatorInfo> obj = new TData<OperatorInfo>();
            string token = HttpContext.Request.Headers["Authorization"].ParseToString();
            obj.Data = await Operator.Instance.Current(token);
            obj.Tag = 1;
            obj.Message = "获取成功";
            return obj;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userName">[FromQuery] string userName</param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<TData<OperatorInfo>> Login(Org_User Entity)
        {
            TData<OperatorInfo> obj = new TData<OperatorInfo>();
            TData<Org_User> userObj = await userBLL.CheckLogin(Entity.UserName, Entity.Password, (int)PlatformEnum.WebApi);
            if (userObj.Tag == 1)
            {
                await new UserBLL().UpdateUser(userObj.Data);
                await Operator.Instance.AddCurrent(userObj.Data.ApiToken);
                obj.Data = await Operator.Instance.Current(userObj.Data.ApiToken);
            }
            obj.Tag = userObj.Tag;
            obj.Message = userObj.Message;
            return obj;
        }
        /// <summary>
        /// 用户退出登录
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeFilter]
        public async Task<TData> LoginOff()
        {
            var obj = new TData();
            string token = HttpContext.Request.Headers["Authorization"].ParseToString();
            OperatorInfo currentUser = await Operator.Instance.Current(token);
            Org_User userObj = await userBLL.LoginOff(currentUser);
            if (userObj == null)
            {
                obj.Message = "登出失败";
                return obj;
                
            }
            await new UserBLL().UpdateUser(userObj);
            Operator.Instance.RemoveCurrent(token);
            obj.Tag = 1;
            obj.Message = "登出成功";
            return obj;
        }
        #endregion
    }
}
