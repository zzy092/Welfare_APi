using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tools;
using Welfare.Common;
using Welfare.Models;
using Welfare.Models.JDRequest;

namespace Welfare.Controllers
{
    public class AdminController : ApiController
    {

        const string ERRMESSAGE = "服务异常，请稍后重试";  //5001

        /// <summary>
        /// 根据分类 添加商品
        /// </summary>
        /// <param name="cid3"></param>
        /// <param name="addNumber"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult addSkus(string token, string cid3, int addNumber)
        {
            try
            {
                BaseBLL<Shopping_Product_Info> bllProduct = new BaseBLL<Shopping_Product_Info>();
                var tokenModel = tokenCustomerHelper.getCustomerToken(token);
                var corpSale = jdCommon.getCorpSale(tokenModel.corpid);
                int number = 0;
                parameterSeach pSeach = new parameterSeach()
                {
                    catId = cid3,
                    pageIndex = 1,
                    pageSize = 100
                };

                //获取分类信息
                var result = jdCommon.getseachSkus(pSeach).result;

                for (int i = 1; i < result.pageCount; i++)
                {
                    List<Shopping_Product_Info> listProduct = new List<Shopping_Product_Info>();
                    pSeach.pageIndex = i;
                    result = jdCommon.getseachSkus(pSeach).result;

                    //拿到本次所有sku
                    var lisSkusString = result.hitResult.Select(a => a.wareId).ToList();
                    //类型转换
                    var listSkuLong = SystemTypeHelper.string4long(lisSkusString);

                    var arrSkulong = listSkuLong.ToArray();
                    var dbSKu = bllProduct.GetList(a => arrSkulong.Contains(a.sku)).Select(a => a.sku).ToList().ToArray();  //已存在

                    //去除本次库里已存在的sku
                    foreach (var item in dbSKu)
                    {
                        listSkuLong.Remove(item);
                    }
                    var listSkuString = SystemTypeHelper.long4string(listSkuLong);
                    var arrSkuString = listSkuString.ToArray(); //库里没有的
                    result.hitResult = result.hitResult.Where(a => arrSkuString.Contains(a.wareId)).ToList();

                    foreach (var item in result.hitResult)
                    {
                        //获取价格
                        string skus = string.Join(",", arrSkuString);
                        var listPrice = jdCommon.getSkuPrice(skus);
                        var skuid = long.Parse(item.wareId);

                        Shopping_Product_Info model = new Shopping_Product_Info()
                        {
                            brandName = item.brand,
                            category = item.cid1 + "_" + item.cid2 + "_" + cid3,
                            cid_1 = Convert.ToInt32(item.cid1),
                            cid_2 = Convert.ToInt32(item.cid2),
                            cid_3 = Convert.ToInt32(item.catId),
                            create_time = DateTime.Now,
                            is_delete = 0,
                            is_sale_check = Convert.ToInt32(item.wyn),
                            is_sku_state = Convert.ToInt32(item.wstate),
                            modified_time = DateTime.Now,
                            product_imagePath = item.imageUrl,
                            product_jdPrice = listPrice.result.Where(a => a.skuId == skuid).FirstOrDefault().jdPrice,
                            product_name = item.wareName,
                            sku = skuid,
                            tax = listPrice.result.Where(a => a.skuId == skuid).FirstOrDefault().tax,
                            taxPrice = listPrice.result.Where(a => a.skuId == skuid).FirstOrDefault().taxPrice,
                            product_price = listPrice.result.Where(a => a.skuId == skuid).FirstOrDefault().price
                        };

                        listProduct.Add(model);
                    }

                    bllProduct.AddTran(listProduct);
                    number += listProduct.Count;
                    if (number >= addNumber)
                        break;
                }

                return Json(new Result()
                {
                    success = true,
                    resultCode = "0000",
                    resultMessage = ERRMESSAGE,
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[添加商品库]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }


    }
}

