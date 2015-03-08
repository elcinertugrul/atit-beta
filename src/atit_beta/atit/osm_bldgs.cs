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
            
            //WebRequest request = WebRequest.Create(url);
            //WebResponse myresponse = request.GetResponse();

            //var result = new System.Net.WebClient().DownloadString(url);

            string response = null;
            HttpWebRequest httpWebRequest = null;//Declare an HTTP-specific implementation of the WebRequest class.
            HttpWebResponse httpWebResponse = null;//Declare an HTTP-specific implementation of the WebResponse class

            //Creates an HttpWebRequest for the specified URL.
            httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            try
            {
                //byte[] bytes;
                //bytes = System.Text.Encoding.ASCII.GetBytes(xmlContent);
                ////Set HttpWebRequest properties
                //httpWebRequest.Method = "POST";
                //httpWebRequest.ContentLength = bytes.Length;
                //httpWebRequest.ContentType = "text/xml; encoding='utf-8'";

                //using (Stream requestStream = httpWebRequest.GetRequestStream())
                //{
                //    //Writes a sequence of bytes to the current stream 
                //    requestStream.Write(bytes, 0, bytes.Length);
                //    requestStream.Close();//Close stream
                //}

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
            
            // parse xml response
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response);

            XmlNodeList nodeData = doc.GetElementsByTagName("node");

            Dictionary<string, string> nodes = new Dictionary<string, string>();
            for (int i = 0; i < nodeData.Count; i++)
            {
                string id = nodeData[i].Attributes["id"].Value;
                string lat = nodeData[i].Attributes["lat"].Value;
                string lon = nodeData[i].Attributes["lon"].Value;
            }

            // get all the data for ways
			XmlNodeList	wayData = doc.GetElementsByTagName("way");

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
    }
}