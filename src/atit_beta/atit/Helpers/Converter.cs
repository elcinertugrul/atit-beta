using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjNet;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems;

namespace atit.Helpers
{
    public class Converter
    {
        private static string GetBand(double latitude)
        {
            if (latitude <= 84 && latitude >= 72)
                return "X";
            else if (latitude < 72 && latitude >= 64)
                return "W";
            else if (latitude < 64 && latitude >= 56)
                return "V";
            else if (latitude < 56 && latitude >= 48)
                return "U";
            else if (latitude < 48 && latitude >= 40)
                return "T";
            else if (latitude < 40 && latitude >= 32)
                return "S";
            else if (latitude < 32 && latitude >= 24)
                return "R";
            else if (latitude < 24 && latitude >= 16)
                return "Q";
            else if (latitude < 16 && latitude >= 8)
                return "P";
            else if (latitude < 8 && latitude >= 0)
                return "N";
            else if (latitude < 0 && latitude >= -8)
                return "M";
            else if (latitude < -8 && latitude >= -16)
                return "L";
            else if (latitude < -16 && latitude >= -24)
                return "K";
            else if (latitude < -24 && latitude >= -32)
                return "J";
            else if (latitude < -32 && latitude >= -40)
                return "H";
            else if (latitude < -40 && latitude >= -48)
                return "G";
            else if (latitude < -48 && latitude >= -56)
                return "F";
            else if (latitude < -56 && latitude >= -64)
                return "E";
            else if (latitude < -64 && latitude >= -72)
                return "D";
            else if (latitude < -72 && latitude >= -80)
                return "C";
            else
                return null;
        }

        private static int GetZone(double latitude, double longitude)
        {
            // Norway
            if (latitude >= 56 && latitude < 64 && longitude >= 3 && longitude < 13)
                return 32;

            // Spitsbergen
            if (latitude >= 72 && latitude < 84)
            {
                if (longitude >= 0 && longitude < 9)
                    return 31;
                else if (longitude >= 9 && longitude < 21)
                    return 33;
                if (longitude >= 21 && longitude < 33)
                    return 35;
                if (longitude >= 33 && longitude < 42)
                    return 37;
            }

            return (int)Math.Ceiling((longitude + 180) / 6);
        }

        /// <summary>
        /// Converts WGS LL to UTM (in meters)
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="easting"></param>
        /// <param name="northing"></param>
        public static void ConvertToUtmString(double longitude, double latitude, ref double easting, ref double northing)
        {
            //if (latitude < -80 || latitude > 84)
                // return null;

            int zone = GetZone(latitude, longitude);
            string band = GetBand(latitude);

            //Transform to UTM
            CoordinateTransformationFactory ctfac = new CoordinateTransformationFactory();
            ICoordinateSystem wgs84geo = ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84;
            ICoordinateSystem utm = ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WGS84_UTM(zone, latitude > 0);
            ICoordinateTransformation trans = ctfac.CreateFromCoordinateSystems(wgs84geo, utm);
            double[] pUtm = trans.MathTransform.Transform(new double[] { longitude, latitude });

            easting = pUtm[0];
            northing = pUtm[1];

            //return String.Format("{0}{1} {2:0} {3:0}", zone, band, easting, northing);
        }


        /// <summary>
        /// Define conversion factor for TTX length conversions
        /// </summary>
        /// <param name="toUnit">The Length unit to convert TO</param>
        /// <param name="fromUnit">The Length unit to convert FROM</param>
        /// <returns></returns>
        public static double defineConversion(string toUnit, string fromUnit)
        {
            toUnit = toUnit.ToLower();
            fromUnit = fromUnit.ToLower();
            double conversionFactor = 1.0;

            //if project units = meters
            if (fromUnit == "m")
            {
                if (toUnit == "m")
                { conversionFactor = 1; }
                else if (toUnit == "cm")
                { conversionFactor = 100; }
                else if (toUnit == "mm")
                { conversionFactor = 1000; }
                else if (toUnit == "ft")
                { conversionFactor = 1 / 0.3048; }
                else if (toUnit == "in")
                { conversionFactor = 1000 / 25.4; }
            }

            //else if project units = centimeters
            else if (fromUnit == "cm")
            {
                if (toUnit == "m")
                { conversionFactor = 0.01; }
                else if (toUnit == "cm")
                { conversionFactor = 1; }
                else if (toUnit == "mm")
                { conversionFactor = 10; }
                else if (toUnit == "ft")
                { conversionFactor = 1 / 30.48; }
                else if (toUnit == "in")
                { conversionFactor = 10 / 25.4; }
            }

            //else if project units = milimeters
            else if (fromUnit == "mm")
            {
                if (toUnit == "m")
                { conversionFactor = 0.001; }
                else if (toUnit == "cm")
                { conversionFactor = 0.1; }
                else if (toUnit == "mm")
                { conversionFactor = 1; }
                else if (toUnit == "ft")
                { conversionFactor = 1 / 304.8; }
                else if (toUnit == "in")
                { conversionFactor = 1 / 25.4; }
            }

            //else if project units = feet
            else if (fromUnit == "ft")
            {
                if (toUnit == "m")
                { conversionFactor = .3048; }
                else if (toUnit == "cm")
                { conversionFactor = 30.48; }
                else if (toUnit == "mm")
                { conversionFactor = 304.8; }
                else if (toUnit == "ft")
                { conversionFactor = 1; }
                else if (toUnit == "in")
                { conversionFactor = 12; }
            }

            //else if project units = inches
            else if (fromUnit == "in")
            {
                if (toUnit == "m")
                { conversionFactor = .0254; }
                else if (toUnit == "cm")
                { conversionFactor = 2.54; }
                else if (toUnit == "mm")
                { conversionFactor = 25.4; }
                else if (toUnit == "ft")
                { conversionFactor = 1 / 12.000; }
                else if (toUnit == "in")
                { conversionFactor = 1; }
            }

            return conversionFactor;
        }
    }
}
