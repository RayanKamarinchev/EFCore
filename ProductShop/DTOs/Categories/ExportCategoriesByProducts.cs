using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ProductShop.DTOs.Categories
{
    [JsonObject]
    public class ExportCategoriesByProducts
    {
        [JsonProperty("category")]
        public string Name { get; set; }
        [JsonProperty("productsCount")]
        public int ProductsCount { get; set; }
        [JsonProperty("averagePrice")]
        public string AveragePrice { get; set; }
        [JsonProperty("totalRevenue")]
        public string TotalRevenue { get; set; }
    }
}
