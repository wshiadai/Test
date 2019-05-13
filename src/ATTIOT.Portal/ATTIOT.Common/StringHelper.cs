using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web;
using System.Net;
using System.Management;
using System.IO;
using ATTIOT.Model;

namespace ATTIOT.Common
{
    public class StringHelper
    {
        public static bool IsEmpty(object value)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string GetConfigValue(string key)
        {
            string value = string.Empty;
            string config = ConfigurationManager.AppSettings[key];
            if (!IsEmpty(config))
            {
                value = config;
            }
            return value;
        }

        /// <summary>
        /// 获取服务器信息
        /// </summary>
        /// <returns></returns>
        public static ServerInfo GetSystemInfo()
        {
            try
            {
                ServerInfo server = new ServerInfo();
                DateTime now = DateTime.Now;
                string name = string.Empty;  //计算机名称
                string ip = HttpContext.Current.Request.ServerVariables["LOCAl_ADDR"]; //内网IP
                string model = string.Empty;  //型号
                DateTime installDate = new DateTime();  //安装时间
                DateTime startDate = new DateTime();  //系统启动时间
                string runSpan = string.Empty;  //运行时长（天+小时）
                string _OS = string.Empty;  //操作系统
                int bit = Environment.Is64BitOperatingSystem ? 64 : 32;  //系统位数
                string cpu = string.Empty;  //CPU名称
                float totalMemory = 0;  //物理内存
                float totalDisk = 0;  //硬盘总容量
                float freeDisk = 0;   //硬盘可用容量            
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("select Name,Model,TotalPhysicalMemory from Win32_ComputerSystem");
                foreach (ManagementObject mo in searcher.Get()) //通过WMI获取系统相关信息
                {
                    name = mo["Name"].ToString();
                    model = mo["Model"].ToString();
                    totalMemory = Convert.ToSingle(mo["TotalPhysicalMemory"]);
                    break;
                }
                searcher = new ManagementObjectSearcher("select Caption,LastBootUpTime,InstallDate from Win32_OperatingSystem");
                foreach (ManagementObject mo in searcher.Get())
                {
                    startDate = ManagementDateTimeConverter.ToDateTime(mo["LastBootUpTime"].ToString());
                    installDate = ManagementDateTimeConverter.ToDateTime(mo["InstallDate"].ToString());
                    _OS = mo["Caption"].ToString();
                    break;
                }
                searcher = new ManagementObjectSearcher("select Name from Win32_Processor");
                foreach (ManagementObject mo in searcher.Get())
                {
                    cpu = mo["Name"].ToString();
                    break;
                }
                searcher = new ManagementObjectSearcher("select DriveType,Size,FreeSpace from Win32_LogicalDisk");
                ManagementObjectCollection diskcollection = searcher.Get();
                if (diskcollection != null && diskcollection.Count > 0)
                {
                    foreach (ManagementObject disk in diskcollection)
                    {
                        int type = Convert.ToInt32(disk["DriveType"]);
                        if (type != Convert.ToInt32(DriveType.Fixed))  //只统计固定磁盘
                        {
                            continue;
                        }
                        else
                        {
                            totalDisk += Convert.ToSingle(disk["Size"]);
                            freeDisk += Convert.ToSingle(disk["FreeSpace"]);
                        }
                    }
                    totalDisk = totalDisk / (1024 * 1024 * 1024);
                    freeDisk = freeDisk / (1024 * 1024 * 1024);
                }
                totalMemory = totalMemory / (1024 * 1024 * 1024);
                TimeSpan span = DateTime.Now - startDate;
                if (span.Days > 0)
                {
                    runSpan = string.Format("{0}天{1}小时", span.Days, (span.TotalHours - 24 * span.Days).ToString("0.0"));
                }
                else
                {
                    runSpan = string.Format("{0}小时", span.TotalHours.ToString("0.0"));
                }
                server.Name = name;
                server.IP = ip;
                server.Model = model;
                server.InstallDate = installDate;
                server.StartDate = startDate;
                server.RunSpan = runSpan;
                server.OS = _OS;
                server.Bit = bit;
                server.CPU_Name = cpu;
                server.TotalMemory = totalMemory;
                server.TotalDisk = totalDisk;
                server.FreeDisk = freeDisk;
                return server;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
