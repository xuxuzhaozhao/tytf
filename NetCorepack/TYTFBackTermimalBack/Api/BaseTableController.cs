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
    [Route("api/BaseTable")]
    public class BaseTableController : Controller
    {
        [HttpGet, Route("{tableName}/getlist")]
        public object GetList(string tableName)
        {
            string sql = $"SELECT * FROM {tableName.DelSpace()} ORDER BY Sort";
            var dt = XDataHelper.ExcuteReader<object>(sql);
            return new XResult()
            {
                data = dt
            };
        }

        [HttpPost, Route("{tableName}/save")]
        public object Save(string tableName, [FromBody]MenuType type)
        {
            string sql = "";
            if (type.Id > 0)
            {
                sql = $"UPDATE {tableName.DelSpace()} SET Name=@Name,Sort=@Sort WHERE Id = @Id";
            }
            else
            {
                sql = $"INSERT INTO {tableName.DelSpace()}(Sort, NAME) VALUES(@Sort, @Name)";
            }

            var xresult = new XResult();
            if (!XDataHelper.ExcuteNonQuery(sql, type))
            {
                xresult.code = 10000;
                xresult.message = "操作失败！";
            };
            return xresult;
        }

        [HttpDelete, Route("{tableName}/del/{id}")]
        public object Del(string tableName, int id)
        {
            string sql = $"DELETE FROM {tableName.DelSpace()} WHERE Id = @id";

            var xresult = new XResult();
            xresult.message = "删除成功";
            if (!XDataHelper.ExcuteNonQuery(sql, new { id }))
            {
                xresult.code = 10000;
                xresult.message = "操作失败！";
            };
            return xresult;
        }

        [HttpGet, Route("{tableName}/select")]
        public object GetSelectList(string tableName)
        {
            string extField = "";
            if (tableName.ToUpper() == "MENU")
            {
                extField = ",Price";
            }

            string sql = $"SELECT Id,Name{extField} FROM {tableName.DelSpace()} ORDER BY Sort";

            var result = XDataHelper.ExcuteReader<Select>(sql);

            return new XResult()
            {
                data = result
            };
        }
    }
}