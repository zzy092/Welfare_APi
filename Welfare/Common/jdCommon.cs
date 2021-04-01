using BLL;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Welfare.Models.JDRequest;
using Welfare.Models.JDResponsSerialize;

namespace Welfare.Common
{
    public static class jdCommon
    {

        #region 公司
        /// <summary>
        /// 公司折扣
        /// </summary>
        /// <param name="corpid"></param>
        /// <returns></returns>
        public static decimal getCorpSale(int corpid)
        {
            BaseBLL<Cfg_Corp_Sale> bllCorpSale = new BaseBLL<Cfg_Corp_Sale>();
            return bllCorpSale.GetSingle(a => a.is_delete == 0 && a.corp_id == corpid).sale_value;
        } 
        #endregion

        #region 用户
        /// <summary>
        /// 获取商城 token
        /// </summary>
        /// <returns></returns>
        public static string getShoppingToken()
        {
            BaseBLL<Shopping_Token> bllShoppingToken = new BaseBLL<Shopping_Token>();
            return bllShoppingToken.GetSingle(a => a.token_type_name == "京东自营").access_token;
        }

        /// <summary>
        /// 是否有有默认收货地址
        /// </summary>
        /// <param name="customerid"></param>
        /// <param name="areaIds"></param>
        /// <returns></returns>
        public static bool isMasterAddress(int customerid, out string areaIds)
        {
            BaseBLL<Shopping_Customer_Address> bllAddress = new BaseBLL<Shopping_Customer_Address>();
            var model = bllAddress.GetSingle(a => a.customer_id == customerid && a.is_delete == 0 && a.is_master == 1);
            if (model != null)
            {
                areaIds = model.area_str_ids;
                return true;
            }
            else
            {
                areaIds = "";
                return false;
            }
        }

        /// <summary>
        /// 是否有有默认收货地址
        /// </summary>
        /// <param name="customerid"></param>
        /// <param name="areaIds"></param>
        /// <returns></returns>
        public static bool isMasterAddress(int customerid, out string areaIds, out string addressMsg)
        {
            BaseBLL<Shopping_Customer_Address> bllAddress = new BaseBLL<Shopping_Customer_Address>();
            var model = bllAddress.GetSingle(a => a.customer_id == customerid && a.is_delete == 0 && a.is_master == 1);
            if (model != null)
            {
                addressMsg = $"{model.province_name} {model.city_name} {model.county_name} {model.town_name} {model.customer_address}";
                areaIds = model.area_str_ids;
                return true;
            }
            else
            {
                addressMsg = "";
                areaIds = "";
                return false;
            }
        }
        #endregion

        #region 商品
        /// <summary>
        /// 获取库存
        /// </summary>
        /// <param name="skuNums"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public static resultStock getStock(string skuNums, string area)
        {
            Dictionary<string, string> myPrivateDir = new Dictionary<string, string>();
            myPrivateDir.Add("token", getShoppingToken());
            myPrivateDir.Add("skuNums", skuNums);
            myPrivateDir.Add("area", area);
            var jdStr = jdApi.requesUrl(myPrivateDir, jdApi.urlStock);
            jdStr = jdStr.Replace("\"[", "[");
            jdStr = jdStr.Replace("]\"", "]");
            jdStr = jdStr.Replace("\\", "");
            var model = JsonConvert.DeserializeObject<resultStock>(jdStr);
            return model;
        }

        /// <summary>
        /// 特定区域是否可售
        /// </summary>
        /// <param name="skuIds"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="county"></param>
        /// <param name="town"></param>
        /// <returns></returns>
        public static resultCheckAreaLimit getCheckAreaLimt(string skuIds, string province, string city, string county, string town)
        {
            Dictionary<string, string> myPrivateDir = new Dictionary<string, string>();
            myPrivateDir.Add("token", getShoppingToken());
            myPrivateDir.Add("skuIds", skuIds);
            myPrivateDir.Add("province", province);
            myPrivateDir.Add("city", city);
            myPrivateDir.Add("county", county);
            myPrivateDir.Add("town", town);

            var jdStr = jdApi.requesUrl(myPrivateDir, jdApi.urlAreaCheck);
            jdStr = jdStr.Replace("\"[", "[");
            jdStr = jdStr.Replace("]\"", "]");
            jdStr = jdStr.Replace("\\", "");
            var model = JsonConvert.DeserializeObject<resultCheckAreaLimit>(jdStr);
            return model;
        }


