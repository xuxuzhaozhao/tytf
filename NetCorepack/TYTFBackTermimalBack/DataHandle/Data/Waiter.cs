using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TYTFBackTermimalBack.DataHandle.Data
{
    public class Waiter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string WeiXinId { get; set; }
        public bool IsUsed { get; set; } = true;
    }
}
