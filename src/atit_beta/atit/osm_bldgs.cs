// by Elcin Ertugrul 2015/02/15
// OSM data

//http://www.datasciencetoolkit.org/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Net;
using System.Xml.Linq;
using Newtonsoft.Json; // http://json.codeplex.com/
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using atit.Helpers;
using atit.Properties;
using System.IO;
using System.Xml;

namespace atit
{
    public class osm_bldgs : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the osm_bldgs class.
        /// </summary>
        public osm_bldgs()
            : base("osm_Bldgs", "osm_Buildings",
                "Imports & Draws OSM 3D Buildings",
                "@it", "OSM")
        {
        }

        public override GH_Exposure Exposure
        {
            //expose the object in the section on the toolbar
            get { return GH_Exposure.secondary; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Latitude,Longitude", "LL", "{latitude,longitude}", GH_ParamAccess.item);
            pManager.AddNumberParameter("Lat_Range", "lat_r", "Latitude Range", GH_ParamAccess.item, 100);
            pManager.AddNumberParameter("Long_Range", "lon_r", "Longitude Range", GH_ParamAccess.item, 100);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("osm_IDs", "osm_IDs", "osm IDs", GH_ParamAccess.list);
            pManager.AddBrepParameter("osm_3D_bldgs", "osm_3D_Bldgs", "OSM 3D Buildings", GH_ParamAccess.list); 
            pManager.AddColourParameter("osm_Bldg_Colours", "osm_Bldg_Colours", "For OSM Buildings and Land Use Colours See http://wiki.openstreetmap.org/wiki/OpenStreetBrowser/Landuse-_and_Building_Colors", GH_ParamAccess.list);
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

            List<string> out_bldgIDs = new List<string>();
            List<Brep> out_Geometry = new List<Brep>();
            List<Color> out_Colors = new List<Color>();

            // Construct url = (min long, min lat, max long, max lat)
            string[] words = Location.Split(',');
            double lat = 0;
            double lon = 0;
            Double.TryParse(words[0], out lat);
            Double.TryParse(words[1], out lon);

            string minlong = (lon - x).ToString();
            string maxlog = (lon + x).ToString();
            string minlat = (lat - y).ToString();
            string maxlat = (lat + y).ToString();

           string url = "http://api.openstreetmap.org/api/0.6/map?bbox="+ minlong + "," + minlat + "," +  maxlog + "," + maxlat;
            // string url = "http://api.openstreetmap.org/api/0.6/map?bbox=-73.9930835,40.7338633,-73.9890835,40.7378633";
            //"https://api.openstreetmap.org/api/0.6/map?bbox=-73.9915635,40.7380371,-73.9835635,40.7460371";

            string response = GetResponse(url);

            //// parse xml response
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response);

            //// get all the data for ways
            XmlNodeList wayData = doc.GetElementsByTagName("way");

            string roofshape = "flat";
            double roofheight = 0;
            double height = 0;
            double minheight = 0;

            for (int i = 0; i < wayData.Count; i++)
            {
                string id = wayData[i].Attributes["id"].Value;

                foreach (XmlNode child in wayData[i].ChildNodes)
                {

                    // add this to tag output
                    if (child.Name == "tag")
                    {
                        string k = child.Attributes["k"].Value;
                        if (k == "roof:shape")
                        {
                            roofshape = child.Attributes["v"].Value;
                        }
                        else if (k == "height")
                        {
                            Double.TryParse(child.Attributes["v"].Value, out height);
                        }
                        else if (k == "min_height")
                        {
                            Double.TryParse(child.Attributes["v"].Value, out minheight);
                        }
                        else if (k == "roof:height")
                        {
                            Double.TryParse(child.Attributes["v"].Value, out roofheight);
                        }
                    }
                }

                Color c = Color.White;

                string inUnits = Rhino.RhinoDoc.ActiveDoc.GetUnitSystemName(true, false, false, true);
                if (inUnits.ToLower() == "millimeter" || inUnits.ToLower() == "mm") inUnits = "mm";
                else if (inUnits.ToLower() == "meter" || inUnits.ToLower() == "m") inUnits = "m";
                else if (inUnits.ToLower() == "inch" || inUnits.ToLower() == "inches" || inUnits.ToLower() == "in") inUnits = "in";
                else if (inUnits.ToLower() == "foot" || inUnits.ToLower() == "feet" || inUnits.ToLower() == "ft") inUnits = "ft";

                double factor = Helpers.Converter.defineConversion(inUnits, "m"); // test it out

                Brep outB = Get3Dbuilding(id, height, minheight, roofshape, roofheight, ref c, factor);

                if (outB != null)
                {
                    out_Geometry.Add(outB);
                    out_bldgIDs.Add(id);
                    out_Colors.Add(c);
                }

            }

            // Set Outputs
            DA.SetDataList(0, out_bldgIDs);
            DA.SetDataList(1, out_Geometry);
            DA.SetDataList(2, out_Colors);
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
                return Resources.OSM_bldgs_icon_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{5115ba5a-2938-4938-99be-955b5e625639}"); }
        }


