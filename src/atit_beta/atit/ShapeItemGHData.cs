using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using atit.Helpers;
using atit.Properties;

namespace atit
{
    // http://www.grasshopper3d.com/forum/topics/custom-data-and-parameter-no-implicit-reference-conversion
    class ShapeItemGHData : GH_Goo<ShapeItem>
    {
        public override IGH_Goo Duplicate()
        {
            ShapeItemGHData sid = new ShapeItemGHData();
            sid.Value = Value;
            return sid;
            //throw new NotImplementedException();
        }

        public override bool IsValid
        {
            get
            {
                //return Value.Pts.Count >= 0;
                //throw new NotImplementedException(); 
                return Value.AttsDict.Count >= 0;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", Value.Pts, Value.AttsDict);
            //throw new NotImplementedException();
        }

        public override string TypeDescription
        {
            get {
                return "Shape Objects to hold data";
                //throw new NotImplementedException(); 
            }
        }

        public override string TypeName
        {
            get
            {
                return "ShpObject";
                //throw new NotImplementedException(); 
            }
        }
    }
}
