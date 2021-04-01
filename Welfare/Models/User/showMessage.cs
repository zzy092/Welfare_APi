using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WelfareApi.Models.User
{
    public class showUserMessage
    {
        public string name { get; set; }
        public string headImage { get; set; }
        public decimal user_point { get; set; }
        public decimal user_money { get; set; }
        public string corpName { get; set; }
        public string corpLogo { get; set; }
        public List<Welfare_Enum> listWelfare { get; set; }
    }
}