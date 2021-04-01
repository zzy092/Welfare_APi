using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace Tools
{
    public static class YuanFuSendPhoneMsgHelper
    {
        public static string ChannelB(string _mesg, string _phone)
        {
            return ChannelYFW(_mesg, _phone, "B");
        }
        private static string ChannelYFW(string _mesg, string _phone, string channel)
        {
            string key = "H5gOs1ZshKZ6WikN";
            string times = DateTime.Now.ToString("yyyyMMddHH");
            string _msg = System.Web.HttpUtility.UrlEncode(_mesg);
            string md5pass = FormsAuthentication.HashPasswordForStoringInConfigFile(key + times, "MD5");
            string url = "http://wapi.all.yuanfuwang.com/WebMSGService.asmx/HttpMsg1Receiver";
            string param = "keysign=" + md5pass + "&channel=" + channel + "&message=" + _msg + "&phone=" + _phone;
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            Encoding encoding = Encoding.UTF8;
            //encoding.GetBytes(postData);
            byte[] bs = Encoding.ASCII.GetBytes(param);
            string responseData = String.Empty;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = bs.Length;
            try
            {
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                    reqStream.Close();
                }
                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
                    {
                        responseData = reader.ReadToEnd().ToString();
                    }
                    return responseData;
                }
            }
            catch (Exception ex)
            {
                //return ex.Message;//显示异常信息
                return "ERR|" + ex.Message;
            }
        }
        public static string ChannelE(string _mesg, string _phone)
        {
            return ChannelYFWCall(_mesg, _phone, "E");
        }
        private static string ChannelYFWCall(string _mesg, string _phone, string channel)
        {
            string key = "H5gOs1ZshKZ6WikN";
            string times = DateTime.Now.ToString("yyyyMMddHH");
            string _msg = System.Web.HttpUtility.UrlEncode(_mesg);
            string md5pass = FormsAuthentication.HashPasswordForStoringInConfigFile(key + times, "MD5");
            string url = "http://wapi.all.yuanfuwang.com/WebMSGService.asmx/HttpMsg1Receiver";
            string param = "keysign=" + md5pass + "&channel=" + channel + "&message=" + _msg + "&phone=" + _phone;
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            Encoding encoding = Encoding.UTF8;
            //encoding.GetBytes(postData);
            byte[] bs = Encoding.ASCII.GetBytes(param);
            string responseData = String.Empty;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = bs.Length;
            try
            {
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                    reqStream.Close();
                }
                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
                    {
                        responseData = reader.ReadToEnd().ToString();
                    }
                    return responseData;
                }
            }
            catch (Exception ex)
            {
                //return ex.Message;//显示异常信息
                return "ERR|" + ex.Message;
            }
        }
    }
}
