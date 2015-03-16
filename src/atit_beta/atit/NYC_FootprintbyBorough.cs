using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using atit.Helpers;
using atit.Properties;

namespace atit
{
    public class NYC_FootprintbyBorough : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the FootprintbyBorough class.
        /// </summary>
        public NYC_FootprintbyBorough()
            : base("FootprintbyBorough", "Footprint data set",
                "Sort by Borough. Use shapefile of footprint outlines of buildings in NYC.\n\n" + "https://data.cityofnewyork.us/Housing-Development/Building-Footprints/tb92-6tj8"+
            "\n\n You could also use any shape file has feature 'BBL' to sort by borough",
                "@it", "SHAPE_NYC")
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
            pManager.AddTextParameter("File_Path", "P", "File_Path", GH_ParamAccess.item);
            pManager.AddTextParameter("Borough Code", "B", "Set value from 1 to 5. Borough Codes: 1=Manhattan 2=Bronx 3=Brooklyn 4=Queens 5=Staten Islad ", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Boolean", "T/F", "Set boolean True to import .shp file", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Shapes", "S", "Shapes", GH_ParamAccess.list);
            pManager.AddTextParameter("Features", "F", " Features of the Shapefile", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //input variable
            string filepath = string.Empty;
            string b = string.Empty;
            bool trigger = false;

            //outputVariables
            List<ShapeItem> OutShapes = new List<ShapeItem>();
            List<string> myattnames = new List<string>();

            if (!DA.GetData(0, ref filepath)) { return; }
            if (!DA.GetData(1, ref b)) { return; }
            if (!DA.GetData(2, ref trigger)) { return; }

            if (trigger == true)
            {
                // check first borough code is given not out of index
                
                if (b == "1" || b == "2" || b== "3" || b=="4" || b=="5" )
                {
                    // instantiate
                    Helpers.GIS gisfile = new Helpers.GIS(filepath, b);

                    OutShapes = gisfile.Shapes;
                    //populate attnames list
                    //myattnames = gisfile.GetAtt(filepath); //IT AGAIN READ ALL DATA ??? 
                    foreach (var att in gisfile.Shapes[1].AttsDict)
                    {
                        myattnames.Add(att.Key);
                    }   
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Borough code value is out of Index! Please input '1' for Manhattan; '2' for Bronx; '3' for Brooklyn; '4' for Queens; '5' for Staten Islad ");
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
                return Resources._5_Boroughs;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{d1325b6b-0aeb-4afb-bced-9705c8cba39d}"); }
        }
    }
}