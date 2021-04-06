using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Welfare.Models.JDRequest;

namespace WelfareApi.Models.Order
{
    public class paramOrderFreight
    {
        public string defAddress { get; set; }
        public List<parameterSkuNums> skuUums { get; set; }
    }
}