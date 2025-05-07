using System;
using System.Text.Json.Serialization;

namespace MeteoApp.Models
{
    public class Reminder
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }

        [JsonPropertyName("locationName")]
        public string LocationName { get; set; }

        [JsonPropertyName("threshold")]
        public double Threshold { get; set; } // Soglia di temperatura

        [JsonPropertyName("isMax")]
        public bool IsMax { get; set; } // Indica se la soglia è massima o minima
    }
}
