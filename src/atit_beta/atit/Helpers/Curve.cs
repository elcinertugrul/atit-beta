using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace atit.Helpers
{
    public class Curve
    {
        private List<Point> myPts = new List<Point>();

        // Properties
        public List<Point> Pts 
        {
            get { return myPts; }
            set { myPts = value; }
        }


        //Constructors
        public Curve() { }
        public Curve(List<Point> Vertexes)
        {
            myPts = Vertexes;       
        }

    }
}
