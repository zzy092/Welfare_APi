using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tools;
using Welfare.Models;

namespace Welfare.Common
{
    public static class tokenCustomerHelper
    {
        public static Token getCustomerToken()
        {
            string value = HttpContext.Current.Request.Headers["token"];
            BaseBLL<Welfare_Customer_Token> bllToken = new BaseBLL<Welfare_Customer_Token>();
            BaseBLL<Cfg_Corp_Sale> bllSale = new BaseBLL<Cfg_Corp_Sale>();
            var singleToken = bllToken.GetSingle(a => a.access_token == value && a.is_delete == 0);
            var saleModel = bllSale.GetSingle(a=>a.is_delete==0&&a.corp_id==singleToken.corp_id);
            Token resultToken = new Token()
            {
                access_token = singleToken.access_token,
                expires_in = singleToken.expires_in,
                refresh_token_expires = singleToken.refresh_token_expires,
                token_time = singleToken.token_time,
                corpid = singleToken.corp_id,
                customerId = singleToken.customer_id,
                corpSale=saleModel.sale_value
            };

            var efreshTime = DateTimeHelper.GetTime(singleToken.refresh_token_expires.ToString());
            return resultToken;
        }

        public static Token getCustomerToken(string token)
        {
            BaseBLL<Welfare_Customer_Token> bllToken = new BaseBLL<Welfare_Customer_Token>();
            var singleToken = bllToken.GetSingle(a => a.access_token == token && a.is_delete == 0);
            Token resultToken = new Token()
            {
                access_token = singleToken.access_token,
                expires_in = singleToken.expires_in,
                refresh_token_expires = singleToken.refresh_token_expires,
                token_time = singleToken.token_time,
                corpid = singleToken.corp_id,
                customerId = singleToken.customer_id
            };

            var efreshTime = DateTimeHelper.GetTime(singleToken.refresh_token_expires.ToString());
            return resultToken;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Token getCustomerToken(string token, out string message)
        {
            BaseBLL<Welfare_Customer_Token> bllToken = new BaseBLL<Welfare_Customer_Token>();
            var singleToken = bllToken.GetSingle(a => a.access_token == token && a.is_delete == 0);
            if (singleToken == null)
            {
                message = "无效token";
                return null;
            }
            else
            {
                Token resultToken = new Token()
                {
                    access_token = singleToken.access_token,
                    expires_in = singleToken.expires_in,
                    refresh_token_expires = singleToken.refresh_token_expires,
                    token_time = singleToken.token_time,
                    corpid = singleToken.corp_id,
                    customerId = singleToken.customer_id
                };

                var efreshTime = DateTimeHelper.GetTime(singleToken.refresh_token_expires.ToString());
                if (DateTime.Now > efreshTime)
                    message = "过期token";
                else
                    message = "";

                return resultToken;
            }
        }
    }
}