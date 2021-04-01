using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Welfare.Models.HomePage
{
    public class showHomePageInfo
    {
        public List<Cfg_Shopping_HomePage_Category> homeCategory { get; set; }
        public List<Cfg_Shopping_HomePage_Img> homeCarouselImg { get; set; }
        
    }

    public class showHomeProductInfo
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public int count { get; set; }
        public List<Shopping_Product_Info> listSkus { get; set; }
    }
}

