using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Welfare.Models.UserAddress
{
    /// <summary>
    /// 添加收获地址
    /// </summary>
    public class showUserAddress
    {
        public int addressId { get; set; }
        public int customerId { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string fixedTelephone { get; set; }
        public string address { get; set; }
        public string areaIds { get; set; }
        public string provinceName { get; set; }
        public string cityName { get; set; }
        public string countyName { get; set; }
        public string townName { get; set; }

    }
}