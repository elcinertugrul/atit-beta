using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Grasshopper.Kernel;
using Rhino.Geometry;

using Grasshopper;
using atit.Helpers;
using atit.Properties;

namespace atit
{
    public class NYC_BindShps : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the BindShps class.
        /// </summary>
        public NYC_BindShps()
            : base("Bind two Shapefiles (datasets)", "Data Bind",
                "Bind data sets per common attribute BIN = Building Identification Number",
                "@it", "SHP_NYC")
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
            pManager.AddGenericParameter("S_NYC_Taxmap", "S_NYC_TaxMap", "Input Shapes imported from NYC Tax Map", GH_ParamAccess.list);
            pManager.AddGenericParameter("S_NYC_BldgFootprints", "S_NYC_BldgFootPrints", "Input Shapes imported from NYC Buildings Footprint", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Linked Shapes", "Linked_Shapes", "Linked Shapes", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //input variable
            List<ShapeItem> InShapes_Tax = new List<ShapeItem>();
            List<ShapeItem> InShapes_Foot = new List<ShapeItem>();

            //output vaiable
            List<ShapeItem> OutShapes = new List<ShapeItem>();

            // pass the input parameters
            if (!DA.GetDataList(0, InShapes_Tax)) { return; }
            if (!DA.GetDataList(1, InShapes_Foot)) { return; }

            Helpers.GIS gis = new Helpers.GIS();
            // create new list of shape 
            // iterate the tax map 
            for (int i = 0; i < InShapes_Tax.Count; i++)
            {
                // local variable

                Helpers.Curve myTaxCurve = new Helpers.Curve(InShapes_Tax[i].PtsLists[0]);
                // populate dict
                Dictionary<string, List<string>> mydict = new Dictionary<string, List<string>>();

                List<Helpers.Curve> myfootprintcurves = new List<Helpers.Curve>();

                List<string[]> FPAtt = new List<string[]>();

                // parse BBL of tax map here
                //string taxbbl = gis.Parse(InShapes_Tax[i].AttsDict["BBL"]);
                string taxbbl = gis.combineBBL(InShapes_Tax[i].AttsDict["Borough"], InShapes_Tax[i].AttsDict["Block"], InShapes_Tax[i].AttsDict["Lot"]);
               
                // add dictionary
                foreach (var item in InShapes_Tax[i].AttsDict)
                {
                    if (item.Key == "BBL")
                    {
                        List<string> mylist = new List<string>();
                        mylist.Add(taxbbl);
                        mydict.Add(item.Key, mylist);
                    }
                    else
                    {
                        List<string> mylist = new List<string>();
                        mylist.Add(item.Value);
                        mydict.Add(item.Key, mylist);
                    }
                }

                // us linq inquiry 
                var footp = (from ft in InShapes_Foot
                             where ft.AttsDict["BBL"] == taxbbl
                             select ft).ToList(); // may be ,pre then one element

                foreach (var fp in footp)
                {
                        // this add to footprints
                foreach (var points in fp.PtsLists)
                    {
                        myfootprintcurves.Add(new Helpers.Curve(points));
                    }

                        string[] Att = new string[fp.AttsDict.Count];

                        int index = 0;
                        foreach (var item in fp.AttsDict)
                        {
                            Att[index] = item.Value;
                            index++;
                        }
                        FPAtt.Add(Att);
                }


                //foreach (var FP in InShapes_Foot)
                //{
                //    if (FP.AttsDict["BBL"] == taxbbl)
                //    {
                //        // this add to footprints
                //        myfootprintcurves.Add(new MappingGIS.Curve(FP.Pts));
                //        //outshape.Footprints.Add(new MappingGIS.Curve(FP.Pts));

                //        string[] Att = new string[FP.AttsDict.Count];

                //        // add the dictionary check firs the key is exist
                //        int index = 0;
                //        foreach (var item in FP.AttsDict)
                //        {
                //            //if (!mydict.ContainsKey(item.Key))
                //            //{
                //            //    List<string> mylist = new List<string>();
                //            //    mylist.Add(item.Value);
                //            //    mydict.Add(item.Key, mylist);
                //            //}
                //            Att[index] = item.Value;

                //            index++;
                //        }

                //        FPAtt.Add(Att);
                //    }
                //}


                // iterate all list of array and put this 
                int e = 0;
                foreach (var item in InShapes_Foot[0].AttsDict)
                {
                    if (!mydict.ContainsKey(item.Key))
                    {
                        List<string> dictval = new List<string>();
                        foreach (var a in FPAtt)
                        {
                            dictval.Add(a[e]);
                        }

                        mydict.Add(item.Key, dictval);
                        e++;
                    }
                    else 
                    {
                        e++;
                    }
                }

                ShapeItem outshape = new ShapeItem(myTaxCurve, myfootprintcurves, mydict);

                // populate the output
                OutShapes.Add(outshape);
            }


            // set Outputs
            DA.SetDataList(0, OutShapes);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                return Resources.dbind;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{f0847cef-75af-4aca-8096-46ae4616b341}"); }
        }
    }
}