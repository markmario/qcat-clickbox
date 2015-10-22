using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClickBox.Web.Models
{
    public class ChargeData
    {
        public int Quantity { get; set; }
        public string Organisation { get; set; }
        public string TokenId { get; set; }
        public string Email { get; set; }
        public string ProductId { get; set; }
        public decimal Price { get; set; }
    }
}