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
    
    public partial class Shopping_JD_Order_Log
    {
        public int id { get; set; }
        public long jdOrderId { get; set; }
        public string thirdOrder { get; set; }
        public decimal orderPrice { get; set; }
        public decimal orderNakedPrice { get; set; }
        public decimal orderTaxPrice { get; set; }
        public string sku { get; set; }
        public System.DateTime create_time { get; set; }
        public System.DateTime modified_time { get; set; }
        public int is_delete { get; set; }
        public decimal freight { get; set; }
        public int submitState { get; set; }
    }
}
