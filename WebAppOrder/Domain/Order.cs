using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppOrder.Domain
{
    public sealed class Order
    {
        public int OrderNumber { get; set; }
        public string ItemName { get; set; }
        public float Price { get; set; }
    }
}
