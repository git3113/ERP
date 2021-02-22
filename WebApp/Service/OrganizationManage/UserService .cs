using Data.Repository;
using Entity.OrganizationManage;
using Enum.OrganizationManage;
using Model.Param.OrganizationManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Util;
using Util.Extension;
using Util.Model;

namespace Service.OrganizationManage
{
    public class UserService : RepositoryFactory
    {
        #region 获取数据
        public async Task<List<Org_User>> GetList(UserListParam param)
        {
            var expression = ListFilter(param);
            var list = await this.BaseRepository().FindList(expression);
            return list.ToList();
        }

        public async Task<List<Org_User>> GetPageList(UserListParam param, Pagination pagination)
        {
            var expression = ListFilter(param);
            var list = await this.BaseRepository().FindList(expression, pagination);
            return list.ToList();
        }

        public async Task<Org_User> GetEntity(long id)
        {
            return await this.BaseRepository().FindEntity<Org_User>(id);
        }

        public async Task<Org_User> GetEntity(string userName)
        {
            return await this.BaseRepository().FindEntity<Org_User>(p => p.UserName == userName);
        }

        public async Task<Org_User> CheckLogin(string userName)
        {
            var expression = LinqExtensions.True<Org_User>();
            expression = expression.And(t => t.UserName == userName);
            expression = expression.Or(t => t.Mobile == userName);
            expression = expression.Or(t => t.Email == userName);
            return await this.BaseRepository().FindEntity(expression);
        }

        public bool ExistUserName(Org_User entity)
        {
            var expression = LinqExtensions.True<Org_User>();
            expression = expression.And(t => t.BaseIsDelete == 0);
            if (entity.Id.IsNullOrZero())
            {
                expression = expression.And(t => t.UserName == entity.UserName);
            }
            else
            {
                expression = expression.And(t => t.UserName == entity.UserName && t.Id != entity.Id);
            }
            return this.BaseRepository().IQueryable(expression).Count() > 0 ? true : false;
        }
        #endregion

        #region 提交数据
        public async Task UpdateUser(Org_User entity)
        {
            await this.BaseRepository().Update(entity);
        }

        public async Task SaveForm(Org_User entity)
        {
            var db = await this.BaseRepository().BeginTrans();
            try
            {
                if (entity.Id.IsNullOrZero())
                {
                    await entity.Create();
                    await db.Insert(entity);
                }
                else
                {
                    await db.Delete<Org_UserBelong>(t => t.UserId == entity.Id);

                    // 密码不进行更新，有单独的方法更新密码
                    entity.Password = null;
                    await entity.Modify();
                    await db.Update(entity);
                }
                // 职位
                if (!string.IsNullOrEmpty(entity.PositionIds))
                {
                    foreach (long positionId in TextHelper.SplitToArray<long>(entity.PositionIds, ','))
                    {
                        Org_UserBelong positionBelongEntity = new Org_UserBelong();
                        positionBelongEntity.UserId = entity.Id;
                        positionBelongEntity.BelongId = positionId;
                        positionBelongEntity.BelongType = UserBelongTypeEnum.Position.ParseToInt();
                        await positionBelongEntity.Create();
                        await db.Insert(positionBelongEntity);
                    }
                }
                // 角色
                if (!string.IsNullOrEmpty(entity.RoleIds))
                {
                    foreach (long roleId in TextHelper.SplitToArray<long>(entity.RoleIds, ','))
                    {
                        Org_UserBelong departmentBelongEntity = new Org_UserBelong();
                        departmentBelongEntity.UserId = entity.Id;
                        departmentBelongEntity.BelongId = roleId;
                        departmentBelongEntity.BelongType = UserBelongTypeEnum.Role.ParseToInt();
                        await departmentBelongEntity.Create();
                        await db.Insert(departmentBelongEntity);
                    }
                }
                await db.CommitTrans();
            }
            catch
            {
                await db.RollbackTrans();
                throw;
            }
        }

        public async Task DeleteForm(string ids)
        {
            var db = await this.BaseRepository().BeginTrans();
            try
            {
                long[] idArr = TextHelper.SplitToArray<long>(ids, ',');
                await db.Delete<Org_User>(idArr);
                await db.Delete<Org_UserBelong>(t => idArr.Contains(t.UserId.Value));
                await db.CommitTrans();
            }
            catch
            {
                await db.RollbackTrans();
                throw;
            }
        }

        public async Task ResetPassword(Org_User entity)
        {
            await entity.Modify();
            await this.BaseRepository().Update(entity);
        }

        public async Task ChangeUser(Org_User entity)
        {
            await entity.Modify();
            await this.BaseRepository().Update(entity);
        }
        #endregion

        #region 私有方法
        private Expression<Func<Org_User, bool>> ListFilter(UserListParam param)
        {
            var expression = LinqExtensions.True<Org_User>();
            if (param != null)
            {
                if (!string.IsNullOrEmpty(param.UserName))
                {
                    expression = expression.And(t => t.UserName.Contains(param.UserName));
                }
                if (!string.IsNullOrEmpty(param.UserIds))
                {
                    long[] userIdList = TextHelper.SplitToArray<long>(param.UserIds, ',');
                    expression = expression.And(t => userIdList.Contains(t.Id));
                }
                if (!string.IsNullOrEmpty(param.Mobile))
                {
                    expression = expression.And(t => t.Mobile.Contains(param.Mobile));
                }
                if (param.Status > -1)
                {
                    expression = expression.And(t => t.Status == param.Status);
                }
                if (!string.IsNullOrEmpty(param.StartTime.ParseToString()))
                {
                    expression = expression.And(t => t.BaseModifyTime >= param.StartTime);
                }
                if (!string.IsNullOrEmpty(param.EndTime.ParseToString()))
                {
                    param.EndTime = (param.EndTime.Value.ToString("yyyy-MM-dd") + " 23:59:59").ParseToDateTime();
                    expression = expression.And(t => t.BaseModifyTime <= param.EndTime);
                }
                if (param.ChildrenDepartmentIdList != null && param.ChildrenDepartmentIdList.Count > 0)
                {
                    expression = expression.And(t => param.ChildrenDepartmentIdList.Contains(t.DepartmentId.Value));
                }
            }
            return expression;
        }
        #endregion
    }
}
