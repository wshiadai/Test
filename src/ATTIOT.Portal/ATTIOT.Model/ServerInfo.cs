using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATTIOT.Model
{
    /// <summary>
    /// 服务器基本信息
    /// </summary>
    public class ServerInfo
    {
        /// <summary>
        /// 服务器名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 服务器IP
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// 安装时间
        /// </summary>
        public DateTime InstallDate { get; set; }
        /// <summary>
        /// 启动时间
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 运行时长
        /// </summary>
        public string RunSpan { get; set; }
        /// <summary>
        /// 操作系统
        /// </summary>
        public string OS { get; set; }
        /// <summary>
        /// 系统位数
        /// </summary>
        public int Bit { get; set; }
        /// <summary>
        /// CPU名称
        /// </summary>
        public string CPU_Name { get; set; }
        /// <summary>
        /// 物理内存（单位：G）
        /// </summary>
        public float TotalMemory { get; set; }
        /// <summary>
        /// 硬盘总容量（单位：G）
        /// </summary>
        public float TotalDisk { get; set; }
        /// <summary>
        /// 硬盘可用容量（单位：G）
        /// </summary>
        public float FreeDisk { get; set; }
    }
}