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
    [Route("api/Menu")]
    public class MenuController : Controller
    {
        [Route("getlist")]
        [HttpPost]
        public object GetList([FromBody]QueryMenuPagination page)
        {
            var selectSql = $@"select * from VM_Menu {page.GetWhereSql()}
                            ORDER BY MenuTypeId,Sort
                            LIMIT {(page.Page - 1) * page.RowsPerPage},{page.RowsPerPage}";
            var countSql = $"SELECT COUNT(1) AS count FROM VM_Menu {page.GetWhereSql()}";

            var list = XDataHelper.ExcuteReader<object>(selectSql).ToList();

            var count = XDataHelper.ExcuteScalar<int>(countSql);

            var result = new XResult();
            result.data = new
            {
                content = list,
                count = count
            };
            return result;
        }

        [HttpPost, Route("save")]
        public object Save([FromBody]Menu menu)
        {
            string sql = "";
            if (menu.Id > 0)
            {
                sql = $@"UPDATE Menu SET 
                            Name=@Name,
                            MenuTypeId=@MenuTypeId,
                            IsUsed=@IsUsed, 
                            Price=@Price, 
                            PriceType=@PriceType, 
                            Description=@Description,
                            IsUsed=@IsUsed, 
                            Sort=@Sort
                         WHERE Id = @Id";
            }
            else
            {
                sql = $@"INSERT INTO Menu(MenuTypeId, NAME, IsUsed,Price,PriceType,Description,CreateTime,Sort) 
                         VALUES(@MenuTypeId, @Name, @IsUsed,@Price,@PriceType,@Description,DATE_ADD(NOW(), INTERVAL 12 HOUR),@Sort)";
            }

            var xresult = new XResult();
            if (!XDataHelper.ExcuteNonQuery(sql, menu))
            {
                xresult.code = 10000;
                xresult.message = "操作失败！";
            };
            return xresult;
        }
    }
}