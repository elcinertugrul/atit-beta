using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using System.Drawing;

using atit.Helpers;
using atit.Properties;

namespace atit
{
    class atitinfo : GH_AssemblyInfo
    {
        public override string AssemblyName
        {
            get
            {
                return "atit";
            }
        }
        public override string Name
        {
            get
            {
                return "shp@it";
            }
        }
        public override string Description
        {
            get
            {
                return "Provides tools for parsing vector data format(.shp), OSM, geo coder, data binding and more! ";
                //return base.Description;
            }
        }
        public override string AuthorName
        {
            get
            {
                return "Elcin Ertugrul";
            }
        }
        public override string AuthorContact
        {
            get
            {
                return "elcinertugrul@gmail.com";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                return Resources.shp_it24X24;

            }
        }
    }
}
