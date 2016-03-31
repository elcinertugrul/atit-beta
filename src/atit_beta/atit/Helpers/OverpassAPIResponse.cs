using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace atit.Helpers
{
    public class OverpassAPIResponse
    {
        public elements[] results { get; set; }
    }

    public class elements
    {
        public string type { get; set; }
        public string id { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public List<string> nodes { get; set; }
        public Dictionary<string, string> tags { get; set; }
        //public nodes[] nodes { get; set; }
    }

    //public class nodes
    //{
       
    //}

    //public class tags
    //{

    //}

}
