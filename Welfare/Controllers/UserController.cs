using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tools;
using Welfare.Common;
using Welfare.Models;
using Welfare.Models.UserAddress;
using WelfareApi.Models.User;

namespace Welfare.Controllers
{
    [tokenCheckAttribute]
    public class UserController : ApiController
    {
        /*
         * 收获地址
         * 更新头像
         * 个人资料
         * 我的消息
         * 客户建议
         * 修改密码
         */

        string OutMessage = "";

        #region 地址信息

        /// <summary>
        /// 添加地址
        /// </summary>
        /// <param name="paramAddress"></param>
        /// <returns></returns>
        public IHttpActionResult addUserAddress(pamarUserAddress paramAddress)
        {
            try
            {
                #region 参数校验
                if (string.IsNullOrEmpty(paramAddress.userName) || string.IsNullOrEmpty(paramAddress.phone) || paramAddress.provinceId <= 0 || paramAddress.cityId <= 0 || paramAddress.countyId <= 0)
                {
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = "参数格式错误",
                    });
                }

                if (RegexHelper.isMoblePhone(paramAddress.phone) == false)
                {
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = "手机号格式错误",
                    });
                }

                if (!string.IsNullOrEmpty(paramAddress.fixedTelephone))
                {
                    if (RegexHelper.isFixeTelephone(paramAddress.fixedTelephone) == false)
                    {
                        return Json(new Result()
                        {
                            success = false,
                            resultCode = "1003",
                            resultMessage = "固定电话格式错误",
                        });
                    }
                }

                if (!string.IsNullOrEmpty(paramAddress.email))
                {
                    if (RegexHelper.isEmail(paramAddress.email) == false)
                    {
                        return Json(new Result()
                        {
                            success = false,
                            resultCode = "1003",
                            resultMessage = "Email格式错误",
                        });
                    }
                }
                #endregion

                int isMaster = 0;
                var tokenModel = tokenCustomerHelper.getCustomerToken();
                int customerId = tokenModel.customerId;
                List<int> listAreaId = new List<int>();
                listAreaId.Add(paramAddress.provinceId);
                listAreaId.Add(paramAddress.cityId);
                listAreaId.Add(paramAddress.countyId);
                listAreaId.Add(paramAddress.townId);
                var arrAreaId = listAreaId.ToArray();
                var strAreaId = string.Join("_", arrAreaId);
                BaseBLL<Shopping_Customer_Address> bllAddress = new BaseBLL<Shopping_Customer_Address>();
                //查重 略
                var maxNumer = Convert.ToInt32(ConfigurationManager.AppSettings["addressMaxNumber"]);
                var listAddress = bllAddress.GetList(a => a.is_delete == 0 && a.customer_id == customerId).ToList();

                if (listAddress.Count == 0)
                    isMaster = 1;
                else if (listAddress.Count() >= maxNumer)
                {
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = "收获地址已达上限",
                    });
                }

                BaseBLL<Shopping_Area> bllArea = new BaseBLL<Shopping_Area>();
                var listArea = bllArea.GetList(a => arrAreaId.Contains(a.area_id));

                Shopping_Customer_Address customerAddress = new Shopping_Customer_Address()
                {
                    address_email = paramAddress.email,
                    address_fixed_telephone = paramAddress.fixedTelephone,
                    address_phone = paramAddress.phone,
                    address_user_name = paramAddress.userName,
                    area_str_ids = strAreaId,
                    city_name = listArea.Where(a => a.area_id == paramAddress.cityId).FirstOrDefault().area_name,
                    county_name = listArea.Where(a => a.area_id == paramAddress.countyId).FirstOrDefault().area_name,
                    create_time = DateTime.Now,
                    customer_address = paramAddress.address,
                    customer_id = customerId,
                    modified_time = DateTime.Now,
                    province_name = listArea.Where(a => a.area_id == paramAddress.provinceId).FirstOrDefault().area_name,
                    town_name = paramAddress.townId > 0 ? listArea.Where(a => a.area_id == paramAddress.townId).FirstOrDefault().area_name : "",
                    is_master = isMaster
                };

                bllAddress.Add(customerAddress);
                return Json(new Result()
                {
                    success = true,
                    resultCode = "0000"
                });

            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[添加收获地址]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = "服务异常，请稍后重试",
                });
            }
        }

        /// <summary>
        /// 删除地址
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult deleteUserAddress(int addressId)
        {
            try
            {
                var tokenModel = tokenCustomerHelper.getCustomerToken();
                int customerId = tokenModel.customerId;
                BaseBLL<Shopping_Customer_Address> bllAddress = new BaseBLL<Shopping_Customer_Address>();
                var addressModel = bllAddress.GetSingle(a => a.id == addressId && a.customer_id == customerId);
                addressModel.is_delete = 1;
                addressModel.modified_time = DateTime.Now;
                var isOk = bllAddress.Modify(addressModel);
                return Json(new Result()
                {
                    success = isOk,
                    resultCode = "0000"
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[删除用户地址]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = "服务异常，请稍后重试",
                });
            }
        }


        /// <summary>
        /// 修改地址
        /// </summary>
        /// <param name="paramAddress"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult updateUserAddress(pamarUserAddress paramAddress)
        {
            try
            {
                #region 参数校验
                if (string.IsNullOrEmpty(paramAddress.userName) || string.IsNullOrEmpty(paramAddress.phone) || paramAddress.provinceId <= 0 || paramAddress.cityId <= 0 || paramAddress.countyId <= 0)
                {
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = "参数格式错误",
                    });
                }

                if (RegexHelper.isMoblePhone(paramAddress.phone) == false)
                {
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = "手机号格式错误",
                    });
                }

                if (!string.IsNullOrEmpty(paramAddress.fixedTelephone))
                {
                    if (RegexHelper.isFixeTelephone(paramAddress.fixedTelephone) == false)
                    {
                        return Json(new Result()
                        {
                            success = false,
                            resultCode = "1003",
                            resultMessage = "固定电话格式错误",
                        });
                    }
                }

                if (!string.IsNullOrEmpty(paramAddress.email))
                {
                    if (RegexHelper.isEmail(paramAddress.email) == false)
                    {
                        return Json(new Result()
                        {
                            success = false,
                            resultCode = "1003",
                            resultMessage = "Email格式错误",
                        });
                    }
                }
                #endregion

                var tokenModel = tokenCustomerHelper.getCustomerToken();
                int customerId = tokenModel.customerId;
                List<int> listAreaId = new List<int>();
                listAreaId.Add(paramAddress.provinceId);
                listAreaId.Add(paramAddress.cityId);
                listAreaId.Add(paramAddress.countyId);
                listAreaId.Add(paramAddress.townId);
                var arrAreaId = listAreaId.ToArray();
                var strAreaId = string.Join("_", arrAreaId);
                BaseBLL<Shopping_Customer_Address> bllAddress = new BaseBLL<Shopping_Customer_Address>();
                var addressModel = bllAddress.GetSingle(a => a.is_delete == 0 && a.id == paramAddress.addressId);
                //查重 略
                BaseBLL<Shopping_Area> bllArea = new BaseBLL<Shopping_Area>();
                var listArea = bllArea.GetList(a => arrAreaId.Contains(a.area_id));

                {
                    addressModel.address_email = paramAddress.email;
                    if (paramAddress.fixedTelephone.Contains("*"))
                        addressModel.address_fixed_telephone = paramAddress.fixedTelephone;
                    if (paramAddress.phone.Contains("*"))
                        addressModel.address_phone = paramAddress.phone;
                    addressModel.address_user_name = paramAddress.userName;
                    addressModel.area_str_ids = strAreaId;
                    addressModel.city_name = listArea.Where(a => a.area_id == paramAddress.cityId).FirstOrDefault().area_name;
                    addressModel.county_name = listArea.Where(a => a.area_id == paramAddress.countyId).FirstOrDefault().area_name;
                    addressModel.customer_address = paramAddress.address;
                    addressModel.customer_id = customerId;
                    addressModel.modified_time = DateTime.Now;
                    addressModel.province_name = listArea.Where(a => a.area_id == paramAddress.provinceId).FirstOrDefault().area_name;
                    addressModel.town_name = paramAddress.townId > 0 ? listArea.Where(a => a.area_id == paramAddress.townId).FirstOrDefault().area_name : "";
                }

                var isOk = bllAddress.Modify(addressModel);
                return Json(new Result()
                {
                    success = isOk,
                    resultCode = "0000"
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[修改地址]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = "服务异常，请稍后重试",
                });
            }
        }

        /// <summary>
        /// 设置默认地址
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult setMasterAddress(int addressId)
        {
            try
            {
                var tokenModel = tokenCustomerHelper.getCustomerToken();
                int customerId = tokenModel.customerId;
                BaseBLL<Shopping_Customer_Address> bllAddress = new BaseBLL<Shopping_Customer_Address>();
                var listAddress = bllAddress.GetList(a => a.is_delete == 0 && a.customer_id == customerId).ToList();
                foreach (var item in listAddress)
                {
                    if (item.id == addressId)
                        item.is_master = 1;
                    else
                        item.is_master = 0;
                    bllAddress.Modify(item);
                }

                return Json(new Result()
                {
                    success = true,
                    resultCode = "0000"
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[设置默认地址]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = "服务异常，请稍后重试",
                });
            }
        }

        /// <summary>
        /// 全部地址
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult getAllUserAddress()
        {
            try
            {
                var tokenModel = tokenCustomerHelper.getCustomerToken();
                BaseBLL<Shopping_Customer_Address> bllAddress = new BaseBLL<Shopping_Customer_Address>();
                var listAddress = bllAddress
                    .GetList(a => a.is_delete == 0 && a.customer_id == tokenModel.customerId)
                    .ToList();
                foreach (var item in listAddress)
                {
                    item.address_phone = RegexHelper.vaguePhone(item.address_phone);
                }
                
                return Json(new Result()
                {
                    success = true,
                    resultCode = "0000",
                    result = listAddress
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[获取所有用户地址]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = "服务异常，请稍后重试",
                });
            }
        }

        /// <summary>
        /// 获取默认地址
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult getDefaultAddress()
        {
            try
            {
                var tokenModel = tokenCustomerHelper.getCustomerToken();
                BaseBLL<Shopping_Customer_Address> bllAddress = new BaseBLL<Shopping_Customer_Address>();
                var defAddress = bllAddress.GetSingle(a => a.is_delete == 0 && a.customer_id == tokenModel.customerId&&a.is_master==1);
                if (defAddress != null)
                    defAddress.address_phone = RegexHelper.vaguePhone(defAddress.address_phone);

                return Json(new Result()
                {
                    success = true,
                    resultCode = "0000",
                    result = defAddress
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[获取所有用户地址]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = "服务异常，请稍后重试",
                });
            }
        }

        #endregion

        #region 个人资料
        /// <summary>
        /// 获取个人中心信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult getUserInfo()
        {
            try
            {
                var tokenModel = tokenCustomerHelper.getCustomerToken();
                showUserMessage userMessage = new showUserMessage();
                using (Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities())
                {
                    var usermsg = (from welfareInfo in db.Welfare_Customer
                                   join corp in db.Welfare_Corporation on welfareInfo.corp_id equals corp.corp_id
                                   where welfareInfo.customer_id == tokenModel.customerId 
                                   select new showUserMessage()
                                   {
                                       corpName = corp.corp_name,
                                       headImage = welfareInfo.head_img,
                                       name = welfareInfo.customer_name,
                                       user_money = welfareInfo.user_money,
                                       user_point = welfareInfo.user_point,
                                        corpLogo=corp.logo_path
                                   }).ToList();
                    userMessage = usermsg[0];

                    var listWelf = (from cfgWelfar in db.Cfg_Welfare_Customer
                                    join welfar in db.Welfare_Enum on cfgWelfar.welfare_id equals welfar.id
                                    where cfgWelfar.customer_id == tokenModel.customerId
                                    select welfar).ToList();
                    userMessage.listWelfare = listWelf;
                }

                return Json(new Result()
                {
                    success = true,
                    resultCode = "0000",
                    result = userMessage
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[个人信息]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = "服务异常，请稍后重试",
                });
            }
        }
        #endregion
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
                    resultMessage = "服务异常，请稍后重试",
                });
            }
        }
    }
}