        public static string GetResponse(string url)
        {
            string response = null;
            HttpWebRequest httpWebRequest = null;//Declare an HTTP-specific implementation of the WebRequest class.
            HttpWebResponse httpWebResponse = null;//Declare an HTTP-specific implementation of the WebResponse class

            //Creates an HttpWebRequest for the specified URL.
            httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            try
            {
                //Sends the HttpWebRequest, and waits for a response.
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    //Get response stream into StreamReader
                    using (Stream responseStream = httpWebResponse.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                            response = reader.ReadToEnd();
                    }
                }
                httpWebResponse.Close();//Close HttpWebResponse
            }
            catch (WebException we)
            {   //TODO: Add custom exception handling
                throw new Exception(we.Message);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            finally
            {
                httpWebResponse.Close();
                //Release objects
                httpWebResponse = null;
                httpWebRequest = null;
            }

            return response;
        }

        public static Brep Get3Dbuilding(string id, double osm_height, double osm_minheight, string roofshape, double roofheight, ref Color c , double factor)
        {

            Brep b = null;

            string url = "http://data.osmbuildings.org/0.2/rkc8ywdl/feature/" + id + ".json";
            string response2 = GetResponse(url);

            OSMbuilding bldg = JsonConvert.DeserializeObject<OSMbuilding>(response2);
            // http://data.osmbuildings.org/0.2/rkc8ywdl/feature/248143998.json


            if (bldg.features.Length > 0)
            {
                // Create Brep
                foreach (var feat in bldg.features)
                {
                    if (bldg.features[0].geometry.type == "Polygon" && bldg.features[0].properties.height != 0)
                    {
                        // Color 
                        if (bldg.features[0].properties.color != null)
                        {
                            c = System.Drawing.ColorTranslator.FromHtml(bldg.features[0].properties.color); 
                        }

                        // Local variables
                        List<Point3d> mypoints = new List<Point3d>();
                        foreach (var coord in bldg.features[0].geometry.coordinates[0])
                        {
                            double easting = 0;
                            double northing = 0;
                            Helpers.Converter.ConvertToUtmString(coord[0], coord[1], ref easting, ref northing);

                            mypoints.Add(new Point3d(easting*factor, northing*factor, 0));
                        }

                        //mypoints.RemoveAt(mypoints.Count - 1);

                        // Create GH_Curves
                        Rhino.Geometry.Curve footprint = new PolylineCurve(mypoints).ToNurbsCurve();
                        bool isclosed = footprint.IsClosed;

                        // get polygon profile + extrude 
                        if (bldg.features[0].properties.roofShape == "flat" || bldg.features[0].properties.roofShape == null) // || bldg.features[0].properties.tags.roofshape == "flat" || bldg.features[0].properties.tags.roofshape == null
                        {
                            // set upper curve
                            Vector3d topdir = new Vector3d(0, 0, bldg.features[0].properties.height * factor);
                            Rhino.Geometry.Curve topCrv = new PolylineCurve(mypoints).ToNurbsCurve();
                            topCrv.Translate(topdir);
                            // set lower curve
                            double minH = bldg.features[0].properties.minHeight; //  bldg.features[0].properties.tags.min_height;
                            if (minH != 0)
                            {
                                Vector3d bottomdir = new Vector3d(0, 0, minH * factor);
                                footprint.Translate(bottomdir);
                            }

                            List<Rhino.Geometry.Curve> crvslist = new List<Rhino.Geometry.Curve>(); crvslist.Add(footprint); crvslist.Add(topCrv);
                            Rhino.Geometry.Curve[] crvs = crvslist.ToArray();

                            Brep[] extrusion = Rhino.Geometry.Brep.CreateFromLoft(crvs, Point3d.Unset, Point3d.Unset, Rhino.Geometry.LoftType.Normal, false);

                            b = extrusion[0].CapPlanarHoles(0.001);
                        }
                        else if (bldg.features[0].properties.roofShape == "pyramidal" || bldg.features[0].properties.roofShape == "pyramid") //|| bldg.features[0].properties.tags.roofshape == "pyramidal"
                        {

                            // Create roof (footprint it on the 0 level)
                            Point3d cpt = Rhino.Geometry.AreaMassProperties.Compute(footprint).Centroid;
                            double aheight = (bldg.features[0].properties.roofHeight) * factor; //bldg.features[0].properties.tags.roofheight * Z_factor;
                            Point3d topPt = Rhino.Geometry.Point3d.Add(cpt, new Vector3d(0, 0, aheight));

                            List<Brep> roofsrfs = new List<Brep>();

                            for (int i = 0; i < mypoints.Count - 1; i++)
                            {
                                roofsrfs.Add(Rhino.Geometry.Brep.CreateFromCornerPoints(mypoints[i], topPt, mypoints[i + 1], 0.001));
                            }

                            //Join extrusion and roof
                            Brep JoinedRoof = Rhino.Geometry.Brep.JoinBreps(roofsrfs, 0.001)[0];
                            double bheight = (bldg.features[0].properties.height - bldg.features[0].properties.roofHeight) * factor;
                            JoinedRoof.Translate(0, 0, bheight);

                            //Create extrusion
                            // set upper curve
                            Vector3d topdir = new Vector3d(0, 0, bheight);
                            Rhino.Geometry.Curve topCrv = new PolylineCurve(mypoints).ToNurbsCurve();
                            topCrv.Translate(topdir);
                            // set lower curve
                            double minH = bldg.features[0].properties.minHeight; //  bldg.features[0].properties.tags.min_height;
                            if (minH != 0)
                            {
                                Vector3d bottomdir = new Vector3d(0, 0, minH * factor);
                                footprint.Translate(bottomdir);
                            }

                            List<Rhino.Geometry.Curve> crvslist = new List<Rhino.Geometry.Curve>(); crvslist.Add(footprint); crvslist.Add(topCrv);
                            Rhino.Geometry.Curve[] crvs = crvslist.ToArray();

                            Brep[] extrusion = Rhino.Geometry.Brep.CreateFromLoft(crvs, Point3d.Unset, Point3d.Unset, Rhino.Geometry.LoftType.Normal, false);


                            //Join Extrusion and roof
                            List<Brep> geos = new List<Brep>(); geos.Add(extrusion[0]); 
                            geos.Add(JoinedRoof);
                            Brep joinedgeo = Rhino.Geometry.Brep.JoinBreps(geos, 0.001)[0];

                            b = joinedgeo.CapPlanarHoles(0.001);

                        }
                        else if (bldg.features[0].properties.roofShape == "gabled") //|| bldg.features[0].properties.tags.roofshape == "gabled"
                        {
                            double aheight = bldg.features[0].properties.roofHeight * factor;
                            double bheight = (bldg.features[0].properties.height - bldg.features[0].properties.roofHeight) * factor;
                            if (mypoints.Count == 4)
                            {
                                //create roof
                                List<Point3d> list1 = new List<Point3d>(); list1.Add(mypoints[0]); list1.Add(mypoints[1]);
                                Rhino.Geometry.Curve crv1 = new PolylineCurve(list1).ToNurbsCurve();
                                Point3d mid0 = crv1.PointAt(0.5);
                                mid0.Z = aheight;

                                List<Point3d> list2 = new List<Point3d>(); list2.Add(mypoints[2]); list2.Add(mypoints[3]);
                                Rhino.Geometry.Curve crv2 = new PolylineCurve(list2).ToNurbsCurve();
                                Point3d mid1 = crv2.PointAt(0.5);
                                mid1.Z = aheight;

                                Brep roofSrf = Rhino.Geometry.Brep.CreateFromCornerPoints(mypoints[0], mid0, mid1, mypoints[3], 0);
                                roofSrf.Translate(0, 0, bheight);
                                Brep roofSrf2 = Rhino.Geometry.Brep.CreateFromCornerPoints(mypoints[1], mid0, mid1, mypoints[2], 0);
                                roofSrf2.Translate(0, 0, bheight);

                                //Create extrusion
                                // set upper curve
                                Vector3d topdir = new Vector3d(0, 0, bheight);
                                Rhino.Geometry.Curve topCrv = new PolylineCurve(mypoints).ToNurbsCurve();
                                topCrv.Translate(topdir);
                                // set lower curve
                                double minH = bldg.features[0].properties.minHeight; //  bldg.features[0].properties.tags.min_height;
                                if (minH != 0)
                                {
                                    Vector3d bottomdir = new Vector3d(0, 0, minH * factor);
                                    footprint.Translate(bottomdir);
                                }

                                List<Rhino.Geometry.Curve> crvslist = new List<Rhino.Geometry.Curve>(); crvslist.Add(footprint); crvslist.Add(topCrv);
                                Rhino.Geometry.Curve[] crvs = crvslist.ToArray();

                                Brep[] extrusion = Rhino.Geometry.Brep.CreateFromLoft(crvs, Point3d.Unset, Point3d.Unset, Rhino.Geometry.LoftType.Normal, false);

                                //Join Extrusion and roof
                                List<Brep> geos = new List<Brep>(); 
                                geos.Add(extrusion[0]); 
                                geos.Add(roofSrf); 
                                geos.Add(roofSrf2);
                                Brep joinedgeo = Rhino.Geometry.Brep.JoinBreps(geos, 0.001)[0];

                                b = joinedgeo.CapPlanarHoles(0.001);
                            }

                        }
                        else if (bldg.features[0].properties.roofShape == "skillion") // || bldg.features[0].properties.tags.roofshape == "skillion"
                        {
                            // first two points is the lowest 
                            double aheight = bldg.features[0].properties.roofHeight * factor;
                            double bheight = (bldg.features[0].properties.height - bldg.features[0].properties.roofHeight) * factor;
                            if (mypoints.Count == 4)
                            {
                                // Create roof
                                Point3d pt2 = new Point3d(mypoints[2].X, mypoints[2].Y, aheight);
                                Point3d pt3 = new Point3d(mypoints[3].X, mypoints[3].Y, aheight);
                                Brep roofSrf = Rhino.Geometry.Brep.CreateFromCornerPoints(mypoints[0], mypoints[1], pt2, pt3, 0);
                                roofSrf.Translate(0, 0, bheight);

                                //Create extrusion
                                // set upper curve
                                Vector3d topdir = new Vector3d(0, 0, bheight);
                                Rhino.Geometry.Curve topCrv = new PolylineCurve(mypoints).ToNurbsCurve();
                                topCrv.Translate(topdir);
                                // set lower curve
                                double minH = bldg.features[0].properties.minHeight; //  bldg.features[0].properties.tags.min_height;
                                if (minH != 0)
                                {
                                    Vector3d bottomdir = new Vector3d(0, 0, minH * factor);
                                    footprint.Translate(bottomdir);
                                }

                                List<Rhino.Geometry.Curve> crvslist = new List<Rhino.Geometry.Curve>(); crvslist.Add(footprint); crvslist.Add(topCrv);
                                Rhino.Geometry.Curve[] crvs = crvslist.ToArray();

                                Brep[] extrusion = Rhino.Geometry.Brep.CreateFromLoft(crvs, Point3d.Unset, Point3d.Unset, Rhino.Geometry.LoftType.Normal, false);

                                //Join Extrusion and roof
                                List<Brep> geos = new List<Brep>();
                                geos.Add(extrusion[0]); 
                                geos.Add(roofSrf);
                                Brep joinedgeo = Rhino.Geometry.Brep.JoinBreps(geos, 0.001)[0];

                                b = joinedgeo.CapPlanarHoles(0.001);
                            }

                        }
                    }
                }

            }
            return b;

        }

    }

}