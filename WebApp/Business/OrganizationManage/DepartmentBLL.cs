using Entity.OrganizationManage;
using Service.OrganizationManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.OrganizationManage
{
    public class DepartmentBLL
    {
        private DepartmentService departmentService = new DepartmentService();
        private UserService userService = new UserService();

        #region 公共方法
        /// <summary>
        /// 获取当前部门及下面所有的部门
        /// </summary>
        /// <param name="departmentList"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public async Task<List<long>> GetChildrenDepartmentIdList(List<Org_Department> departmentList, long departmentId)
        {
            if (departmentList == null)
            {
                departmentList = await departmentService.GetList(null);
            }
            List<long> departmentIdList = new List<long>();
            departmentIdList.Add(departmentId);
            GetChildrenDepartmentIdList(departmentList, departmentId, departmentIdList);
            return departmentIdList;
        }
        #endregion 
        #region 私有方法
        /// <summary>
        /// 获取该部门下面所有的子部门
        /// </summary>
        /// <param name="departmentList"></param>
        /// <param name="departmentId"></param>
        /// <param name="departmentIdList"></param>
        private void GetChildrenDepartmentIdList(List<Org_Department> departmentList, long departmentId, List<long> departmentIdList)
        {
            var children = departmentList.Where(p => p.ParentId == departmentId).Select(p => p.Id).ToList();
            if (children.Count > 0)
            {
                departmentIdList.AddRange(children);
                foreach (long id in children)
                {
                    GetChildrenDepartmentIdList(departmentList, id, departmentIdList);
                }
            }
        }
        #endregion
    }
}
