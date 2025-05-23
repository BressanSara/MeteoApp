﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteoApp.Models
{
    public class MeteoLocation
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public Coord Coord { get; set; }
        public CurrentWeatherData WeatherData { get; set; }

        override
        public string ToString()
        {
            return $"Name: {Name} Country:{Country} Lat:{Latitude} Lon:{Longitude}";
        }
    }
}
