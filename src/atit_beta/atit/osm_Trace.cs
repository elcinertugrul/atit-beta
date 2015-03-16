using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Xml;
using System.Drawing;
using Grasshopper;
using Grasshopper.Kernel.Data;

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
            pManager.AddBooleanParameter("UTM<->WGS84", "UTM<->Lat/Long", "Set map/data projection: True UTM, False WGS84", GH_ParamAccess.item, true);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("osm_IDs", "osm_IDs", "osm IDs", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Is osm_Bldg?", "Is Bldg?", "is a OSM building or building part", GH_ParamAccess.list);
            pManager.AddGeometryParameter("osm_Trace", "OSm_trace", "Tracing Open street map data", GH_ParamAccess.list);
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
            bool isUTMProjected = true;

            if (!DA.GetData(0, ref Location)) { return; }
            if (!DA.GetData(1, ref x_range)) { }
            if (!DA.GetData(2, ref y_range)) { }
            if (!DA.GetData(3, ref isUTMProjected)) { }

            double x = x_range * Math.Pow(10, -5);
            double y = y_range * Math.Pow(10, -5);

            // Output variables
            List<string> out_IDs = new List<string>();
            List<object> out_Geometry = new List<object>();
            List<Boolean> out_isBldgs = new List<Boolean>();
            DataTree<string> out_Key_Val = new DataTree<string>();

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

            // Get all Nodes at OSM (LL information)
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

            // Find Conversion factor
            string inUnits = Rhino.RhinoDoc.ActiveDoc.GetUnitSystemName(true, false, false, true);
            if (inUnits.ToLower() == "millimeter" || inUnits.ToLower() == "mm") inUnits = "mm";
            else if (inUnits.ToLower() == "meter" || inUnits.ToLower() == "m") inUnits = "m";
            else if (inUnits.ToLower() == "inch" || inUnits.ToLower() == "inches" || inUnits.ToLower() == "in") inUnits = "in";
            else if (inUnits.ToLower() == "foot" || inUnits.ToLower() == "feet" || inUnits.ToLower() == "ft") inUnits = "ft";

            double factor = Helpers.Converter.defineConversion("m", inUnits);
            if (!isUTMProjected)
            {
                factor = 1; // WGS84 or lat/long 
            }

            //// get all the data for ways
            int counter = 0;
            XmlNodeList wayData = doc.GetElementsByTagName("way");
            for (int i = 0; i < wayData.Count; i++)
            {
                // 1.  Get Id and add to output List
                out_IDs.Add(wayData[i].Attributes["id"].Value);

                // 2.  Draw geometry and add to output List
                List<Point3d> mypoints = new List<Point3d>();
                // 2a.Get  Node List
                foreach (XmlNode child in wayData[i].ChildNodes)
                {
                    // geometry
                    if (child.Name == "nd")
                    {
                        string nodeid = child.Attributes["ref"].Value;
                        if (nodes.ContainsKey(nodeid))
                        {
                            List<double> coords = nodes[nodeid]; // lat, longt
                            if (isUTMProjected)
                            {
                                // Convert lat/long
                                double easting = 0;
                                double northing = 0;
                                Helpers.Converter.ConvertToUtmString(coords[1], coords[0], ref easting, ref northing);

                                mypoints.Add(new Point3d(easting * factor, northing * factor, 0));
                            }
                            else
                            {
                                mypoints.Add(new Point3d(coords[1],coords[0], 0));
                            }

                        }
                    }
                }

                Curve myGeometry = new PolylineCurve(mypoints).ToNurbsCurve();

                out_Geometry.Add(myGeometry);


                // 3. Get Key/value and add to list
                GH_Path myOutPath = new GH_Path(counter);
                Boolean isbldg = false;
                foreach (XmlNode child in wayData[i].ChildNodes)
                {
                    // add this to tag output
                    if (child.Name == "tag")
                    {
                        string k = child.Attributes["k"].Value;
                        string v = child.Attributes["v"].Value;
                        out_Key_Val.Add(k + ":" + v, myOutPath);
                        if (child.Attributes["k"].Value.Contains("building"))
                        {
                            isbldg = true;
                        }
                    }
                }

                out_isBldgs.Add(isbldg);

                counter++;
                 
            }

            // Populate outputs
            DA.SetDataList(0, out_IDs);
            DA.SetDataList(1, out_isBldgs);
            DA.SetDataList(2, out_Geometry);
            DA.SetDataTree(3, out_Key_Val);

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