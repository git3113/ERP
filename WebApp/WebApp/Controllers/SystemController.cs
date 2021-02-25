using Business.OrganizationManage;
using Entity.OrganizationManage;
using Entity.SystemManage;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util.Extension;
using Util.Model;
using Web.Code;
using WebApp.Filter;
using static Enum.CommonEnum;

namespace WebApp.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AuthorizeFilter]
    public class SystemController : ControllerBase
    {
        private readonly UserBLL userBLL = new UserBLL();

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

        #region 菜单管理
        /// <summary>
        /// 获取系统菜单
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet]
        
        public async Task<TData<List<Sys_Module>>> GetMenuData()
        {
            TData<List<Sys_Module>> obj = new TData<List<Sys_Module>>();
            string token = HttpContext.Request.Headers["Authorization"].ParseToString();
            OperatorInfo currentUser = await Operator.Instance.Current(token);
            List<Sys_Module> menus = new List<Sys_Module>
            {
                new Sys_Module()
                {
                    Id = 1,
                    Name = "home",
                    Icon = "icon-home-fill",
                    Path = "/welcome",
                },
                new Sys_Module()
                {
                    Id = 2,
                    Name = "list.table-list",
                    Icon = "icon-packaging",
                    Path = "/list",
                }
            };
            obj.Data = menus;
            obj.Tag = 1;
            obj.Message = "获取成功";
            return obj;
        }
        #endregion
    }
}
