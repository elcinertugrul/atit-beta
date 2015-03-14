// by Elcin Ertugrul 2015/02/15
// Google GeoCoder

//https://developers.google.com/maps/documentation/webservices/
//https://developers.google.com/maps/documentation/geocoding/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Net;
using System.Xml.Linq;
using Newtonsoft.Json; // http://json.codeplex.com/

namespace atit.Helpers
{

    public class GoogleGeoCodeResponse
    {

        public string status { get; set; }
        public results[] results { get; set; }

    }

    public class results
    {
        public string formatted_address { get; set; }
        public geometry geometry { get; set; }
        public string[] types { get; set; }
        public address_component[] address_components { get; set; }
    }

    public class geometry
    {
        public string location_type { get; set; }
        public location location { get; set; }
        public string type { get; set; }
        public List<List<List<double>>> coordinates { get; set; }
    }

    public class location
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class address_component
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public string[] types { get; set; }
    }


    public class GeoCoder
    {
        //response content type : json
        public GoogleGeoCodeResponse AdressWedRequest(string address) //  string country, string zipcode
        {
            //string[] formatted_addresses ;

            //http://www.ozgurcakmak.com.tr/google-maps-geocoding-in-c/
            //http://stackoverflow.com/questions/16274508/how-to-call-google-geocode-service-from-c-sharp-code
            string url = "https://maps.googleapis.com/maps/api/geocode/json?address=" + address + "&sensor=false"; // + country zip code
            WebRequest request = WebRequest.Create(url);
            WebResponse myresponse = request.GetResponse();

            var result = new System.Net.WebClient().DownloadString(url);
            GoogleGeoCodeResponse test = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>(result);
            string f_address = test.results[0].formatted_address;
            return test;
        }

        public GoogleGeoCodeResponse ReverseGeocoding(string latlng)
        {
            string url = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + latlng; // + country zip code
            WebRequest request = WebRequest.Create(url);
            WebResponse myresponse = request.GetResponse();

            var result = new System.Net.WebClient().DownloadString(url);
            GoogleGeoCodeResponse test = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>(result);
            string f_address = test.results[0].formatted_address;
            return test;
        
        }

        public GeoCoder() { }
    }
}