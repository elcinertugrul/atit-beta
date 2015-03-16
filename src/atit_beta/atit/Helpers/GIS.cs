using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//dotSpatial dlls
using DotSpatial;
using DotSpatial.Data;
using DotSpatial.Topology;
using DotSpatial.Controls;
using DotSpatial.Modeling.Forms;
using System.Data;
using DotSpatial.Projections;


namespace atit.Helpers
{
    public class GIS
    {
        //Properties
        public string FilePath;

        public List<ShapeItem> Shapes = new List<ShapeItem>(); // to hold the all shapeitems and attributes to populate for GH output :)

        public Dictionary<string, List<ShapeItem>> ShapesTwo = new Dictionary<string, List<ShapeItem>>();

        public List<string> AttNames = new List<string>();

        public string GeoType = string.Empty;
        public string ProjType = string.Empty;

        //METHODS

        /// <summary>
        ///  Gets only Att Names
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public List<string> GetAtt(String Path)
        {
            List<string>attnames = new List<string>();
            DataTable dt = null;
            IFeatureSet FS = FeatureSet.Open(Path);
            FS.FillAttributes();
            dt = FS.DataTable;
            ProjType = FS.Projection.ToString();

            // get the column Name 
            foreach (DataColumn column in dt.Columns)
            {
                attnames.Add(column.ColumnName);
            }
            try
            {
                IFeature feature = FS.GetFeature(1);
                GeoType = feature.BasicGeometry.GeometryType;
            }
            catch (Exception)
            {   
                //throw;
            }
            return attnames;
        }
        /// <summary>
        /// Populate all shapeitems from shp file
        /// </summary>
        public void GetShapes()
        {
            //DotSpatial.Data.ShapefileReader reader = new DotSpatial.Data.ShapefileReader(FilePath);
            DataTable dt = null;
            IFeatureSet FS = FeatureSet.Open(FilePath);
            FS.FillAttributes();
            dt = FS.DataTable;

            // get the column Name 
                foreach ( DataColumn column in dt.Columns)
                {
                    AttNames.Add(column.ColumnName);
                }

            // this able to get spricif 
            //if (dt.Rows.Count > 0)
            //{
            //    string val = dt.Rows[0]["BBL"].ToString();
            //}
            
            for (int i = 0; i < FS.Features.Count; i++)
            {
                //local variables
                //List<Point> myPoints = new List<Point>();
                List<List<Point>> PointsList = new List<List<Point>>();
                Dictionary<string, string> myAttsDict = new Dictionary<string, string>();

                
                // populate vertices
                IFeature feature = FS.GetFeature(i);
                string type = feature.BasicGeometry.GeometryType;
                int Numofgeo = feature.NumGeometries;

                for (int n = 0; n < Numofgeo; n++)
                {
                    List<Point> myPoints = new List<Point>();
                    IList<Coordinate> coords = feature.BasicGeometry.GetBasicGeometryN(n).Coordinates;
                    //IList<Coordinate> coords = feature.BasicGeometry.Coordinates;
                    //FeatureSet fs = new FeatureSet(FeatureType.Polygon);
                    Shape s = FS.GetShape(i, true);

                    //populate myPoints
                    foreach (var c in coords)
                    {
                        if (c.Z.ToString() != "NaN")
                        {
                            myPoints.Add(new Point(c.X, c.Y, c.Z));
                        }
                        else
                        {
                            //myPoints.Add(new Point(c.X, c.Y));
                            myPoints.Add(new Point(c.X, c.Y, 0 ));
                        }
                    }

                    PointsList.Add(myPoints);
                }

                //populate attributes
                for (int j = 0; j < AttNames.Count; j++)
                {
                    myAttsDict.Add(AttNames[j],dt.Rows[i][AttNames[j]].ToString());
                }

                // populate shapeitems
                //Shapes.Add(new ShapeItem(myPoints,myAttsDict));
                Shapes.Add(new ShapeItem(type,PointsList,myAttsDict));
     
            }
		        
	        
            
            
            //DotSpatial.Controls.MapPolygonLayer
            //System.Data.DataTable  dt = new System.Data.DataTable();
            //dt = FS.GetAttributes(1, 3);
            //List<IFeature>  features = new List <IFeature>();
            //features = FS.Features.ToList();
            //Polygon pl = new Polygon(


        }

