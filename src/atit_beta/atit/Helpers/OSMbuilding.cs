using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

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
        public string roofShape { get; set; }
        public double roofHeight { get; set; }
        public tags tags { get; set; }
    }

    public class tags
    {
        //height
        public double height { get; set;}
        public double min_height { get; set; }
        //roof
        [DataMember(Name= "roof:shape", EmitDefaultValue = false)]
        public string roofshape { get; set; }
        [DataMember(Name = "roof:height", EmitDefaultValue = false)]
        public double roofheight { get; set; }
        [DataMember(Name = "roof:orientation", EmitDefaultValue = false)]
        public string rooforientation { get; set; }
        [DataMember(Name = "roof:angle", EmitDefaultValue = false)]
        public string roofangle { get; set; }
        //surface

    }
}
