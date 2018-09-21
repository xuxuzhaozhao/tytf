using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TYTFBackTermimalBack.DataHandle.Config;
using TYTFBackTermimalBack.Models;

namespace TYTFBackTermimalBack.Api
{
    [Produces("application/json")]
    [Route("api/Printer")]
    public class PrinterController : Controller
    {
        public static string USER = "xuxuzhaozhao87@gmail.com";  //*必填*：登录管理后台的账号名
        public static string UKEY = "xJh8DzW4YUCFMK6u";//*必填*: 注册账号后生成的UKEY
        public static string SN = "817513557";        //*必填*：打印机编号，必须要在管理后台里手动添加打印机或者通过API添加之后，才能调用API

        public static string URL = "http://api.feieyun.cn/Api/Open/";//不需要修改



        [HttpGet, Route("PrintOrder/{orderId}/{printonly}")]
        public object PrintOrder(string orderId, int printonly)
        {
            string sqlOrder = "SELECT * FROM VM_Order WHERE OrderId = @orderId;";
            string sqlDetail = "SELECT * FROM VM_DetailOrder WHERE OrderId = @orderId;";
            var result = XDataHelper.ExcuteMultipleReader<VM_Order, VM_DetailOrder>(sqlOrder + sqlDetail, new { orderId });

            var xres = new XResult();
            var resPrint = PrintSanLiangDan(printonly, result);
            if (resPrint.ret == 0)
            {
                xres.message = "已将订单发送至打印机！";
            }
            else if (resPrint.ret == 99)
            {
                xres.message = resPrint.msg;
                xres.code = printonly != 0 ? resPrint.ret : 20000;
            }
            else
            {
                xres.code = 10000;
                xres.message = $"打印机错误码为：{resPrint.ret}，错误信息为：{resPrint.msg}";
            }

            return xres;
        }

        //标签说明：
        //单标签: 
        //"<BR>"为换行,"<CUT>"为切刀指令(主动切纸,仅限切刀打印机使用才有效果)
        //"<LOGO>"为打印LOGO指令(前提是预先在机器内置LOGO图片),"<PLUGIN>"为钱箱或者外置音响指令
        //成对标签：
        //"<CB></CB>"为居中放大一倍,"<B></B>"为放大一倍,"<C></C>"为居中,<L></L>字体变高一倍
        //<W></W>字体变宽一倍,"<QR></QR>"为二维码,"<BOLD></BOLD>"为字体加粗,"<RIGHT></RIGHT>"为右对齐
        private FeiEReturnMessage Print(string orderInfo)
        {
            orderInfo = Uri.EscapeDataString(orderInfo);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(URL);
            req.Method = "POST";
            UTF8Encoding encoding = new UTF8Encoding();

            string postData = "sn=" + SN;
            postData += ("&content=" + orderInfo);
            postData += ("&times=" + "1");//默认1联

            int itime = DateTimeToStamp(DateTime.Now.AddHours(12));//时间戳秒数
            string stime = itime.ToString();
            string sig = sha1(USER, UKEY, stime);

            //公共参数
            postData += ("&user=" + USER);
            postData += ("&stime=" + stime);
            postData += ("&sig=" + sig);
            postData += ("&apiname=" + "Open_printMsg");

            byte[] data = encoding.GetBytes(postData);

            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = data.Length;
            Stream resStream = req.GetRequestStream();

            resStream.Write(data, 0, data.Length);
            resStream.Close();

            HttpWebResponse response;
            string strResult;
            try
            {
                response = (HttpWebResponse)req.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                strResult = reader.ReadToEnd();
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
                strResult = response.StatusCode.ToString();//错误信息
            }

            response.Close();
            req.Abort();
            //服务器返回的JSON字符串，建议要当做日志记录起来
            var res = JsonConvert.DeserializeObject<FeiEReturnMessage>(strResult);
            return res;
        }

        private FeiEReturnMessage PrintSanLiangDan(int 打印选项, Tuple<IEnumerable<VM_Order>, IEnumerable<VM_DetailOrder>> result)
        {
            var order = result.Item1.First();
            var detailOrders = result.Item2.ToList();

            List<VM_DetailOrder> 大厨房 = new List<VM_DetailOrder>();

            List<VM_DetailOrder> 凉菜房 = new List<VM_DetailOrder>();
            foreach (var detail in detailOrders)
            {
                if (detail.Description != null && (detail.Description.Contains("（凉菜）") || detail.Description.Contains("(凉菜)")))
                {
                    凉菜房.Add(detail);
                }
                else
                {
                    大厨房.Add(detail);
                }
            }

            switch (打印选项)
            {
                case 0:
                    {
                        string message = "";
                        var zong = Print总台(order, detailOrders);
                        var da = Print大厨房(order, 大厨房);
                        if (da.ret == 99)
                        {
                            message += "但是没有大厨房的菜品。";
                            zong.ret = 99;
                        }
                        var liang = Print凉菜房(order, 凉菜房);
                        if (liang.ret == 99)
                        {
                            message += "但是没有凉菜房的菜品。";
                            zong.ret = 99;
                        }
                        zong.msg = "已将订单发送至打印机！" + message;
                        return zong;
                    }
                case 1:
                    return Print总台(order, detailOrders);
                case 2:
                    return Print大厨房(order, 大厨房);
                case 3:
                    return Print凉菜房(order, 凉菜房);
                default:
                    throw new Exception("打印设置有错，问一哈徐程意！");
            }
        }

