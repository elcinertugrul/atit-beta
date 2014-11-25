using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Grasshopper.Kernel;
using GeoClient;


namespace atit
{
    public class nyc_geoclient_address: GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GeoClient_Address class.
        /// </summary>
        public nyc_geoclient_address()
            : base("nyc_geoclient_address", "nyc_geoclient_address",
                "NYC Geoclient request with BIN 'Building Identificatin Number'",
                "@it", "NYC")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("House#", "House#", "House Number", GH_ParamAccess.item);
            pManager.AddTextParameter("Street", "Street", "Street", GH_ParamAccess.item);
            pManager.AddTextParameter("Borough", "Borough", "Borough", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("BBL", "BBL", "Borough Block Lot", GH_ParamAccess.item);
            pManager.AddTextParameter("BoroughCode", "B", "Borough Code", GH_ParamAccess.item);
            pManager.AddTextParameter("Block", "B", "Tax Block", GH_ParamAccess.item);
            pManager.AddTextParameter("Lot", "L", "Tax Lot", GH_ParamAccess.item);
            pManager.AddTextParameter("BIN", "BIN", "Building Identification Number", GH_ParamAccess.item);
            pManager.AddTextParameter("Neighborhood", "N", "Neighborhood", GH_ParamAccess.item);
            pManager.AddTextParameter("BuildingClass", "Class", "Building Class", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string _housenumber = string.Empty;
            string _street = string.Empty;
            string _borough = string.Empty;

            if (!DA.GetData(0, ref _housenumber)) { return; }
            if (!DA.GetData(1, ref _street)) { return; }
            if (!DA.GetData(2, ref _borough)) { return; }

            Geoclient gc = new Geoclient();

            Address_Result response = gc.Address_WebRequest(_housenumber, _street, _borough);

            //DA.SetData(0, response.Address.assemblyDistrict);
            DA.SetData(0, response.Address.bbl);
            DA.SetData(1, response.Address.bblBoroughCode);
            DA.SetData(2, response.Address.bblTaxBlock);
            DA.SetData(3, response.Address.bblTaxLot);
            DA.SetData(4, response.Address.buildingIdentificationNumber);
            DA.SetData(5, response.Address.ntaName);
            DA.SetData(6, response.Address.rpadBuildingClassificationCode);


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
            get { return new Guid("{cade2fc4-e6d4-49f6-a528-b48163e0e77c}"); }
        }

    }
}
