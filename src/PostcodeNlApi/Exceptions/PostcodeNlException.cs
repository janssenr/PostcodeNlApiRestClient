using System.Runtime.Serialization;

namespace PostcodeNlApi.Exceptions
{
    /// <summary>
    /// Summary description for PostcodeNlException
    /// </summary>
    [DataContract]
    public class PostcodeNlException
    {
        [DataMember(Name = "exception")]
        public string Exception { get; set; }

        [DataMember(Name = "exceptionId")]
        public string ExceptionId { get; set; }
    }
}