using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Tools;
using Welfare.Models;
using System.Web.Services;
using Model;
using Welfare.Common;
using BLL;
using WelfareApi.Models.Login;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;

namespace Welfare.Controllers
{
  
    public class LoginController : ApiController
    {
        /// <summary>
        /// 帐号登陆
        /// </summary>
        /// <param name="eModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult accountLogin([FromBody]ELoginModel eModel)
        {
            try
            {
                if (string.IsNullOrEmpty(eModel.login))
                {
                    addLoginLog("", -1, eModel.device, 2, 0);
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = "姓名或密码不可为空",
                    });
                }

                //先解密
                string s_json = Tools.DES.AesDecrpt(eModel.login);
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(s_json));
                DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(paramLogin));
                paramLogin pLogin = (paramLogin)deseralizer.ReadObject(ms);

                var efreshTime = DateTimeHelper.GetTime(pLogin.refresh_expires.ToString());
                if (DateTime.Now > efreshTime)
                {
                    addLoginLog(pLogin.loginName, pLogin.corpid, eModel.device, 2, 0);
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = "登陆超时",
                    });
                }
                    

                if (string.IsNullOrEmpty(pLogin.loginName) && string.IsNullOrEmpty(pLogin.password))
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = "请求参数有误",
                    });

                BaseBLL<Welfare_Customer> bllCustomer = new BaseBLL<Welfare_Customer>();
                var listCustomer = bllCustomer.GetList(a => a.is_delete == 0 && a.login_name == pLogin.loginName).ToList();
                if (listCustomer.Count == 0)
                {
                    return Json(new Result
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = "帐号不存在"
                    });

                }

                var listCustomerPwd = listCustomer.Where(a => a.login_password == pLogin.password).ToList();
                if (listCustomerPwd.Count == 0)
                {
                    addLoginLog(pLogin.loginName,pLogin.corpid , pLogin.deviceType, 2, 0);
                    return Json(new Result
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = "密码错误"
                    });
                }

                var listCustomerCorp = listCustomerPwd.Where(a => a.corp_id == pLogin.corpid).ToList();
                if (listCustomerCorp.Count == 0)
                {
                    addLoginLog(pLogin.loginName, pLogin.corpid, pLogin.deviceType, 2, 0);
                    return Json(new Result
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = "登陆地址错误"
                    });
                }

                var listCustomerState = listCustomerCorp.Where(a => a.customer_state == 0).ToList();
                if (listCustomerState.Count == 0)
                {
                    addLoginLog(pLogin.loginName, pLogin.corpid, pLogin.deviceType, 2, 0);
                    return Json(new Result
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = "帐号已停用"
                    });
                }
                var customerModel = listCustomerState.FirstOrDefault();
                var objToken = getUserToken(customerModel);
                addLoginLog(customerModel.login_name,pLogin.corpid, pLogin.deviceType, 2);

                return Json(new Result
                {
                    success = true,
                    resultCode = "0000",
                    resultMessage = "",
                    result = objToken
                });

            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[用户登陆]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = "服务异常，请稍后重试",
                });
            }
        }
        ///// <summary>
        ///// 帐号登陆
        ///// </summary>
        ///// <param name="pLogin"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public IHttpActionResult accountLogin_old([FromBody] paramLogin pLogin)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(pLogin.loginName) && string.IsNullOrEmpty(pLogin.password))
        //        {
        //            return Json(new Result()
        //            {
        //                success = false,
        //                resultCode = "1003",
        //                resultMessage = "请求参数有误",
        //            });
        //        }

        //        BaseBLL<Welfare_Customer> bllCustomer = new BaseBLL<Welfare_Customer>();
        //        var listCustomer = bllCustomer.GetList(a => a.is_delete == 0 && a.login_name == pLogin.loginName).ToList();
        //        if (listCustomer.Count == 0)
        //        {
        //            return Json(new Result
        //            {
        //                success = false,
        //                resultCode = "1003",
        //                resultMessage = "帐号不存在"
        //            });

        //        }

        //        var  listCustomerPwd = listCustomer.Where(a => a.login_password == pLogin.password).ToList();
        //        if (listCustomerPwd.Count==0)
        //        {
        //            addLoginLog(listCustomer[0].customer_id, pLogin.deviceType, 2, 0);
        //            return Json(new Result
        //            {
        //                success = false,
        //                resultCode = "1003",
        //                resultMessage = "密码错误"
        //            });
        //        }

        //        var  listCustomerCorp = listCustomerPwd.Where(a => a.corp_id== pLogin.corpid).ToList();
        //        if (listCustomerCorp.Count == 0)
        //        {
        //            addLoginLog(listCustomerPwd[0].customer_id, pLogin.deviceType, 2, 0);
        //            return Json(new Result
        //            {
        //                success = false,
        //                resultCode = "1003",
        //                resultMessage = "登陆地址错误"
        //            });
        //        }

        //        var  listCustomerState = listCustomerCorp.Where(a => a.customer_state==0).ToList();
        //        if (listCustomerState.Count==0)
        //        {
        //            addLoginLog(listCustomerCorp[0].customer_id, pLogin.deviceType, 2, 0);
        //            return Json(new Result
        //            {
        //                success = false,
        //                resultCode = "1003",
        //                resultMessage = "帐号已停用"
        //            });
        //        }
        //        var customerModel = listCustomerState.FirstOrDefault();
        //        var objToken = getUserToken(customerModel);
        //        addLoginLog(customerModel.customer_id, pLogin.deviceType, 2);

        //        return Json(new Result
        //        {
        //            success = true,
        //            resultCode = "0000",
        //            resultMessage = "",
        //            result = objToken
        //        });

        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.LogWriteBug("[用户登陆]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
        //        return Json(new Result()
        //        {
        //            success = false,
        //            resultCode = "5001",
        //            resultMessage = "服务异常，请稍后重试",
        //        });
        //    }
        //}

        /// <summary>
        /// 短信登陆
        /// </summary>
        /// <param name="Phone">手机号</param>
        /// <param name="code">验证码</param>
        /// <param name="corpid">公司id</param>
        /// <param name="deviceType">设备类型</param>
        /// <returns></returns>
        public IHttpActionResult mobilePhoneMessageLogin(string Phone, string code, int corpid, int deviceType)
        {
            try
            {
                var ipAddress = WebHelper.GetClientIpAddress(this.Request);
                if (string.IsNullOrEmpty(Phone) && string.IsNullOrEmpty(code))
                {
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "1003",
                        resultMessage = "请求参数有误",
                    });
                }

                Welfare_Customer customer = null;
                using (Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities())
                {
                    customer = db.Welfare_Customer.Where(a => a.corp_id == corpid && a.mobile_phone == Phone).FirstOrDefault();
                    if (customer == null)
                    {
                        return Json(new Result
                        {
                            success = false,
                            resultCode = "1003",
                            resultMessage = "登陆帐号错误"
                        });
                    }
                    else if (customer.customer_state == 1)
                    {
                        return Json(new Result
                        {
                            success = false,
                            resultCode = "1003",
                            resultMessage = "帐号已停用"
                        });
                    }

                    //校验短信验证码
                    string errMsg = "";
                    bool isPhoneMsgOk = phoneMsgHelper.isPhoneMsgCode(Phone, code, out errMsg);
                    if (isPhoneMsgOk == false)
                    {
                        return Json(new Result
                        {
                            success = false,
                            resultCode = "1003",
                            resultMessage = errMsg
                        });
                    }

                    var objToken = getUserToken(customer);
                    addLoginLog(Phone, corpid, deviceType, 1);

                    return Json(new Result
                    {
                        success = true,
                        resultCode = "0000",
                        resultMessage = "",
                        result = objToken
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[用户登陆]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = "服务异常，请稍后重试",
                });
            }

        }

        /// <summary>
        /// 退出登陆
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult accountOutLogin(string token)
        {
            try
            {
                int corpid = -1;
                var isOutOk = updateUserToken(token, out corpid);
                if (isOutOk)
                {
                    return Json(new Result()
                    {
                        success = true,
                        resultCode = "0000",
                        result = new { corpid = corpid }
                    });
                }
                else
                {
                    return Json(new Result()
                    {
                        success = false,
                        resultCode = "6001",
                        resultMessage = "未获取到用户登陆信息"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[用户登陆]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = "服务异常，请稍后重试",
                });
            }
        }


        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="corpid"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult sendPhoneMessage(string phone, int corpid)
        {
            try
            {


                using (Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities())
                {
                    //短信限制
                    var maxNumber = Convert.ToInt32(ConfigurationManager.AppSettings["phoneMessageMaxNumber"]);
                    var customer = db.Welfare_Customer.Where(a => a.corp_id == corpid && a.mobile_phone == phone).FirstOrDefault();
                    if (customer == null)
                    {
                        return Json(new Result
                        {
                            success = false,
                            resultCode = "6001",
                            resultMessage = "手机不存在"
                        });
                    }
                    else if (customer.customer_state == 1)
                    {
                        return Json(new Result
                        {
                            success = false,
                            resultCode = "1003",
                            resultMessage = "帐号已停用"
                        });
                    }

                    var year = DateTime.Now.Year;
                    var month = DateTime.Now.Month;
                    var day = DateTime.Now.Day;
                    string strDate = $"{year}-{month}-{day} 00:00:00";
                    var d_datatime = Convert.ToDateTime(strDate);
                    var m_datatime = d_datatime.AddDays(1);

                    if (customer.is_test_customer == 0)
                    {
                        var listMessage = db.Welfare_Mobile_Phone_Message.Where(a => a.phone == phone && a.create_time >= d_datatime && a.create_time < m_datatime && a.is_delete == 0).ToList();
                        if (listMessage.Count >= maxNumber)
                        {
                            return Json(new Result
                            {
                                success = false,
                                resultCode = "1003",
                                resultMessage = "当天短信次数已用尽"
                            });
                        }
                    }


                    Random rd = new Random();
                    int RollNum = rd.Next(192800, 981105);
                    var minute = Convert.ToInt32(ConfigurationManager.AppSettings["minute"]);
                    var cfgCorpHeader = db.Cfg_Corp_Phone_Message_Header.Where(a => a.corp_id == corpid).FirstOrDefault();
                    var ipAddress = WebHelper.GetClientIpAddress(this.Request);
                    if (cfgCorpHeader == null)
                    {
                        return Json(new Result
                        {
                            success = false,
                            resultCode = "6001",
                            resultMessage = "未配置公司【Title】"
                        });
                    }
                    string sendMesage = $"{cfgCorpHeader.message_header}您的验证码是{RollNum}，此信息{minute}分钟内有效。";
                    string SendState = YuanFuSendPhoneMsgHelper.ChannelB(sendMesage, phone);
                    if (SendState == "SUCCESS00")
                    {
                        Welfare_Mobile_Phone_Message CmfMsg = new Welfare_Mobile_Phone_Message()
                        {
                            create_time = DateTime.Now,
                            expires_in = minute * 60,
                            ip_address = ipAddress,
                            modified_time = DateTime.Now,
                            phone = phone,
                            phone_message = sendMesage,
                            verification_code = RollNum.ToString()
                        };
                        db.Welfare_Mobile_Phone_Message.Add(CmfMsg);
                        db.SaveChanges();


                        return Json(new Result
                        {
                            success = true,
                            resultCode = "0000",
                            result = new
                            {
                                code = RollNum
                            }
                        });
                    }
                    else
                    {
                        return Json(new Result
                        {
                            success = false,
                            resultCode = "6001",
                            resultMessage = "发短信失败"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[短信登陆]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
                return Json(new Result()
                {
                    success = false,
                    resultCode = "5001",
                    resultMessage = "服务异常，请稍后重试",
                });
            }

        }

        /// <summary>
        /// 获取最新 Token
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        private Token getUserToken(Welfare_Customer customer)
        {
            using (Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities())
            {
                var tokenModel = db.Welfare_Customer_Token.Where(a => a.customer_id == customer.customer_id).FirstOrDefault();
                //更新token参数
                var tokenHour = Convert.ToInt32(ConfigurationManager.AppSettings["TokenHour"]);
                var nowDate = DateTime.Now;
                var time = DateTimeHelper.ConvertDateTiemp(nowDate);  //时间戳
                int expires_in = tokenHour * 60 * 60;  //秒
                var addDate = nowDate.AddSeconds(expires_in);  //添加（秒）
                var refresh_token_expires = DateTimeHelper.ConvertDateTiemp(addDate);

                if (tokenModel == null)
                {
                    Welfare_Customer_Token newToken = new Welfare_Customer_Token()
                    {
                        access_token = Guid.NewGuid().ToString().Replace("-", ""),
                        customer_id = customer.customer_id,
                        expires_in = expires_in,
                        token_time = time,
                        create_time = DateTime.Now,
                        modified_time = DateTime.Now,
                        refresh_token_expires = refresh_token_expires,
                        corp_id = customer.corp_id
                    };

                    db.Welfare_Customer_Token.Add(newToken);
                    db.SaveChanges();

                    return new Token
                    {
                        access_token = newToken.access_token,
                        expires_in = newToken.expires_in,
                        refresh_token_expires = newToken.refresh_token_expires,
                        token_time = newToken.token_time,
                        corpid = newToken.corp_id,
                        customerId = newToken.customer_id
                    };
                }
                else
                {
                    tokenModel.access_token = Guid.NewGuid().ToString().Replace("-", "");
                    tokenModel.expires_in = expires_in;
                    tokenModel.modified_time = DateTime.Now;
                    tokenModel.refresh_token_expires = refresh_token_expires;
                    tokenModel.token_time = time;
                    db.SaveChanges();

                    return new Token
                    {
                        access_token = tokenModel.access_token,
                        expires_in = tokenModel.expires_in,
                        refresh_token_expires = tokenModel.refresh_token_expires,
                        token_time = tokenModel.token_time,
                        corpid = tokenModel.corp_id,
                        customerId = tokenModel.customer_id
                    };
                }
            }
        }

        /// <summary>
        /// 退出登陆
        /// </summary>
        /// <param name="token"></param>
        /// <param name="corpid"></param>
        /// <returns></returns>
        private bool updateUserToken(string token, out int corpid)
        {
            using (Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities())
            {
                var customerToken = db.Welfare_Customer_Token.Where(a => a.access_token == token && a.is_delete == 0).FirstOrDefault();
                if (customerToken == null)
                {
                    customerToken = db.Welfare_Customer_Token.Where(a => a.access_token == token).FirstOrDefault();
                    if (customerToken == null)
                    {
                        corpid = -1;
                        return false;
                    }
                    else
                    {
                        corpid = customerToken.corp_id;
                        return true;
                    }
                }
                else
                {
                    customerToken.is_delete = 1;
                    customerToken.modified_time = DateTime.Now;
                    db.SaveChanges();

                    corpid = customerToken.corp_id;
                    return true;
                }
            }
        }

        /// <summary>
        /// 登陆日志
        /// </summary>
        /// <param name="customerId">用户id</param>
        /// <param name="deviceType">设备类型 1：手机 2：电脑 3：平板 </param>
        /// <param name="loginType">登陆方式 1：短信 2：帐号</param>
        /// <param name="isLoginok">是否登陆成功</param>
        private void addLoginLog(string logiName,int corpid, int deviceType, int loginType, int isLoginok = 1)
        {
            try
            {
                using (Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities())
                {
                    var ipAddress = WebHelper.GetClientIpAddress(this.Request);
                    Welfare_Customer_Login_Log newLog = new Welfare_Customer_Login_Log()
                    {
                        device_type = deviceType,
                        login_ip = string.IsNullOrEmpty(ipAddress) ? "::" : ipAddress,
                        is_loginok = isLoginok,
                        login_time = DateTime.Now,
                        login_type = loginType,
                        modified_time = DateTime.Now,
                        create_time = DateTime.Now,
                        login_name = logiName,
                        corp_id = corpid
                    };
                    db.Welfare_Customer_Login_Log.Add(newLog);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogWriteBug("[登陆日志]!", "ex:" + ex + "\r\nStackTrace:" + ex.StackTrace, "1");
            }
        }



    }
}
