using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Util;

namespace Entity.OrganizationManage
{
    [Table("Org_User")]
    public class Org_User : BaseExtensionEntity
    {
        [Description("用户名（登录账号）")]
        public string UserName { get; set; }
        [Description("登录密码")]
        public string Password { get; set; }
        [Description("盐值")]
        [JsonIgnore]
        public string Salt { get; set; }
        [Description("真实姓名")]
        public string RealName { get; set; }
        [Description("性别")]
        public int Gender { get; set; }
        [Description("生日")]
        public string Birthday { get; set; }
        [Description("头像")]
        public string Portrait { get; set; }
        [Description("邮箱")]
        public string Email { get; set; }
        [Description("手机号")]
        public string Mobile { get; set; }
        [Description("QQ")]
        public string QQ { get; set; }
        [Description("微信号")]
        public string Wechat { get; set; }
        [Description("登录次数")]
        public int LoginCount { get; set; }
        [Description("用户状态")]
        public int Status { get; set; }
        [Description("是否系统用户")]
        public int IsSystem { get; set; }
        [Description("是否在线")]
        public int IsOnline { get; set; }
        [Description("首次登录时间")]
        public DateTime? FirstVisit { get; set; }
        [Description("上次登录时间")]
        public DateTime? PreviousVisit { get; set; }
        [Description("最后一次登录时间")]
        public DateTime? LastVisit { get; set; }
        [Description("备注")]
        public string Remark { get; set; }
        public string WebToken { get; set; }
        public string ApiToken { get; set; }

        [Description("部门Id")]
        [JsonConverter(typeof(StringJsonConverter))]
        public long? DepartmentId { get; set; }

        [NotMapped]
        public string DepartmentName { get; set; }

        /// <summary>
        /// 岗位Id
        /// </summary>
        [NotMapped]
        public string PositionIds { get; set; }

        /// <summary>
        /// 角色Id
        /// </summary>
        [NotMapped]
        public string RoleIds { get; set; }
    }
}
