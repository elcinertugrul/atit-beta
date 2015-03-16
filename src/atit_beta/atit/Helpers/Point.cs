using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace atit.Helpers
{
    public class Point
    {
        private double x;
        private double y;
        private double z;

        // Properties
        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        public double Z
        {
            get { return z; }
            set {z = value; }
        }
        public Point(double X, double Y)
        {
            x = X;
            y = Y;      
        }

        public Point(double X, double Y, double Z)
        {
            x = X;
            y = Y;
            z = Z;  
        }

    }
}
