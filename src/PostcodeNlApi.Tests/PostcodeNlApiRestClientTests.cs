using System.Configuration;
using NUnit.Framework;

namespace PostcodeNlApi.Tests
{
    [TestFixture]
    public class PostcodeNlApiRestClientTests
    {
        private PostcodeNlApiRestClient _postcodeNlApiRestClient;

        [SetUp]
        public void Init()
        {
            _postcodeNlApiRestClient = new PostcodeNlApiRestClient(ConfigurationManager.AppSettings["appKey"],
                ConfigurationManager.AppSettings["appSecret"]);
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
        [ExpectedException(typeof(PostcodeNlApiRestClientAddressNotFoundException))]
        public void ExistingPostcodeWithOnlyOnePossibleHousenumberAddition()
        {
            _postcodeNlApiRestClient.LookupAddress("1011AE", "36", "B", true);
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
        public void ResultShowingDifferenceBetweenStreetAndStreetNen()
        {
            var address = _postcodeNlApiRestClient.LookupAddress("1011DG", "2", "", true);
            Assert.AreEqual("1011DG", address.Postcode);
            Assert.AreEqual("2", address.HouseNumber);
            Assert.AreEqual("", address.HouseNumberAddition);
        }
    }
}
