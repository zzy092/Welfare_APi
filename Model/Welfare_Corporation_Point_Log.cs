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
    
    public partial class Welfare_Corporation_Point_Log
    {
        public int point_id { get; set; }
        public int corpid_id { get; set; }
        public int source { get; set; }
        public int refer_number { get; set; }
        public decimal change_point { get; set; }
        public decimal before_point { get; set; }
        public decimal after_point { get; set; }
        public int point_log_type { get; set; }
        public string point_remark { get; set; }
        public System.DateTime create_time { get; set; }
        public int is_delete { get; set; }
        public System.DateTime modified_time { get; set; }
    }
}
