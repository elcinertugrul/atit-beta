//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Grasshopper.Kernel;
//using Rhino.Geometry;
//using GeoClient;


//namespace atit
//{
//    public class nyc_geoclient_bin: GH_Component
//    {
//         /// <summary>
//        /// Initializes a new instance of the GeoClient_BIN class.
//        /// </summary>
//        public nyc_geoclient_bin()
//            : base("nyc_geoclient_bin", "nyc_geoclient_bin",
//                "NYC Geoclient request with BIN 'Building Identificatin Number'",
//                "@it", "NYC")
//        {
//        }

//        /// <summary>
//        /// Registers all the input parameters for this component.
//        /// </summary>
//        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
//        {
//            pManager.AddTextParameter("BIN", "BIN", "Building Identification Number", GH_ParamAccess.item);
//        }

//        /// <summary>
//        /// Registers all the output parameters for this component.
//        /// </summary>
//        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
//        {
//            pManager.AddTextParameter("BBL", "BBL", "Borough Block Lot", GH_ParamAccess.item);
//            pManager.AddTextParameter("BoroughCode", "B", "Borough Code", GH_ParamAccess.item);
//            pManager.AddTextParameter("Block", "B", "Tax Block", GH_ParamAccess.item);
//            pManager.AddTextParameter("Lot", "L", "Tax Lot", GH_ParamAccess.item);
//            pManager.AddTextParameter("Borough", "Bor", "Name of Borough", GH_ParamAccess.item);
//        }

//        /// <summary>
//        /// This is the method that actually does the work.
//        /// </summary>
//        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
//        protected override void SolveInstance(IGH_DataAccess DA)
//        {
//            string _bin = string.Empty;
//            if (!DA.GetData(0, ref _bin)) { return; }

//            Geoclient gc = new Geoclient();

//            BIN_Result response = gc.BIN_WEbRequest(_bin);
//            DA.SetData(0, response.BIN.bbl);
//            DA.SetData(1, response.BIN.bblBoroughCode);
//            DA.SetData(2, response.BIN.bblTaxBlock);
//            DA.SetData(3, response.BIN.bblTaxLot);
//            DA.SetData(4, response.BIN.firstBoroughName);
//        }

//        /// <summary>
//        /// Provides an Icon for the component.
//        /// </summary>
//        protected override System.Drawing.Bitmap Icon
//        {
//            get
//            {
//                //You can add image files to your project resources and access them like this:
//                // return Resources.IconForThisComponent;
//                return null;
//            }
//        }

//        /// <summary>
//        /// Gets the unique ID for this component. Do not change this ID after release.
//        /// </summary>
//        public override Guid ComponentGuid
//        {
//            get { return new Guid("{4529fbb0-9082-40ec-8c2c-257f59017d7c}"); }
//        }
//    }
//}
