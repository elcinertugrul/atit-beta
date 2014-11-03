
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Net;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace GeoClient
{

    public class bin
    {
        public string bbl { get; set; }
        public string bblBoroughCode { get; set; }
        public string bblTaxBlock { get; set; }
        public string bblTaxLot { get; set; }
        public string buildingIdentificationNumber { get; set; }
        public string buildingIdentificationNumberIn { get; set; }
        public string condominiumBillingBbl { get; set; }
        public string cooperativeIdNumber { get; set; }
        public string cornerCode { get; set; }
        public string crossStreetNamesFlagIn { get; set;}
        public string dcpCommercialStudyArea { get; set;}
        public string firstBoroughName { get; set;}
        public string geosupportFunctionCode { get; set;}
        public string geosupportReturnCode { get; set;}
        public string highBblOfThisBuildingsCondominiumUnits { get; set;}
        public string internalLabelXCoordinate { get; set;}
        public string internalLabelYCoordinate { get; set;}
        public string lowBblOfThisBuildingsCondominiumUnits { get; set;}
        public string lowHouseNumberOfDefiningAddressRange { get; set;}

        //public string gi5DigitStreetCode { get; set; }
        //public string giBoroughCode { get; set; }
        //public string giBuildingIdentificationNumber { get; set; }
        //public string giDcpPreferredLgc { get; set; }
        //public string giHighHouseNumber { get; set; }
        //public string giLowHouseNumber { get; set; }
        //public string giSideOfStreetIndicator { get; set; }
        //public string giStreetCode1 { get; set; }

        //"numberOfEntriesInListOfGeographicIdentifiers": "0004",
        //"numberOfExistingStructuresOnLot": "0001",
        //"numberOfStreetFrontagesOfLot": "03",
        //"rpadBuildingClassificationCode": "O3",
        //"rpadSelfCheckCodeForBbl": "7",
        //"sanbornBoroughCode": "1",
        //"sanbornPageNumber": "011",
        //"sanbornVolumeNumber": "01",
        //"sanbornVolumeNumberSuffix": "S",
        //"taxMapNumberSectionAndVolume": "10102",
        //"workAreaFormatIndicatorIn": "C"
    }

    public class BIN_Result
    {
        public bin BIN { get; set; }
    }

    public class address
    {
        public string assemblyDistrict { get; set; }
        //public string bbl { get; set;}
        //public string bblBoroughCode { get; set; }
        //public string bblTaxBlock { get; set; }
        //public string bblTaxLot { get; set; }
        //"boeLgcPointer": "1",
        //"boePreferredStreetName": "WEST  100 STREET",
        //"boePreferredstreetCode": "13577001",
        //"boroughCode1In": "1",
        //public string buildingIdentificationNumber { get; set;}
        //"censusBlock2000": "6000",
        //"censusBlock2010": "2000",
        //"censusTract1990": "187",
        //"censusTract2000": "187",
        //"censusTract2010": "187",
        //"cityCouncilDistrict": "09",
        //"civilCourtDistrict": "05",
        //"coincidenceSegmentCount": "1",
        //"communityDistrict": "107",
        //"communityDistrictBoroughCode": "1",
        //"communityDistrictNumber": "07",
        //"communitySchoolDistrict": "03",
        //"condominiumBillingBbl": "1018887502",
        //"condominiumFlag": "C",
        //"congressionalDistrict": "10",
        //"cooperativeIdNumber": "0000",
        //"crossStreetNamesFlagIn": "E",
        //"dcpPreferredLgc": "01",
        //"dofCondominiumIdentificationNumber": "1981",
        //"dotStreetLightContractorArea": "1",
        //"dynamicBlock": "601",
        //"electionDistrict": "049",
        //"fireBattalion": "11",
        //"fireCompanyNumber": "022",
        //"fireCompanyType": "L",
        //"fireDivision": "03",
        //"firstBoroughName": "MANHATTAN",
        //"firstStreetCode": "13577001010",
        //"firstStreetNameNormalized": "WEST  100 STREET",
        //"fromLionNodeId": "0023422",
        //"fromPreferredLgcsFirstSetOf5": "01",
        //"genericId": "0060351",
        //"geosupportFunctionCode": "1B",
        //"geosupportReturnCode": "00",
        //"geosupportReturnCode2": "00",
        //"gi5DigitStreetCode1": "35770",
        //"giBoroughCode1": "1",
        //"giBuildingIdentificationNumber1": "1057093",
        //"giDcpPreferredLgc1": "01",
        //"giHighHouseNumber1": "316",
        //"giLowHouseNumber1": "314",
        //"giSideOfStreetIndicator1": "L",
        //"giStreetCode1": "13577001",
        //"giStreetName1": "WEST  100 STREET",
        //"healthArea": "3110",
        //"healthCenterDistrict": "16",
        //"highBblOfThisBuildingsCondominiumUnits": "1018881233",
        //"highCrossStreetB5SC1": "129690",
        //"highCrossStreetCode1": "12969001",
        //"highCrossStreetName1": "RIVERSIDE DRIVE",
        //"highHouseNumberOfBlockfaceSortFormat": "000398000AA",
        //"houseNumber": "314",
        //"houseNumberIn": "314",
        //"houseNumberSortFormat": "000314000AA",
        //"interimAssistanceEligibilityIndicator": "I",
        //"internalLabelXCoordinate": "0991892",
        //"internalLabelYCoordinate": "0230017",
        //"legacySegmentId": "0037349",
        //"lionKeyBoroughCode": "1",
        //"lionKeyFaceCode": "5345",
        //"lionKeyForVanityAddressBoroughCode": "1",
        //"lionKeyForVanityAddressFaceCode": "5345",
        //"lionKeyForVanityAddressSequenceNumber": "00060",
        //"lionKeySequenceNumber": "00060",
        //"listOf4Lgcs": "01",
        //"lowBblOfThisBuildingsCondominiumUnits": "1018881201",
        //"lowCrossStreetB5SC1": "144990",
        //"lowCrossStreetCode1": "14499001",
        //"lowCrossStreetName1": "WEST END AVENUE",
        //"lowHouseNumberOfBlockfaceSortFormat": "000300000AA",
        //"lowHouseNumberOfDefiningAddressRange": "000314000AA",
        //"nta": "MN12",
        //public string ntaName { get; set; }
        //"numberOfCrossStreetB5SCsHighAddressEnd": "1",
        //"numberOfCrossStreetB5SCsLowAddressEnd": "1",
        //"numberOfCrossStreetsHighAddressEnd": "1",
        //"numberOfCrossStreetsLowAddressEnd": "1",
        //"numberOfEntriesInListOfGeographicIdentifiers": "0001",
        //"numberOfExistingStructuresOnLot": "0001",
        //"numberOfStreetFrontagesOfLot": "01",
        //"physicalId": "0069454",
        //"policePatrolBoroughCommand": "2",
        //"policePrecinct": "024",
        //"returnCode1a": "00",
        //"returnCode1e": "00",
        //"roadwayType": "1",
        //public string rpadBuildingClassificationCode { get; set; }
        //"rpadSelfCheckCodeForBbl": "5",
        //"sanbornBoroughCode": "1",
        //"sanbornPageNumber": "034",
        //"sanbornVolumeNumber": "07",
        //"sanbornVolumeNumberSuffix": "S",
        //"sanitationCollectionSchedulingSectionAndSubsection": "5B",
        //"sanitationDistrict": "107",
        //"sanitationRecyclingCollectionSchedule": "ET",
        //"sanitationRegularCollectionSchedule": "TTHS",
        //"sanitationSnowPriorityCode": "P",
        //"segmentAzimuth": "151",
        //"segmentIdentifier": "0037349",
        //"segmentLengthInFeet": "00574",
        //"segmentOrientation": "W",
        //"segmentTypeCode": "U",
        //"selfCheckCodeOfBillingBbl": "5",
        //"sideOfStreetIndicator": "L",
        //"sideOfStreetOfVanityAddress": "L",
        //"splitLowHouseNumber": "000300000AA",
        //"stateSenatorialDistrict": "31",
        //"streetName1In": "west 100 st",
        //"streetStatus": "2",
        //"taxMapNumberSectionAndVolume": "10703",
        //"toLionNodeId": "0023852",
        //"toPreferredLgcsFirstSetOf5": "01",
        //"trafficDirection": "A",
        //"underlyingstreetCode": "13577001",
        //"workAreaFormatIndicatorIn": "C",
        //"xCoordinate": "0992059",
        //"xCoordinateHighAddressEnd": "0991683",
        //"xCoordinateLowAddressEnd": "0992186",
        //"xCoordinateOfCenterofCurvature": "0000000",
        //"yCoordinate": "0230011",
        //"yCoordinateHighAddressEnd": "0230221",
        //"yCoordinateLowAddressEnd": "0229944",
        //"yCoordinateOfCenterofCurvature": "0000000",
        //public string zipCode { get; set;}

    }

    public class Address_Result
    {
        public address Adress { get; set; }
    }

    public class Geoclient
    {
        private string _appID = "&app_id=19de8714";
        private string _appKey = "&app_key=8da912c7bf3b918510e633dfa356d55c";


        private const string baseUri = @"https://api.cityofnewyork.us/geoclient";
        private static string _BINuri = @"/v1/bin.json?bin={0}";
        private static string _AdressUri = @"/v1/address.json?houseNumber={0}&street={1}&borough={2}";

        // https://api.cityofnewyork.us/geoclient/v1/bin.json?bin=1079043&app_id=19de8714&app_key=8da912c7bf3b918510e633dfa356d55c

        // https://api.cityofnewyork.us/geoclient/v1/address.json?houseNumber=314&street=west 100 st&borough=manhattan&app_id=19de8714&app_key=8da912c7bf3b918510e633dfa356d55c

        public BIN_Result BIN_WEbRequest ( string _bin)
        {
        
            string url = baseUri + string.Format(_BINuri,_bin) + _appID + _appKey ; 
            WebRequest request = WebRequest.Create(url);
            WebResponse myresponse = request.GetResponse();
            var result = new System.Net.WebClient().DownloadString(url);


            BIN_Result _binResponse = JsonConvert.DeserializeObject<BIN_Result>(result);
            return _binResponse;
        }

        public Address_Result Address_WebRequest(string _houseNumber, string _street, string _borough)
        {
            List<string> myaddress = new List<string>();
            myaddress.Add(_houseNumber);
            myaddress.Add(_street);
            myaddress.Add(_borough);
            string[] array = myaddress.ToArray();

            string url = baseUri + string.Format(_AdressUri, array) + _appID + _appKey;
            WebRequest request = WebRequest.Create(url);
            WebResponse myresponse = request.GetResponse();
            var result = new System.Net.WebClient().DownloadString(url);

            Address_Result _addresresponse = JsonConvert.DeserializeObject<Address_Result>(result);
            return _addresresponse;
        }

        public Geoclient() { }

    }
}
