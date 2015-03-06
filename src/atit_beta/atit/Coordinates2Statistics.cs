// by Elcin Ertugrul 2015/02/15
// DataScience Toolkit

//http://www.datasciencetoolkit.org/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Net;
using System.Xml.Linq;
using Newtonsoft.Json; // http://json.codeplex.com/

using Grasshopper.Kernel;
using atit.Helpers;
using atit.Properties;

namespace atit
{
    public class Coordinates2Statistics : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Coordinates2Statistics class.
        /// </summary>
        public Coordinates2Statistics()
            : base("Coordinates2Statistics", "Coordinates2Statistics",
                "Description",
                "@it", "Statistics")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Location", "L", "{latitude,longitude}", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Population_Density", "Population", "Population Density", GH_ParamAccess.item);
            pManager.AddTextParameter("Elevation", "Elevation", "Elevation", GH_ParamAccess.item);
            pManager.AddTextParameter("ElevationUnits", "ElevationUnits", "ElevationUnits", GH_ParamAccess.item);
            pManager.AddTextParameter("Landcover", "Landcover", "Landcover", GH_ParamAccess.item);
            pManager.AddTextParameter("Notes", "Notes", "Notes", GH_ParamAccess.list);
            pManager.AddTextParameter("Sources", "Sources", "Sources", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string inCoordinates = string.Empty;
            if (!DA.GetData(0, ref inCoordinates)) { return; }

            char deliminator = ',';
            string[] words = inCoordinates.Split(deliminator);
            string lat = words[0].Substring(0, 5);
            string lon = words[1].Substring(0, 5);

            // output variable
            List<string> descriptions = new List<string>();
            List<string> sources = new List<string>();

            //string url = "http://www.datasciencetoolkit.org/coordinates2statistics/" + inCoordinates + "?statistics=population_density";
            string url = "http://www.datasciencetoolkit.org/coordinates2statistics/" + lat+","+ lon + "?statistics=population_density,elevation,land_cover";
            WebRequest request = WebRequest.Create(url);
            WebResponse myresponse = request.GetResponse();

            var result = new System.Net.WebClient().DownloadString(url);
            Data[] test = JsonConvert.DeserializeObject<Data[]>(result);

            string p = test[0].statistics.population_density.value;
            descriptions.Add(test[0].statistics.population_density.description);
            sources.Add(test[0].statistics.population_density.source_name);

            string e = test[0].statistics.elevation.value;
            string eu = test[0].statistics.elevation.units;
            descriptions.Add(test[0].statistics.elevation.description);
            sources.Add(test[0].statistics.elevation.source_name);

            string lc = test[0].statistics.land_cover.value;
            descriptions.Add(test[0].statistics.land_cover.description);
            sources.Add(test[0].statistics.land_cover.source_name);

            DA.SetData(0, p);
            DA.SetData(1, e);
            DA.SetData(2, eu);
            DA.SetData(3, lc);
            DA.SetDataList(4, descriptions);
            DA.SetDataList(5, sources);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{89917c18-5770-4608-b593-73aeb47d7983}"); }
        }
    }
}