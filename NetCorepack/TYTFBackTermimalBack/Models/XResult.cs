using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TYTFBackTermimalBack.Models
{
    public class XResult
    {
        public XResult()
        {
            this.code = 20000;
            this.message = "操作成功";
        }
        public XResult(int Code, string Message)
        {
            this.code = Code;
            this.message = Message;
        }
        public int code { get; set; }
        public object data { get; set; }
        public string message { get; set; }
    }
}
