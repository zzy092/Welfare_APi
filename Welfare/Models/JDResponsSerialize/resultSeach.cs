using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Welfare.Models.JDResponsSerialize
{
    #region 搜索商品
    /// <summary>
    /// jd搜索商品接口 响应
    /// </summary>
    public class resultSeach : JDResult
    {
        public SeachData result { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SeachData
    {
        /// <summary>
        /// 搜索结果总记录数量
        /// </summary>
        public int resultCount { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int pageCount { get; set; }
        /// <summary>
        /// 每页大小
        /// </summary>
        public int pageSize { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int pageIndex { get; set; }
        /// <summary>
        /// 品牌汇总信息
        /// </summary>
        public FuBrandVo brandAggregate { get; set; }

        /// <summary>
        /// 相关分类汇总信息
        /// </summary>
        public FuCategoryVo categoryAggregate { get; set; }
        /// <summary>
        /// 搜索命中数据json字符串，返回的图片地址拼接规则与查询商品详情规则一致
        /// </summary>
        public List<hitResult> hitResult { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<PriceSection> priceIntervalAggregate { get; set; }
    }

    /// <summary>
    /// 价格区间
    /// </summary>
    public class PriceSection
    {
        public int min { get; set; }
        public int max { get; set; }
    }

    public class FuBrandVo
    {
        //public string[] pinyinAggr { get; set; }
        public List<BrandVo> brandList { get; set; }
    }
    /// <summary>
    /// 品牌汇总信息
    /// </summary>
    public class BrandVo
    {
        /// <summary>
        /// 品牌id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 品牌名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 品牌首字母拼音
        /// </summary>
        public string pinyin { get; set; }
    }


    public class FuCategoryVo
    {
        public List<CategoryVo> firstCategory { get; set; }
        public List<CategoryVo> secondCategory { get; set; }
        public List<CategoryVo> thridCategory { get; set; }
    }
    /// <summary>
    /// 相关分类汇总信息
    /// </summary>
    public class CategoryVo
    {
        /// <summary>
        /// 分类Id
        /// </summary>
        public int catId { get; set; }
        /// <summary>
        /// 分类下商品数量
        /// </summary>
        public int count { get; set; }
        public int parentId { get; set; }
        public string field { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 分类权重
        /// </summary>
        public int weight { get; set; }

    }

    /// <summary>
    /// 搜索命中数据json字符串，返回的图片地址拼接规则与查询商品详情规则一致
    /// </summary>
    public class hitResult
    {
        /// <summary>
        /// 品牌名称
        /// </summary>
        public string brand { get; set; }
        /// <summary>
        /// 商品图片url
        /// </summary>
        public string imageUrl { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string wareName { get; set; }
        /// <summary>
        /// 商品id
        /// </summary>
        public string wareId { get; set; }
        /// <summary>
        /// 商品spuid
        /// </summary>
        public string warePId { get; set; }
        /// <summary>
        /// 品牌id
        /// </summary>
        public string brandId { get; set; }
        /// <summary>
        /// 一级类目id
        /// </summary>
        public string cid1 { get; set; }
        /// <summary>
        /// 二级类目id
        /// </summary>
        public string cid2 { get; set; }
        /// <summary>
        /// 三级类目id
        /// </summary>
        public string catId { get; set; }
        /// <summary>
        /// 上柜状态，1.有效
        /// </summary>
        public string wstate { get; set; }
        /// <summary>
        /// 商品状态，1.有效	
        /// </summary>
        public string wyn { get; set; }
        /// <summary>
        /// 一级分类名
        /// </summary>
        public string cid1Name { get; set; }
        /// <summary>
        /// 二级分类名
        /// </summary>
        public string cid2Name { get; set; }
        /// <summary>
        /// 三级分类名
        /// </summary>
        public string catName { get; set; }
        /// <summary>
        /// 商品同义词
        /// </summary>
        public string synonyms { get; set; }
    }
    #endregion

    #region 商品详情
    public class resultProductDetail : JDResult
    {
        public productDetail result { get; set; }
    }

    public class productDetail
    {
        /// <summary>
        /// 售卖单位
        /// </summary>
        public string saleUnit { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        public string weight { get; set; }
        /// <summary>
        /// 产地
        /// </summary>
        public string productArea { get; set; }
        /// <summary>
        /// 包装清单
        /// </summary>
        public string wareQD { get; set; }
        /// <summary>
        /// 主图
        /// </summary>
        public string imagePath { get; set; }
        /// <summary>
        /// 规格参数
        /// </summary>
        public string param { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? state { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public long sku { get; set; }
        /// <summary>
        /// 品牌名称
        /// </summary>
        public string brandName { get; set; }
        /// <summary>
        /// UPC码  区分实物、图书、音像、三种场景
        /// </summary>
        public string upc { get; set; }
        /// <summary>
        /// 分类 示例"670;729;4837
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 商品详情页大字段 
        /// </summary>
        public string introduction { get; set; }
        //public string LowestBuy { get; set; }
        //public int isSelf { get; set; }
        //public int isJDLogistics { get; set; }
    }
    #endregion

    #region 商品图片
    public class resultImages : JDResult
    {
        public Dictionary<string, List<jdProductImages>> result { get; set; }
    }
    public class jdProductImages
    {
        /// <summary>
        /// 编号
        /// </summary>
        public long id { get; set; }
        public long skuId { get; set; }
        /// <summary>
        /// 图片路径 如3.3商品详情页面返回的图片地址一致。
        /// </summary>
        public string path { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string created { get; set; }
        public string modified { get; set; }
        /// <summary>
        /// 是否主图 1：是 0：否
        /// </summary>
        public int? isPrimary { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? orderSort { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        public int? position { get; set; }
        /// <summary>
        /// 类型（0方图  1长图【服装】）
        /// </summary>
        public int? type { get; set; }
        /// <summary>
        /// 特征
        /// </summary>
        public String features { get; set; }

    }
    #endregion

    #region 同类商品
    public class resultSimilar : JDResult
    {
        public List<SimilarProduct> result { get; set; }
    }
    public class SimilarProduct
    {
        /// <summary>
        /// 维度
        /// </summary>
        public int dim { get; set; }
        /// <summary>
        /// 销售名称
        /// </summary>
        public string saleName { get; set; }
        /// <summary>
        /// 商品销售标签销售属性下可能只有一个标签，此种场景可以选择显示销售名称和标签，也可以不显示
        /// </summary>
        public List<SaleAttr> saleAttrList { get; set; }
    }

    public class SaleAttr
    {
        /// <summary>
        /// 标签图片地址
        /// </summary>
        public string imagePath { get; set; }
        /// <summary>
        /// 标签名称
        /// </summary>
        public string saleValue { get; set; }
        /// <summary>
        /// 当前标签下的同类商品skuId
        /// </summary>
        public long[] skuIds { get; set; }
    }
    #endregion

    #region 商品价格
    public class resultSellPrice : JDResult
    {
        public List<sellPrice> result { get; set; }
    }
    public class sellPrice
    {
        /// <summary>
        /// 京东销售价
        /// </summary>
        public decimal price { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
        public decimal tax { get; set; }
        /// <summary>
        /// 税额
        /// </summary>
        public decimal taxPrice { get; set; }
        public long skuId { get; set; }
        /// <summary>
        /// 京东价
        /// </summary>
        public decimal jdPrice { get; set; }
        /// <summary>
        /// 未税价
        /// </summary>
        public decimal nakedPrice { get; set; }
    }
    #endregion

    #region 商品上下架
    public class resultSkuState : JDResult
    {
        public List<skuState> result { get; set; }
    }
    public class skuState
    {
        /// <summary>
        /// 1：上架，0：下架
        /// </summary>
        public int state { get; set; }
        public long sku { get; set; }
    }
    #endregion

    #region 是否可售
    public class resultSkuCheck : JDResult
    {
        public List<skuCheck> result { get; set; }
    }

    public class skuCheck
    {
        public long skuId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 是否可售，1：是，0：否
        /// </summary>
        public int saleState { get; set; }
        public int isCanVAT { get; set; }
        public int? noReasonToReturn { get; set; }
        public int? thwa { get; set; }
        public int isSelf { get; set; }
        public int isJDLogistics { get; set; }
    }
    #endregion

    #region 商品在特定区域是否可售
    public class resultCheckAreaLimit : JDResult
    {
        public List<checkAreaLimit> result { get; set; }
    }

    public class checkAreaLimit
    {
        public long skuId { get; set; }
        public bool isAreaRestrict { get; set; }
    }
    #endregion

    #region 库存
    public class resultStock : JDResult
    {
        public List<stock> result { get; set; }
    }
    public class stock
    {
        public long skuId { get; set; }
        public string areaId { get; set; }
        /// <summary>
        /// 库存状态编号。参考枚举值：
        /// </summary>
        public decimal stockStateId { get; set; }
        /// <summary>
        /// 库存状态描述。以下为stockStateId不同时，此字段不同的返回值：
        ///33 有货 现货-下单立即发货
        ///39 有货 在途-正在内部配货，预计2 ~6天到达本仓库
        ///40 有货 可配货-下单后从有货仓库配货
        ///36 预订
        ///34 无货
        ///99 无货开预定，此时desc返回的数值代表预计到货天数，并且该功能需要依赖合同上有无货开预定权限的用户，到货周期略长，谨慎采用该功能。
        /// </summary>
        public string stockStateDesc { get; set; }
        /// <summary>
        /// 剩余数量  当此值为-1时，为未查询到
        /// </summary>
        public int remainNum { get; set; }

    }
    #endregion

    #region 配置信息
    public class cfgInfo
    {
        public string imgUrlHeader { get; set; }
        public decimal corpSale { get; set; }
    }
    #endregion

    #region 商品总信息

    public class SkuAllInfo
    {
        public goodsDetail goods { get; set; }
        public List<jdProductImages> skuImags { get; set; }
        public List<SimilarProduct> skuSimilar { get; set; }
        public List<checkService> goodsService { get; set; }
        public bool isState { get; set; }
        public bool isCheck { get; set; }
        public bool isMasterAddress { get; set; }
        public string AddressMessage { get; set; }
        public bool isStock { get; set; }
        public string stockMsg { get; set; }

    }

    public class goodsDetail
    {
        //商品名称
        public string name { get; set; }

        //平台销售价
        public decimal ptPrice { get; set; }
        //平台销售利率sale
        public decimal pttax { get; set; }
        public decimal jdPrice { get; set; }
        public decimal price { get; set; }
        public decimal tax { get; set; }
        //wareQD 清单
        public string wareQD { get; set; }
        //weight 重量
        public decimal weight { get; set; }
        public string param { get; set; }
        public string introduction { get; set; }
        public List<string> detailImgs { get; set; }
        public string masterImage { get; set; }
    }


    public class checkService
    {
        public bool isOk { get; set; }
        public string message { get; set; }
    }
    #endregion

    #region 提交订单
    public class resultSubmitOrder : JDResult
    {
        public submitOrder result { get; set; }
    }

    public class submitOrder
    {
        public int submitState { get; set; }
        public decimal freight { get; set; }
        public long jdOrderId { get; set; }
        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal orderPrice { get; set; }
        /// <summary>
        /// 订单未税金额
        /// </summary>
        public decimal orderNakedPrice { get; set; }
        /// <summary>
        /// 订单税额
        /// </summary>
        public decimal orderTaxPrice { get; set; }
        public List<BizSku> sku { get; set; }
    }
    public class BizSku
    {
        public long skuId { get; set; }
        public int num { get; set; }
        /// <summary>
        /// 商品分类编号
        /// </summary>
        public int category { get; set; }
        /// <summary>
        /// 商品单价
        /// </summary>
        public long price { get; set; }
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal tax { get; set; }
        /// <summary>
        /// 运费拆分价格
        /// </summary>
        public decimal splitFreight { get; set; }
        /// <summary>
        /// 商品税额
        /// </summary>
        public decimal taxPrice { get; set; }
        /// <summary>
        /// 商品未税价
        /// </summary>
        public decimal nakedPrice { get; set; }
        /// <summary>
        /// 商品类型：0普通、1附件、2赠品
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 主商品skuid，如果本身是主商品，则oid为0
        /// </summary>
        public int oid { get; set; }
    }
    #endregion

    #region 订单详情

    public class resultOrderBaseInfo : JDResult
    {
        public baseOrderInfo result { get; set; }
    }

    public class baseOrderInfo
    {
        //public long pOrder 
        public int orderState { get; set; }
        public int submitState { get; set; }
        public int type { get; set; }
    }

    /// <summary>
    /// 父单详情
    /// </summary>
    public class resultOrderParentInfo : JDResult
    {
        public orderParentInfo result { get; set; }
    }


    public class orderParentInfo : baseOrderInfo
    {
        public orderInfo pOrder { get; set; }
        public List<orderInfo> cOrder { get; set; }
    }


    /// <summary>
    /// 拆单详情
    /// </summary>
    public class resultOrderInfo : JDResult
    {
        public orderInfo result { get; set; }
    }

    public class orderInfo : baseOrderInfo
    {
        public long pOrder { get; set; }
        public long jdOrderId { get; set; }
        public decimal freight { get; set; }
        public int state { get; set; }
        public decimal orderPrice { get; set; }
        public decimal orderNakedPrice { get; set; }
        public List<BizSku> sku { get; set; }
        public decimal orderTaxPrice { get; set; }
    }

    #endregion

    #region 确认预占库存订单

    public class resultConfirmOrder : JDResult
    {
        public bool result { get; set; }
    }
    #endregion

    #region 订单运费查询
    public class resultFreight : JDResult
    {
        public _freight result { get; set; }
    }
    public class _freight
    {
        public int freight { get; set; }
        public int baseFreight { get; set; }
        public int remoteRegionFreight { get; set; }
        public string remoteSku { get; set; }
    }
    #endregion
}