using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TYTFBackTermimalBack.DataHandle.Data
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public int PositionId { get; set; }
        public string ClientName { get; set; }
        public string PositionName { get; set; }
        public string WaiterName { get; set; }
        public string WaiterId { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsBuyed { get; set; }
        public decimal ShouldPrice { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal ActuallyPrice { get; set; }
        public string Note { get; set; }
    }
}
