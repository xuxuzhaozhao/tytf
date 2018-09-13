using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TYTFBackTermimalBack.DataHandle.Data
{
    public class Menu
    {
        public int Id { get; set; }
        public int MenuTypeId { get; set; }
        public string MenuTypeName { get; set; }
        public bool IsUsed { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public DateTime CreateTime { get; set; }
        public int Sort { get; set; }
    }
}
