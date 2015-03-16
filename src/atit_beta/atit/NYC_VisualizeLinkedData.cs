using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel.Data;
using atit.Helpers;
using atit.Properties;


namespace atit
{
    public class NYC_VisualizeLinkedData : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the VisualizeLinkedData class.
        /// </summary>
        public NYC_VisualizeLinkedData()
            : base("VisualizeLinkedData", "VisualizeLinkedData",
                "Visualize spatial and non spatial data",
                "@it", "SHP_NYC")
        {
        }

        public override GH_Exposure Exposure
        {
            //expose the object in the section on the toolbar
            get { return GH_Exposure.tertiary; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Linked Shapes", "Linked_Shapes", "Input Linked Shape Objects", GH_ParamAccess.list);
            pManager.AddTextParameter("FilterByAtt", "[FilterByAtt]", "Filter By Att", GH_ParamAccess.item);
            pManager.AddTextParameter("FilterAttValues", "[FilterValues]", "Filter by Att_Names or Values", GH_ParamAccess.list);
            pManager.AddTextParameter("GetAtt_Value", "GetAtt_Value", "GetAtt_Value", GH_ParamAccess.list);
            pManager.AddBooleanParameter("True or False", "T||F", "Set Boolean True to import shape files", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //pManager.AddCurveParameter("Polyline", "TP", "Polygon geometry from shapefile", GH_ParamAccess.list);
            pManager.Register_CurveParam("TaxProperty", "TP", "Polyline geometry from tax property");
            pManager.Register_CurveParam("Footprints", "FP", "Polyline geometry from footprint");
            //pManager.Register_PathParam("Path 1", "1", "1st path in tree");
            pManager.Register_StringParam("AttValues", "Val", "Tree_AttValues");

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //input variable
            List<ShapeItem> InShapes = new List<ShapeItem>();
            string FilterbyAtt = string.Empty;
            List<string> FilterVal = new List<string>();
            List<string> GetVal = new List<string>();
            bool trigger = false;

            //outputVariables
            //List<Rhino.Geometry.Curve> myoutcurves = new List<Rhino.Geometry.Curve>();
            DataTree<Rhino.Geometry.Curve> myoutcurves = new DataTree<Rhino.Geometry.Curve>();
            DataTree<Rhino.Geometry.Curve> myOutTree = new DataTree<Rhino.Geometry.Curve>();
            DataTree<string> myOutString = new DataTree<string>();

            // pass the input parameters
            if (!DA.GetDataList(0, InShapes)) { return; }
            DA.GetData(1, ref FilterbyAtt);
            DA.GetDataList(2, FilterVal);
            if (!DA.GetDataList(3, GetVal)) { return; }
            if (!DA.GetData(4, ref trigger)) { return; }

            //IList<GH_Path> paths = Params.Input(3).VolatileData.Paths;
            //paths =Params.Input(3).VolatileData.Paths;
            // shape files has curve for tax map,  list of curve for foot prints

            int counter = 0;

            if (trigger == true)
            {
                for (int i = 0; i < InShapes.Count; i++)
                {
                    // filter by att name and values

                    if (FilterbyAtt != string.Empty & FilterVal.Count > 0)
                    {
                        foreach (var val in FilterVal)
                        {
                            foreach (var atval in InShapes[i].BindDict[FilterbyAtt])
                            {
                                if (val == atval)
                                {
                                    //data trees set the path
                                    GH_Path myOutPath = new GH_Path(counter);

                                    // local variables
                                    List<Point3d> mypoints = new List<Point3d>();

                                    foreach (var pt in InShapes[i].TaxCurve.Pts)
                                    {
                                        mypoints.Add(new Point3d(pt.X, pt.Y, 0));
                                    }
                                    // create GH tax Curve
                                    PolylineCurve mycurve = new PolylineCurve(mypoints);
                                    myoutcurves.Add(mycurve, myOutPath);

                                    // set footprint output
                                    foreach (var fp in InShapes[i].Footprints)
                                    {
                                        List<Point3d> myfootprint = new List<Point3d>();
                                        foreach (var pt in fp.Pts)
                                        {
                                            myfootprint.Add(new Point3d(pt.X, pt.Y, 0));
                                        }
                                        // create GH tax Curve
                                        PolylineCurve myfcurve = new PolylineCurve(myfootprint);
                                        myOutTree.Add(myfcurve, myOutPath);
                                    }

                                    // set the output attributes
                                    foreach (var att in GetVal)
                                    {
                                        foreach (var item in InShapes[i].BindDict[att])
                                        {
                                            myOutString.Add(item, myOutPath);
                                        }
                                        //foreach (var item in InShapes[i].AttDict_1)
                                        //{
                                        //    if (item.Key == att)
                                        //    {
                                        //        myOutString.Add(item.Value, myOutPath);
                                        //    }
                                        //}
                                        //myOutString.Add(InShapes[i].AttDict_1[att], myOutPath);
                                    }
                                    counter++;
                                }
                            }

                        }

                    }
                    else  // iterate all shapes
                    {
                        //data trees set the path
                        GH_Path myOutPath = new GH_Path(i);
                        // local variables

                        List<Point3d> mypoints = new List<Point3d>();

                        foreach (var pt in InShapes[i].TaxCurve.Pts)
                        {
                            mypoints.Add(new Point3d(pt.X, pt.Y, 0));
                        }
                        // create GH tax Curve
                        PolylineCurve mycurve = new PolylineCurve(mypoints);
                        myoutcurves.Add(mycurve, myOutPath);

                        // set footprint output
                        foreach (var fp in InShapes[i].Footprints)
                        {
                            List<Point3d> myfootprint = new List<Point3d>();
                            foreach (var pt in fp.Pts)
                            {
                                myfootprint.Add(new Point3d(pt.X, pt.Y, 0));
                            }
                            // create GH foot print Curve
                            PolylineCurve myfcurve = new PolylineCurve(myfootprint);
                            myOutTree.Add(myfcurve, myOutPath);
                        }

                        // set the output attributes
                        foreach (var att in GetVal)
                        {
                            foreach (var item in InShapes[i].BindDict[att])
                            {
                                myOutString.Add(item, myOutPath);
                            }
                            //myOutString.Add(InShapes[i].AttsDict[att], myOutPath);
                        }

                    }
                }
            }

            // set Outputs
            DA.SetDataTree(0, myoutcurves);
            DA.SetDataTree(1, myOutTree);
            DA.SetDataTree(2, myOutString);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                return Resources.VizLinkeddata_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{169bdc2e-7dcf-4b82-a370-be7bd1a55806}"); }
        }
    }
}