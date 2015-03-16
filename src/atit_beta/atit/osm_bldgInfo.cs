using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using atit.Helpers;
using Newtonsoft.Json;


namespace atit
{
    public class osm_bldgInfo : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the osm_bldgInfo class.
        /// </summary>
        public osm_bldgInfo()
            : base("osm_bldgInfo", "osm_bldgInfo",
                "Gets information of OSM Bldgs",
                "@it", "OSM")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("osmID", "osm_ID", "osm ID", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("geoJson", "geoJson", "geoJson", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string id = string.Empty;

            if (!DA.GetData(0, ref id)) { return; }

            string out_json = string.Empty;

            string url = "http://data.osmbuildings.org/0.2/rkc8ywdl/feature/" + id + ".json";
            string response2 = osm_bldgs.GetResponse(url);

            OSMbuilding bldg = JsonConvert.DeserializeObject<OSMbuilding>(response2);

            DA.SetData(0, response2);
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
            get { return new Guid("{94ec8213-063c-4dcf-9bdf-97a7337df166}"); }
        }
    }
}