        /// <summary>
        /// 是否可售
        /// </summary>
        /// <param name="skuIds"></param>
        /// <returns></returns>
        public static resultSkuCheck getSKuCheck(string skuIds)
        {
            Dictionary<string, string> myPrivateDir = new Dictionary<string, string>();
            myPrivateDir.Add("token", getShoppingToken());
            myPrivateDir.Add("skuIds", skuIds);
            myPrivateDir.Add("queryExts", "noReasonToReturn,thwa,isSelf,isJDLogistics");
            var jdStr = jdApi.requesUrl(myPrivateDir, jdApi.urlProductCheck);
            var model = JsonConvert.DeserializeObject<resultSkuCheck>(jdStr);

            return model;
        }

        /// <summary>
        /// 上下架
        /// </summary>
        /// <param name="skuids"></param>
        /// <returns></returns>
        public static resultSkuState getSkuState(string skuids)
        {
            Dictionary<string, string> myPrivateDir = new Dictionary<string, string>();
            myPrivateDir.Add("token", getShoppingToken());
            myPrivateDir.Add("sku", skuids);
            var jdStr = jdApi.requesUrl(myPrivateDir, jdApi.urlProductState);
            var model = JsonConvert.DeserializeObject<resultSkuState>(jdStr);
            return model;
        }

        /// <summary>
        /// 价格
        /// </summary>
        /// <param name="skuids"></param>
        /// <returns></returns>
        public static resultSellPrice getSkuPrice(string skuids)
        {
            Dictionary<string, string> myPrivateDir = new Dictionary<string, string>();
            myPrivateDir.Add("token", getShoppingToken());
            myPrivateDir.Add("sku", skuids);
            myPrivateDir.Add("queryExts", "containsTax ");
            var jdStr = jdApi.requesUrl(myPrivateDir, jdApi.urlProductPrice);
            var model = JsonConvert.DeserializeObject<resultSellPrice>(jdStr);
            return model;
        }

        

        /// <summary>
        /// 返回sku商品名
        /// </summary>
        /// <param name="sku"></param>
        /// <returns></returns>
        public static string getSkuName(long sku)
        {

            var model = getSkuDetail(sku);
            if (model.success == true)
                return model.result.name;
            else
                return "";
        }
        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="sku"></param>
        /// <returns></returns>
        public static resultProductDetail getSkuDetail(long sku)
        {
            Dictionary<string, string> myPrivateDir = new Dictionary<string, string>();
            myPrivateDir.Add("token", getShoppingToken());
            myPrivateDir.Add("sku", sku.ToString());
            //myPrivateDir.Add("queryExts", "LowestBuy,isJDLogistics,isSelf"); //起购数量 
            var jdStr = jdApi.requesUrl(myPrivateDir, jdApi.urlProductDetail);
            var model = JsonConvert.DeserializeObject<resultProductDetail>(jdStr);

            return model;
        }

        /// <summary>
        /// 订单详情
        /// </summary>
        /// <param name="sku"></param>
        /// <returns></returns>
        public static resultOrderBaseInfo getOrderBaseInfo(long jdorderId)
        {
            Dictionary<string, string> dir = new Dictionary<string, string>();
            dir.Add("token", jdCommon.getShoppingToken());
            dir.Add("jdOrderId", jdorderId.ToString());
            var jdStr = jdApi.requesUrl(dir, jdApi.urlOrderInfo);
            var model = JsonConvert.DeserializeObject<resultOrderBaseInfo>(jdStr);

            return model;
        }

        /// <summary>
        /// 父单
        /// </summary>
        /// <param name="sku"></param>
        /// <returns></returns>
        public static resultOrderParentInfo getOrderParentInfo(long jdorderId)
        {
            Dictionary<string, string> dir = new Dictionary<string, string>();
            dir.Add("token", jdCommon.getShoppingToken());
            dir.Add("jdOrderId", jdorderId.ToString());
            var jdStr = jdApi.requesUrl(dir, jdApi.urlOrderInfo);
            var model = JsonConvert.DeserializeObject<resultOrderParentInfo>(jdStr);

            return model;
        }

        /// <summary>
        /// 拆单
        /// </summary>
        /// <param name="sku"></param>
        /// <returns></returns>
        public static resultOrderInfo getOrderInfo(long jdorderId)
        {
            Dictionary<string, string> dir = new Dictionary<string, string>();
            dir.Add("token", jdCommon.getShoppingToken());
            dir.Add("jdOrderId", jdorderId.ToString());
            var jdStr = jdApi.requesUrl(dir, jdApi.urlOrderInfo);
            var model = JsonConvert.DeserializeObject<resultOrderInfo>(jdStr);

            return model;
        }

