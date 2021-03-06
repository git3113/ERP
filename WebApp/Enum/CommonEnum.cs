﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enum
{
    public class CommonEnum
    {
        public enum StatusEnum
        {
            [Description("启用")]
            Yes = 1,

            [Description("禁用")]
            No = 0
        }

        public enum IsEnum
        {
            [Description("是")]
            Yes = 1,

            [Description("否")]
            No = 0
        }

        public enum NeedEnum
        {
            [Description("不需要")]
            NotNeed = 0,

            [Description("需要")]
            Need = 1
        }

        public enum OperateStatusEnum
        {
            [Description("失败")]
            Fail = 0,

            [Description("成功")]
            Success = 1
        }

        public enum UploadFileType
        {
            [Description("头像")]
            Portrait = 1,

            [Description("新闻图片")]
            News = 2,

            [Description("导入的文件")]
            Import = 10
        }

        public enum PlatformEnum
        {
            [Description("Web后台")]
            Web = 1,

            [Description("WebApi")]
            WebApi = 2
        }
    }
}
