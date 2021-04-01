using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Welfare.Models
{
    /// <summary>
    /// API 返回模板
    /// </summary>
    public class Result
    {
        public bool success { get; set; }
        public string resultMessage { get; set; }
        public string resultCode { get; set; }
        public object result { get; set; }
    }

    public class Token
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public long refresh_token_expires { get; set; }
        public long token_time { get; set; }
        public int corpid { get; set; }
        public int customerId { get; set; }
        public decimal corpSale { get; set; }
    }

    /// <summary>
    /// JD 返回模板
    /// </summary>
    public class JDResult
    {
        public bool success { get; set; }
        public string resultMessage { get; set; }
        public string resultCode { get; set; }
    }
}