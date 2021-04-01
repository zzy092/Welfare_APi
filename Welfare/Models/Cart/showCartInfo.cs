using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Welfare.Models.Cart
{
    public class showCartInfo
    {
        public long skuid { get; set; }
        public int number { get; set; }
        public string productName { get; set; }
        public string img{get;set;}
        public decimal productJdPrice { get; set; }
        public decimal productPtPrice { get; set; }
        public decimal productPrice { get; set; }
        public decimal sale { get; set; }
        public decimal corpSale { get; set; }
        /// <summary>
        /// 价格是否变动
        /// </summary>
        public bool isPriceChange { get; set; }
        public string priceMessage { get; set; }
        /// <summary>
        /// 库存信息
        /// </summary>
        public string stockStateDesc { get; set; }
    }
}