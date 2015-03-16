using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using atit.Helpers;
using atit.Properties;

namespace atit
{
    public class GeoCoder_AddressLookup : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GeoCoder_AddressLookup class.
        /// </summary>
        public GeoCoder_AddressLookup()
            : base("Address Lookup", "Address Lookup",
                "Reverse Geocoding: Address from {latitude,longitude} value",
                "@it", "GeoCoder")
        {
        }

        public override GH_Exposure Exposure
        {
            //expose the object in the section on the toolbar
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Latitude,Longitude", "LL", "{latitude,longitude}", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Formatted Adress", "Addr", "Formatted Address", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string in_loc = string.Empty;

            if (!DA.GetData(0, ref in_loc)) { return; }
            GeoCoder gc = new GeoCoder();
            GoogleGeoCodeResponse gresponse = gc.ReverseGeocoding(in_loc);

            List<string> out_adress = new List<string>();

            foreach (var item in gresponse.results)
            {
                out_adress.Add(item.formatted_address);
            }
            DA.SetDataList(0, out_adress);
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
                return Resources.Address_lookup;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{6d0404ed-883e-44f0-a990-9bcb78db55e6}"); }
        }
    }
}