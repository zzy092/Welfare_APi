using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Welfare.Common
{
    public static class phoneMsgHelper
    {
        /// <summary>
        /// 短信验证
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="code">验证码</param>
        /// <param name="errMsg">错误码</param>
        /// <returns></returns>
        public static bool isPhoneMsgCode(string phone , string code,out string errMsg)
        {
            bool isok = true;
            string resultMsg = "";
            BaseBLL<Welfare_Mobile_Phone_Message> bllPhoneMessage = new BaseBLL<Welfare_Mobile_Phone_Message>();
            List<Welfare_Mobile_Phone_Message> listPhoneMessage = bllPhoneMessage
                .GetList(a => a.phone == phone && a.is_delete == 0 && a.verification_code == code)
                .OrderByDescending(a => a.create_time)
                .ToList();
            if (listPhoneMessage.Count > 0)
            {
                var refreshTime = listPhoneMessage[0].create_time.AddMinutes(listPhoneMessage[0].expires_in);
                if (refreshTime < DateTime.Now)
                {
                    isok = false;
                    resultMsg = "验证码过期";
                }
            }
            else
            {
                isok = false;
                resultMsg = "无效验证码";
            }

            errMsg = resultMsg;
            return isok;
        }
    }
}