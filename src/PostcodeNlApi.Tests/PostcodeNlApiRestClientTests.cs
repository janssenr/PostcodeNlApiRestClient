using System.Configuration;
using NUnit.Framework;
using PostcodeNlApi.Signal;

namespace PostcodeNlApi.Tests
{
    [TestFixture]
    public class PostcodeNlApiRestClientTests
    {
        private PostcodeNlApiRestClient _postcodeNlApiRestClient;

        [SetUp]
        public void Init()
        {
            _postcodeNlApiRestClient = new PostcodeNlApiRestClient(ConfigurationManager.AppSettings["appKey"], ConfigurationManager.AppSettings["appSecret"]);
        }

        [Test]
        public void ExistingPostcodeWithNoHousenumberAddition()
        {
            var address = _postcodeNlApiRestClient.LookupAddress("2012ES", "30", "", true);
            Assert.AreEqual("2012ES", address.Postcode);
            Assert.AreEqual("30", address.HouseNumber);
            Assert.AreEqual("", address.HouseNumberAddition);
        }

        [Test]
        [ExpectedException(typeof(PostcodeNlApiRestClientInputInvalidException))]
        public void ExistingPostcodeWithOnlyOnePossibleHousenumberAddition()
        {
            _postcodeNlApiRestClient.LookupAddress("2011DW", "8", "RD", true);
        }

        [Test]
        [ExpectedException(typeof(PostcodeNlApiRestClientInputInvalidException))]
        public void ExistingPostcodeWithMultipleHousenumberAdditionsIncorrectAddition()
        {
            _postcodeNlApiRestClient.LookupAddress("2011DW", "9", "ZZZ", true);
        }

        [Test]
        [ExpectedException(typeof(PostcodeNlApiRestClientAddressNotFoundException))]
        public void NonExistingPostcode()
        {
            _postcodeNlApiRestClient.LookupAddress("1234ZZ", "1234", "", true);
        }

        [Test]
        public void SignalTest()
        {
            var address = new PostcodeNlSignalAddress
            {
                Postcode = "2012ES",
                HouseNumber = 30,
                Country = "NL"
            };

            var request = new PostcodeNlSignalRequest
            {
                Customer = new PostcodeNlSignalCustomer
                {
                    Email = "test-address@postcode.nl",
                    PhoneNumber = "+31235325689",
                    Address = address
                },
                Access = new PostcodeNlSignalAccess
                {
                    IpAddress = "123.123.123.123"
                },
                Transaction = new PostcodeNlSignalTransaction
                {
                    Status = "new-checkout",
                    InternalId = "534729",
                    DeliveryAddress = address
                }
            };
            var result = _postcodeNlApiRestClient.DoSignalCheck(request);
            Assert.AreEqual(4, result.Signals.Count);
            Assert.AreEqual(0, result.WarningCount);
        }
    }
}
