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
            : base("osm_bldgs", "Nickname",
                "Import OSM Buildings",
                "@it", "OSM")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Location", "L", "{latitude,longitude}", GH_ParamAccess.item);
            pManager.AddNumberParameter("Range", "r", "Range", GH_ParamAccess.item, 100);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("OSMID", "OSMID", "OSMID", GH_ParamAccess.list);
            pManager.AddBrepParameter("OSM3DBuildings", "OSM3DBLDGS", "OSM 3D Buildings", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double range = 100;
            string Location = string.Empty; // defense for Location

            if (!DA.SetData(0, Location)) { return; }
            if (!DA.SetData(1, range)) { }

            double x = range * Math.Pow(10,-5);


            List<string> out_bldgIDs = new List<string>();
            List<Brep> out_Geometry = new List<Brep>();

            string url = "http://api.openstreetmap.org/api/0.6/map?bbox=-73.9930835,40.7338633,-73.9890835,40.7378633";
            //"https://api.openstreetmap.org/api/0.6/map?bbox=-73.9915635,40.7380371,-73.9835635,40.7460371";

            string response = GetResponse(url);

            //// parse xml response
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response);


            //// Get all Nodes at OSM 
            //XmlNodeList nodeData = doc.GetElementsByTagName("node");

            //Dictionary<string, List<double>> nodes = new Dictionary<string, List<double>>();
            //for (int i = 0; i < nodeData.Count; i++)
            //{
            //    string id = nodeData[i].Attributes["id"].Value;
            //    List<double> coords = new List<double>();

            //    double lat = 0;
            //    Double.TryParse(nodeData[i].Attributes["lat"].Value, out lat);
            //    double lon = 0;
            //    Double.TryParse(nodeData[i].Attributes["lon"].Value, out lon);
            //    coords.Add(lat);
            //    coords.Add(lon);
            //    nodes.Add(id, coords);
            //}

            //// get all the data for ways
            XmlNodeList	wayData = doc.GetElementsByTagName("way");

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

                Brep outB = Get3Dbuilding(id, height,minheight,roofshape,roofheight);

                if (outB != null)
                {
                    out_Geometry.Add(outB);
                    out_bldgIDs.Add(id);
                }
                

                ////For OSM data parsing
                //foreach (XmlNode child in wayData[i].ChildNodes)
                //{
                //    // geometry
                //    if (child.Name =="nd")
                //    {
                //        string nodeid = child.Attributes["ref"].Value;
                //    }
                //    // add this to tag output
                //    if (child.Name == "tag")
                //    {   
                //        string k = child.Attributes["k"].Value;
                //        string v = child.Attributes["v"].Value;
                //    }
                //}
                
                // 
                
            }


            // Output Brep Geometry
            DA.SetDataList(0, out_Geometry);
            DA.SetDataList(1, out_Geometry);

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

        public static Brep Get3Dbuilding(string id, double osm_height, double osm_minheight, string roofshape, double roofheight )
        {

            Brep b = null ;

            string url = "http://data.osmbuildings.org/0.2/rkc8ywdl/feature/" + id + ".json";
            string response2 = GetResponse(url);

            OSMbuilding bldg = JsonConvert.DeserializeObject<OSMbuilding>(response2);
            // http://data.osmbuildings.org/0.2/rkc8ywdl/feature/248143998.json

            if (bldg.features.Length > 0)
            {
                // Create Brep
                foreach (var feat in bldg.features)
                {
                    if (bldg.features[0].geometry.type == "Polygon" && bldg.features[0].properties.tags.height != 0)
                    {
                        // Local variables
                        List<Point3d> mypoints = new List<Point3d>();
                        foreach (var coord in bldg.features[0].geometry.coordinates[0])
                        {
                            mypoints.Add(new Point3d(coord[0], coord[1], 0));
                        }

                        mypoints.RemoveAt(mypoints.Count - 1);

                        // Create GH_Curves
                        Curve footprint = new PolylineCurve(mypoints).ToNurbsCurve();
                        double minH = bldg.features[0].properties.tags.min_height;
                        if (minH != 0)
                        {
                            Vector3d bottomdir = new Vector3d(0, 0, bldg.features[0].properties.tags.min_height);
                            footprint.Translate(bottomdir);
                        }
                        // set upper curve
                        Vector3d topdir = new Vector3d(0, 0, bldg.features[0].properties.tags.height);
                        Curve topCrv = new PolylineCurve(mypoints).ToNurbsCurve();
                        topCrv.Translate(topdir);

                        List<Curve> crvlist = new List<Curve>(); crvlist.Add(footprint); crvlist.Add(topCrv);
                        Curve[] crvs = crvlist.ToArray();


                        Brep[] extrusion = Rhino.Geometry.Brep.CreateFromLoft(crvs, Point3d.Unset, Point3d.Unset, Rhino.Geometry.LoftType.Normal, false);


                        // get polygon profile + extrude 
                        if (bldg.features[0].properties.tags.roofshape == "flat" || bldg.features[0].properties.tags.roofshape == null)
                        {
                            b = extrusion[0].CapPlanarHoles(0.001);
                           
                        }
                        else if (bldg.features[0].properties.tags.roofshape == "pyramidal")
                        {
                            //create roof
                            Point3d cpt = Rhino.Geometry.AreaMassProperties.Compute(footprint).Centroid;
                            double aheight = bldg.features[0].properties.tags.roofheight;
                            Point3d topPt = Rhino.Geometry.Point3d.Add(cpt, new Vector3d(0, 0, aheight));

                            List<Brep> roofsrfs = new List<Brep>();

                            for (int i = 0; i < mypoints.Count - 1; i++)
                            {
                                roofsrfs.Add(Rhino.Geometry.Brep.CreateFromCornerPoints(mypoints[i], topPt, mypoints[i + 1], 0));
                            }

                            //Join extrusion and roof
                            Brep JoinedRoof = Rhino.Geometry.Brep.JoinBreps(roofsrfs, 0)[0];
                            JoinedRoof.Translate(0, 0, bldg.features[0].properties.tags.height);

                            //Join Extrusion and roof
                            List<Brep> geos = new List<Brep>(); geos.Add(extrusion[0]); geos.Add(JoinedRoof);
                            Brep joinedgeo = Rhino.Geometry.Brep.JoinBreps(geos, 0)[0];

                            b = joinedgeo.CapPlanarHoles(0.001);

                        }
                        else if (bldg.features[0].properties.tags.roofshape == "gabled")
                        {
                            double aheight = bldg.features[0].properties.tags.roofheight;
                            if (mypoints.Count == 4)
                            {
                                List<Point3d> list1 = new List<Point3d>(); list1.Add(mypoints[0]); list1.Add(mypoints[1]);
                                Curve crv1 = new PolylineCurve(list1).ToNurbsCurve();
                                Point3d mid0 = crv1.PointAt(0.5);
                                mid0.Z = aheight;

                                List<Point3d> list2 = new List<Point3d>(); list2.Add(mypoints[2]); list2.Add(mypoints[3]);
                                Curve crv2 = new PolylineCurve(list2).ToNurbsCurve();
                                Point3d mid1 = crv2.PointAt(0.5);
                                mid1.Z = aheight;

                                Brep roofSrf = Rhino.Geometry.Brep.CreateFromCornerPoints(mypoints[0], mid0, mid1, mypoints[3], 0);
                                roofSrf.Translate(0, 0, bldg.features[0].properties.tags.height);
                                Brep roofSrf2 = Rhino.Geometry.Brep.CreateFromCornerPoints(mypoints[1], mid0, mid1, mypoints[2], 0);
                                roofSrf2.Translate(0, 0, bldg.features[0].properties.tags.height);

                                //Join Extrusion and roof
                                List<Brep> geos = new List<Brep>(); geos.Add(extrusion[0]); geos.Add(roofSrf); geos.Add(roofSrf2);
                                Brep joinedgeo = Rhino.Geometry.Brep.JoinBreps(geos, 0)[0];

                                b = joinedgeo.CapPlanarHoles(0.001);
                            }

                        }
                        else if (bldg.features[0].properties.tags.roofshape == "skillion")
                        {
                            // first two points is the lowest 
                            double aheight = bldg.features[0].properties.tags.roofheight;
                            if (mypoints.Count == 4)
                            {
                                Point3d pt2 = new Point3d(mypoints[2].X, mypoints[2].Y, aheight);
                                Point3d pt3 = new Point3d(mypoints[3].X, mypoints[3].Y, aheight);
                                Brep roofSrf = Rhino.Geometry.Brep.CreateFromCornerPoints(mypoints[0], mypoints[1], pt2, pt3, 0);
                                roofSrf.Translate(0, 0, bldg.features[0].properties.tags.height);

                                //Join Extrusion and roof
                                List<Brep> geos = new List<Brep>(); geos.Add(extrusion[0]); geos.Add(roofSrf);
                                Brep joinedgeo = Rhino.Geometry.Brep.JoinBreps(geos, 0)[0];

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