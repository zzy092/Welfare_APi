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
    
    public partial class Welfare_System_Bug_Log
    {
        public int id { get; set; }
        public string title { get; set; }
        public int customer_id { get; set; }
        public string source_sn { get; set; }
        public string log_detail { get; set; }
        public System.DateTime create_time { get; set; }
        public System.DateTime modified_time { get; set; }
        public int is_delete { get; set; }
        public string ex_message { get; set; }
    }
}
