using Data.Repository;
using Entity.OrganizationManage;
using Model.Param.OrganizationManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Util;
using Util.Extension;

namespace Service.OrganizationManage
{
    public class DepartmentService : RepositoryFactory
    {
        #region 获取数据
        public async Task<List<Org_Department>> GetList(DepartmentListParam param)
        {
            var expression = ListFilter(param);
            var list = await this.BaseRepository().FindList(expression);
            return list.OrderBy(p => p.DepartmentSort).ToList();
        }

        public async Task<Org_Department> GetEntity(long id)
        {
            return await this.BaseRepository().FindEntity<Org_Department>(id);
        }

        public async Task<int> GetMaxSort()
        {
            object result = await this.BaseRepository().FindObject("SELECT MAX(DepartmentSort) FROM SysDepartment");
            int sort = result.ParseToInt();
            sort++;
            return sort;
        }

        /// <summary>
        /// 部门名称是否存在
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool ExistDepartmentName(Org_Department entity)
        {
            var expression = LinqExtensions.True<Org_Department>();
            expression = expression.And(t => t.BaseIsDelete == 0);
            if (entity.Id.IsNullOrZero())
            {
                expression = expression.And(t => t.DepartmentName == entity.DepartmentName);
            }
            else
            {
                expression = expression.And(t => t.DepartmentName == entity.DepartmentName && t.Id != entity.Id);
            }
            return this.BaseRepository().IQueryable(expression).Count() > 0 ? true : false;
        }

        /// <summary>
        /// 是否存在子部门
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistChildrenDepartment(long id)
        {
            var expression = LinqExtensions.True<Org_Department>();
            expression = expression.And(t => t.ParentId == id);
            return this.BaseRepository().IQueryable(expression).Count() > 0 ? true : false;
        }
        #endregion

        #region 提交数据
        public async Task SaveForm(Org_Department entity)
        {
            if (entity.Id.IsNullOrZero())
            {
                await entity.Create();
                await this.BaseRepository().Insert(entity);
            }
            else
            {
                await entity.Modify();
                await this.BaseRepository().Update(entity);
            }
        }

        public async Task DeleteForm(string ids)
        {
            var db = await this.BaseRepository().BeginTrans();
            try
            {
                long[] idArr = TextHelper.SplitToArray<long>(ids, ',');
                await db.Delete<Org_Department>(idArr);
                await db.CommitTrans();
            }
            catch
            {
                await db.RollbackTrans();
                throw;
            }
        }
        #endregion

        #region 私有方法
        private Expression<Func<Org_Department, bool>> ListFilter(DepartmentListParam param)
        {
            var expression = LinqExtensions.True<Org_Department>();
            if (param != null)
            {
                if (!param.DepartmentName.IsEmpty())
                {
                    expression = expression.And(t => t.DepartmentName.Contains(param.DepartmentName));
                }
            }
            return expression;
        }
        #endregion
    }
}
