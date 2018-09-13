using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TYTFBackTermimalBack.DataHandle.Config;
using TYTFBackTermimalBack.DataHandle.Data;
using TYTFBackTermimalBack.Models;

namespace TYTFBackTermimalBack.Api
{
    [Produces("application/json")]
    [Route("api/People")]
    public class WaiterController : Controller
    {
        [HttpPost, Route("save")]
        public object Save([FromBody]Waiter waiter)
        {
            string sql = "";
            if (waiter.Id > 0)
            {
                sql = $"UPDATE Waiter SET Name=@Name,WeiXinId=@WeiXinId,IsUsed=@IsUsed WHERE Id = @Id";
            }
            else
            {
                sql = $"INSERT INTO Waiter(WeiXinId, NAME, IsUsed) VALUES(@WeiXinId, @Name, @IsUsed)";
            }

            var xresult = new XResult();
            if (!XDataHelper.ExcuteNonQuery(sql, waiter))
            {
                xresult.code = 10000;
                xresult.message = "操作失败！";
            };
            return xresult;
        }
    }
}