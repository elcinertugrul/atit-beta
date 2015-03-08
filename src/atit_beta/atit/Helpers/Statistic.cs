using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace atit.Helpers
{
    public class Data
    {
        public statistics statistics { get; set; }
        [JsonProperty("location")]
        public Location location { get; set; }
    }
    public class statistics
    {
        public population_density population_density { get; set; }
        public elevation elevation { get; set; }
        public land_cover land_cover { get; set; }
        public mean_temperature mean_temperature { get; set; }
        public precipitation precipitation { get; set; }
    }

    public class population_density
    {
        public string description { get; set; }
        public string value { get; set; }
        public string source_name { get; set; }
    }

    public class elevation
    {
        public string description { get; set; }
        public string value { get; set; }
        public string source_name { get; set; }
        public string units { get; set; }
    }

    public class land_cover
    {
        public string description { get; set; }
        public string value { get; set; }
        public string index { get; set; }
        public string source_name { get; set; }
    }

    public class mean_temperature
    {
        public string description { get; set; }
        public List<string> value { get; set; }
        public string source_name { get; set; }
        public string units { get; set; }
    
    }

    public class precipitation
    {
        public string description { get; set; }
        public List<string> value { get; set; }
        public string source_name { get; set; }
        public string units { get; set; }    
    }

    public class Location
    {
        public string latitude { get; set;}
        public string longitude { get; set;}
    }
}
