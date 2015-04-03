using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
using Grasshopper;
using atit.Helpers;
using atit.Properties;

namespace atit
{
    public class SHP_VisualizeSHP : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the VisualizeSHP class.
        /// </summary>
        public SHP_VisualizeSHP()
            : base("DataVis@it", "DataVis@it",
                "Visualize spatial and nonspatial data of speficied shape (.shp) file",
                "@it", "SHP")
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
            pManager.AddGenericParameter("Shapes", "S", "Input Shape Objects", GH_ParamAccess.list);
            pManager.AddTextParameter("FilterByFeat", "[FltrByFeat]", "Filter speficic feature [optional]", GH_ParamAccess.item);
            pManager.AddTextParameter("FilterByFeatureValue", "[FltrByVal]", "Filter by value of the Feature [optional]", GH_ParamAccess.list);
            pManager.AddTextParameter("GetValueofFeature", "GetValofFeat", "Get value of specified features only", GH_ParamAccess.list);
            pManager.AddBooleanParameter("True or False", "T||F", "Set Boolean True to import shape files", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_CurveParam("Polygon", "P", "Polygon geometry of shapefile");
            pManager.Register_CurveParam("Polyline", "PLine", "Polyline geometry of shapefile");
            pManager.Register_PointParam("Point", "Pt", "Point geometry of shapefile");
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
            //List<string> FilterbyAtt = new List<string>();
            string FilterbyAtt = string.Empty;
            List<string> FilterVal = new List<string>();
            List<string> GetVal = new List<string>();
            bool trigger = false;

            //outputVariables
            //List<Rhino.Geometry.Curve> myoutcurves = new List<Rhino.Geometry.Curve>();
            DataTree<Rhino.Geometry.Curve> myoutcurves = new DataTree<Rhino.Geometry.Curve>();
            DataTree<Rhino.Geometry.Curve> myoutPolys = new DataTree<Rhino.Geometry.Curve>();
            DataTree<Rhino.Geometry.Point3d> myoutPts = new DataTree<Rhino.Geometry.Point3d>();
            DataTree<string> myOutString = new DataTree<string>();


            // pass the input parameters
            if (!DA.GetDataList(0, InShapes)) { return; }
            DA.GetData(1, ref FilterbyAtt);
            DA.GetDataList(2, FilterVal);
            if (!DA.GetDataList(3, GetVal)) { return; }
            if (!DA.GetData(4, ref trigger)) { return; }

            int counter = 0;

            if (trigger == true)
            {
                // iterate at shape items
                for (int i = 0; i < InShapes.Count; i++)
                {
                    if (FilterbyAtt != string.Empty & FilterVal.Count == 0)
                    {
                        //input string incorrect
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Value input should be at least one item. You can give multiple values to filter but we highly recommend tp input one feature at a time");
                        return;
                    }
                    // filter by attname & Values
                    //if (FilterVal.Count > 0 & FilterbyAtt.Count > 0)
                    if (FilterVal.Count > 0 & FilterbyAtt != string.Empty)
                    {
                        //// check here the inputs are the same count
                        //if (FilterbyAtt.Count != FilterVal.Count)
                        //{
                        //    //input string incorrect
                        //    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The number of features has to match number of items of values to be filtered. Make sure the information to be match");
                        //    return;
                        //}
                        //else
                        //{

                            for (int j = 0; j < FilterVal.Count; j++)
                            {
                                // check the keys of dictionary if 
                                if (FilterVal[j] == InShapes[i].AttsDict[FilterbyAtt])
                                //if (FilterVal[j] == InShapes[i].AttsDict[FilterbyAtt[j]])
                                {
                                    GH_Path myOutPath = new GH_Path(counter);

                                    if (InShapes[i].PtsLists.Count > 0)
                                    {
                                        foreach (var list in InShapes[i].PtsLists)
                                        {
                                            //local variables
                                            List<Point3d> mypoints = new List<Point3d>();
                                            foreach (var pt in list)
                                            {
                                                mypoints.Add(new Point3d(pt.X, pt.Y, pt.Z));
                                            }

                                            if (InShapes[i].Type == "Polygon" || InShapes[i].Type.ToLower().Contains("polygon"))
                                            {

                                                // Create GH-Curves
                                                //PolylineCurve mycurve = new PolylineCurve(mypoints);
                                                Rhino.Geometry.Curve mycurve = new PolylineCurve(mypoints).ToNurbsCurve();
                                                myoutcurves.Add(mycurve, myOutPath);
                                            }

                                            if (InShapes[i].Type == "Point" || InShapes[i].Type == "MultiPoint" || InShapes[i].Type.ToLower().Contains("point"))
                                            {
                                                foreach (var point in mypoints)
                                                {
                                                    myoutPts.Add(point, myOutPath);
                                                }
                                            }
                                            if (InShapes[i].Type == "Polyline" || InShapes[i].Type == "LineString" || InShapes[i].Type.ToLower().Contains("line"))
                                            {
                                                // Create GH-Curves
                                                PolylineCurve mycurve = new PolylineCurve(mypoints);
                                                myoutPolys.Add(mycurve, myOutPath);
                                            }
                                        }
                                    }
                                    else 
                                    {
                                        foreach (var pt in InShapes[i].Pts)
                                        {
                                            //local variables
                                            List<Point3d> mypoints = new List<Point3d>();
                                            mypoints.Add(new Point3d(pt.X, pt.Y, pt.Z));

                                            if (InShapes[i].Type == "Polygon")
                                            {
                                                // Create GH-Curves
                                                PolylineCurve mycurve = new PolylineCurve(mypoints);
                                                myoutcurves.Add(mycurve, myOutPath);
                                            }

                                            if (InShapes[i].Type == "Point")
                                            {
                                                foreach (var point in mypoints)
                                                {
                                                    myoutPts.Add(point, myOutPath);
                                                }
                                            }
                                            if (InShapes[i].Type == "Polyline" || InShapes[i].Type == "LineString")
                                            {
                                                // Create GH-Curves
                                                PolylineCurve mycurve = new PolylineCurve(mypoints);
                                                myoutPolys.Add(mycurve, myOutPath);
                                            }
                                        }
                                    }

                                    ////local variables
                                    //List<Point3d> mypoints = new List<Point3d>();

                                    //foreach (var pt in InShapes[i].Pts)
                                    //{
                                    //    mypoints.Add(new Point3d(pt.X, pt.Y, 0));
                                    //}
                                    //// Create GH-Curves
                                    //PolylineCurve mycurve = new PolylineCurve(mypoints);
                                    //myoutcurves.Add(mycurve);

                                    // check if any attributes to filter 
                                    // populate output populate values // for all attributes
                                    
                                    //List<string> treebranch = new List<string>();
                                    foreach (var att in GetVal)
                                    {
                                        myOutString.Add(InShapes[i].AttsDict[att], myOutPath);
                                    }
                                    counter++;
                                }
                            }
                        //}
                        
                    }
                    else
                    {
                        GH_Path myOutPath = new GH_Path(i);

                        ////local variables
                        //List<Point3d> mypoints = new List<Point3d>();

                        //foreach (var pt in InShapes[i].Pts)
                        //{
                        //    mypoints.Add(new Point3d(pt.X, pt.Y, 0));
                        //}
                        //// Create GH-Curves
                        //PolylineCurve mycurve = new PolylineCurve(mypoints);
                        //myoutcurves.Add(mycurve);

                        foreach (var list in InShapes[i].PtsLists)
                        {
                            //local variables
                            List<Point3d> mypoints = new List<Point3d>();
                            foreach (var pt in list)
                            {
                                mypoints.Add(new Point3d(pt.X, pt.Y, pt.Z));
                            }

                            if (InShapes[i].Type == "Polygon")
                            {
                                // Create GH-Curves
                                PolylineCurve mycurve = new PolylineCurve(mypoints);
                                myoutcurves.Add(mycurve, myOutPath);
                            }

                            if (InShapes[i].Type == "Point")
                            {
                                foreach (var point in mypoints)
                                {
                                    myoutPts.Add(point, myOutPath);
                                }
                            }
                            if (InShapes[i].Type == "Polyline" || InShapes[i].Type == "LineString")
                            {
                                // Create GH-Curves
                                PolylineCurve mycurve = new PolylineCurve(mypoints);
                                myoutPolys.Add(mycurve, myOutPath);
                            }
                        }

                        // check if any attributes to filter
                        // populate output populate values // for all attributes
                        
                        //foreach (var item in InShapes[i].AttsDict)
                        //{
                        //    myOutString.Add(item.Value,myOutPath);
                        //}
                        foreach (var att in GetVal)
                        {
                            myOutString.Add(InShapes[i].AttsDict[att], myOutPath);
                        }
                    }
                }
            }

            // set Outputs
            //DA.SetDataList(0, myoutcurves);
            DA.SetDataTree(0, myoutcurves);
            DA.SetDataTree(1, myoutPolys);
            DA.SetDataTree(2, myoutPts);
            DA.SetDataTree(3, myOutString);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                return Resources.datavis_it;
                //return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            //get { return new Guid("{182636a6-ee2c-4f01-b583-218639a0f83f}"); }
            get { return new Guid("{7F7EA921-A3CE-4E1A-BFA4-6647AAF1360E}"); }
        }
    }
}