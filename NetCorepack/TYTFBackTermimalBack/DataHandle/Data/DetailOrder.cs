using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TYTFBackTermimalBack.DataHandle.Data
{
    public class DetailOrder
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public int MenuId { get; set; }
        public decimal SinglePrice { get; set; }
        public decimal Weight { get; set; }
        public decimal ShouldPrice { get; set; }
    }
}
