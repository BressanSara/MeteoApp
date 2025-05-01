using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MeteoApp.Models
{
    public class Reminder
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("lat")]
        public Double Lat { get; set; }

        [JsonPropertyName("lon")]
        public Double Lon { get; set; }

        [JsonPropertyName("minTemp")]
        public Double MinTemp { get; set; }

        [JsonPropertyName("maxTemp")]
        public Double MaxTemp { get; set; }
    }
}