        /// <summary>
        /// 商品图片
        /// </summary>
        /// <param name="sku"></param>
        /// <returns></returns>
        public static resultImages getSkuImages(string sku)
        {
            Dictionary<string, string> myPrivateDir = new Dictionary<string, string>();
            myPrivateDir.Add("token", getShoppingToken());
            myPrivateDir.Add("sku", sku.ToString());
            var jdStr = jdApi.requesUrl(myPrivateDir, jdApi.urlProductImage);
            var model = JsonConvert.DeserializeObject<resultImages>(jdStr);
            return model;
        }

        /// <summary>
        /// 确认欲占库存订单
        /// </summary>
        /// <param name="jdorderId"></param>
        /// <returns></returns>
        public static resultConfirmOrder confirmOrder(long jdorderId)
        {
            Dictionary<string, string> myPrivateDir = new Dictionary<string, string>();
            myPrivateDir.Add("token", getShoppingToken());
            myPrivateDir.Add("jdOrderId", jdorderId.ToString());
            var jdStr = jdApi.requesUrl(myPrivateDir, jdApi.urlProductImage);
            var model = JsonConvert.DeserializeObject<resultConfirmOrder>(jdStr);

            return model;
        }
        public static resultSeach getseachSkus(parameterSeach sPamars)
        {
            Dictionary<string, string> dirPams = new Dictionary<string, string>();
            dirPams.Add("token", jdCommon.getShoppingToken());

            if (!string.IsNullOrEmpty(sPamars.keyword))
                dirPams.Add("keyword", sPamars.keyword);

            if (!string.IsNullOrEmpty(sPamars.catId))
                dirPams.Add("catId", sPamars.catId);

            if (sPamars.pageIndex > 0 && sPamars.pageSize > 0)
            {
                dirPams.Add("pageIndex", sPamars.pageIndex.ToString());
                dirPams.Add("pageSize", sPamars.pageSize.ToString());
            }

            if (!string.IsNullOrEmpty(sPamars.min))
                dirPams.Add("min", sPamars.min);

            if (!string.IsNullOrEmpty(sPamars.max))
                dirPams.Add("max", sPamars.max);

            if (!string.IsNullOrEmpty(sPamars.brands))
                dirPams.Add("brands", sPamars.brands);

            if (!string.IsNullOrEmpty(sPamars.cid1))
                dirPams.Add("cid1", sPamars.cid1);

            if (!string.IsNullOrEmpty(sPamars.cid2))
                dirPams.Add("cid2", sPamars.cid2);


            if (!string.IsNullOrEmpty(sPamars.sortType))
                dirPams.Add("sortType", sPamars.sortType);

            //价格汇总
            if (!string.IsNullOrEmpty(sPamars.priceCol))
                dirPams.Add("priceCol", sPamars.priceCol);

            //扩展属性汇总
            if (!string.IsNullOrEmpty(sPamars.extAttrCol))
                dirPams.Add("extAttrCol", sPamars.extAttrCol);

            //带库存有货
            if (sPamars.redisStore == true && !string.IsNullOrEmpty(sPamars.areaIds))
            {
                dirPams.Add("redisStore", sPamars.redisStore.ToString());
                dirPams.Add("areaIds", sPamars.areaIds);
            }

            string apiStr = jdApi.requesUrl(dirPams, jdApi.urlSeachProduct);
            var model = JsonConvert.DeserializeObject<resultSeach>(apiStr);

            return model;
        }

        /// <summary>
        /// 同类商品
        /// </summary>
        /// <param name="skuId"></param>
        /// <returns></returns>
        public static resultSimilar getSkuSimilar(long skuId)
        {
            Dictionary<string, string> myPrivateDir = new Dictionary<string, string>();
            myPrivateDir.Add("token", getShoppingToken());
            myPrivateDir.Add("skuId", skuId.ToString());
            var jdStr = jdApi.requesUrl(myPrivateDir, jdApi.urlSimilarSku);
            var model = JsonConvert.DeserializeObject<resultSimilar>(jdStr);
            return model;
        }

        /// <summary>
        /// 获取分类 父id
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public static Shopping_Category getParentId(int cid)
        {
            BaseBLL<Shopping_Category> bllCategory = new BaseBLL<Shopping_Category>();
            var model = bllCategory.GetSingle(a => a.is_delete == 0 && a.category_catId == cid);
            return model;
        }
        #endregion
    }
}