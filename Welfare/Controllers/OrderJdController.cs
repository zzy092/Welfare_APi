using BLL;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Tools;
using Welfare.Common;
using Welfare.Models;
using Welfare.Models.JDRequest;
using Welfare.Models.JDResponsSerialize;
using Welfare.Models.Order;
using WelfareApi.Models.Order;

namespace Welfare.Controllers
{
    /// <summary>
    /// 订单业务[京东自营]
    /// </summary>
    public class OrderJdController : ApiController
    {
        private static object locker = new object();
        const string ERRMESSAGE = "服务异常，请稍后重试";  //5001
        /*
         * 提交订单
         * 待处理订单
         * 我的订单
         * 退换货
         */

        /// <summary>
        /// 提交订单
        /// </summary>
        /// <param name="paramModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult submitOrder([FromBody] pamarSubitOrder paramModel)
        {
            try
            {
                var tokenModel = tokenCustomerHelper.getCustomerToken();
                var corpSale = jdCommon.getCorpSale(tokenModel.corpid);
                var arrSkus = paramModel.skus.Select(a => a.skuId).ToList().ToArray();
                BaseBLL<Shopping_Cart> bllCate = new BaseBLL<Shopping_Cart>();
                var listDeleteCare = bllCate.GetList(a => arrSkus.Contains(a.skuid) && a.customer_id == tokenModel.customerId && a.is_delete == 0).ToList();
                BaseBLL<Shopping_Customer_Address> bllAddress = new BaseBLL<Shopping_Customer_Address>();
                BaseBLL<Shopping_JD_Order_Log> bllJdOrderLog = new BaseBLL<Shopping_JD_Order_Log>();

                string errorMessage = "";
                List<productDetail> listProductDetail = new List<productDetail>();
                //商品详情
                bool isSkuDetailOk = isAllSkuDetail(arrSkus, listDeleteCare, out listProductDetail, out errorMessage);
                if (isSkuDetailOk == false)
                {
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = $"京东未获取到以下商品信息:{errorMessage}"
                    });
                }
                //公司限制
                bool isSkuCategoryOk = isSkuCfgCategory(tokenModel.corpid, listProductDetail, out errorMessage);
                if (isSkuCategoryOk == false)
                {
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = $"受公司限制，以下商品不在销售范围:{errorMessage}"
                    });
                }

                //获取地址信息
                var addressModel = bllAddress.GetSingle(a => a.is_delete == 0 && a.id == paramModel.addressId);
                if (addressModel == null)
                {
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = "收获地址错误"
                    });
                }


                //京东下单
                var skuIds = string.Join(",", paramModel.skus.Select(a => a.skuId).ToList().ToArray());

                //获取价格
                List<OrderPriceSnap> outListOsnap = null;
                string outSellMessage = "";
                var isAllSale = isAllSellPrice(skuIds, out outListOsnap, out outSellMessage);
                if (isAllSale == false)
                {
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = outSellMessage,
                    });
                }

                //获取下单 
                var random = new Random();
                var code = random.Next(100, 999).ToString();//随机码
                string thirdOrder = DateTime.Now.ToString("yyyyMMddHHmmssfff") + code;
                var orderSku = getSubimtOrderSku(paramModel.skus);
                var orderParams = getSubmitParams(thirdOrder, JsonConvert.SerializeObject(outListOsnap), orderSku, addressModel);
                Dictionary<string, string> dirParams = SystemTypeHelper.Class4Dictionary(orderParams);
                string strResult = jdApi.requesUrl(dirParams, jdApi.urlSubmitOrder);
                resultSubmitOrder subOrderModel = null;
                try
                {
                    subOrderModel = JsonConvert.DeserializeObject<resultSubmitOrder>(strResult);
                    //保存
                    bllJdOrderLog.Add(new Shopping_JD_Order_Log()
                    {
                        submitState = 0,
                        freight = subOrderModel.result.freight,
                        jdOrderId = subOrderModel.result.jdOrderId,
                        orderNakedPrice = subOrderModel.result.orderNakedPrice,
                        orderPrice = subOrderModel.result.orderPrice,
                        orderTaxPrice = subOrderModel.result.orderTaxPrice,
                        sku = JsonConvert.SerializeObject(subOrderModel.result.sku),
                        thirdOrder = thirdOrder,
                        modified_time = DateTime.Now,
                        create_time = DateTime.Now
                    });
                    if (subOrderModel.success == false)
                        return Json(subOrderModel);
                }
                catch (Exception ex)
                {
                    BaseBLL<Welfare_System_Bug_Log> bllBugLog = new BaseBLL<Welfare_System_Bug_Log>();
                    bllBugLog.Add(new Welfare_System_Bug_Log()
                    {
                        create_time = DateTime.Now,
                        customer_id = tokenModel.customerId,
                        log_detail = strResult,
                        modified_time = DateTime.Now,
                        source_sn = thirdOrder,
                        title = "京东[提交订单]--序列化错误",
                        ex_message = ex + ""
                    });
                }


                //订单入库
                using (Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities())
                {
                    var Tran = db.Database.BeginTransaction();

                    decimal sysPayMoney = 0;
                    foreach (var item in subOrderModel.result.sku)
                    {
                        sysPayMoney += item.price * corpSale * item.num;
                    }
                    sysPayMoney += subOrderModel.result.freight;

                    bool isSubmitOk = true;
                    Shopping_Order_Master masterOrder = new Shopping_Order_Master()
                    {
                        customer_id = tokenModel.customerId,
                        province_name = addressModel.province_name,
                        city_name = addressModel.city_name,
                        county_name = addressModel.county_name,
                        town_name = addressModel.town_name,
                        user_address = addressModel.customer_address,
                        shopping_user_name = addressModel.address_user_name,
                        shopping_phone = addressModel.address_phone,
                        corp_sale = corpSale,

                        is_pay = 0,
                        freight = subOrderModel.result.freight,
                        orderNakedPrice = subOrderModel.result.orderNakedPrice,
                        orderTaxPrice = subOrderModel.result.orderTaxPrice,
                        order_jd_money = subOrderModel.result.orderPrice,
                        order_pt_money = sysPayMoney,
                        order_sn = subOrderModel.result.jdOrderId,
                        payment_method = -1, //积分支付
                        thirdOrder = thirdOrder,
                        create_time = DateTime.Now,
                        modified_time = DateTime.Now,
                        is_order_split = -1,
                        jd_order_state = 1,
                        logistics_state = 0
                    };

                    db.Shopping_Order_Master.Add(masterOrder);
                    var changeNum = db.SaveChanges();
                    if (changeNum <= 0)
                        isSubmitOk = false;
                    foreach (var item in subOrderModel.result.sku)
                    {
                        var modelcid_3 = jdCommon.getParentId(item.category);
                        var modelcid_2 = jdCommon.getParentId(modelcid_3.parentId);
                        var modelcid_1 = jdCommon.getParentId(modelcid_2.parentId);
                        Shopping_Order_Detail newOderDetail = new Shopping_Order_Detail()
                        {
                            cid_1 = modelcid_1 != null ? modelcid_1.category_catId : 0,
                            cid_2 = modelcid_2 != null ? modelcid_2.category_catId : 0,
                            cid_3 = item.category,
                            cid_1_name = modelcid_1 != null ? modelcid_1.category_name : "暂无",
                            cid_2_name = modelcid_2 != null ? modelcid_2.category_name : "暂无",
                            cid_3_name = modelcid_3 != null ? modelcid_3.category_name : "暂无",
                            create_time = DateTime.Now,
                            goods_jd_price = item.price,
                            goods_name = item.name,
                            goods_pt_price = item.price * corpSale,
                            goods_type = item.type,
                            jd_OrderId = -1,
                            modified_time = DateTime.Now,
                            nakedPrice = item.nakedPrice,
                            num = item.num,
                            oid = item.oid,
                            order_id = masterOrder.order_id,
                            skuId = item.skuId,
                            splitFreight = item.splitFreight,
                            tax = item.tax,
                            taxPrice = item.taxPrice,
                            img_path = listProductDetail.Where(a => a.sku == item.skuId).FirstOrDefault().imagePath
                        };
                        db.Shopping_Order_Detail.Add(newOderDetail);
                        changeNum = db.SaveChanges();
                        if (changeNum <= 0)
                            isSubmitOk = false;
                    }

                    //更新购物车
                    foreach (var item in listDeleteCare)
                    {
                        item.is_delete = 1;
                        item.modified_time = DateTime.Now;
                        bllCate.Modify(item);
                    }
                    var newCart = bllCate.GetList(a=>a.is_delete==0&&a.customer_id==tokenModel.customerId).ToList();

                    if (isSubmitOk == true)
                    {
                        Tran.Commit();
                        return Json(new Result()
                        {
                            success = true,
                            resultCode = "0000",
                            result = new
                            {
                                newCart=newCart,
                                order_sn= masterOrder.order_sn
                            }
                        }); 
                    }
                    else
                    {
                        Tran.Rollback();
                        return Json(new Result()
                        {
                            success = false,
                            resultCode = "1003",
                            resultMessage = "入库失败，请稍后再试"
                        });
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[提交订单]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }

        /// <summary>
        /// 支付订单
        /// </summary>
        /// <param name="order_sn">订单编号</param>
        /// <param name="payMethod">支付方式 [1,积分] [2,混合]，[3,微信]，[4,免积分]</param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult payOrder(long order_sn, int payMethod, string code)
        {
            try
            {
                string errMsg = "";
                var tokenModel = tokenCustomerHelper.getCustomerToken();
                BaseBLL<Welfare_Customer> bllCustomer = new BaseBLL<Welfare_Customer>();
                var customerModel = bllCustomer.GetSingle(a => a.customer_id == tokenModel.customerId);
                BaseBLL<Shopping_Order_Master> bllMaster = new BaseBLL<Shopping_Order_Master>();

                //订单状态校验
                var mastOrder = bllMaster.GetSingle(a => a.customer_id == tokenModel.customerId && a.order_sn == order_sn && a.is_delete == 0);
                if (mastOrder == null)
                    return Json(new Result
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = "订单不存在"
                    });

                var baseModel = jdCommon.getOrderBaseInfo(mastOrder.order_sn);
                if (baseModel.success == false)
                    return Json(baseModel);
                if (baseModel.result.orderState == 0)
                    return Json(new Result
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = "订单已取消"
                    });
                if (baseModel.result.submitState == 1)
                    return Json(new Result
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = "订单已支付"
                    });


                //校验短信验证码
                bool isPhoneMsgOk = phoneMsgHelper.isPhoneMsgCode(customerModel.mobile_phone, code, out errMsg);
                if (isPhoneMsgOk == false)
                    return Json(new Result
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = errMsg
                    });

                //价格校验
                decimal payMoney = 0;
                var isChange = isOrderPriceChange(order_sn, out payMoney, out errMsg);
                if (isChange)
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = errMsg
                    });

                switch (payMethod)
                {
                    case 1:  //积分
                        if (customerModel.user_point < payMoney)
                            return Json(new Result()
                            {
                                success = false,
                                resultCode = "1003",
                                resultMessage = "积分不足"
                            });

                        //积分扣除
                        lock(locker)
                        {
                            updateUserPoint(customerModel, payMoney, mastOrder, "京东订单付款", 1);
                        }

                        bool isConfirOk = true;

                        //确认预占库存接口
                        BaseBLL<Cfg_Corp_ConfirmOrder> bllCorpConfirm = new BaseBLL<Cfg_Corp_ConfirmOrder>();
                        var corpConfirm = bllCorpConfirm.GetSingle(a=>a.is_delete==0&&a.corpid==tokenModel.corpid);
                        if (corpConfirm!=null)
                        {
                            //var confirmOrderModer = jdCommon.confirmOrder(mastOrder.order_sn);
                            //if (!(confirmOrderModer.success == true && confirmOrderModer.result == true))
                            //    isConfirOk = false;
                        }
                        else
                        {
                            return Json(new Result()
                            {
                                success = false,
                                resultCode = "1003",
                                resultMessage = "该公司未配置发货权限"
                            });
                        }

                        if (isConfirOk==false)
                        {
                            updateUserPoint(customerModel, payMoney, mastOrder, "确认预占库存失败，积分退回", 2);
                            return Json(new Result()
                            {
                                success = false,
                                resultCode = "1003",
                                 resultMessage= "确认预占库存失败"
                            });
                        }
                        else
                            return Json(new Result()
                            {
                                success = true,
                                resultCode = "0000"
                            });
                    default:
                        return Json(new Result()
                        {
                            success = false,
                            resultCode = "1003",
                            resultMessage = "支付类型无效"
                        });
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[订单支付]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }

        /// <summary>
        /// 京东接口查询
        /// </summary>
        /// <param name="token"></param>
        /// <param name="jdorderid"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult getJDApiInfo( long jdorderid)
        {
            try
            {
                var baseModel = jdCommon.getOrderBaseInfo(jdorderid);
                if (baseModel.success == false)
                    return Json(baseModel);
                if (baseModel.result.type == 1)  //1:父单  2：拆单
                {
                    var orderFuModel = jdCommon.getOrderParentInfo(jdorderid);
                    return Json(orderFuModel);
                }
                else if (baseModel.result.type == 2)
                {
                    var OrderInfo = jdCommon.getOrderInfo(jdorderid);
                    return Json(OrderInfo);
                }
                else
                {
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = "查询异常"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[京东接口详情]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }

        /// <summary>
        /// 订单列表
        /// </summary>
        /// <param name="token"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult getOrders( int pageIndex = 1, int pageSize = 50)
        {
            try
            {
                int pageCount = 0;
                var tokenModel = tokenCustomerHelper.getCustomerToken();
                List<showOrder> listShowOrder = new List<showOrder>();
                BaseBLL<Shopping_Order_Master> bllOrderMaster = new BaseBLL<Shopping_Order_Master>();
                BaseBLL<Shopping_Order_Detail> bllOrderDetail = new BaseBLL<Shopping_Order_Detail>();
                var listMaster = bllOrderMaster.GetPageList<int>(a => a.order_id, true, a => a.customer_id == tokenModel.customerId && a.is_delete == 0, pageSize, pageIndex, out pageCount).ToList();
                foreach (var item in listMaster)
                {
                    showOrder sModel = new showOrder();
                    SystemTypeHelper.ClassT4ClassW(item, sModel);
                    sModel.create_time_str = sModel.create_time.ToString("F");
                    sModel.pay_time_str = sModel.pay_time == null ? "" : sModel.pay_time.Value.ToString("F");
                    var listDetail = bllOrderDetail.GetList(a => a.order_id == item.order_id).ToList();
                    sModel.listDetail = listDetail;
                    listShowOrder.Add(sModel);
                }

                return Json(new Result()
                {
                    success = true,
                    resultCode = "0000",
                    result = listShowOrder
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[订单列表]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }

        /// <summary>
        /// 订单详情
        /// </summary>
        /// <param name="token"></param>
        /// <param name="orderMastId"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult getOderDetail( int orderMastId)
        {
            try
            {
                var tokenModel = tokenCustomerHelper.getCustomerToken();

                BaseBLL<Shopping_Order_Detail> bllOrderDetail = new BaseBLL<Shopping_Order_Detail>();
                var listDetail = bllOrderDetail.GetList(a => a.is_delete == 0 && a.order_id == orderMastId).ToList();
                //获取分类集合
                var listGroupInfo = listDetail
                    .GroupBy(a => a.jd_OrderId)
                    .Select(G => new jdorder { jdOrderid = G.Key });

                List<showFuOrderDetail> listFuDetail = new List<showFuOrderDetail>();
                foreach (var item in listGroupInfo)
                {
                    List<Shopping_Order_Detail> listShowOrderDetail = listDetail.Where(a => a.jd_OrderId == item.jdOrderid).ToList();
                    List<showOrderDetail> listOrderDetail = new List<showOrderDetail>();
                    showFuOrderDetail fuModel = new showFuOrderDetail();

                    foreach (var itemData in listShowOrderDetail)
                    {
                        showOrderDetail itemDetail = new showOrderDetail();
                        SystemTypeHelper.ClassT4ClassW(itemData, itemDetail);
                        listOrderDetail.Add(itemDetail);
                    }

                    fuModel.listDetail = listOrderDetail;
                    listFuDetail.Add(fuModel);
                }

                return Json(new Result()
                {
                    success = true,
                    resultCode = "0000",
                    result = listFuDetail
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[订单详情]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }

        /// <summary>
        /// 获取订单运费
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult getOrderFreight([FromBody] paramOrderFreight queryOrderFreight)
        {
            try
            {
                var arrArea = queryOrderFreight.defAddress.Split('_');
                var sku = JsonConvert.SerializeObject(queryOrderFreight.skuUums);
                var model = jdCommon.getOrderFreight(sku,arrArea[0], arrArea[1], arrArea[2], arrArea[3],4);
                return Json(model);
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[获取订单运费]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }

        /// <summary>
        /// 复制粘贴用
        /// </summary>
        /// <returns></returns>
        private object muban()
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[主要业务]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = ERRMESSAGE,
                });
            }
        }

        /// <summary>
        /// 修改积分
        /// </summary>
        /// <param name="customer">用户信息</param>
        /// <param name="payMoney">变动金额</param>
        /// <param name="OrderMaster">订单信息</param>
        /// <param name="logMessage">日志详情</param>
        /// <param name="type">1:支出  2：入账</param>
        private void updateUserPoint(Welfare_Customer customer, decimal payMoney, Shopping_Order_Master OrderMaster, string logMessage, int type)
        {
            using (Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities())
            {
                var modelCustomer = db.Welfare_Customer.Where(a=>a.customer_id==customer.customer_id).FirstOrDefault();
                var beforePoint = modelCustomer.user_point;
                switch(type)
                {
                    case 1:
                        modelCustomer.user_point = modelCustomer.user_point - payMoney;
                        db.SaveChanges();
                        Welfare_Customer_Point_Log pointLog_1 = new Welfare_Customer_Point_Log()
                        {
                            before_point = beforePoint,
                            after_point = modelCustomer.user_point,
                            change_point = payMoney,
                            create_time = DateTime.Now,
                            customer_id = customer.customer_id,
                            is_delete = 0,
                            modified_time = DateTime.Now,
                            point_log_type = type,
                            point_remark = logMessage,
                            refer_number = OrderMaster.order_id,
                            source = type
                        };
                        db.Welfare_Customer_Point_Log.Add(pointLog_1);
                        db.SaveChanges();
                        break;
                    case 2:
                        modelCustomer.user_point = modelCustomer.user_point + payMoney;
                        db.SaveChanges();
                        Welfare_Customer_Point_Log pointLog_2 = new Welfare_Customer_Point_Log()
                        {
                            before_point = beforePoint,
                            after_point = modelCustomer.user_point,
                            change_point = payMoney,
                            create_time = DateTime.Now,
                            customer_id = customer.customer_id,
                            is_delete = 0,
                            modified_time = DateTime.Now,
                            point_log_type = type,
                            point_remark = logMessage,
                            refer_number = OrderMaster.order_id,
                            source = type
                        };
                        db.Welfare_Customer_Point_Log.Add(pointLog_2);
                        db.SaveChanges();
                        break;
                }
            }
        }

        /// <summary>
        /// 校验库里价格
        /// </summary>
        /// <returns></returns>
        private bool isOrderPriceChange(long order_sn, out decimal payMoney, out string ErrorMessage)
        {
            bool isok = false;
            string resultMsg = "";
            decimal resultPayMoney = 0;

            BaseBLL<Shopping_Order_Master> bllOrderMaster = new BaseBLL<Shopping_Order_Master>();
            var masterModel = bllOrderMaster.GetSingle(a => a.order_sn == order_sn && a.is_delete == 0);
            if (masterModel == null)
            {
                isok = true;
                resultMsg = "该订单已被删除或不存在";
            }

            BaseBLL<Shopping_Order_Detail> bllOrderDetail = new BaseBLL<Shopping_Order_Detail>();
            var listOrderDetail = bllOrderDetail.GetList(a => a.order_id == masterModel.order_id && a.is_delete == 0).ToList();
            foreach (var item in listOrderDetail)
            {
                resultPayMoney += item.goods_pt_price;
            }

            resultPayMoney += masterModel.freight;
            if (resultPayMoney != masterModel.order_pt_money)
            {
                isok = true;
                resultMsg = "价格记录异常";
            }

            if (resultPayMoney <= 0)
                isok = true;

            payMoney = resultPayMoney;
            ErrorMessage = resultMsg;

            return isok;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderPriceSnap"></param>
        /// <param name="sku"></param>
        /// <returns></returns>
        private parameterSubmitOrder getSubmitParams(string thirdOrder, string orderPriceSnap, string sku, Shopping_Customer_Address addressModel)
        {
            var arrArea = addressModel.area_str_ids.Split('_');
            parameterSubmitOrder orderParam = new parameterSubmitOrder()
            {
                token = jdCommon.getShoppingToken(),
                thirdOrder = thirdOrder,
                address = addressModel.customer_address,
                province = Convert.ToInt32(arrArea[0]),
                city = Convert.ToInt32(arrArea[1]),
                county = Convert.ToInt32(arrArea[2]),
                town = Convert.ToInt32(arrArea[3]),
                email = string.IsNullOrEmpty(addressModel.address_email) ? "123456@163.com" : addressModel.address_email,
                doOrderPriceMode = 1,
                orderPriceSnap = orderPriceSnap,
                sku = sku,
                name = addressModel.address_user_name,
                mobile = addressModel.address_phone,
                invoiceState = 2,
                invoiceType = 2,
                selectedInvoiceTitle = 4,
                invoiceContent = 1,
                invoicePhone = addressModel.address_phone,
                paymentType = 4,
                isUseBalance = 1,
                submitState = 0
            };

            return orderParam;
        }

        private string getSubimtOrderSku(List<parameterSkuNums> listSkuNums)
        {
            List<submitParamSku> listsubmitSku = new List<submitParamSku>();
            foreach (var item in listSkuNums)
            {
                submitParamSku sParam = new submitParamSku()
                {
                    bNeedAnnex = true,
                    bNeedGift = true,
                    num = item.num,
                    skuId = item.skuId
                };
                listsubmitSku.Add(sParam);
            }

            return JsonConvert.SerializeObject(listsubmitSku);
        }

        /// <summary>
        /// 获取下单 价格校验 信息
        /// </summary>
        /// <param name="skuids"></param>
        /// <param name="outListOsnap"></param>
        /// <param name="outErrorMessage"></param>
        /// <returns></returns>
        private bool isAllSellPrice(string skuids, out List<OrderPriceSnap> outListOsnap, out string outErrorMessage)
        {
            var model = jdCommon.getSkuPrice(skuids);
            List<OrderPriceSnap> listOrderPriceSnap = new List<OrderPriceSnap>();

            List<long> errorSku = new List<long>();
            string errorMsg = "";
            bool isOk = true;
            if (model.result.Count > 0)
            {
                var arrSku = skuids.Split(',');
                foreach (var item in arrSku)
                {

                    var sku = long.Parse(item);
                    var result = model.result.Where(a => a.skuId == sku).FirstOrDefault();
                    if (result == null)
                    {
                        errorSku.Add(sku);
                        isOk = false;
                    }
                    else
                    {
                        OrderPriceSnap oSnap = new OrderPriceSnap()
                        {
                            price = result.price,
                            skuId = result.skuId
                        };

                        listOrderPriceSnap.Add(oSnap);
                    }
                }
                if (errorSku.Count > 0)
                {
                    foreach (var item in errorSku)
                    {
                        var name = jdCommon.getSkuName(item);
                        if (string.IsNullOrEmpty(errorMsg))
                            errorMsg = $"【{name}】";
                        else
                            errorMsg += $",【{name}】";
                    }
                }

                outErrorMessage = errorMsg;
                outListOsnap = listOrderPriceSnap;
                return isOk;
            }
            else
            {
                outErrorMessage = "订单所有商品不可售";
                outListOsnap = null;
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skus"></param>
        /// <param name="outProductDetail"></param>
        /// <returns></returns>
        private bool isAllSkuDetail(long[] arrSku, List<Shopping_Cart> listCart, out List<productDetail> outProductDetail, out string outErrorMessage)
        {
            List<productDetail> resultProductDetail = new List<productDetail>();
            string errorMsg = "";
            bool isOk = true;
            foreach (var item in arrSku)
            {
                var model = jdCommon.getSkuDetail(item);
                if (model.success == true && model.result != null)
                {
                    resultProductDetail.Add(model.result);
                }
                else
                {
                    isOk = false;
                    var name = listCart.Where(a => a.skuid == item).FirstOrDefault().product_name;
                    if (string.IsNullOrEmpty(errorMsg))
                        errorMsg = $"【{name}】";
                    else
                        errorMsg += $",【{name}】";
                }
            }

            outProductDetail = resultProductDetail;
            outErrorMessage = errorMsg;
            return isOk;
        }

        /// <summary>
        /// sku分类 是否在 分类配置表中存在
        /// </summary>
        /// <param name="corpid"></param>
        /// <param name="listProductDetail"></param>
        /// <param name="outErrorMessage"></param>
        /// <returns></returns>
        private bool isSkuCfgCategory(int corpid, List<productDetail> listProductDetail, out string outErrorMessage)
        {
            bool isOk = true;
            string errorMsg = "";
            BaseBLL<Cfg_Corp_Category> bllCfgCategory = new BaseBLL<Cfg_Corp_Category>();
            var listCid3 = bllCfgCategory.GetList(a => a.corp_id == corpid && a.is_delete == 0 && a.catClass == 2).Select(a => a.catId).ToList();
            foreach (var item in listProductDetail)
            {
                var cid3 = Convert.ToInt32(item.category.Split(';')[2]);
                var isCategoryOk = listCid3.Contains(cid3);
                if (isCategoryOk == false)
                {
                    isOk = false;
                    if (string.IsNullOrEmpty(errorMsg))
                        errorMsg = $"【{item.name}】";
                    else
                        errorMsg += $",【{item.name}】";
                }
            }

            outErrorMessage = errorMsg;
            return isOk;
        }


    }
}
