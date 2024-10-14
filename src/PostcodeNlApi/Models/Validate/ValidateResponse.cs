using System.Runtime.Serialization;

namespace PostcodeNlApi.Models.Validate
{
    [DataContract]
    public class ValidateResponse
    {
        [DataMember(Name = "country")]
        public Country Country { get; set; }

        [DataMember(Name = "matches")]
        public Match[] Matches { get; set; }
    }
}
