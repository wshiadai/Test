using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATTIOT.Model
{
    /// <summary>
    /// 服务器登录帐号
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 服务器IP
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}
