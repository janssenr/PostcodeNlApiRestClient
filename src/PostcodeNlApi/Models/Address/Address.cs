using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PostcodeNlApi.Address
{
    /// <summary>
    /// Summary description for PostCodeNlAddress
    /// </summary>
    [DataContract]
    public class Address
    {
        [DataMember(Name = "street")]
        public string Street { get; set; }

        [DataMember(Name = "streetNen")]
        public string StreetNen { get; set; }

        [DataMember(Name = "houseNumber")]
        public string HouseNumber { get; set; }

        [DataMember(Name = "houseNumberAddition")]
        public string HouseNumberAddition { get; set; }

        [DataMember(Name = "postcode")]
        public string Postcode { get; set; }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "cityShort")]
        public string CityShort { get; set; }

        [DataMember(Name = "municipality")]
        public string Municipality { get; set; }

        [DataMember(Name = "municipalityShort")]
        public string MunicipalityShort { get; set; }

        [DataMember(Name = "province")]
        public string Province { get; set; }

        [DataMember(Name = "rdX")]
        public string RdX { get; set; }

        [DataMember(Name = "rdY")]
        public string RdY { get; set; }

        [DataMember(Name = "latitude")]
        public double? Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public double? Longitude { get; set; }

        [DataMember(Name = "bagNumberDesignationId")]
        public string BagNumberDesignationId { get; set; }

        [DataMember(Name = "bagAddressableObjectId")]
        public string BagAddressableObjectId { get; set; }

        [DataMember(Name = "addressType")]
        public string AddressType { get; set; }

        [DataMember(Name = "purposes")]
        public List<string> Purposes { get; set; }

        [DataMember(Name = "surfaceArea")]
        public string SurfaceArea { get; set; }

        [DataMember(Name = "houseNumberAdditions")]
        public List<string> HouseNumberAdditions { get; set; }
    }
}