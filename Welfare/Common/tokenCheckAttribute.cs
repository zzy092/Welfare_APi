using BLL;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Tools;
using Welfare.Models;

namespace Welfare.Common
{
    
    public class tokenCheckAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            string value = HttpContext.Current.Request.Headers["token"];

            if (!string.IsNullOrEmpty(value))
            {

                var outMessage = "";
                var tokenModel = tokenCustomerHelper.getCustomerToken(value, out outMessage);
                if (tokenModel==null)
                {
                    var result_str = JsonConvert.SerializeObject(new Result()
                    {
                        resultCode = "1007",
                        resultMessage = outMessage,
                        success = false,
                        result = null
                    });

                    actionContext.Response = new HttpResponseMessage
                    {
                        Content = new StringContent(result_str, Encoding.GetEncoding("UTF-8"), "application/json"),
                        StatusCode = HttpStatusCode.OK
                    };
                    return;
                }
                else if (!string.IsNullOrEmpty(outMessage))
                {
                    var result_str = JsonConvert.SerializeObject(new Result()
                    {
                        resultCode = "1006",
                        resultMessage = "用户token过期",
                        success = false,
                        result = null
                    });

                    actionContext.Response = new HttpResponseMessage
                    {
                        Content = new StringContent(result_str, Encoding.GetEncoding("UTF-8"), "application/json"),
                        StatusCode = HttpStatusCode.OK
                    };
                    return;
                }
            }
            else
            {
                var result_str = JsonConvert.SerializeObject(new Result()
                {
                    success = false,
                    resultCode = "1001",
                    resultMessage = "token不可为空",
                    result = null
                });

                actionContext.Response = new HttpResponseMessage
                {
                    Content = new StringContent(result_str, Encoding.GetEncoding("UTF-8"), "application/json"),
                    StatusCode = HttpStatusCode.InternalServerError
                };
                return;
            }

            base.IsAuthorized(actionContext);
        }
        private static string getPara(string url, string name)
        {
            Regex reg = new Regex(@"(?:^|\?|&)" + name + "=(?<VAL>.+?)(?:&|$)");
            Match m = reg.Match(url);
            return m.Groups["VAL"].ToString(); ;
        }

      

    }
}