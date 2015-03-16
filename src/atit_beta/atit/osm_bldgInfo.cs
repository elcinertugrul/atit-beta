using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using atit.Helpers;
using Newtonsoft.Json;
using System.Drawing;
using System.Runtime.Serialization;
using System.IO;
using System.Text;


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
            pManager.AddTextParameter("osm_Bldg_ID", "osm_Bldg_ID", "Parses OSM Building data", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("name", "name", "name", GH_ParamAccess.item);
            pManager.AddTextParameter("building", "building", "building", GH_ParamAccess.item);
            pManager.AddTextParameter("building:part", "building:part", "building:part", GH_ParamAccess.item);
            pManager.AddTextParameter("building:use", "building:use", "building:use", GH_ParamAccess.item);
            pManager.AddNumberParameter("building:levels", "building:levels", "building:levels", GH_ParamAccess.item);
            pManager.AddNumberParameter("building:min_level", "building:min_level", "building:min_level", GH_ParamAccess.item);
            pManager.AddTextParameter("building:material", "building:material", "building:material", GH_ParamAccess.item);
            pManager.AddColourParameter("building:color", "building:color", "building:color", GH_ParamAccess.item);

            pManager.AddNumberParameter("height", "height", "height", GH_ParamAccess.item);
            pManager.AddNumberParameter("min_height", "min_height", "min_height", GH_ParamAccess.item);

            pManager.AddNumberParameter("roof:height", "roof:height", "roof:height", GH_ParamAccess.item);
            pManager.AddTextParameter("roof:shape", "roof:shape", "roof:shape", GH_ParamAccess.item);
            pManager.AddTextParameter("roof:orientation", "roof:orientation", "roof:orientation", GH_ParamAccess.item);
            pManager.AddNumberParameter("roof:angle", "roof:angle", "roof:angle", GH_ParamAccess.item);
            pManager.AddTextParameter("roof:material", "roof:material", "roof:material", GH_ParamAccess.item);
            pManager.AddColourParameter("roof:color", "roof:color", "roof:color", GH_ParamAccess.item);
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

            DA.SetData(0, bldg.features[0].properties.tags.name);
            DA.SetData(1, bldg.features[0].properties.tags.building);
            DA.SetData(2, bldg.features[0].properties.tags.buildingpart);
            DA.SetData(3, bldg.features[0].properties.tags.buildinguse);
            DA.SetData(4, bldg.features[0].properties.tags.buildinglevels);
            DA.SetData(5, bldg.features[0].properties.tags.buildingminlevel);
            DA.SetData(6, bldg.features[0].properties.tags.buildingmaterial);            
            // Color 
            Color c = Color.White;
            if (bldg.features[0].properties.tags.buildingcolor != null)
            {
                c = System.Drawing.ColorTranslator.FromHtml(bldg.features[0].properties.tags.buildingcolor);
                DA.SetData(7, c);
            }
            else if (bldg.features[0].properties.color != null)
            {
                c = System.Drawing.ColorTranslator.FromHtml(bldg.features[0].properties.color);
                DA.SetData(7, c);
            }

            DA.SetData(8, bldg.features[0].properties.tags.height);
            DA.SetData(9, bldg.features[0].properties.tags.min_height);

            DA.SetData(10, bldg.features[0].properties.tags.roofheight);
            DA.SetData(11, bldg.features[0].properties.tags.roofshape);
            DA.SetData(12, bldg.features[0].properties.tags.rooforientation);
            DA.SetData(13, bldg.features[0].properties.tags.roofangle);
            DA.SetData(14, bldg.features[0].properties.tags.roofmaterial);
            Color rc= Color.White;
            if (bldg.features[0].properties.tags.roofcolor != null)
            {
                rc = System.Drawing.ColorTranslator.FromHtml(bldg.features[0].properties.tags.roofcolor);
                DA.SetData(15, rc);
            }
            else if (bldg.features[0].properties.roofColor != null)
            {
                rc = System.Drawing.ColorTranslator.FromHtml(bldg.features[0].properties.roofColor);
                DA.SetData(15, rc);
            }

            DA.SetData(16, response2);
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