using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using atit.Helpers;
using atit.Properties;

namespace atit
{
    public class SHP_ReadAtt : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ReadAtt class.
        /// </summary>
        public SHP_ReadAtt()
            : base("feat@it", "feat@it",
                "Read features of specified Shape (.shp) file",
                "@it", "SHAPE")
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
            pManager.AddTextParameter("Path", "P", "Specify path of the file", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Geometry Type", "T", "Geometric Data Type of shape (.shp) file", GH_ParamAccess.item);
            pManager.AddTextParameter("Projection Type", "P", "Projection Type of shape (.shp) file", GH_ParamAccess.item);
            pManager.AddTextParameter("Features", "F", " Features of specified shape (.shp) file ", GH_ParamAccess.list);
           
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //input variable
            string filepath = string.Empty;

            //outputVariables
            List<string> myattnames = new List<string>();

            //pass incoming data to local variable
            if (!DA.GetData(0, ref filepath)) { return; }

            Helpers.GIS gisfile = new Helpers.GIS();

            //populate attnames list
            myattnames = gisfile.GetAtt(filepath);

            // set Outputs
            DA.SetData(0, gisfile.GeoType);
            DA.SetData(1, gisfile.ProjType);
            DA.SetDataList(2, myattnames);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                return Resources.feat_it;
                //return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            //get { return new Guid("{84886147-41e0-4db0-a054-69672b72649b}"); }
            get { return new Guid("{14758C80-5E51-4E8E-A851-C1107C8982E2}"); }
        }
    }
}