using System.Runtime.Serialization;

namespace PostcodeNlApi.Models.Validate
{
    [DataContract]
    public class Country
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "iso3Code")]
        public string Iso3Code { get; set; }

        [DataMember(Name = "iso2Code")]
        public string Iso2Code { get; set; }
    }
}
