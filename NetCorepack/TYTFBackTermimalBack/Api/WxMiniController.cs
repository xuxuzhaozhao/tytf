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
    [Route("api/WxMini")]
    public class WxMiniController : Controller
    {
        [HttpGet, Route("InitializeData")]
        public object InitializeData()
        {
            string sql = @"SELECT * FROM VM_Menu WHERE IsUsed = 1 ORDER BY Sort;
                           SELECT * FROM MenuType ORDER BY Sort;";

            var x = XDataHelper.ExcuteMultipleReader<Vm_MenuClass, MenuTypeClass>(sql);

            var goodsList = new List<Goods>();
            foreach (var item in x.Item2)
            {
                var good = new Goods();
                good.name = item.Name;
                foreach (var menu in x.Item1)
                {
                    if (menu.MenuTypeName == item.Name)
                    {
                        var food = new Food();
                        food.id = menu.Id;
                        food.icon = menu.RootUrl + menu.ICon;
                        food.name = menu.Name;
                        food.price = menu.Price;
                        food.pricetype = menu.PriceType;
                        food.description = menu.Description;

                        good.foods.Add(food);
                    }
                }
                goodsList.Add(good);
            }
            return new XResult() { data = goodsList };
        }

        [HttpPost, Route("CreateOrder")]
        public object CreateOrder([FromBody]OrderTemp order)
        {
           var waiter =  GetWaiter(order.WaiterName);

            string orderId = "";

            int resultCount = 1;
            while (resultCount > 0)
            {
                orderId = GenerateOrderId(); 

                string sqlExists = "SELECT OrderId FROM TotalOrder WHERE OrderId = @orderId";
                var result = XDataHelper.ExcuteReader<object>(sqlExists, new { orderId }).ToList();

                resultCount = result.Count;
            }

            order.OrderId = orderId;

            string sql = $@"INSERT INTO TotalOrder(OrderId,PositionId,ClientName,WaiterId,CreateTime,IsBuyed,Note)
                            VALUES(@OrderId,@PositionId,@ClientName,{waiter.Id},DATE_ADD(NOW(), INTERVAL 12 HOUR),0,@Note);";

            foreach (var item in order.detailTemps)
            {
                sql += $@"INSERT INTO DetailOrder(OrderId,MenuId,SinglePrice,Weight,ShouldPrice)
                          VALUES(@OrderId,{item.MenuId},'{item.SinglePrice}',{item.Weight},'{item.ShouldPrice}');";
            }

            var xresult = new XResult();
            xresult.data = orderId;
            if (!XDataHelper.ExcuteNonQuery(sql, order))
            {
                xresult.code = 10000;
                xresult.message = "操作失败！";
            };
            return xresult;
        }

        [HttpGet, Route("GetTablePosition")]
        public object GetTablePosition()
        {
            string sql = "SELECT * FROM TablePosition ORDER BY Sort";

            var result = XDataHelper.ExcuteReader<object>(sql);

            return new XResult() { data = result };
        }

        private static string GenerateOrderId()
        {
            Random ro = new Random();
            var id = ro.Next(1000, 9999);
            string orderId = $"tytf{DateTime.Now.ToString("yyyyMMddHHmmss")}{id}";
            return orderId;
        }

        [HttpGet, Route("GetWaiter/{waiterName}")]
        public Waiter GetWaiter(string waiterName)
        {
            string sql = "SELECT * FROM Waiter WHERE (WeiXinId=@waiterName OR Name=@waiterName ) AND IsUsed = 1";
            var result = XDataHelper.ExcuteReader<Waiter>(sql, new { waiterName });
            if (result.Count() == 0)
            {
                throw new Exception("非法账号！");
            }

            return result.First();
        }
    }

    public class Goods
    {
        public string name { get; set; }
        public List<Food> foods = new List<Food>();
    }

    public class Food
    {
        public int id { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public string pricetype { get; set; }
        public string description { get; set; }
        public int Count { get; set; } = 0;
        public string icon { get; set; }
    }

    public class OrderTemp
    {
        public string OrderId { get; set; }
        public int PositionId { get; set; }
        public string ClientName { get; set; }
        public string WaiterName { get; set; }
        public string Note { get; set; }

        public List<DetailTemp> detailTemps = new List<DetailTemp>();

    }
    public class DetailTemp
    {
        public int MenuId { get; set; }
        public decimal SinglePrice { get; set; }
        public int Weight { get; set; } = 1;
        public decimal ShouldPrice => SinglePrice * Weight;
    }

    public class Vm_MenuClass
    {
        public int Id { get; set; }
        public string MenuTypeName { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string PriceType { get; set; }
        public string Description { get; set; }
        public string RootUrl { get; set; }
        public string ICon { get; set; }
    }
    public class MenuTypeClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Sort { get; set; }
    }
}