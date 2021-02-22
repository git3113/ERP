using Data.Repository;
using Entity.OrganizationManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Extension;

namespace Service.OrganizationManage
{
    public class UserBelongService : RepositoryFactory
    {
        #region 获取数据
        public async Task<List<Org_UserBelong>> GetList(Org_UserBelong entity)
        {
            var expression = LinqExtensions.True<Org_UserBelong>();
            if (entity != null)
            {
                if (entity.BelongType != null)
                {
                    expression = expression.And(t => t.BelongType == entity.BelongType);
                }
                if (entity.UserId != null)
                {
                    expression = expression.And(t => t.UserId == entity.UserId);
                }
            }
            var list = await this.BaseRepository().FindList(expression);
            return list.ToList();
        }

        public async Task<Org_UserBelong> GetEntity(long id)
        {
            return await this.BaseRepository().FindEntity<Org_UserBelong>(id);
        }
        #endregion

        #region 提交数据
        public async Task SaveForm(Org_UserBelong entity)
        {
            if (entity.Id.IsNullOrZero())
            {
                await entity.Create();
                await this.BaseRepository().Insert(entity);
            }
            else
            {
                await this.BaseRepository().Update(entity);
            }
        }

        public async Task DeleteForm(long id)
        {
            await this.BaseRepository().Delete<Org_UserBelong>(id);
        }
        #endregion
    }
}
