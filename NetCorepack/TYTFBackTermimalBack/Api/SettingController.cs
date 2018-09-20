using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TYTFBackTermimalBack.DataHandle.Config;
using TYTFBackTermimalBack.Models;

namespace TYTFBackTermimalBack.Api
{
    [Produces("application/json")]
    [Route("api/Setting")]
    public class SettingController : Controller
    {
        [HttpPost, Route("save")]
        public object Save([FromBody]Setting setting)
        {
            string sql = "";
            sql = $"UPDATE TytfSetting SET Value=@Value WHERE Id = @Id";
            var xresult = new XResult();
            if (!XDataHelper.ExcuteNonQuery(sql, setting))
            {
                xresult.code = 10000;
                xresult.message = "操作失败！";
            };
            return xresult;
        }
    }

    public class Setting
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Sort { get; set; }
    }
}