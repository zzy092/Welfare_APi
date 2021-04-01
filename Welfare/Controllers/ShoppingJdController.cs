using BLL;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using Tools;
using Welfare.Common;
using Welfare.Models;
using Welfare.Models.Cart;
using Welfare.Models.HomePage;
using Welfare.Models.JDRequest;
using Welfare.Models.JDResponsSerialize;

namespace Welfare.Controllers
{

    [tokenCheckAttribute]
    public class ShoppingJdController : ApiController
    {
        const string ERRMESSAGE = "服务异常，请稍后重试";  //5001
        const string IMAGEHEADER = "jdImageUrlHeader";
        const string STOCKMSG = "请设置默认地址";
        #region 地址分类信息
        /// <summary>
        /// 获取地址
        /// </summary>
        /// <param name="token"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult getArea(string token, int parentId)
        {
            try
            {
                BaseBLL<Shopping_Area> bllArea = new BaseBLL<Shopping_Area>();
                var list = bllArea.GetList(a => a.parent_id == parentId && a.is_delete == 0)
                    .Select(a => new
                    {
                        area_id = a.area_id,
                        area_level = a.area_level,
                        area_name = a.area_name
                    })
                    .ToList();

                return Json(new Result
                {
                    success = true,
                    resultCode = "0000",
                    resultMessage = "",
                    result = list
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[获取Area]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }
        #endregion

        #region 商品信息
        /// <summary>
        /// 商品分类
        /// </summary>
        /// <param name="token"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult getCategory(string token, int parentId)
        {
            try
            {
                var tokenModel = tokenCustomerHelper.getCustomerToken(token);
                var corpid = tokenModel.corpid;
                BaseBLL<Cfg_Corp_Category> bllCorpCategory = new BaseBLL<Cfg_Corp_Category>();
                var list = bllCorpCategory.GetList(a => a.corp_id == corpid && a.parentId == parentId && a.is_delete == 0)
                    .Select(a => new
                    {
                        category_catId = a.catId,
                        category_name = a.name,
                        parentId = a.parentId
                    })
                    .ToList();

                return Json(new Result
                {
                    success = true,
                    resultCode = "0000",
                    resultMessage = "",
                    result = list
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[商品分类]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }



        /// <summary>
        /// 商品搜索
        /// </summary>
        /// <param name="sPamars"></param>
        /// <returns></returns>
        public IHttpActionResult seachGoods(parameterSeach sPamars)
        {
            try
            {
                var model = jdCommon.getseachSkus(sPamars);
                return Json(model);
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[商品搜索]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }

        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="sku"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult goodsDetail(long sku)
        {
            try
            {
                var model = jdCommon.getSkuDetail(sku);
                return Json(model);
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[商品详情]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }

        /// <summary>
        /// 商品图片
        /// </summary>
        /// <param name="sku"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult goodsImages(string sku)
        {
            try
            {
                var model = jdCommon.getSkuImages(sku);
                return Json(model);
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[商品图片]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }

        /// <summary>
        /// 同类sku
        /// </summary>
        [HttpPost]
        public IHttpActionResult skuGoodsSimilar(long skuId)
        {
            try
            {
                var model = jdCommon.getSkuSimilar(skuId);
                return Json(model);
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[同类sku]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }

        /// <summary>
        /// 商品价格[每次最多100个,逗号分隔]
        /// </summary>
        /// <param name="sku"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult goodsPrice(string sku)
        {
            try
            {
                var model = jdCommon.getSkuPrice(sku);
                return Json(model);
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[商品价格]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }

        /// <summary>
        /// 商品上下架[每次最多100个,逗号分隔]
        /// </summary>
        /// <param name="skus"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult goodsState(string skus)
        {
            try
            {
                var model = jdCommon.getSkuState(skus);
                return Json(model);
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[上下架]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }

        /// <summary>
        /// 是否可售[每次最多100个,逗号分隔]
        /// </summary>
        /// <param name="skuIds"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult goodsCheck(string skuIds)
        {
            try
            {
                var model = jdCommon.getSKuCheck(skuIds);
                return Json(model);
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[是否可售]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }


        /// <summary>
        /// 特定区域是否可售[每次最多100个,逗号分隔]
        /// </summary>
        /// <param name="paramsAreaCheck"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult goodsCheckAreaLimit(parameterCheckAreaLimit paramsAreaCheck)
        {
            try
            {
                var model = jdCommon.getCheckAreaLimt(paramsAreaCheck.skuIds, paramsAreaCheck.province, paramsAreaCheck.city, paramsAreaCheck.county, paramsAreaCheck.town);
                return Json(model);
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[特定区域是否可售]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }

        /// <summary>
        /// 库存[每次最多100个,逗号分隔]
        /// </summary>
        /// <param name="paramStock"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult goodsStock(parametereStock paramStock)
        {
            try
            {
                var model = jdCommon.getStock(paramStock.skuNums, paramStock.area);
                return Json(model);
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[库存]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }

        /// <summary>
        /// 详情汇总[商品详情、商品图片等等]
        /// </summary>
        /// <param name="sku"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult getAllGoodsDteil(long sku)
        {
            try
            {
                var tokenModel = tokenCustomerHelper.getCustomerToken();
                var sale = tokenModel.corpSale;
                var imgHeader = ConfigurationManager.AppSettings[IMAGEHEADER];
                SkuAllInfo skuAllInfo = new SkuAllInfo();

                //商品详情
                var modelSkuInfo = jdCommon.getSkuDetail(sku);
                if (modelSkuInfo.success == false || modelSkuInfo.result == null)
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = modelSkuInfo.resultMessage
                    });

                //商品图片
                var modelSkuImages = jdCommon.getSkuImages(sku.ToString());
                if (modelSkuImages.success)
                {
                    foreach (var item in modelSkuImages.result[sku.ToString()])
                    {
                        item.path = imgHeader + item.path;
                    }
                    skuAllInfo.skuImags = modelSkuImages.result[sku.ToString()];
                }
                else
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = modelSkuImages.resultMessage
                    });



                //同类商品sku
                var modelSkuSimilar = jdCommon.getSkuSimilar(sku);
                if (modelSkuSimilar.success)
                {
                    foreach (var item in modelSkuSimilar.result)
                    {
                        foreach (var item2 in item.saleAttrList)
                        {
                            item2.imagePath = imgHeader + item2.imagePath;
                        }
                    }
                    skuAllInfo.skuSimilar = modelSkuSimilar.result;
                }
                else
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = modelSkuSimilar.resultMessage
                    });


                //商品价格
                var modelPrice = jdCommon.getSkuPrice(sku.ToString());
                if (!(modelPrice.success && modelPrice.result.Count > 0))
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = modelPrice.resultMessage
                    });

                //上下架 略 从商品详情 state 取
                var modelSate = jdCommon.getSkuState(sku.ToString());
                if (modelSate.success && modelSate.result.Count > 0)
                {
                    if (modelSate.result[0].state == 1)
                        skuAllInfo.isState = true;
                    else
                        skuAllInfo.isState = false;
                }
                else
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = modelSate.resultMessage
                    });

                //可售性
                var modelCheck = jdCommon.getSKuCheck(sku.ToString());
                if (modelCheck.success && modelPrice.result.Count > 0)
                {
                    if (modelCheck.result[0].saleState == 1)
                        skuAllInfo.isCheck = true;
                    else
                        skuAllInfo.isCheck = false;
                }
                else
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = modelCheck.resultMessage
                    });

                string areaIds = "";
                string addressMsg = ""; //省市县
                //是否有默认地址
                var isMasterAddressOk = jdCommon.isMasterAddress(tokenModel.customerId, out areaIds, out addressMsg);
                if (isMasterAddressOk)
                {
                    skuAllInfo.isMasterAddress = true;
                    skuAllInfo.AddressMessage = addressMsg;
                    List<parameterSkuNums> listStokeParams = new List<parameterSkuNums>();
                    listStokeParams.Add(new parameterSkuNums()
                    {
                        num = 1,
                        skuId = sku
                    });
                    var jsonStr = JsonConvert.SerializeObject(listStokeParams);
                    var apiStockModel = jdCommon.getStock(jsonStr, areaIds);
                    if (apiStockModel.success == true)
                    {
                        if (apiStockModel.result.Count > 0 && apiStockModel.result[0].stockStateDesc == "有货")
                            skuAllInfo.isStock = true;
                        else
                            skuAllInfo.isStock = false;
                        skuAllInfo.stockMsg = apiStockModel.result[0].stockStateDesc;
                    }
                    else
                        return Json(new Result()
                        {
                            success = false,
                            resultCode = "1003",
                            resultMessage = apiStockModel.resultMessage
                        });

                }
                else
                {
                    skuAllInfo.isMasterAddress = false;
                    skuAllInfo.isStock = false;  //没有默认收货地址  库存默认 【有货】
                    skuAllInfo.stockMsg = STOCKMSG;
                }

                var listDetailImages = getImgUrl(modelSkuInfo.result.introduction);
                //梳理信息
                goodsDetail goodsdetail = new goodsDetail
                {
                    jdPrice = modelPrice.result[0].jdPrice,
                    price = modelPrice.result[0].price,
                    ptPrice = modelPrice.result[0].price * sale,
                    pttax = sale,
                    tax = modelPrice.result[0].tax,
                    name = modelSkuInfo.result.name,
                    wareQD = modelSkuInfo.result.wareQD,
                    weight = Convert.ToDecimal(modelSkuInfo.result.weight),
                    introduction = modelSkuInfo.result.introduction,
                    param = modelSkuInfo.result.param,
                    detailImgs = listDetailImages,
                    masterImage = imgHeader + modelSkuInfo.result.imagePath
                };

                //7天无理由
                skuAllInfo.goods = goodsdetail;
                var wuliyouMsg = "";
                bool isWuliYou = isCheckServic(modelCheck.result[0].noReasonToReturn, modelCheck.result[0].thwa, out wuliyouMsg);
                List<checkService> listCheckService = new List<checkService>();
                checkService service_01 = new checkService()
                {
                    isOk = isWuliYou,
                    message = wuliyouMsg
                };
                checkService service_02 = new checkService()
                {
                    isOk = Convert.ToBoolean(modelCheck.result[0].isSelf),
                    message = "自营"
                };
                checkService service_03 = new checkService()
                {
                    isOk = Convert.ToBoolean(modelCheck.result[0].isJDLogistics),
                    message = "京东配送"
                };

                listCheckService.Add(service_02);
                listCheckService.Add(service_03);
                listCheckService.Add(service_01);

                skuAllInfo.goodsService = listCheckService;

                return Json(new Result()
                {
                    success = true,
                    resultCode = "0000",
                    result = skuAllInfo
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[商品详情汇总接口]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }

        /// <summary>
        /// 获取字符串中img的url集合
        /// </summary>
        /// <param name="content">字符串</param>
        /// <returns></returns>
        private List<string> getImgUrl(string content)
        {
            Regex rg = new Regex("url\\([\"\']?(.*?)[\"\']?\\)", RegexOptions.IgnoreCase);

            var m = rg.Match(content);
            List<string> imgUrl = new List<string>();
            while (m.Success)
            {
                imgUrl.Add(m.Groups[1].Value); //这里就是图片路径                
                m = m.NextMatch();
            }
            return imgUrl;
        }
        private bool isCheckServic(int? noReasonToReturn, int? thwa, out string outMsg)
        {
            var noMsg = "";
            if (noReasonToReturn == null)
                noMsg = "支持7天无理由退货";
            else
            {
                switch (noReasonToReturn)
                {
                    case 0:
                    case 3:
                        noMsg = "不支持7天无理由退货";
                        break;
                    case 1:
                    case 5:
                    case 8:
                        noMsg = "支持7天无理由退货";
                        break;
                    case 2:
                        noMsg = "支持90天无理由退货";
                        break;
                    case 4:
                    case 7:
                        noMsg = "支持15天无理由退货";
                        break;
                    case 6:
                        noMsg = "支持30天无理由退货";
                        break;
                }
            }

            var thwaMsg = "";
            switch (thwa)
            {
                case 1:
                    thwaMsg = "支持7天无理由退货";
                    break;
                case 2:
                    thwaMsg = "支持7天无理由退货（拆封后不支持）";
                    break;
                case 3:
                    thwaMsg = "支持7天无理由退货（激活后不支持）";
                    break;
                case 4:
                    thwaMsg = "支持7天无理由退货（使用后不支持）";
                    break;
                case 5:
                    thwaMsg = "支持7天无理由退货（安装后不支持）";
                    break;
                case 12:
                    thwaMsg = "支持15天无理由退货";
                    break;
                case 13:
                    thwaMsg = "支持15天无理由退货（拆封后不支持）";
                    break;
                case 14:
                    thwaMsg = "支持15天无理由退货（激活后不支持）";
                    break;
                case 15:
                    thwaMsg = "支持15天无理由退货（使用后不支持）";
                    break;
                case 16:
                    thwaMsg = "支持15天无理由退货（安装后不支持）";
                    break;
                case 22:
                    thwaMsg = "支持30天无理由退货";
                    break;
                case 23:
                    thwaMsg = "支持30天无理由退货（安装后不支持）";
                    break;
                case 24:
                    thwaMsg = "支持30天无理由退货（拆封后不支持）";
                    break;
                case 25:
                    thwaMsg = "支持30天无理由退货（使用后不支持）";
                    break;
                case 26:
                    thwaMsg = "支持30天无理由退货（激活后不支持";
                    break;
            }

            if (noReasonToReturn == 0 || noReasonToReturn == 3)
            {
                outMsg = "不支持7天无理由退货";
                return false;
            }
            else if (thwa == null || thwa == 0)
            {
                outMsg = noMsg;
                return true;
            }
            else
            {
                outMsg = thwaMsg;
                return true;
            }
        }

        /// <summary>
        /// 首页商品
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult getHomePageProductInfo(int pageIndex, int pageSize)
        {
            try
            {
                //首页图片列表
                var imgHeader = ConfigurationManager.AppSettings[IMAGEHEADER];
                var tokenModel = tokenCustomerHelper.getCustomerToken();

                showHomeProductInfo resultModel = new showHomeProductInfo();
                int totalCount = 0;

                //每个公司有自己的独立商品表
                switch (tokenModel.corpid)
                {
                    case 1:
                        BaseBLL<Shopping_Product_Info> bllProducts = new BaseBLL<Shopping_Product_Info>();
                        var listProducts = bllProducts.GetPageList<Guid>(a => Guid.NewGuid(), true, a => a.is_delete == 0, pageSize, pageIndex, out totalCount).ToList();
                        foreach (var item in listProducts)
                        {
                            item.product_imagePath = imgHeader + item.product_imagePath;
                        }
                        resultModel.pageIndex = pageIndex;
                        resultModel.pageSize = pageSize;
                        resultModel.count = totalCount;
                        resultModel.listSkus = listProducts;
                        break;
                }



                return Json(new Result()
                {
                    success = true,
                    resultCode = "0000",
                    result = resultModel
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[获取首页商品信息]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }

        /// <summary>
        /// 获取首页信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult getHomePageInfo()
        {
            try
            {
                //轮播图
                //商品分类
                BaseBLL<Cfg_Shopping_HomePage_Category> bllHomePageCategory = new BaseBLL<Cfg_Shopping_HomePage_Category>();
                BaseBLL<Cfg_Shopping_HomePage_Img> bllHomePageImg = new BaseBLL<Cfg_Shopping_HomePage_Img>();
                var tokenModel = tokenCustomerHelper.getCustomerToken();

                var listHomePageCategory = bllHomePageCategory.GetList(a => a.is_delete == 0 && a.corp_id == tokenModel.corpid).OrderBy(a => a.orderSort).ToList();
                var listHomePageImg = bllHomePageImg.GetList(a => a.is_delete == 0 && a.corpid == tokenModel.corpid).ToList();
                showHomePageInfo showinfo = new showHomePageInfo()
                {
                    homeCarouselImg = listHomePageImg,
                    homeCategory = listHomePageCategory,
                };

                return Json(new Result()
                {
                    success = true,
                    resultCode = "0000",
                    result = showinfo
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[获取首页信息]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }
        #endregion

        #region 购物车业务
        /// <summary>
        /// [添加购物车]某商品
        /// </summary>
        /// <param name="shoppingCart"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult addSkuCart([FromBody] Shopping_Cart shoppingCart)
        {
            try
            {
                var singleShoppingNumber = Convert.ToInt32(ConfigurationManager.AppSettings["singleShoppingNumber"]);
                if (shoppingCart.product_num > singleShoppingNumber)
                {
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = $"单次购买数量最多{singleShoppingNumber}个。"
                    });
                }

                var cartMaxNumber = Convert.ToInt32(ConfigurationManager.AppSettings["cartMaxNumber"]);
                //用户
                var tokenModel = tokenCustomerHelper.getCustomerToken();
                BaseBLL<Shopping_Cart> bllCate = new BaseBLL<Shopping_Cart>();
                int customerId = tokenModel.customerId;
                var whereShoppingCate = bllCate.GetList(a => a.customer_id == customerId && a.is_delete == 0).ToList();

                if (whereShoppingCate.Count > 0)
                {
                    if (whereShoppingCate.Count() >= cartMaxNumber)
                    {
                        return Json(new Result()
                        {
                            success = false,
                            resultCode = "1003",
                            resultMessage = "购物车已满。"
                        });
                    }
                    else if (whereShoppingCate.Where(a => a.skuid == shoppingCart.skuid).FirstOrDefault() != null)
                    {
                        return Json(new Result()
                        {
                            success = false,
                            resultCode = "1003",
                            resultMessage = "购物车已存在该商品。"
                        });
                    }
                }

                ////商品详情
                //var modelSkuInfo = jdCommon.getSkuDetail(shoppingCart.skuid);
                //if (modelSkuInfo.success == false)
                //{
                //    return Json(new Result()
                //    {
                //        success = false,
                //        resultCode = modelSkuInfo.resultCode,
                //        resultMessage = modelSkuInfo.resultMessage
                //    });
                //}

                ////价格
                //var modelPrice = jdCommon.getSkuPrice(shoppingCart.skuid.ToString());
                //if (modelPrice.success == false || modelPrice.result.Count == 0)
                //{
                //    return Json(new Result()
                //    {
                //        success = false,
                //        resultCode = modelPrice.resultCode,
                //        resultMessage = modelPrice.resultMessage
                //    });
                //}

                var sale = jdCommon.getCorpSale(tokenModel.corpid);

                shoppingCart.create_time = DateTime.Now;
                shoppingCart.customer_id = tokenModel.customerId;
                shoppingCart.is_delete = 0;
                shoppingCart.modified_time = DateTime.Now;
                shoppingCart.product_id = -1;

                bllCate.Add(shoppingCart);

                return Json(new Result()
                {
                    success = true,
                    resultCode = "0000"
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[购物车:添加商品]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }

        /// <summary>
        /// 购物车[更新商品数量]
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="addNumber">增加数量（例如：1或-1）</param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult updateSkuCart([FromBody] Shopping_Cart shoppingCart)
        {
            try
            {
                string stockMessage = "有货";
                var singleShoppingNumber = Convert.ToInt32(ConfigurationManager.AppSettings["singleShoppingNumber"]);
                if (shoppingCart.product_num > singleShoppingNumber)
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = $"单次购买数量最多{singleShoppingNumber}个。"
                    });
                if (shoppingCart.product_num < 1)
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = "该商品起购数1"
                    });

                var tokenModel = tokenCustomerHelper.getCustomerToken();

                var strArea = "";
                bool isMassterAddressOk = jdCommon.isMasterAddress(tokenModel.customerId, out strArea);
                if (isMassterAddressOk)
                {
                    List<parameterSkuNums> listSKuNums = new List<parameterSkuNums>();
                    listSKuNums.Add(new parameterSkuNums()
                    {
                        num = shoppingCart.product_num,
                        skuId = shoppingCart.skuid
                    });

                    var stockModel = jdCommon.getStock(JsonConvert.SerializeObject(listSKuNums), strArea);
                    if (stockModel.result.Count > 0 && stockModel.result[0].stockStateDesc == "有货")
                        stockMessage = "有货";
                    else
                        stockMessage = "无货";
                }
                else
                    stockMessage = "库存:请设置默认地址";

                BaseBLL<Shopping_Cart> bllCart = new BaseBLL<Shopping_Cart>();
                var cartModel = bllCart.GetSingle(a => a.customer_id == tokenModel.customerId && a.is_delete == 0 && a.skuid == shoppingCart.skuid);
                cartModel.product_num = shoppingCart.product_num;
                cartModel.modified_time = DateTime.Now;
                var isok = bllCart.Modify(cartModel);

                return Json(new Result()
                {
                    success = isok,
                    resultCode = "0000",
                    resultMessage = stockMessage
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[购物车:更新商品]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }

        /// <summary>
        /// [删除购物车]某商品
        /// </summary>
        /// <param name="shoppingCart"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult deleteSkuCart([FromBody] Shopping_Cart shoppingCart)
        {
            try
            {
                var tokenModel = tokenCustomerHelper.getCustomerToken();
                int customerId = tokenModel.customerId;
                BaseBLL<Shopping_Cart> bllCart = new BaseBLL<Shopping_Cart>();
                var cartModel = bllCart.GetSingle(a => a.customer_id == customerId && a.is_delete == 0 && a.skuid == shoppingCart.skuid);
                cartModel.is_delete = 1;
                cartModel.modified_time = DateTime.Now;
                var isOk = bllCart.Modify(cartModel);
                return Json(new Result()
                {
                    success = isOk,
                    resultCode = "0000"
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[购物车:删除商品]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }

        /// <summary>
        /// 购物车[全部信息]
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult getCartAllInfo()
        {
            try
            {
                var tokenModel = tokenCustomerHelper.getCustomerToken();
                int customerId = tokenModel.customerId;
                var sale = jdCommon.getCorpSale(tokenModel.corpid);
                BaseBLL<Shopping_Cart> bllCart = new BaseBLL<Shopping_Cart>();
                var listCartModel = bllCart.GetList(a => a.customer_id == customerId && a.is_delete == 0).ToList();
                List<showCartInfo> listShowCart = new List<showCartInfo>();
                Dictionary<long, int> dirSku = new Dictionary<long, int>();
                foreach (var item in listCartModel)
                {
                    dirSku.Add(item.skuid, item.product_num);
                }

                string strArea = "";
                bool isMasterAddressOk = jdCommon.isMasterAddress(customerId, out strArea);
                List<parameterSkuNums> listSkuNmus = new List<parameterSkuNums>();
                List<long> listSku = new List<long>();

                foreach (var item in dirSku)
                {
                    if (isMasterAddressOk)
                    {
                        parameterSkuNums numsNumsModel = new parameterSkuNums()
                        {
                            num = item.Value,
                            skuId = item.Key
                        };
                        listSkuNmus.Add(numsNumsModel);
                    }
                    listSku.Add(item.Key);
                }

                var listPrice = jdCommon.getSkuPrice(string.Join(",", listSku.ToArray()));
                resultStock r_Stock = null;
                if (isMasterAddressOk)
                {
                    r_Stock = jdCommon.getStock(JsonConvert.SerializeObject(listSkuNmus), strArea);
                    if(r_Stock.success==false)
                        return Json(new Result()
                        {
                            success = false,
                            resultCode = "1003",
                             resultMessage=r_Stock.resultMessage
                        });
                }
                    

                foreach (var item in listCartModel)
                {
                    showCartInfo showModel = new showCartInfo()
                    {
                        img = item.product_img,
                        number = item.product_num,
                        productName = item.product_name,
                        skuid = item.skuid
                    };

                    if (isMasterAddressOk)
                        showModel.stockStateDesc = r_Stock.result.Where(a => a.skuId == item.skuid).FirstOrDefault().stockStateDesc;
                    else
                        showModel.stockStateDesc = STOCKMSG;


                    var itemNewPrice = listPrice.result.Where(a => a.skuId == item.skuid).FirstOrDefault();
                    if (itemNewPrice.price != item.product_price)
                    {
                        var nowPtPrice = itemNewPrice.price * sale;
                        decimal diffPrice = Math.Abs(nowPtPrice - item.product_ptprice);
                        showModel.isPriceChange = true;

                        if (item.product_price > itemNewPrice.price)
                            showModel.priceMessage = $"比加入时降{diffPrice}元";
                        else
                            showModel.priceMessage = $"比加入时涨{diffPrice}元";

                        //更新价格
                        item.product_ptprice = nowPtPrice;
                        item.product_jdprice = itemNewPrice.jdPrice;
                        item.product_price = itemNewPrice.price;
                        bllCart.Modify(item);
                    }
                    else
                        showModel.isPriceChange = false;

                    showModel.productPtPrice = item.product_ptprice;
                    showModel.productJdPrice = item.product_price;
                    showModel.sale = sale;
                    showModel.productPrice = itemNewPrice.price;

                    listShowCart.Add(showModel);
                }

                return Json(new Result()
                {
                    success = true,
                    resultCode = "0000",
                    result = listShowCart
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[获取购物车]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }



        /// <summary>
        /// 购物车Tips
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult getCartTips(string token)
        {
            try
            {
                var tokenModel = tokenCustomerHelper.getCustomerToken(token);
                int customerId = tokenModel.customerId;
                BaseBLL<Shopping_Cart> bllCart = new BaseBLL<Shopping_Cart>();
                var listModel = bllCart.GetList(a => a.customer_id == customerId && a.is_delete == 0);
                return Json(new Result()
                {
                    success = true,
                    resultCode = "0000",
                    result = listModel.Count()
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[购物车[共多少商品]]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }
        #endregion

    }
}
