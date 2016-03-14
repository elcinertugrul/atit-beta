using System;
using System.Collections.Generic;
using System.IO;

using Grasshopper.Kernel;
using Rhino.Geometry;

using atit.Helpers;
using atit.Properties;
using System.Net;

namespace atit
{
    public class SHP_ImportSHP : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ImportSHP class.
        /// </summary>
        public SHP_ImportSHP()
            : base("Imp@It", "Imp@It",
                "Import and store shapes",
                "@it", "SHP")
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
            pManager.AddTextParameter("Shapefile (.shp) Path", "C:/", "Specify path of the shapefile", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Project 2 WGS84", "-->WGS84", "Set True if change the map projection to WGS84 datum, default is false", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("True or False", "T||F", "Set Boolean True to import shape files", GH_ParamAccess.item);
            pManager[2].Optional = false;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Shapes", "S", "Shapes", GH_ParamAccess.list);
            pManager.AddTextParameter("Features", "F", " Features of given shape (.shp) file ", GH_ParamAccess.list);
            //pManager.RegisterParam(IGH_Param Param, "Objects", "out","Shapeobjets","",GH_ParamAccess.list);
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
            bool reproject = false;

            //outputVariables
            List<ShapeItem> OutShapes = new List<ShapeItem>();           
            List<string> myattnames = new List<string>();

            if (!DA.GetData(0, ref filepath)) { return; }
            if (!DA.GetData(1, ref reproject)) { return; }
            if (!DA.GetData(2, ref trigger)) { return; }

            //
            string extention = Path.GetExtension(filepath);
            if (extention != ".shp")
            {
               //input string incorrect
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Not valid file type! Specify valid shape (.shp) file.");
                return; 
            }

            if (trigger == true)
            {
                Helpers.GIS gisfile = new Helpers.GIS(filepath, reproject);
                OutShapes = gisfile.Shapes;

                //populate attnames list
                //myattnames = gisfile.GetAtt(filepath); // IT AGAIN READ ALL DATA ??? FIND BETTER WAY
                foreach (var att in gisfile.Shapes[1].AttsDict)
                {
                    myattnames.Add(att.Key);
                }
            }

            // set Outputs
            DA.SetDataList(0, OutShapes);
            DA.SetDataList(1, myattnames);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                return Resources.Imp_it;
                //return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            //get { return new Guid("{d08b9d8d-9b39-4154-9683-af7746581214}"); }
            get { return new Guid("{643697DC-0CD4-4FDF-890C-D02E166A22AC}"); }
        }
    }
}