using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Param.OrganizationManage
{
    public class UserListParam : DateTimeParam
    {
        public string UserName { get; set; }

        public string Mobile { get; set; }

        public int? Status { get; set; }

        public long? DepartmentId { get; set; }

        public List<long> ChildrenDepartmentIdList { get; set; }

        public string UserIds { get; set; }
    }

    public class ChangePasswordParam
    {
        public long? Id { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
    }
}