        public void GetShapes(bool IsReProject)
        {
            //DotSpatial.Data.ShapefileReader reader = new DotSpatial.Data.ShapefileReader(FilePath);
            DataTable dt = null;
            IFeatureSet FS = FeatureSet.Open(FilePath);
            FS.FillAttributes();
            dt = FS.DataTable;

            // Projections
            ProjectionInfo source = FS.Projection;
            ProjectionInfo dest = ProjectionInfo.FromProj4String("+proj=longlat +ellps=WGS84 +datum=WGS84 +no_defs"); //WGS84

            // get the column Name 
            foreach (DataColumn column in dt.Columns)
            {
                AttNames.Add(column.ColumnName);
            }

            for (int i = 0; i < FS.Features.Count; i++)
            {
                //local variables
                //List<Point> myPoints = new List<Point>();
                List<List<Point>> PointsList = new List<List<Point>>();
                Dictionary<string, string> myAttsDict = new Dictionary<string, string>();


                // populate vertices
                IFeature feature = FS.GetFeature(i);
                string type = feature.BasicGeometry.GeometryType;
                int Numofgeo = feature.NumGeometries;

                for (int n = 0; n < Numofgeo; n++)
                {
                    List<Point> myPoints = new List<Point>();
                    IList<Coordinate> coords = feature.BasicGeometry.GetBasicGeometryN(n).Coordinates;
                    Shape s = FS.GetShape(i, true);

                    //populate myPoints
                    foreach (var c in coords)
                    {
                        if (IsReProject)
                        {
                            double[] xy = new double[2];
                            double[] z = new double[1];

                            xy[0] = c.X; // long is X
                            xy[1] = c.Y; // lat is Y  

                            if (c.Z.ToString() != "NaN")
                            {
                                z[0] = c.Z;
                                Reproject.ReprojectPoints(xy, z, source, dest, 0, 1);
                                myPoints.Add(new Point(xy[0], xy[1], z[0]));
                            }
                            else
                            {
                                Reproject.ReprojectPoints(xy, null, source, dest, 0, 1);
                                myPoints.Add(new Point(xy[0], xy[1], 0));
                            }
                        }
                        else // dont reproject
                        {

                            if (c.Z.ToString() != "NaN")
                            {
                                myPoints.Add(new Point(c.X, c.Y, c.Z));
                            }
                            else
                            {
                                myPoints.Add(new Point(c.X, c.Y, 0));
                            }
                        
                        }

                    }

                    PointsList.Add(myPoints);
                }

                //populate attributes
                for (int j = 0; j < AttNames.Count; j++)
                {
                    myAttsDict.Add(AttNames[j], dt.Rows[i][AttNames[j]].ToString());
                }

                // populate shapeitems
                Shapes.Add(new ShapeItem(type, PointsList, myAttsDict));

            }
        }

