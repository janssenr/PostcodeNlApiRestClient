using System.Runtime.Serialization;

namespace PostcodeNlApi.Models.Validate
{
    [DataContract]
    public class Location
    {
        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "precision")]
        public string Precision { get; set; }
    }
}
