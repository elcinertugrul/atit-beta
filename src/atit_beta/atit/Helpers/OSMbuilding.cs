using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;

//http://wiki.openstreetmap.org/wiki/Simple_3D_Buildings

namespace atit.Helpers
{

    public class OSMbuilding
    {
        public string type { get; set; }
        public features[] features { get; set; }
    }

    
    public class features
    {
        public string type { get; set; }
        public geometry geometry { get; set; }
        public properties properties { get; set; }

    }

  
    public class properties
    {
        public string type { get; set; }
        public double height { get; set; }
        public double minHeight { get; set; }
        public string color { get; set; }
        public string roofColor { get; set; }
        public string roofShape { get; set; }
        public double roofHeight { get; set; }
        public tags tags { get; set; }
    }


    public class tags
    {
        //buildings
        public string name { get; set; }
        public string building { get; set; }
        [JsonProperty ("building:part")]
        public string buildingpart { get; set; }
        [JsonProperty("building:use")]
        public string buildinguse { get; set; }
        [JsonProperty("building:levels")]
        public string buildinglevels { get; set; }
        [JsonProperty("building:min_level")]
        public double buildingminlevel { get; set; }

        //height
        public double height { get; set;}
        public double min_height { get; set; }

        //roof
        [JsonProperty("roof:height")]
        public double roofheight { get; set; }
        [JsonProperty("roof:shape")]
        public string roofshape { get; set; }
        [JsonProperty("roof:orientation")]
        public string rooforientation { get; set; }
        [JsonProperty("roof:angle")]
        public double roofangle { get; set; }

        //surface
        [JsonProperty("building:colour")]
        public string buildingcolor { get; set; }
        [JsonProperty("roof:colour")]
        public string roofcolor { get; set; }
        [JsonProperty("building:material")]
        public string buildingmaterial { get; set; }
        [JsonProperty("roof:material")]
        public string roofmaterial { get; set; }
    }
}
