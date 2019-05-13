using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using ATTIOT.Common;
using ATTIOT.Model;

namespace ATTIOT.Portal.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            List<DotNet> netList = new List<DotNet>();
            List<Java> javaList = new List<Java>();
            var netTemp = GetDotNetList(0);
            if (netTemp != null && netTemp.Count > 0)
            {
                netList = netTemp;
            }
            var javaTemp = GetJavaList(1);
            if (javaTemp != null && javaTemp.Count > 0)
            {
                javaList = javaTemp;
            }
            ViewBag.netList = netList;
            ViewBag.javaList = javaList;
            string file = StringHelper.GetConfigValue("site_list");
            ServerInfo server = StringHelper.GetSystemInfo();
            ViewBag.server = server;
            string version = StringHelper.GetConfigValue("_version");
            ViewBag.version = version;
            UserInfo user = new UserInfo();
            UserInfo _user = GetAccountList().FirstOrDefault();
            if (_user != null)
            {
                user = _user;
            }
            ViewBag.user = user;
            return View();
        }
        private List<DotNet> GetDotNetList(int index)
        {
            try
            {
                string file = StringHelper.GetConfigValue("site_list");
                DataTable dt = NpoiHelper.ExclImprotDataTable(file, index);  //将Excel文件转成DataTable
                List<DotNet> dotNetList = new List<DotNet>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        DotNet model = new DotNet();
                        model.No = Convert.ToInt32(row["编号"].ToString());
                        model.Name = row["项目名称"].ToString();
                        model.URL = row["访问地址"].ToString();
                        model.Port = row["端口"].ToString();
                        model.Path = row["项目路径"].ToString();
                        model.DB = row["对应数据库"].ToString();
                        model.Remark = row["备注"] != null ? row["备注"].ToString() : string.Empty;
                        dotNetList.Add(model);
                    }
                }
                return dotNetList;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private List<Java> GetJavaList(int index)
        {
            try
            {
                string file = StringHelper.GetConfigValue("site_list");
                DataTable dt = NpoiHelper.ExclImprotDataTable(file, index);
                List<Java> javaList = new List<Java>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        Java model = new Java();
                        model.No = Convert.ToInt32(row["编号"].ToString());
                        model.Name = row["项目名称"].ToString();
                        model.URL = row["访问地址"].ToString();
                        model.ShutdownPort = row["Shutdown端口"].ToString();
                        model.HttpPort = row["HTTP访问端口"].ToString();
                        model.AJPPort = row["AJP协议访问端口"].ToString();
                        model.Path = row["项目路径"].ToString();
                        model.DB = row["对应数据库"].ToString();
                        model.Remark = row["备注"] != null ? row["备注"].ToString() : string.Empty;
                        javaList.Add(model);
                    }
                }
                return javaList;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Download()
        {
            try
            {
                string path = StringHelper.GetConfigValue("site_list");
                FileInfo file = new FileInfo(path);
                if (file.Exists)
                {
                    using (FileStream fs = file.OpenRead())
                    {
                        byte[] bytes = new byte[file.Length];
                        int r = fs.Read(bytes, 0, bytes.Length);
                        Response.ClearHeaders();
                        Response.ContentType = "application/octet-stream";
                        Response.ContentEncoding = Encoding.UTF8;
                        Response.Charset = "";
                        Response.AppendHeader("Content-Disposition", string.Format("attachment;filename={0}", HttpUtility.UrlEncode(file.Name)));
                        Response.AppendHeader("Content-Transfer-Encoding", "binary");
                        Response.AppendHeader("Pragma", "public");
                        Response.AppendHeader("Cache-Control", "must-revalidate, post-check=0, pre-check=0");
                        Response.BinaryWrite(bytes);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取服务器登录帐号列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetUserList()
        {
            List<UserInfo> list = this.GetAccountList();
            return Json(list);
        }

        [HttpPost]
        public JsonResult GetProjectList()
        {
            List<Item> list = new List<Item>();
            var netTemp = GetDotNetList(0);
            if (netTemp != null && netTemp.Count > 0)
            {
                foreach (var item in netTemp)
                {
                    var itemModel = new Item();
                    itemModel.label = item.Name;
                    itemModel.id = string.Format("{0}_{1}", "dotnet", item.No);
                    list.Add(itemModel);
                }
            }
            var javaTemp = GetJavaList(1);
            if (javaTemp != null && javaTemp.Count > 0)
            {
                foreach (var item in javaTemp)
                {
                    var itemModel = new Item();
                    itemModel.label = item.Name;
                    itemModel.id = string.Format("{0}_{1}", "java", item.No);
                    list.Add(itemModel);
                }
            }
            return Json(list);
        }

        public ActionResult Test()
        {
            return View();
        }

        private List<UserInfo> GetAccountList()
        {
            List<UserInfo> list = new List<UserInfo>();
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\Content\\Server.json";
                FileInfo file = new FileInfo(path);
                if (file.Exists)
                {
                    using (FileStream fs = file.OpenRead())
                    {
                        byte[] bytes = new byte[file.Length];
                        int r = fs.Read(bytes, 0, bytes.Length);
                        string json = Encoding.ASCII.GetString(bytes);
                        json = json.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");
                        if (!string.IsNullOrEmpty(json))
                        {
                            list = JsonConvert.DeserializeObject<List<UserInfo>>(json);
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return list;
        }
    }
}
