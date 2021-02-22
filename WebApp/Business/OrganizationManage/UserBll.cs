using Cache.Factory;
using Entity.OrganizationManage;
using Enum.OrganizationManage;
using Model.Param;
using Model.Param.OrganizationManage;
using Service.OrganizationManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;
using Util.Extension;
using Util.Model;
using Web.Code;
using static Enum.CommonEnum;

namespace Business.OrganizationManage
{
    public class UserBLL
    {
        private readonly UserService userService = new UserService();
        private readonly UserBelongService userBelongService = new UserBelongService();
        private DepartmentService departmentService = new DepartmentService();

        private DepartmentBLL departmentBLL = new DepartmentBLL();

        #region 获取数据
        public async Task<TData<List<Org_User>>> GetList(UserListParam param)
        {
            TData<List<Org_User>> obj = new TData<List<Org_User>>
            {
                Data = await userService.GetList(param),
                Tag = 1
            };
            return obj;
        }

        public async Task<TData<List<Org_User>>> GetPageList(UserListParam param, Pagination pagination)
        {
            TData<List<Org_User>> obj = new TData<List<Org_User>>();
            if (param?.DepartmentId != null)
            {
                param.ChildrenDepartmentIdList = await departmentBLL.GetChildrenDepartmentIdList(null, param.DepartmentId.Value);
            }
            else
            {
                OperatorInfo user = await Operator.Instance.Current();
                param.ChildrenDepartmentIdList = await departmentBLL.GetChildrenDepartmentIdList(null, user.DepartmentId.Value);
            }
            obj.Data = await userService.GetPageList(param, pagination);
            List<Org_UserBelong> userBelongList = await userBelongService.GetList(new Org_UserBelong { UserIds = obj.Data.Select(p => p.Id).ParseToStrings<long>() });
            List<Org_Department> departmentList = await departmentService.GetList(new DepartmentListParam { Ids = userBelongList.Select(p => p.BelongId.Value).ParseToStrings<long>() });
            foreach (Org_User user in obj.Data)
            {
                user.DepartmentName = departmentList.Where(p => p.Id == user.DepartmentId).Select(p => p.DepartmentName).FirstOrDefault();
            }
            obj.Total = pagination.TotalCount;
            obj.Tag = 1;
            return obj;
        }

        public async Task<TData<Org_User>> GetEntity(long id)
        {
            TData<Org_User> obj = new TData<Org_User>();
            obj.Data = await userService.GetEntity(id);

            await GetUserBelong(obj.Data);

            if (obj.Data.DepartmentId > 0)
            {
                Org_Department Org_Department = await departmentService.GetEntity(obj.Data.DepartmentId.Value);
                if (Org_Department != null)
                {
                    obj.Data.DepartmentName = Org_Department.DepartmentName;
                }
            }

            obj.Tag = 1;
            return obj;
        }

        public async Task<TData<Org_User>> CheckLogin(string userName, string password, int platform)
        {
            TData<Org_User> obj = new TData<Org_User>();
            if (userName.IsEmpty() || password.IsEmpty())
            {
                obj.Message = "用户名或密码不能为空";
                return obj;
            }
            Org_User user = await userService.CheckLogin(userName);
            if (user != null)
            {
                if (user.Status == (int)StatusEnum.Yes)
                {
                    if (user.Password == EncryptUserPassword(password, user.Salt))
                    {
                        user.LoginCount++;
                        user.IsOnline = 1;

                        #region 设置日期
                        if (user.FirstVisit == GlobalConstant.DefaultTime)
                        {
                            user.FirstVisit = DateTime.Now;
                        }
                        if (user.PreviousVisit == GlobalConstant.DefaultTime)
                        {
                            user.PreviousVisit = DateTime.Now;
                        }
                        if (user.LastVisit != GlobalConstant.DefaultTime)
                        {
                            user.PreviousVisit = user.LastVisit;
                        }
                        user.LastVisit = DateTime.Now;
                        #endregion

                        switch (platform)
                        {
                            case (int)PlatformEnum.Web:
                                if (GlobalContext.SystemConfig.LoginMultiple)
                                {
                                    #region 多次登录用同一个token
                                    if (string.IsNullOrEmpty(user.WebToken))
                                    {
                                        user.WebToken = SecurityHelper.GetGuid();
                                    }
                                    #endregion
                                }
                                else
                                {
                                    user.WebToken = SecurityHelper.GetGuid();
                                }
                                break;

                            case (int)PlatformEnum.WebApi:
                                user.ApiToken = SecurityHelper.GetGuid();
                                break;
                        }
                        await GetUserBelong(user);

                        obj.Data = user;
                        obj.Message = "登录成功";
                        obj.Tag = 1;
                    }
                    else
                    {
                        obj.Message = "密码不正确，请重新输入";
                    }
                }
                else
                {
                    obj.Message = "账号被禁用，请联系管理员";
                }
            }
            else
            {
                obj.Message = "账号不存在，请重新输入";
            }
            return obj;
        }
        #endregion

        #region 提交数据
        public async Task<TData<string>> SaveForm(Org_User entity)
        {
            TData<string> obj = new TData<string>();
            if (userService.ExistUserName(entity))
            {
                obj.Message = "用户名已经存在！";
                return obj;
            }
            if (entity.Id.IsNullOrZero())
            {
                entity.Salt = GetPasswordSalt();
                entity.Password = EncryptUserPassword(entity.Password, entity.Salt);
            }
            if (!entity.Birthday.IsEmpty())
            {
                entity.Birthday = entity.Birthday.ParseToDateTime().ToString("yyyy-MM-dd");
            }
            await userService.SaveForm(entity);

            await RemoveCacheById(entity.Id);

            obj.Data = entity.Id.ParseToString();
            obj.Tag = 1;
            return obj;
        }

