using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public tags tags { get; set; }
    }

    public class tags
    {
        public string height { get; set;}
    }
}
