// by Elcin Ertugrul 2015/02/15
// Google GeoCoder

using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using atit.Helpers;
using atit.Properties;

namespace atit
{
    public class GeoCoder_AddressParser : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GeoCoder_AdressParser class.
        /// </summary>
        public GeoCoder_AddressParser()
            : base("Address Parser", "Address Parser",
                "Uses Google API to parse addresses",
                "@it", "GeoCoder")
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
            pManager.AddTextParameter("Address", "Addr", "Input string of any address or name of landmark, no need it to be perfect!  Will find it for u!", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Formatted Adress", "Addr", "Formatted Address", GH_ParamAccess.list);
            pManager.AddTextParameter("Latitude,Longitude", "LL", "{latitude,longitude}", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string inadress = string.Empty;

            if (!DA.GetData(0, ref inadress)) { return; }

            GeoCoder gc = new GeoCoder();
            GoogleGeoCodeResponse gresponse = gc.AdressWedRequest(inadress);

            List<string> out_adress = new List<string>();
            List<string> out_locations = new List<string>();
            foreach (var item in gresponse.results)
            {
                out_adress.Add(item.formatted_address);
                string loc = item.geometry.location.lat + "," + item.geometry.location.lng;
                out_locations.Add(loc);
            }
            DA.SetDataList(0, out_adress);
            DA.SetDataList(1, out_locations);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Resources.GeoCoder;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{027d0a8b-33ae-4ada-8467-f390cfafa5ef}"); }
        }
    }
}