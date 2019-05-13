using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATTIOT.Portal.Models
{
    public class Java
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
        /// Shutdown端口
        /// </summary>
        public string ShutdownPort { get; set; }
        /// <summary>
        /// HTTP访问端口
        /// </summary>
        public string HttpPort { get; set; }
        /// <summary>
        /// AJP协议访问端口
        /// </summary>
        public string AJPPort { get; set; }
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