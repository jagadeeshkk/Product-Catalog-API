using System.Text.Json.Serialization;

namespace ProductCatalog.API.Utility.Model
{
    public class Metrics
    {
        [JsonPropertyName("totalProducts")]
        public int TotalProducts { get; set; }
        [JsonPropertyName("averagePrice")]
        public decimal AveragePrice { get; set; }
        [JsonPropertyName("mostExpensive")]
        public MostExpensive MostExpensiveProduct { get; set; }
        [JsonPropertyName("leastExpensive")]
        public LeastExpensive LeastExpensiveProduct { get; set; }
        [JsonPropertyName("byPriceUnit")]
        public ByPriceUnit ByPriceUnitProduct { get; set; }
    }
    public class MostExpensive
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        [JsonPropertyName("price")]
        public string Price { get; set; } = string.Empty;
    }
    public class LeastExpensive
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        [JsonPropertyName("price")]
        public string Price { get; set; } = string.Empty;
    }
    public class ByPriceUnit 
    {
        [JsonPropertyName("/lb")]
        public int LB { get; set; }
        [JsonPropertyName("/each")]
        public int Each { get; set; }
        [JsonPropertyName("/bunch")]
        public int Bunch { get; set; }
        [JsonPropertyName("/head")]
        public int Head { get; set; }
        [JsonPropertyName("/bag")]
        public int Bag { get; set; }
    }
}
