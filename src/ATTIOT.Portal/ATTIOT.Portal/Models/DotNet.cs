using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATTIOT.Portal.Models
{
    /// <summary>
    /// .NET项目模型
    /// </summary>
    public class DotNet
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int? No { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 访问地址
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// 项目路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 对应数据库
        /// </summary>
        public string DB { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}