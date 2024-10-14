using System.Runtime.Serialization;

namespace PostcodeNlApi.Models.Validate
{
    [DataContract]
    public class Status
    {
        [DataMember(Name = "grade")]
        public string Grade { get; set; }

        [DataMember(Name = "validationLevel")]
        public string ValidationLevel { get; set; }

        [DataMember(Name = "isAmbiguous")]
        public bool IsAmbiguous { get; set; }
    }
}
