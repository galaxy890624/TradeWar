/// <summary>
/// 建築的種類 <br></br>
/// 表示建築的「大分類」 <br></br>
/// 取代用字串識別類型（避免拼錯、方便程式判斷） <br></br>
/// 範例：House 表示住宅類、Market 表示交易所類
/// </summary>
public enum BuildingCategory
{
    None,
    House, // 住宅
    Market, // 交易所 - 期貨交易
    Lab // 研究所 - 科技樹
}