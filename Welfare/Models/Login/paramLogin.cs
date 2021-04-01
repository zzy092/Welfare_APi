using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WelfareApi.Models.Login
{
    public class paramLogin
    {
        public string loginName { get; set; }
        /// <summary>
        /// 密码md5
        /// </summary>
        public string password { get; set; }
        public int corpid { get; set; }
        /// <summary>
        /// 设备类型 1 手机 2 pc
        /// </summary>
        public int deviceType { get; set; }
        /// <summary>
        /// 过期时间戳
        /// </summary>
        public long refresh_expires { get; set; }

    }

    public class ELoginModel
    {
        public string login { get; set; }
        public int device { get; set; }
    }
}