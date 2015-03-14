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
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string url = "https://api.openstreetmap.org/api/0.6/map?bbox=-73.9915635,40.7380371,-73.9835635,40.7460371";
            



            List<string> bldgIDs = new List<string>();

            //char[] delimiterChars = {'<way id="'};
            //string[] words = response.Split(delimiterChars);

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

            for (int i = 0; i < wayData.Count; i++)
            {
                string id = wayData[i].Attributes["id"].Value;
                bldgIDs.Add(id);
            }

            List<OSMbuilding> Bldgs = new List<OSMbuilding>();
            foreach (var id in bldgIDs)
            {
                url = "http://data.osmbuildings.org/0.2/rkc8ywdl/feature/" + id + ".json";
                string response2 = GetResponse(url);

                OSMbuilding bldg = JsonConvert.DeserializeObject<OSMbuilding>(response2);
                // http://data.osmbuildings.org/0.2/rkc8ywdl/feature/248143998.json

                //if (bldg.features[0].geometry != null)
                //{
                //    //double[,] coor= bldg.features[0].geometry.Coordinates.

                //    //Bldgs.Add(bldg);
                //}
                
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
    }

}