using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public static class  DateTimeHelper
    {
        /// <summary>
        /// 获取时间戳（精确到毫秒）
        /// </summary>
        /// <param name="time">时间</param>
        public static long ConvertDateTiemp(DateTime time)
        {
            return ((time.ToUniversalTime().Ticks - 621355968000000000) / 10000000) * 1000;
            //等价于：
            //return ((time.ToUniversalTime().Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0).Ticks) / 10000000) * 1000;
        }

        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime GetTime(string timeStamp)
        {
            if (timeStamp.Length > 10)
            {
                timeStamp = timeStamp.Substring(0, 10);
            }
            DateTime dateTimeStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dateTimeStart.Add(toNow);
        }
    }
}
