using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ECommerce.API.Model
{
    public class ApiCheckoutSummary
    {
        [JsonPropertyName("products")]
        public List<ApiCheckoutProduct> Products { get; set; }

        [JsonPropertyName("totalPrice")]
        public double TotalPrice { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
    }
}
