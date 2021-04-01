using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tools
{
    public static class RegexHelper
    {
        /// <summary>
        /// 验证电话号码的主要代码
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static bool isFixeTelephone(string str_telephone)
        {
            return Regex.IsMatch(str_telephone, @"^(\d{3,4}-)?\d{6,8}$");
        }
        /// <summary>
        /// 验证手机号码的主要代码
        /// </summary>
        /// <param name="str_handset"></param>
        /// <returns></returns>
        public static bool isMoblePhone(string str_handset)
        {
            return Regex.IsMatch(str_handset, @"^[1]+[3,5]+\d{9}");
        }

        /// <summary>
        /// 验证身份证号的主要代码
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static bool isIDcard(string str_idcard)
        {
            return Regex.IsMatch(str_idcard, @"(^\d{18}$)|(^\d{15}$)");
        }

        /// <summary>
        /// 验证输入为数字的主要代码
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static bool isNumber(string str_number)
        {
            return Regex.IsMatch(str_number, @"^[0-9]*$");
        }

        /// <summary>
        /// 验证邮编的主要代码
        /// </summary>
        /// <param name="str_postalcode"></param>
        public static bool isPostalcode(string str_postalcode)
        {
            return Regex.IsMatch(str_postalcode, @"^\d{6}$");
        }

        /// <summary>
        /// 验证邮箱
        /// </summary>
        /// <param name="str_postalcode"></param>
        public static bool isEmail(string str_Email)
        {
            return Regex.IsMatch(str_Email, @" ^\w + ([-+.]\w +) *@\w + ([-.]\w +)*\.\w + ([-.]\w +)*$");
        }

        /// <summary>
        /// 模糊手机中间 137****5615
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static string vaguePhone(string phone)
        {
            return Regex.Replace(phone, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
        }

        /// <summary>
        /// 模糊座机后四位 0310-591****
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static string vagueFixeTelePhone(string fixeTelePhone)
        {
            return Regex.Replace(fixeTelePhone, "([\\w\\W]*)([\\w\\W]{4})", "$1****");
        }
    }
}