        private FeiEReturnMessage Print总台(VM_Order order, List<VM_DetailOrder> 总台)
        {
            if (总台.Count() == 0)
            {
                throw new Exception("没有点任何菜品，无法打印！");
            }
            //打印总单
            string orderInfo = "";
            orderInfo = $"<CB>{order.PositionName}</CB><BR>";
            orderInfo = $"<BOLD>总台 留</BOLD>";
            orderInfo += "名称　　　　　单价 数量   金额<BR>";
            orderInfo += "--------------------------------<BR>";
            foreach (var detail in 总台)
            {
                orderInfo += $"{detail.MenuName.PadRight(12 - detail.MenuName.Length)} {detail.SinglePrice.ToString().PadRight(5)} {detail.Weight.ToString().PadRight(3)} {detail.ShouldPrice.ToString().PadLeft(6)}<BR>";
            }
            for (int i = 0; i < 6; i++)
            {
                orderInfo += "<BR>";
            }
            orderInfo += "--------------------------------<BR>";
            orderInfo += $"合计：￥{order.ShouldPrice}元<BR>";
            orderInfo += $"备注：{order.Note}<BR>";
            orderInfo += $"订餐时间：{order.CreateTime}<BR>";
            orderInfo += "--------------------------------<BR>";

            return Print(orderInfo);
        }

        private FeiEReturnMessage Print大厨房(VM_Order order, List<VM_DetailOrder> 大厨房)
        {
            if (大厨房.Count() == 0)
            {
                return new FeiEReturnMessage() { ret = 99, msg = "没有大厨房的菜品！" };
            }
            string orderInfo1 = "";
            orderInfo1 = $"<CB>{order.PositionName}</CB><BR>";
            orderInfo1 = $"<BOLD>大厨房</BOLD>";
            orderInfo1 += "名称　　　　　单价 数量   金额<BR>";
            orderInfo1 += "--------------------------------<BR>";
            foreach (var detail in 大厨房)
            {
                orderInfo1 += $"{detail.MenuName.PadRight(12 - detail.MenuName.Length)} {detail.SinglePrice.ToString().PadRight(5)} {detail.Weight.ToString().PadRight(3)} {detail.ShouldPrice.ToString().PadLeft(6)}<BR>";
            }
            for (int i = 0; i < 3; i++)
            {
                orderInfo1 += "<BR>";
            }
            orderInfo1 += "--------------------------------<BR>";
            orderInfo1 += $"备注：{order.Note}<BR>";
            orderInfo1 += $"订餐时间：{order.CreateTime}<BR>";
            orderInfo1 += "--------------------------------<BR>";

            return Print(orderInfo1);
        }

        private FeiEReturnMessage Print凉菜房(VM_Order order, List<VM_DetailOrder> 凉菜房)
        {
            if (凉菜房.Count() == 0)
            {
                return new FeiEReturnMessage() { ret = 99, msg = "没有凉菜房的菜品！" };
            }
            string orderInfo2 = "";
            orderInfo2 = $"<CB>{order.PositionName}</CB><BR>";
            orderInfo2 = $"<BOLD>凉菜房</BOLD>";
            orderInfo2 += "名称　　　　　单价 数量   金额<BR>";
            orderInfo2 += "--------------------------------<BR>";
            foreach (var detail in 凉菜房)
            {
                orderInfo2 += $"{detail.MenuName.PadRight(12 - detail.MenuName.Length)} {detail.SinglePrice.ToString().PadRight(5)} {detail.Weight.ToString().PadRight(3)} {detail.ShouldPrice.ToString().PadLeft(6)}<BR>";
            }
            for (int i = 0; i < 3; i++)
            {
                orderInfo2 += "<BR>";
            }
            orderInfo2 += "--------------------------------<BR>";
            orderInfo2 += $"备注：{order.Note}<BR>";
            orderInfo2 += $"订餐时间：{order.CreateTime}<BR>";
            orderInfo2 += "--------------------------------<BR>";

            return Print(orderInfo2);
        }

        public static string sha1(string user, string ukey, string stime)
        {
            var buffer = Encoding.UTF8.GetBytes(user + ukey + stime);
            var data = SHA1.Create().ComputeHash(buffer);

            var sb = new StringBuilder();
            foreach (var t in data)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString().ToLower();

        }


        private static int DateTimeToStamp(System.DateTime time)
        {
            //DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            return (int)(time - startTime).TotalSeconds;
        }
    }


    public class FeiEReturnMessage
    {
        public string msg { get; set; }
        public int ret { get; set; }
        public string data { get; set; }
        public int serverExecutedTime { get; set; }
    }

    public class VM_Order
    {
        public bool IsBuyed { get; set; }
        public string OrderId { get; set; }
        public DateTime CreateTime { get; set; }
        public string Note { get; set; }
        public string WaiterName { get; set; }
        public string PositionName { get; set; }
        public decimal? ShouldPrice { get; set; }
    }
    public class VM_DetailOrder
    {
        public string OrderId { get; set; }
        public string MenuName { get; set; }
        public decimal SinglePrice { get; set; }
        public decimal Weight { get; set; }
        public decimal ShouldPrice { get; set; }
        public string MenuTypeName { get; set; }
        public string Description { get; set; }
    }
}