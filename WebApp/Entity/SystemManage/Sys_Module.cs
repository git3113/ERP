using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Util;

namespace Entity.SystemManage
{
    [Table("Sys_Module")]
    public class Sys_Module : BaseExtensionEntity
    {
            [JsonConverter(typeof(StringJsonConverter))]
            public long? ParentId { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }
            public string Icon { get; set; }
    }
}
