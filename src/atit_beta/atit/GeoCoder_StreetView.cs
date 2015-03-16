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
using Rhino.Geometry;

namespace atit
{
    public class GeoCoder_StreetView : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GeoCoder_StreetView class.
        /// </summary>
        public GeoCoder_StreetView()
            : base("Street View", "Street View",
                "Uses Google API to get street view",
                "@it", "GeoCoder")
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
            pManager.AddTextParameter("Location", "L", "{latitude,longitude}", GH_ParamAccess.item);
            pManager.AddNumberParameter("width", "w", "Width", GH_ParamAccess.item, 600);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("height", "h", "height", GH_ParamAccess.item, 400);
            pManager[2].Optional = true;
            pManager.AddNumberParameter("compass", "c", "indicates the compass heading of camera", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddNumberParameter("field of view", "fov", "field of view: max alloved value 120", GH_ParamAccess.item, 90);
            pManager[4].Optional = true;
            pManager.AddNumberParameter("picth", "p", "specifies the up & down angle of camera: set values -90 to 90 ", GH_ParamAccess.item, 0);
            pManager[5].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGeometryParameter("Image", "I", "Mesh presentation of Image", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string in_loc = null;
            double width = 600;
            double height = 400;
            double compass = 0;
            double fov = 90;
            double picth = 0;

            if (!DA.GetData(0, ref in_loc)) { return; }
            DA.GetData(1, ref width);
            DA.GetData(2, ref height);
            DA.GetData(3, ref compass);
            DA.GetData(4, ref fov);
            DA.GetData(5, ref picth);


            // Get Street View
            //string url = "http://maps.googleapis.com/maps/api/streetview?size=400x400&location=" + in_loc + "&fov=90&heading=235&pitch=10&sensor=false";
            //string url = "http://maps.googleapis.com/maps/api/streetview?size=600x400&location=" + in_loc;

            string url = String.Format("http://maps.googleapis.com/maps/api/streetview?size={0}x{1}&location={2}", (int)width, (int)height, in_loc);

            if (this.Params.Input[3].SourceCount > 0 ) // compass
            {
                if (compass >= 0 && compass <= 360)
                {
                    url = String.Format(url + "&heading={0}", (int)compass);
                }
                else 
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Out of range! Accepted values are from 0 to 360 (both values indicating North), with 90 indicating East, and 180 South");
                }
                
            }
            if( this.Params.Input[4].SourceCount > 0 ) // fov
            {
                if (fov  >= 0 && fov <= 120)
                {
                    url = String.Format(url + "&fov={0}", (int)fov);
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Out of range! Maximum allowed value is 120");
                }
                
            }
            if (this.Params.Input[5].SourceCount > 0) // pitch
	        {
                if (fov >= -90  && fov <= 90 )
                {
                    url = String.Format(url + "&pitch={0}", (int) picth);
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Out of range! Accepted values are from -90 to 90. 90 degree indicating straight up");
                }
                
	        }

            

            System.Net.WebRequest request = System.Net.WebRequest.Create(url);
            System.Net.WebResponse myresponse = request.GetResponse();
            // Open data stream:
            System.IO.Stream _WebStream = myresponse.GetResponseStream();
            //string path = @"C:\Temp\streetview";
            var result = new System.Net.WebClient().DownloadData(url);

            Image image = Image.FromStream(_WebStream, true);
            Bitmap image1 = (Bitmap)image;


            int h = image.Size.Height;
            int w = image.Size.Width;
            

            Rectangle3d rec = new Rectangle3d(Plane.WorldXY, new Point3d(0, 0, 0), new Point3d(w, h, 0));
            Rhino.Geometry.Mesh rm = Mesh.CreateFromPlane(Plane.WorldXY, rec.X, rec.Y, w - 1, h - 1);

            int test = rm.Vertices.Count();

            List<Point3d> Pts = new List<Point3d>();
            List<Color> colors = new List<Color>();

            int[] facesint = new int[4];


            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    Color c = image1.GetPixel(j, h-i-1);
                    colors.Add(c);
                }
            }

            rm.VertexColors.CreateMonotoneMesh(Color.White);
            rm.VertexColors.SetColors(colors.ToArray());

            DA.SetData(0, rm);
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