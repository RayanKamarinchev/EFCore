using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.DTOs.Users;

namespace ProductShop.DTOs
{
    [JsonObject]
    public class UsersAndProductsDto
    {
        [JsonProperty("usersCount")]
        public int UsersCount => this.Users.Any() ? Users.Length : 0;
        [JsonProperty("users")]
        public ExportUserWithSoldProductAdvanced[] Users { get; set; }
    }
}
