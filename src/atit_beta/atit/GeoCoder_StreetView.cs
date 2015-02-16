// by Elcin Ertugrul 2015/02/15
// Google API Street View https://developers.google.com/maps/documentation/streetview/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using Rhino;

using atit.Properties;
using Rhino.Display;

namespace atit
{
    public class GeoCoder_StreetView : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GeoCoder_StreetView class.
        /// </summary>
        public GeoCoder_StreetView()
            : base("GeoCoder_StreetView", "StreetView",
                "Uses Google API to get street view",
                "@it", "GeoCoder")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Location", "L", "{latitude,longitude}", GH_ParamAccess.item);
            pManager.AddTextParameter("File Path", "FP", "Specify the file path such as C:\\Temp\\StreetView.jpg ", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Image", "I", "Mesh presentation of Image", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string in_loc = null;
            string inPath = null;

            if (!DA.GetData(0, ref in_loc)) { return; }
            if (!DA.GetData<string>(1, ref inPath)) { return; }

            //File Directory Exists
            try
            {
                string[] colonFrags = inPath.Split(':');
                if (colonFrags.Length > 2 || inPath.Contains(";"))
                {
                    throw new Exception();
                }

                string inputFileName = Path.GetFileName(inPath);
                char[] inValidChars = Path.GetInvalidFileNameChars();
                string inValidString = Regex.Escape(new string(inValidChars));
                string myNewValidFileName = Regex.Replace(inputFileName, "[" + inValidString + "]", "");

                //if the replace worked, throw an error at the user.
                if (inputFileName != myNewValidFileName)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                //warn the user
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                    "Your file name is invalid - check your input and try again. No file path will be set.");
            }

            if (!Directory.Exists(Path.GetDirectoryName(inPath)))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                     "The directory you specified does not exist. Please double check your input. No file path will be set.");
            }

            if (!isJpegFile(Path.GetExtension(inPath)))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                    "Please provide a file of type .jpeg  Something like: 'C:\\Temp\\StreetView.jpg'. No file path will be set.");
            }

            // Get Street View
            //string url = "http://maps.googleapis.com/maps/api/streetview?size=400x400&location=" + in_loc + "&fov=90&heading=235&pitch=10&sensor=false";
            string url = "http://maps.googleapis.com/maps/api/streetview?size=600x400&location=" + in_loc;
            System.Net.WebRequest request = System.Net.WebRequest.Create(url);
            System.Net.WebResponse myresponse = request.GetResponse();
            // Open data stream:
            System.IO.Stream _WebStream = myresponse.GetResponseStream();
            //string path = @"C:\Temp\streetview";
            var result = new System.Net.WebClient().DownloadData(url);

            Image image = Image.FromStream(_WebStream);
            image.Save(inPath, ImageFormat.Jpeg);


            GH_Mesh m = new GH_Mesh();

            
            
            RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.SetWallpaper(inPath, true, true);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Resources.StreetView;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{09a042d4-a43c-4965-990b-2b48d028dac4}"); }
        }

        private bool isJpegFile(string fileExtension)
        {
            if (fileExtension.ToLower() == ".jpg")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}