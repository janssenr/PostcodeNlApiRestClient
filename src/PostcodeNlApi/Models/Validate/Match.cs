using System.Runtime.Serialization;

namespace PostcodeNlApi.Models.Validate
{
    [DataContract]
    public class Match
    {
        [DataMember(Name = "status")]
        public Status Status { get; set; }

        [DataMember(Name = "language")]
        public string Language { get; set; }

        [DataMember(Name = "address")]
        public Address Address { get; set; }

        [DataMember(Name = "mailLines")]
        public string[] MailLines { get; set; }

        [DataMember(Name = "location")]
        public Location Location { get; set; }

        [DataMember(Name = "isPoBox")]
        public bool IsPoBox { get; set; }

        [DataMember(Name = "country")]
        public Country Country { get; set; }
    }
}
