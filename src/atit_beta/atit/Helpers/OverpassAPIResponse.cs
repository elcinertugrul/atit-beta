using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace atit.Helpers
{
    public class OverpassAPIResponse
    {
        public elements[] elements { get; set; }
    }

    public class elements
    {
        public string type { get; set; }
        public double id { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public List<double> nodes { get; set; }
        //public Dictionary<string, string> tags { get; set; }
        public tags tags { get; set; }
        //public nodes[] nodes { get; set; }
    }

}
