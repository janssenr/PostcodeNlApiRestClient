using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PostcodeNlApi.Signal
{
    /// <summary>
    /// Summary description for PostcodeNlSignal
    /// </summary>
    [DataContract]
    public class PostcodeNlSignalRequest
    {
        [DataMember(Name = "customer", EmitDefaultValue = false)]
        public PostcodeNlSignalCustomer Customer { get; set; }

        [DataMember(Name = "access", EmitDefaultValue = false)]
        public PostcodeNlSignalAccess Access { get; set; }

        [DataMember(Name = "transaction", EmitDefaultValue = false)]
        public PostcodeNlSignalTransaction Transaction { get; set; }

        [DataMember(Name = "config", EmitDefaultValue = false)]
        public PostcodeNlSignalConfig Config { get; set; }
    }

    [DataContract]
    public class PostcodeNlSignalCustomer
    {
        [DataMember(Name = "firstName", EmitDefaultValue = false)]
        public string FirstName { get; set; }

        [DataMember(Name = "lastName", EmitDefaultValue = false)]
        public string LastName { get; set; }

        [DataMember(Name = "birthDate", EmitDefaultValue = false)]
        public DateTime? BirthDate { get; set; }

        [DataMember(Name = "email", EmitDefaultValue = false)]
        public string Email { get; set; }

        [DataMember(Name = "emailDomain", EmitDefaultValue = false)]
        public string EmailDomain { get; set; }

        [DataMember(Name = "phoneNumber", EmitDefaultValue = false)]
        public string PhoneNumber { get; set; }

        [DataMember(Name = "bankNumber", EmitDefaultValue = false)]
        public string BankNumber { get; set; }

        [DataMember(Name = "site", EmitDefaultValue = false)]
        public string Site { get; set; }

        [DataMember(Name = "internalId", EmitDefaultValue = false)]
        public string InternalId { get; set; }

        [DataMember(Name = "address", EmitDefaultValue = false)]
        public PostcodeNlSignalAddress Address { get; set; }

        [DataMember(Name = "company", EmitDefaultValue = false)]
        public PostcodeNlSignalCompany Company { get; set; }
    }

    [DataContract]
    public class PostcodeNlSignalAddress
    {
        [DataMember(Name = "postcode", EmitDefaultValue = false)]
        public string Postcode { get; set; }

        [DataMember(Name = "houseNumber", EmitDefaultValue = false)]
        public int? HouseNumber { get; set; }

        [DataMember(Name = "houseNumberAddition", EmitDefaultValue = false)]
        public string HouseNumberAddition { get; set; }

        [DataMember(Name = "street", EmitDefaultValue = false)]
        public string Street { get; set; }

        [DataMember(Name = "city", EmitDefaultValue = false)]
        public string City { get; set; }

        [DataMember(Name = "region", EmitDefaultValue = false)]
        public string Region { get; set; }

        [DataMember(Name = "country", EmitDefaultValue = false)]
        public string Country { get; set; }

    }

    [DataContract]
    public class PostcodeNlSignalCompany
    {
        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(Name = "governmentId", EmitDefaultValue = false)]
        public string GovernmentId { get; set; }

        [DataMember(Name = "country", EmitDefaultValue = false)]
        public string Country { get; set; }
    }

    [DataContract]
    public class PostcodeNlSignalAccess
    {
        [DataMember(Name = "ipAddress", EmitDefaultValue = false)]
        public string IpAddress { get; set; }

        [DataMember(Name = "additionalIpAddresses", EmitDefaultValue = false)]
        public List<string> AdditionalIpAddresses { get; set; }

        [DataMember(Name = "sessionId", EmitDefaultValue = false)]
        public string SessionId { get; set; }

        [DataMember(Name = "time", EmitDefaultValue = false)]
        public DateTime? Time { get; set; }

        [DataMember(Name = "browser", EmitDefaultValue = false)]
        public PostcodeNlSignalBrowser Browser { get; set; }
    }

    [DataContract]
    public class PostcodeNlSignalBrowser
    {
        [DataMember(Name = "userAgent", EmitDefaultValue = false)]
        public string UserAgent { get; set; }

        [DataMember(Name = "acceptLanguage", EmitDefaultValue = false)]
        public string AcceptLanguage { get; set; }
    }

    [DataContract]
    public class PostcodeNlSignalTransaction
    {
        [DataMember(Name = "internalId", EmitDefaultValue = false)]
        public string InternalId { get; set; }

        [DataMember(Name = "status", EmitDefaultValue = false)]
        public string Status { get; set; }

        [DataMember(Name = "cost", EmitDefaultValue = false)]
        public double? Cost { get; set; }

        [DataMember(Name = "costCurrency", EmitDefaultValue = false)]
        public string CostCurrency { get; set; }

        [DataMember(Name = "paymentType", EmitDefaultValue = false)]
        public string PaymentType { get; set; }

        [DataMember(Name = "weight", EmitDefaultValue = false)]
        public int? Weight { get; set; }

        [DataMember(Name = "deliveryAddress", EmitDefaultValue = false)]
        public PostcodeNlSignalAddress DeliveryAddress { get; set; }
    }

    [DataContract]
    public class PostcodeNlSignalConfig
    {
        [DataMember(Name = "selectServices", EmitDefaultValue = false)]
        public List<string> SelectServices { get; set; }

        [DataMember(Name = "excludeServices", EmitDefaultValue = false)]
        public List<string> ExcludeServices { get; set; }

        [DataMember(Name = "selectTypes", EmitDefaultValue = false)]
        public List<string> SelectTypes { get; set; }

        [DataMember(Name = "excludeTypes", EmitDefaultValue = false)]
        public List<string> ExcludeTypes { get; set; }
    }

    [DataContract]
    public class PostcodeNlSignalResponse
    {
        [DataMember(Name = "checkId", EmitDefaultValue = false)]
        public string CheckId { get; set; }

        [DataMember(Name = "signals", EmitDefaultValue = false)]
        public List<PostcodeNlSignalResponseSignal> Signals { get; set; }

        [DataMember(Name = "warningCount", EmitDefaultValue = false)]
        public int WarningCount { get; set; }

        [DataMember(Name = "reportPdfUrl", EmitDefaultValue = false)]
        public string ReportPdfUrl { get; set; }
    }

    [DataContract]
    public class PostcodeNlSignalResponseSignal
    {
        [DataMember(Name = "concerning", EmitDefaultValue = false)]
        public string Concerning { get; set; }

        [DataMember(Name = "type", EmitDefaultValue = false)]
        public string Type { get; set; }

        [DataMember(Name = "warning", EmitDefaultValue = false)]
        public bool Warning { get; set; }

        [DataMember(Name = "message", EmitDefaultValue = false)]
        public string Message { get; set; }

        [DataMember(Name = "data", EmitDefaultValue = false)]
        public object Data { get; set; }
    }
}