        public async Task<TData> DeleteForm(string ids)
        {
            TData obj = new TData();
            if (string.IsNullOrEmpty(ids))
            {
                obj.Message = "参数不能为空";
                return obj;
            }
            await userService.DeleteForm(ids);

            await RemoveCacheById(ids);

            obj.Tag = 1;
            return obj;
        }

        public async Task<TData<Org_User>> ResetPassword(Org_User entity)
        {
            TData<Org_User> obj = new TData<Org_User>();
            if (entity.Id > 0)
            {
                Org_User dbOrg_User = await userService.GetEntity(entity.Id);
                //if (dbOrg_User.Password == entity.Password)
                //{
                //    obj.Message = "密码未更改";
                //    return obj;
                //}
                dbOrg_User.Salt = GetPasswordSalt();
                dbOrg_User.Password = EncryptUserPassword(SecurityHelper.MD5Encrypt("123456"), dbOrg_User.Salt);
                await userService.ResetPassword(dbOrg_User);

                await RemoveCacheById(dbOrg_User.Id);

                obj.Data = null;
                obj.Message = "密码重置成功！";
                obj.Tag = 1;
            }
            return obj;
        }

        public async Task<TData<long>> ChangePassword(ChangePasswordParam param)
        {
            TData<long> obj = new TData<long>();
            if (param.Id > 0)
            {
                if (string.IsNullOrEmpty(param.Password) || string.IsNullOrEmpty(param.NewPassword))
                {
                    obj.Message = "新密码不能为空";
                    return obj;
                }
                Org_User dbOrg_User = await userService.GetEntity(param.Id.Value);
                if (dbOrg_User.Password != EncryptUserPassword(param.Password, dbOrg_User.Salt))
                {
                    obj.Message = "旧密码不正确";
                    return obj;
                }
                dbOrg_User.Salt = GetPasswordSalt();
                dbOrg_User.Password = EncryptUserPassword(param.NewPassword, dbOrg_User.Salt);
                await userService.ResetPassword(dbOrg_User);

                await RemoveCacheById(param.Id.Value);

                obj.Data = dbOrg_User.Id;
                obj.Tag = 1;
            }
            return obj;
        }

        /// <summary>
        /// 用户自己修改自己的信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<TData<long>> ChangeUser(Org_User entity)
        {
            TData<long> obj = new TData<long>();
            if (entity.Id > 0)
            {
                await userService.ChangeUser(entity);

                await RemoveCacheById(entity.Id);

                obj.Data = entity.Id;
                obj.Tag = 1;
            }
            return obj;
        }

        public async Task<TData> UpdateUser(Org_User entity)
        {
            TData obj = new TData();
            await userService.UpdateUser(entity);

            obj.Tag = 1;
            return obj;
        }

        public async Task<TData> ImportUser(ImportParam param, List<Org_User> list)
        {
            TData obj = new TData();
            if (list.Any())
            {
                foreach (Org_User entity in list)
                {
                    Org_User dbEntity = await userService.GetEntity(entity.UserName);
                    if (dbEntity != null)
                    {
                        entity.Id = dbEntity.Id;
                        if (param.IsOverride == 1)
                        {
                            await userService.SaveForm(entity);
                            await RemoveCacheById(entity.Id);
                        }
                    }
                    else
                    {
                        await userService.SaveForm(entity);
                        await RemoveCacheById(entity.Id);
                    }
                }
                obj.Tag = 1;
            }
            else
            {
                obj.Message = " 未找到导入的数据";
            }
            return obj;
        }

        #endregion

        #region 私有方法
        /// <summary>
        /// 密码MD5处理
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        private string EncryptUserPassword(string password, string salt)
        {
            string md5Password = SecurityHelper.MD5Encrypt(password);
            string encryptPassword = SecurityHelper.MD5Encrypt(md5Password + salt);
            return encryptPassword;
        }

        /// <summary>
        /// 密码盐
        /// </summary>
        /// <returns></returns>
        private string GetPasswordSalt()
        {
            return new Random().Next(1, 100000).ToString();
        }

        /// <summary>
        /// 移除缓存里面的token
        /// </summary>
        /// <param name="id"></param>
        private async Task RemoveCacheById(string ids)
        {
            foreach (long id in ids.Split(',').Select(p => long.Parse(p)))
            {
                await RemoveCacheById(id);
            }
        }

        private async Task RemoveCacheById(long id)
        {
            var dbEntity = await userService.GetEntity(id);
            if (dbEntity != null)
            {
                CacheFactory.Cache.RemoveCache(dbEntity.WebToken);
            }
        }

        /// <summary>
        /// 获取用户的职位和角色
        /// </summary>
        /// <param name="user"></param>
        private async Task GetUserBelong(Org_User user)
        {
            List<Org_UserBelong> userBelongList = await userBelongService.GetList(new Org_UserBelong { UserId = user.Id });

            List<Org_UserBelong> roleBelongList = userBelongList.Where(p => p.BelongType == UserBelongTypeEnum.Role.ParseToInt()).ToList();
            if (roleBelongList.Count > 0)
            {
                user.RoleIds = string.Join(",", roleBelongList.Select(p => p.BelongId).ToList());
            }

            List<Org_UserBelong> positionBelongList = userBelongList.Where(p => p.BelongType == UserBelongTypeEnum.Position.ParseToInt()).ToList();
            if (positionBelongList.Count > 0)
            {
                user.PositionIds = string.Join(",", positionBelongList.Select(p => p.BelongId).ToList());
            }
        }
        #endregion
    }
}
