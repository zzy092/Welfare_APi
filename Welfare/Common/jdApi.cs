using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Welfare.Models.JDRequest;

namespace Welfare.Common
{

    /// <summary>
    /// 京东自营 接口API
    /// </summary>
    public static class jdApi
    {

        /// <summary>
        /// 商品详情url
        /// </summary>
        public static string urlProductDetail = "https://bizapi.jd.com/api/product/getDetail?";
        /// <summary>
        /// 商品图片
        /// </summary>
        public static string urlProductImage = "https://bizapi.jd.com/api/product/skuImage?";
        /// <summary>
        /// 搜索商品
        /// </summary>
        public static string urlSeachProduct = "https://bizapi.jd.com/api/search/search?";
        /// <summary>
        /// 商品分类
        /// </summary>
        public static string urlSimilarSku = "https://bizapi.jd.com/api/product/getSimilarSku?";
        /// <summary>
        /// 商品价格
        /// </summary>
        public static string urlProductPrice = "https://bizapi.jd.com/api/price/getSellPrice?";
        /// <summary>
        /// 上下架
        /// </summary>
        public static string urlProductState = "https://bizapi.jd.com/api/product/skuState?";
        /// <summary>
        /// 是否可售
        /// </summary>
        public static string urlProductCheck = "https://bizapi.jd.com/api/product/check?";
        /// <summary>
        /// 商品在特定区域是否可售
        /// </summary>
        public static string urlAreaCheck = "https://bizapi.jd.com/api/product/checkAreaLimit?";
        /// <summary>
        /// 库存
        /// </summary>
        public static string urlStock = "https://bizapi.jd.com/api/stock/getNewStockById?";

        /// <summary>
        /// 提交订单
        /// </summary>
        public static string urlSubmitOrder = "https://bizapi.jd.com/api/order/submitOrder?";

        /// <summary>
        /// 提交订单
        /// </summary>
        public static string urlOrderInfo = "https://bizapi.jd.com/api/order/selectJdOrder?";

        /// <summary>
        /// 确认预占库存订单
        /// </summary>
        public static string urlOrderConfirm = "https://bizapi.jd.com/api/order/confirmOrder?";

        /// <summary>
        /// 查询订单运费
        /// </summary>
        public static string urlOrderFreight = "https://bizapi.jd.com/api/order/getFreight?";

        /// <summary>
        /// 接口复用
        /// </summary>
        /// <returns></returns>
        public static string requesUrl(Dictionary<string, string> dirPamars, string _url)
        {
            var uri = _url;
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.None };
            var httpclient = new HttpClient(handler);
            httpclient.BaseAddress = new Uri(uri);
            var content = new FormUrlEncodedContent(dirPamars);

            Task<HttpResponseMessage> response = httpclient.PostAsync(uri, content);
            response.Wait();
            Task<string> reString = response.Result.Content.ReadAsStringAsync();
            reString.Wait();
            string restr = reString.Result;
            return restr;
        }
    }
}