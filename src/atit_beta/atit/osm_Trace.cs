using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Xml;

namespace atit
{
    public class osm_Trace : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the osm_Trace class.
        /// </summary>
        public osm_Trace()
            : base("osm_Trace", "osm_Trace",
                "Trace OSM data",
                "@it", "OSM")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Location", "LL", "{latitude,longitude}", GH_ParamAccess.item);
            pManager.AddNumberParameter("X_Range", "x_r", "X Range", GH_ParamAccess.item, 100);
            pManager.AddNumberParameter("Y_Range", "y_r", "Y Range", GH_ParamAccess.item, 100);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("osmID", "osm_ID", "osm ID", GH_ParamAccess.list);
            pManager.AddGeometryParameter("trace", "trace", "Tracing OSM", GH_ParamAccess.list);
            pManager.AddTextParameter("key:value", "key:value", "key:value pairs", GH_ParamAccess.tree);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double x_range = 100;
            double y_range = 100;
            string Location = string.Empty; // defense for LL

            if (!DA.GetData(0, ref Location)) { return; }
            if (!DA.GetData(1, ref x_range)) { }
            if (!DA.GetData(2, ref y_range)) { }

            double x = x_range * Math.Pow(10, -5);
            double y = y_range * Math.Pow(10, -5);

            List<object> out_geometry = new List<object>();

            // Construct url = (min long, min lat, max long, max lat)
            string[] words = Location.Split(',');
            double in_lat = 0;
            double in_lon = 0;
            Double.TryParse(words[0], out in_lat);
            Double.TryParse(words[1], out in_lon);

            string minlong = (in_lon - x).ToString();
            string maxlog = (in_lon + x).ToString();
            string minlat = (in_lat - y).ToString();
            string maxlat = (in_lat + y).ToString();

            string url = "http://api.openstreetmap.org/api/0.6/map?bbox=" + minlong + "," + minlat + "," + maxlog + "," + maxlat;

            string response = osm_bldgs.GetResponse(url);

            //// parse xml response
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response);

            // Get all Nodes at OSM 
            XmlNodeList nodeData = doc.GetElementsByTagName("node");

            Dictionary<string, List<double>> nodes = new Dictionary<string, List<double>>();
            for (int i = 0; i < nodeData.Count; i++)
            {
                string id = nodeData[i].Attributes["id"].Value;
                List<double> coords = new List<double>();

                double lat = 0;
                Double.TryParse(nodeData[i].Attributes["lat"].Value, out lat);
                double lon = 0;
                Double.TryParse(nodeData[i].Attributes["lon"].Value, out lon);
                coords.Add(lat);
                coords.Add(lon);
                nodes.Add(id, coords);
            }

            //// get all the data for ways
            XmlNodeList wayData = doc.GetElementsByTagName("way");
            for (int i = 0; i < wayData.Count; i++)
            {
                
            }

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
            get { return new Guid("{393e099b-199d-4cf4-978f-fa2869202403}"); }
        }
    }
}