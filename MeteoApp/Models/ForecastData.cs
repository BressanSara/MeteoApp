using System;
using System.Collections.Generic;
using System.Linq;

namespace MeteoApp.Models
{
    public class ForecastData
    {
        public string Cod { get; set; }
        public int Message { get; set; }
        public int Cnt { get; set; }
        public List<ForecastItem> List { get; set; }
        public City City { get; set; }
    }

    public class ForecastItem
    {
        public long Dt { get; set; }
        public Main Main { get; set; }
        public List<Weather> Weather { get; set; }
        public Clouds Clouds { get; set; }
        public Wind Wind { get; set; }
        public int Visibility { get; set; }
        public double Pop { get; set; }
        public Rain Rain { get; set; }
        public Sys Sys { get; set; }
        public string DtTxt { get; set; }
    }

    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Coord Coord { get; set; }
        public string Country { get; set; }
        public int Population { get; set; }
        public int Timezone { get; set; }
        public long Sunrise { get; set; }
        public long Sunset { get; set; }
    }
} 