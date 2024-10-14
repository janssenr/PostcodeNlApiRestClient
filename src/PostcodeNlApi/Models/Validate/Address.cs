using System.Runtime.Serialization;

namespace PostcodeNlApi.Models.Validate
{
    [DataContract]
    public class Address
    {
        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "locality")]
        public string Locality { get; set; }

        [DataMember(Name = "street")]
        public string Street { get; set; }

        [DataMember(Name = "postcode")]
        public string Postcode { get; set; }

        [DataMember(Name = "building")]
        public string Building { get; set; }

        [DataMember(Name = "buildingNumber")]
        public int? BuildingNumber { get; set; }

        [DataMember(Name = "buildingNumberAddition")]
        public string BuildingNumberAddition { get; set; }

        [DataMember(Name = "region")]
        public string Region { get; set; }
    }
}
