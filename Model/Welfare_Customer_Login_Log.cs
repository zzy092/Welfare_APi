//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Welfare_Customer_Login_Log
    {
        public int login_id { get; set; }
        public string login_name { get; set; }
        public System.DateTime login_time { get; set; }
        public string login_ip { get; set; }
        public int login_type { get; set; }
        public int device_type { get; set; }
        public int is_loginok { get; set; }
        public System.DateTime create_time { get; set; }
        public System.DateTime modified_time { get; set; }
        public int is_delete { get; set; }
        public int corp_id { get; set; }
    }
}
