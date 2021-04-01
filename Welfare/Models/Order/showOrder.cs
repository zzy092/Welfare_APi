using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Welfare.Models.Order
{
    /// <summary>
    /// 父与子
    /// </summary>
    public class showOrder : Shopping_Order_Master
    {
        public string create_time_str { get; set; }
        public string pay_time_str { get; set; }
        public List<Shopping_Order_Detail> listDetail { get; set; }
    }

    public class showOrderDetail: Shopping_Order_Detail
    {
        public string create_time_str { get; set; }
    }

    public class showFuOrderDetail
    {
       public List<showOrderDetail> listDetail { get; set; }
    }

    public class jdorder
    {
        public long jdOrderid { get; set; }
    }
}