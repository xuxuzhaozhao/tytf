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
    [Route("api/Order")]
    public class OrderController : Controller
    {
        [Route("getlist")]
        [HttpPost]
        public object GetList([FromBody]QueryOrderPagination page)
        {
            var selectSql = $@"select * from VM_Order {page.GetWhereSql()}
                            ORDER BY {page.Sort}
                            LIMIT {(page.Page - 1) * page.RowsPerPage},{page.RowsPerPage}";
            var countSql = $"SELECT COUNT(1) AS count FROM VM_Order {page.GetWhereSql()}";

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
        public object Save([FromBody]Order order)
        {
            if (order.Id <= 0)
            {
                throw new Exception("哦豁，没有获取到这个订单的Id。");
            }

            string sql = "";
            order.ActuallyPrice = order.ShouldPrice;
            order.DiscountRate = 1;
            sql = $@"UPDATE TotalOrder SET 
                        PositionId=@PositionId,
                        IsBuyed=@IsBuyed
                    WHERE Id = @Id";

            var xresult = new XResult();
            if (!XDataHelper.ExcuteNonQuery(sql, order))
            {
                xresult.code = 10000;
                xresult.message = "操作失败！";
            };
            return xresult;
        }

        /// <summary>
        /// 此Id为OrderId
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetDetail/{orderId}")]
        public object GetDetail(string orderId)
        {
            //北京与洛杉矶的时差有这么多，SELECT DATE_ADD(NOW(), INTERVAL 12 HOUR);
            var BeijinTime = XCommons.GetChineseDateTime();

            string sql = "SELECT * FROM VM_DetailOrder WHERE OrderId = @orderId";
            var result = XDataHelper.ExcuteReader<object>(sql, new { orderId });

            return new XResult()
            {
                data = result,
                message = BeijinTime.ToString("yyyy年MM月dd日 HH:mm:ss")
            };
        }

        //添加一个菜
        [HttpPost, Route("saveDetailOrder")]
        public object Save([FromBody]DetailOrder detailOrder)
        {
            string sql = "";
            if (detailOrder.Weight == 0)
            {
                detailOrder.Weight = 1;
            }
            detailOrder.ShouldPrice = detailOrder.SinglePrice * detailOrder.Weight;
            if (detailOrder.Id > 0)
            {
                sql = $@"UPDATE DetailOrder SET 
                            MenuId=@MenuId,
                            SinglePrice=@SinglePrice,
                            Weight=@Weight,
                            ShouldPrice=@ShouldPrice
                         WHERE Id = @Id";
            }
            else
            {
                sql = $@"INSERT INTO DetailOrder(MenuId, SinglePrice, Weight, ShouldPrice, OrderId) 
                         VALUES(@MenuId, @SinglePrice, @Weight, @ShouldPrice, @OrderId)";
            }

            var xresult = new XResult();
            if (!XDataHelper.ExcuteNonQuery(sql, detailOrder))
            {
                xresult.code = 10000;
                xresult.message = "操作失败！";
            };
            return xresult;
        }

        [HttpGet, Route("SubmitBuy/{orderId}")]
        public object SubmitBuy(string orderId)
        {
            string sql = $@"UPDATE TotalOrder SET 
                                IsBuyed=1
                            WHERE OrderId = @OrderId";

            var xresult = new XResult();
            if (!XDataHelper.ExcuteNonQuery(sql, new { orderId }))
            {
                xresult.code = 10000;
                xresult.message = "操作失败！";
            };
            return xresult;
        }

        [HttpGet, Route("NoBuy/{orderId}")]
        public object NoBuy(string orderId)
        {
            string sql = $@"UPDATE TotalOrder SET 
                                IsBuyed=0
                            WHERE OrderId = @OrderId";

            var xresult = new XResult();
            if (!XDataHelper.ExcuteNonQuery(sql, new { orderId }))
            {
                xresult.code = 10000;
                xresult.message = "操作失败！";
            };
            return xresult;
        }

        [HttpDelete, Route("DeleteOrder/{orderId}")]
        public object DeleteOrder(string orderId)
        {
            string sql = @"DELETE FROM TotalOrder WHERE OrderId=@orderId;
                           DELETE FROM DetailOrder WHERE OrderId=@orderId;";

            var xresult = new XResult();
            if (!XDataHelper.ExcuteNonQuery(sql, new { orderId }))
            {
                xresult.code = 10000;
                xresult.message = "操作失败！";
            };
            return xresult;
        }
    }
}