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
    
    public partial class Shopping_Customer_Address
    {
        public int id { get; set; }
        public int customer_id { get; set; }
        public string address_user_name { get; set; }
        public string address_email { get; set; }
        public string address_phone { get; set; }
        public string address_fixed_telephone { get; set; }
        public string province_name { get; set; }
        public string city_name { get; set; }
        public string county_name { get; set; }
        public string town_name { get; set; }
        public string area_str_ids { get; set; }
        public string customer_address { get; set; }
        public int is_master { get; set; }
        public System.DateTime create_time { get; set; }
        public System.DateTime modified_time { get; set; }
        public int is_delete { get; set; }
        public string tage_name { get; set; }
    }
}