        public List<ShapeItem> GetShapes_temp(string path)
        {
            List<ShapeItem> myshapes = new List<ShapeItem>();
            //DotSpatial.Data.ShapefileReader reader = new DotSpatial.Data.ShapefileReader(FilePath);
            DataTable dt = null;
            IFeatureSet FS = FeatureSet.Open(path);
            FS.FillAttributes();
            dt = FS.DataTable;

            // //get the column Name 
            //foreach (DataColumn column in dt.Columns)
            //{
            //    AttNames.Add(column.ColumnName);
            //}

            // //this able to get spricif 
            //if (dt.Rows.Count > 0)
            //{
            //    string val = dt.Rows[0]["BBL"].ToString();
            //}

            for (int i = 30000; i < 50000; i++)
            {
                string val = dt.Rows[i]["BBL"].ToString();
                if (val.StartsWith("1"))
                {
                    //local variables
                    //List<Point> myPoints = new List<Point>();
                    List<List<Point>> PointsList = new List<List<Point>>();
                    Dictionary<string, string> myAttsDict = new Dictionary<string, string>();

                    // populate vertices
                    IFeature feature = FS.GetFeature(i);
                    string type = feature.BasicGeometry.GeometryType;
                    int Numofgeo = feature.NumGeometries;

                    for (int n = 0; n < Numofgeo; n++)
                    {
                        List<Point> myPoints = new List<Point>();
                        IList<Coordinate> coords = feature.BasicGeometry.GetBasicGeometryN(n).Coordinates;
                        //IList<Coordinate> coords = feature.BasicGeometry.Coordinates;
                        //FeatureSet fs = new FeatureSet(FeatureType.Polygon);
                        Shape s = FS.GetShape(i, true);

                        //populate myPoints
                        foreach (var c in coords)
                        {
                            if (c.Z.ToString() != "NaN")
                            {
                                myPoints.Add(new Point(c.X, c.Y, c.Z));
                            }
                            else
                            {
                                //myPoints.Add(new Point(c.X, c.Y));
                                myPoints.Add(new Point(c.X, c.Y, 0));
                            }
                        }

                        PointsList.Add(myPoints);
                    }

                    //populate attributes
                    //for (int j = 0; j < AttNames.Count; j++)
                    //{
                    myAttsDict.Add("BIN", dt.Rows[i]["BIN"].ToString());
                    myAttsDict.Add("HEIGHT_ROO", dt.Rows[i]["HEIGHT_ROO"].ToString());
                    //}

                    // populate shapeitems
                    //Shapes.Add(new ShapeItem(myPoints,myAttsDict));
                    myshapes.Add(new ShapeItem(type, PointsList, myAttsDict)); 
                }

            }

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    string val = dt.Rows[i]["BBL"].ToString();

            //    if (val.StartsWith("1")) // This is for BBL can be more generic like val == value
            //    {
            //        //local variables
            //        List<Point> myPoints = new List<Point>();
            //        Dictionary<string, string> myAttsDict = new Dictionary<string, string>();

            //        // populate vertices
            //        IFeature feature = FS.GetFeature(i);
            //        IList<Coordinate> coords = feature.BasicGeometry.Coordinates;

            //        //populate myPoints
            //        foreach (var c in coords)
            //        {
            //            //myPoints.Add(new Point(c.X, c.Y));
            //            myPoints.Add(new Point(c.X, c.Y, 0));
            //        }

            //        //populate attributes
            //        //for (int j = 0; j < AttNames.Count; j++)
            //        //{
            //        //    myAttsDict.Add(AttNames[j], dt.Rows[i][AttNames[j]].ToString());
            //        //}
            //        myAttsDict.Add("BIN", dt.Rows[i]["BIN"].ToString());
            //        //myAttsDict.Add("BBL", dt.Rows[i]["BBL"].ToString());
            //        myAttsDict.Add("HEIGHT_ROO", dt.Rows[i]["HEIGHT_ROO"].ToString());
            //        //myAttsDict.Add("BUILDING_T", dt.Rows[i]["BUILDING_T"].ToString());
            //        //myAttsDict.Add("FACILITY_T", dt.Rows[i]["FACILITY_T"].ToString());
            //        //myAttsDict.Add("GROUND_ELE", dt.Rows[i]["GROUND_ELE"].ToString());

            //        // populate shapeitems
            //        Shapes.Add(new ShapeItem(myPoints, myAttsDict));
            //    }
            //}


            return myshapes;

            //DotSpatial.Controls.MapPolygonLayer
            //System.Data.DataTable  dt = new System.Data.DataTable();
            //dt = FS.GetAttributes(1, 3);
            //List<IFeature>  features = new List <IFeature>();
            //features = FS.Features.ToList();
            //Polygon pl = new Polygon(

        }

