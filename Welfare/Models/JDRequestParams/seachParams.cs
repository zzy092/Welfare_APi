using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Welfare.Models.JDRequest
{
    /// <summary>
    /// 搜索商品
    /// </summary>
    public class parameterSeach
    {
        /// <summary>
        /// 搜索关键字，需要编码
        /// </summary>
        public string keyword { get; set; }
        /// <summary>
        /// 分类Id,只支持三级类目Id 
        /// </summary>
        public string catId { get; set; }
        /// <summary>
        /// 当前第几页
        /// </summary>
        public int pageIndex { get; set; }
        /// <summary>
        /// 当前页显示
        /// </summary>
        public int pageSize { get; set; }
        /// <summary>
        /// 价格区间搜索，低价
        /// </summary>
        public string min { get; set; }
        /// <summary>
        /// 价格区间搜索，高价
        /// </summary>
        public string max { get; set; }
        /// <summary>
        /// 品牌搜索 多个品牌以逗号分隔，需要编码
        /// </summary>
        public string brands { get; set; }
        /// <summary>
        /// 一级分类
        /// </summary>
        public string cid1 { get; set; }
        /// <summary>
        /// 二级分类
        /// </summary>
        public string cid2 { get; set; }
        /// <summary>
        /// 销量降序="sale_desc" ||  价格升序="price_asc" || 价格降序="price_desc" || 上架时间降序="winsdate_desc" ||按销量排序_15天销售额="sort_totalsales15_desc"; || 按15日销量排序="sort_days_15_qtty_desc" || 按30日销量排序="sort_days_30_qtty_desc" || 按15日销售额排序="sort_days_15_gmv_desc" || 按30日销售额排序="sort_days_30_gmv_desc"
        /// </summary>
        public string  sortType { get; set; }
        /// <summary>
        /// 价格汇总 priceCol=”yes”
        /// </summary>
        public string priceCol { get; set; }
        /// <summary>
        /// 扩展属性汇总 priceCol=”yes”
        /// </summary>
        public string extAttrCol { get; set; }
        /// <summary>
        /// 地址 1,2810,51081
        /// </summary>
        public string areaIds { get; set; }
        /// <summary>
        /// 带库存有货
        /// </summary>
        public bool redisStore { get; set; }
    }   

    /// <summary>
    /// 商品详情
    /// </summary>
    public class parameterProductDetail
    {
        public string sku { get; set; }
        public string queryExts { get; set; }
    }
    /// <summary>
    /// 商品图片
    /// </summary>
    public class parameterProductImages
    {
        public string sku { get; set; }
    }
    /// <summary>
    /// 同类sku 
    /// </summary>
    public class parameterProductSimilar
    {
        public string skuId { get; set; }
    }
    /// <summary>
    /// 商品价格
    /// </summary>
    public class parameterProductPrice
    {
        public string sku { get; set; }
        public string queryExts { get; set; }
    }

    /// <summary>
    /// 上下架
    /// </summary>
    public class parameterSkuState
    {
        public string sku { get; set; }
    }

    /// <summary>
    /// 是否可售
    /// </summary>
    public class parameterSkuCheck
    {
        public string skuIds { get; set; }
    }

    /// <summary>
    /// 商品在特定区域是否可售
    /// </summary>
    public class parameterCheckAreaLimit
    {
        public string skuIds { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string town { get; set; }
    }

  

    /// <summary>
    /// 库存
    /// </summary>
    public class parametereStock
    {
        /// <summary>
        /// 商品和数量  [{skuId: 569172,num:101}]。
        ///“{skuId: 569172,num:101}”为1条记录，此参数最多传入100条记录
        /// </summary>
        public string skuNums { get; set; }
        /// <summary>
        /// 格式：1_0_0 (分别代表1、2、3级地址)
        /// </summary>
        public string area { get; set; }
    }

    /// <summary>
    /// 库存 skuNums参数
    /// </summary>
    public class parameterSkuNums
    {
        public long skuId { get; set; }
        public int num { get; set; }
    }

    public class parameterSubmitOrder
    {
        public string token { get; set; }
        /// <summary>
        /// 第三方的订单单号，必须在100字符以内
        /// </summary>
        public string thirdOrder { get; set; }
        /// <summary>
        /// 下单商品信息 例如:[{"skuId":100004017833,"num":1,"bNeedGift":false,"bNeedAnnex ":true,"yanbao":[]}]
        /// </summary>
        public string sku { get; set; }
        /// <summary>
        /// 收货人姓名，最多20个字符
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 一级地址编码
        /// </summary>
        public int province { get; set; }
        /// <summary>
        /// 二级地址编码
        /// </summary>
        public int city { get; set; }
        /// <summary>
        /// 三级地址编码
        /// </summary>
        public int county { get; set; }
        /// <summary>
        /// 四级级地址编码 有就传 没有填 0
        /// </summary>
        public int town { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string mobile { get; set; }
        public string email { get; set; }
        /// <summary>
        /// 开票方式(2为集中开票，4 订单完成后开票)
        /// </summary>
        public int invoiceState { get; set; }
        /// <summary>
        /// 发票类型（2增值税专用发票；3 电子票）
        /// </summary>
        public int invoiceType { get; set; }
        /// <summary>
        /// 发票类型：4：个人，5：单位
        /// </summary>
        public int selectedInvoiceTitle { get; set; }
        /// <summary>
        /// 1:明细，100：大类 备注:若增值税专用发票则只能选1 明细
        /// </summary>
        public int invoiceContent { get; set; }
        /// <summary>
        /// 1价格校验（建议传1）；0 不需要
        /// </summary>
        public int doOrderPriceMode { get; set; }
        /// <summary>
        /// Json格式的数据  [{"price":1299, "skuId":100004017833}]
        /// </summary>
        public string orderPriceSnap { get; set; }
        /// <summary>
        /// 使用余额paymentType=4时，此值固定是1其他支付方式0
        /// </summary>
        public int isUseBalance { get; set; }
        /// <summary>
        /// 是否预占库存，0是预占库存（需要调用确认订单接口），1是不预占库存
        /// </summary>
        public int submitState { get; set; }
        /// <summary>
        /// 收票人电话
        /// </summary>
        public string invoicePhone { get; set; }
        /// <summary>
        /// paymentType=4
        /// </summary>
        public int paymentType { get; set; }

    }

    /// <summary>
    /// sku附加信息
    /// </summary>
    public class submitParamSku: parameterSkuNums
    {
        /// <summary>
        /// 表示附件，必须传true
        /// </summary>
        public bool bNeedGift { get; set; }
        /// <summary>
        /// 表示赠品，true要赠品，false不要赠品;（传true是当订单主商品存在赠品活动且赠品还有库存时生成订单会有对应的赠品）
        /// </summary>
        public bool bNeedAnnex { get; set; }
    }

    /// <summary>
    /// 提交订单时 价格校验提交参数
    /// </summary>
    public class OrderPriceSnap
    {
        public long skuId { get; set; }
        public decimal price { get; set; }
    }


    public class pamarSubitOrder
    {
        public string token { get; set; }

        public int addressId { get; set; }
        public List<parameterSkuNums> skus { get; set; }
        public decimal payMoney { get; set; }
    }
}