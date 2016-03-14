using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Net;
using GeoJSON;
using GeoJSON.Net;
using GeoJSON.Net.Converters;
using Newtonsoft.Json;
using System.IO;

// http://geojson.org/geojson-spec.html

namespace atit
{
    public class GeoJSON_Import : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GeoJSON_Import class.
        /// </summary>
        public GeoJSON_Import()
            : base("GeoJSON_Import", "Nickname",
                "Description",
                "@it", "GeoJSON")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Shapefile (.shp) Path", "C:/", "Specify path of the geoJSON file", GH_ParamAccess.item);
            pManager.AddBooleanParameter("True or False", "T||F", "Set Boolean True to import geoJSON files", GH_ParamAccess.item);
            pManager[1].Optional = false;
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
            //input variable
            string filepath = string.Empty;
            bool trigger = false;

            if (!DA.GetData(0, ref filepath)) { return; }
            if (!DA.GetData(1, ref trigger)) { return; }

            //
            string response = null;
            HttpWebRequest httpWebRequest = null;//Declare an HTTP-specific implementation of the WebRequest class.
            HttpWebResponse httpWebResponse = null;//Declare an HTTP-specific implementation of the WebResponse class

            //Creates an HttpWebRequest for the specified URL.
            httpWebRequest = (HttpWebRequest)WebRequest.Create(filepath);

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


            //GeoJSONObject obj = JsonConvert.DeserializeObject<GeoJSONObject>(response);
            
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
            get { return new Guid("{dc0f3bcf-a825-443f-afd8-0a57cb141fb3}"); }
        }
    }
}