        public void FilterbyAtt_1(string att, List<string> values)
        {
            DataTable dt = null;
            IFeatureSet FS = FeatureSet.Open(FilePath);
            FS.FillAttributes();
            dt = FS.DataTable;

            // learn how to make the performance better
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string val = dt.Rows[i][att].ToString();
                    foreach (var value in values)
                    {
                        if (val == value) // This is for BBL can be more generic like val == value
                        {
                            //local variables
                            List<Point> myPoints = new List<Point>();
                            Dictionary<string, string> myAttsDict = new Dictionary<string, string>();

                            // populate vertices
                            IFeature feature = FS.GetFeature(i);
                            IList<Coordinate> coords = feature.BasicGeometry.Coordinates;

                            //populate myPoints
                            foreach (var c in coords)
                            {
                                //myPoints.Add(new Point(c.X, c.Y));
                                myPoints.Add(new Point(c.X, c.Y, c.Z));
                            }

                            // populate att dict
                            myAttsDict.Add(att, val);

                            // populate shapeitems
                            Shapes.Add(new ShapeItem(myPoints, myAttsDict));
                        }
                        
                    }

            }

        }
        public void FilterbyAtt(string att, List<string> values)
        {
            DataTable dt = null;
            IFeatureSet FS = FeatureSet.Open(FilePath);
            FS.FillAttributes();
            dt = FS.DataTable;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string val = dt.Rows[i][att].ToString();
                foreach (var value in values)
                {
                    if ( val.StartsWith(value)) // This is for BBL can be more generic like val == value
                    {
                        //local variables
                        List<Point> myPoints = new List<Point>();
                        Dictionary<string, string> myAttsDict = new Dictionary<string, string>();

                        // populate vertices
                        IFeature feature = FS.GetFeature(i);
                        IList<Coordinate> coords = feature.BasicGeometry.Coordinates;

                        //populate myPoints
                        foreach (var c in coords)
                        {
                            //myPoints.Add(new Point(c.X, c.Y));
                            myPoints.Add(new Point(c.X, c.Y, c.Z));
                        }

                        // populate att dict
                        myAttsDict.Add(att, val);

                        // populate shapeitems
                        Shapes.Add(new ShapeItem(myPoints,myAttsDict));
                    }
                }

            }
        
        }

        public void DataBinding(string pathOne, string pathTwo, List<string> AttNames)
        { 
            // populate the dts
            DataTable dtOne = null;
            IFeatureSet FS1 = FeatureSet.Open(pathOne);
            FS1.FillAttributes();
            dtOne = FS1.DataTable;

            DataTable dtTwo = null;
            IFeatureSet FS2 = FeatureSet.Open(pathTwo);
            FS2.FillAttributes();
            dtTwo = FS2.DataTable;

            // One datatable PropertyTax
            // second datatable Footprint 

            List<int> MH = new List<int>();
            for (int i = 0; i < dtTwo.Rows.Count; i++)
            {
                    string bblTwo = dtTwo.Rows[i]["BBL"].ToString();
                    if (bblTwo.StartsWith("1"))
                    {
                        MH.Add(i);
                    }
            }

            //  for loop 
            for (int i = 0; i < dtOne.Rows.Count; i++)
            {
                string bblOne = dtOne.Rows[i]["BBL"].ToString();
                double myfloat = Double.Parse(bblOne, System.Globalization.NumberStyles.Float);
                int e = Convert.ToInt32(myfloat);
                bblOne = e.ToString();

                //populate shapeitem  and att list for this property tax item
                List<Point> myPointsOne = new List<Point>();
                Dictionary<string, string> myAttsDict = new Dictionary<string, string>();

                // populate vertices
                IFeature featureone = FS1.GetFeature(i);
                IList<Coordinate> Coords = featureone.BasicGeometry.Coordinates;

                //populate myPoints
                foreach (var c in Coords)
                {
                    //myPointsOne.Add(new Point(c.X, c.Y));
                    myPointsOne.Add(new Point(c.X, c.Y, c.Z));
                }

                //populate attributes
                foreach (var item in AttNames)
                {
                    string value = dtOne.Rows[i][item].ToString();
                    myAttsDict.Add(item, value);
                }

                Shapes.Add(new ShapeItem(myPointsOne,myAttsDict));
                

                List<ShapeItem> myShapes = new List<ShapeItem>();
                //loop into footprint shapefile and check if it is in MH
                foreach (var j in MH)
                {
                      string bblTwo = dtTwo.Rows[j]["BBL"].ToString();
                      if (bblOne == bblTwo)
                        {
                            // do things here

                            //local variables
                            List<Point> myPointsTwo = new List<Point>();

                            //populate shape
                            // populate vertices
                            IFeature feature = FS2.GetFeature(j);
                            IList<Coordinate> coords = feature.BasicGeometry.Coordinates;

                            //populate myPoints
                            foreach (var c in coords)
                            {
                                //myPointsTwo.Add(new Point(c.X, c.Y));
                                myPointsTwo.Add(new Point(c.X, c.Y, c.Z));
                            }

                            myShapes.Add(new ShapeItem(myPointsTwo));
                        }
                }

                    //for (int j = 0; j < dtTwo.Rows.Count; i++)
                    //{
                    //    string bblTwo = dtTwo.Rows[j]["BBL"].ToString();
                    //    if (bblTwo.StartsWith("1") & bblOne == bblTwo)
                    //    {
                    //        // do things here

                    //        //local variables
                    //        List<Point> myPointsTwo = new List<Point>();

                    //        //populate shape
                    //        // populate vertices
                    //        IFeature feature = FS2.GetFeature(j);
                    //        IList<Coordinate> coords = feature.BasicGeometry.Coordinates;

                    //        //populate myPoints
                    //        foreach (var c in coords)
                    //        {
                    //            myPointsTwo.Add(new Point(c.X, c.Y));
                    //        }

                    //        myShapes.Add(new ShapeItem(myPointsTwo));
                    //    }
                    //}

                    ShapesTwo.Add(bblOne, myShapes);
            }

        }

        public void DataBinding_1(string pathOne, string pathTwo, List<string> AttNames, string FilterAtt, List<string>FilterAttValues)
        {
            // populate the dts
            DataTable dtOne = null;
            IFeatureSet FS1 = FeatureSet.Open(pathOne);
            FS1.FillAttributes();
            dtOne = FS1.DataTable;

            DataTable dtTwo = null;
            IFeatureSet FS2 = FeatureSet.Open(pathTwo);
            FS2.FillAttributes();
            dtTwo = FS2.DataTable;

            // One datatable PropertyTax
            // second datatable Footprint 

            List<int> MH = new List<int>();
            for (int i = 0; i < dtTwo.Rows.Count; i++)
            {
                string bblTwo = dtTwo.Rows[i]["BBL"].ToString();
                if (bblTwo.StartsWith("1"))
                {
                    MH.Add(i);
                }
            }

            //  for loop 
            for (int i = 0; i < dtOne.Rows.Count; i++)
            {
  
                    string val  = dtOne.Rows[i][FilterAtt].ToString();
                    foreach (var attval in FilterAttValues)
                    {
                        if (val == attval)
                        {

                            string bblOne = dtOne.Rows[i]["BBL"].ToString();
                            double myfloat = Double.Parse(bblOne, System.Globalization.NumberStyles.Float);
                            int e = Convert.ToInt32(myfloat);
                            bblOne = e.ToString();

                            //populate shapeitem  and att list for this property tax item
                            List<Point> myPointsOne = new List<Point>();
                            Dictionary<string, string> myAttsDict = new Dictionary<string, string>();

                            // populate vertices
                            IFeature featureone = FS1.GetFeature(i);
                            IList<Coordinate> Coords = featureone.BasicGeometry.Coordinates;

                            //populate myPoints
                            foreach (var c in Coords)
                            {
                                //myPointsOne.Add(new Point(c.X, c.Y));
                                myPointsOne.Add(new Point(c.X, c.Y, c.Z));
                            }

                            //populate attributes
                            foreach (var item in AttNames)
                            {
                                string value = dtOne.Rows[i][item].ToString();
                                myAttsDict.Add(item, value);
                            }

                            Shapes.Add(new ShapeItem(myPointsOne, myAttsDict));


                            List<ShapeItem> myShapes = new List<ShapeItem>();
                            //loop into footprint shapefile and check if it is in MH
                            foreach (var j in MH)
                            {
                                string bblTwo = dtTwo.Rows[j]["BBL"].ToString();
                                if (bblOne == bblTwo)
                                {
                                    // do things here

                                    //local variables
                                    List<Point> myPointsTwo = new List<Point>();

                                    //populate shape
                                    // populate vertices
                                    IFeature feature = FS2.GetFeature(j);
                                    IList<Coordinate> coords = feature.BasicGeometry.Coordinates;

                                    //populate myPoints
                                    foreach (var c in coords)
                                    {
                                        //myPointsTwo.Add(new Point(c.X, c.Y));
                                        myPointsTwo.Add(new Point(c.X, c.Y,c.Z));
                                    }

                                    myShapes.Add(new ShapeItem(myPointsTwo));
                                }
                            }
                            ShapesTwo.Add(bblOne, myShapes);
                        }
                        
                    }
  
            }

        }

        public void SortbyBorough(string Borough)
        {
            DataTable dt = null;
            IFeatureSet FS = FeatureSet.Open(FilePath);
            FS.FillAttributes();
            dt = FS.DataTable;

            // get the column Name 
            //foreach (DataColumn column in dt.Columns)
            //{
            //    AttNames.Add(column.ColumnName);
            //}

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string val = dt.Rows[i]["BBL"].ToString();
                    if (val.StartsWith(Borough)) // This is for BBL can be more generic like val == value
                    {
                        //local variables
                        List<Point> myPoints = new List<Point>();
                        Dictionary<string, string> myAttsDict = new Dictionary<string, string>();

                        // populate vertices
                        IFeature feature = FS.GetFeature(i);
                        IList<Coordinate> coords = feature.BasicGeometry.Coordinates;

                        //populate myPoints
                        foreach (var c in coords)
                        {
                            //myPoints.Add(new Point(c.X, c.Y));
                            myPoints.Add(new Point(c.X, c.Y, c.Z));
                        }

                        //populate attributes
                        //for (int j = 0; j < AttNames.Count; j++)
                        //{
                        //    myAttsDict.Add(AttNames[j], dt.Rows[i][AttNames[j]].ToString());
                        //}
                        myAttsDict.Add("BIN", dt.Rows[i]["BIN"].ToString());
                        myAttsDict.Add("BBL", dt.Rows[i]["BBL"].ToString());
                        myAttsDict.Add("HEIGHT_ROO", dt.Rows[i]["HEIGHT_ROO"].ToString());
                        myAttsDict.Add("BUILDING_T", dt.Rows[i]["BUILDING_T"].ToString());
                        myAttsDict.Add("FACILITY_T", dt.Rows[i]["FACILITY_T"].ToString());
                        myAttsDict.Add("GROUND_ELE", dt.Rows[i]["GROUND_ELE"].ToString());

                        // populate shapeitems
                        Shapes.Add(new ShapeItem(myPoints, myAttsDict));
                    }
            }

        }

        public string Parse(string name)
        {
            string Name = string.Empty;
            if (name.Contains("e+"))
            {
                double myfloat = Double.Parse(name, System.Globalization.NumberStyles.Float);
                int e = Convert.ToInt32(myfloat);
                Name = e.ToString();
            }
            return Name;
        }
        //Constructor
        public GIS(string filepath)
        {
            FilePath = filepath;
            GetShapes();
        }

        public GIS(string filepath, bool IsReproject)
        {
            FilePath = filepath;
            GetShapes(IsReproject);
        }

        public GIS()
        { }

        public GIS(string filePath, string AttName, List<string> AttValues)
        {
            FilePath = filePath; 
        }

        // for footprint sort by borough "BBL" start with
        public GIS(string filePath, string Boroughenum)
        {
            FilePath = filePath;
            SortbyBorough(Boroughenum);
        }
    }
}
