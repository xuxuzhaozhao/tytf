using Microsoft.AspNetCore.Mvc;
using TYTFBackTermimalBack.DataHandle.Config;
using TYTFBackTermimalBack.Models;

namespace TYTFBackTermimalBack.Api
{
    [Produces("application/json")]
    [Route("api/OrderSummary")]
    public class OrderSummaryController : Controller
    {
        [HttpPost, Route("GetList")]
        public object GetList()
        {
            string sql = $@"SELECT COUNT(1) AS 订单数量,
                                   SUM(ShouldPrice) AS 当日总销售价,
                                   DATE_FORMAT(CreateTime,'%Y-%m-%d') AS 日期 
                            FROM VM_Order WHERE IsBuyed = 1
                            GROUP BY DATE_FORMAT(CreateTime,'%Y-%m-%d')";

            var result = XDataHelper.ExcuteReader<object>(sql);
            return new XResult() { data = result };
        }
    }
}