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



        [HttpGet, Route("PrintOrder/{orderId}")]
        public object PrintOrder(string orderId)
        {
            string sqlOrder = "SELECT * FROM VM_Order WHERE OrderId = @orderId;";
            string sqlDetail = "SELECT * FROM VM_DetailOrder WHERE OrderId = @orderId;";
            var result = XDataHelper.ExcuteMultipleReader<VM_Order, VM_DetailOrder>(sqlOrder + sqlDetail, new { orderId });

            var xres = new XResult();
            var resPrint = Print(result);
            if (resPrint.ret == 0)
            {
                xres.message = "已将订单发送至打印机！";
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
        private FeiEReturnMessage Print(Tuple<IEnumerable<VM_Order>, IEnumerable<VM_DetailOrder>> result)
        {
            var order = result.Item1.FirstOrDefault();
            var detailOrders = result.Item2;

            string orderInfo = "";
            orderInfo = $"<CB>{order.PositionName}</CB><BR>";
            orderInfo += "名称　　　　　单价 数量   金额<BR>";
            orderInfo += "--------------------------------<BR>";
            foreach (var detail in detailOrders)
            {
                orderInfo += $"{detail.MenuName.PadRight(12-detail.MenuName.Length)} {detail.SinglePrice.ToString().PadRight(5)} {detail.Weight.ToString().PadRight(3)} {detail.ShouldPrice.ToString().PadLeft(6)}<BR>";
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
    }
}