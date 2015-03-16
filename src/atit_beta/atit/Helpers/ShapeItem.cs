using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace atit.Helpers
{
    [DataContract]
    public class ShapeItem 
    {
        //Field
        [DataMember]
        public List<Point> Pts = new List<Point>();
        [DataMember]
        public Dictionary<string, string> AttsDict = new Dictionary<string, string>(); // <Field, Value>
        public Dictionary<string, List<string>> BindDict = new Dictionary<string, List<string>>();

        public string Type; // Polygon, Polyline, Point
        public Point[] PtsArray;
        public List<List<Point>> PtsLists = new List<List<Point>>();

        public Curve TaxCurve = new Curve { };
        public List<Curve> Footprints = new List<Curve>();

        public override string ToString()
        {
            return "Shape";
        }

        //Constructor
        public ShapeItem() { }
        public ShapeItem(List<Point> Vrtx, Dictionary<string, string> attDict)
        {
            Pts = Vrtx;
            AttsDict = attDict;      
        }

        public ShapeItem(List<Point> Vrtx)
        {
            Pts = Vrtx;
        }

        public ShapeItem(Curve tax, List<Curve> footprints, Dictionary<string, List<string>> attDict)
        {
            TaxCurve = tax;
            Footprints = footprints;
            BindDict = attDict;
        }

        public ShapeItem(string type, List<List<Point>> Vrtx, Dictionary<string, string> attDict)
        {
            Type = type;
            PtsLists = Vrtx;
            AttsDict = attDict;         
        }
    }
}
