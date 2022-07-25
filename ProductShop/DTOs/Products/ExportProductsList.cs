using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace ProductShop.DTOs.Products
{
    [JsonObject]
    public class ExportProductsList
    {
        [JsonProperty("count")]
        public int Count => this.Products.Any() ? Products.Length : 0;
        [JsonProperty("products")]
        public ExportSimpleProduct[] Products { get; set; }
    }
}
