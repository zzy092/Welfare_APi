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
    
    public partial class Shopping_Product_Info
    {
        public int product_id { get; set; }
        public long sku { get; set; }
        public string product_name { get; set; }
        public decimal product_price { get; set; }
        public decimal product_jdPrice { get; set; }
        public string product_imagePath { get; set; }
        public string category { get; set; }
        public decimal tax { get; set; }
        public decimal taxPrice { get; set; }
        public string brandName { get; set; }
        public int cid_1 { get; set; }
        public int cid_2 { get; set; }
        public int cid_3 { get; set; }
        public int is_sku_state { get; set; }
        public int is_sale_check { get; set; }
        public System.DateTime modified_time { get; set; }
        public System.DateTime create_time { get; set; }
        public int is_delete { get; set; }
    }
}
