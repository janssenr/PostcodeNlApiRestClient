using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using PostcodeNlApi.Address;
using PostcodeNlApi.Exceptions;
using PostcodeNlApi.Helpers;

namespace PostcodeNlApi
{
    /// <summary>
    /// Base superclass for Exceptions raised by this class.
    /// </summary>
    public class PostcodeNlApiRestClientException : Exception
    {
        public PostcodeNlApiRestClientException(string message) : base(message) { }
    }

    /// <summary>
    /// Exception raised when user input is invalid.
    /// </summary>
    public class PostcodeNlApiRestClientInputInvalidException : PostcodeNlApiRestClientException
    {
        public PostcodeNlApiRestClientInputInvalidException(string message) : base(message) { }
    }

    /// <summary>
    /// Exception raised when address input contains no formatting errors, but no address could be found.
    /// </summary>
    public class PostcodeNlApiRestClientAddressNotFoundException : PostcodeNlApiRestClientException
    {
        public PostcodeNlApiRestClientAddressNotFoundException(string message) : base(message) { }
    }

    /// <summary>
    /// Exception raised when an unexpected error occurred in this client.
    /// </summary>
    public class PostcodeNlApiRestClientClientException : PostcodeNlApiRestClientException
    {
        public PostcodeNlApiRestClientClientException(string message) : base(message) { }
    }

    /// <summary>
    /// Exception raised when an unexpected error occurred on the remote service.
    /// </summary>
    public class PostcodeNlApiRestClientServiceException : PostcodeNlApiRestClientException
    {
        public PostcodeNlApiRestClientServiceException(string message) : base(message) { }
    }

    /// <summary>
    /// Exception raised when there is a authentication problem.
    /// In a production environment, you probably always want to catch, log and hide these exceptions.
    /// </summary>
    public class PostcodeNlApiRestClientAuthenticationException : PostcodeNlApiRestClientException
    {
        public PostcodeNlApiRestClientAuthenticationException(string message) : base(message) { }
    }

    /// <summary>
    /// Class to connect to the Postcode.nl API web services via the REST endpoint.
    /// </summary>
    public class PostcodeNlApiRestClient
    {
        /// <summary>
        /// Default URL where the REST web service is located
        /// </summary>
        private readonly string _restApiUrl = "https://api.postcode.nl/rest";
        /// <summary>
        /// Internal storage of the application key of the authentication.
        /// </summary>
        private readonly string _appKey;
        /// <summary>
        /// Internal storage of the application secret of the authentication.
        /// </summary>
        private readonly string _appSecret;

        /// <summary>
        /// If debug data is stored.
        /// </summary>
        private bool _debugEnabled;
        /// <summary>
        /// Debug data storage.
        /// </summary>
        private Dictionary<string, string> _debugData;
        /// <summary>
        /// Last response
        /// </summary>
        private string _lastResponseData;

        /// <summary>
        /// PostcodeNl_Api_RestClient constructor.
        /// </summary>
        /// <param name="appKey">Application Key as provided by Postcode.nl</param>
        /// <param name="appSecret">Application Secret as provided by Postcode.nl</param>
        /// <param name="restApiUrl">Service URL to call.</param>
        public PostcodeNlApiRestClient(string appKey, string appSecret, string restApiUrl = null)
        {
            _appKey = appKey;
            _appSecret = appSecret;
            if (!string.IsNullOrEmpty(restApiUrl))
                _restApiUrl = restApiUrl;

            if (string.IsNullOrEmpty(_appKey) || string.IsNullOrEmpty(_appSecret))
                throw new PostcodeNlApiRestClientException("No application key / secret configured, you can obtain these at https://api.postcode.nl.");
        }

        /// <summary>
        /// Toggle debug option.
        /// </summary>
        /// <param name="debugEnabled"></param>
        public void SetDebugEnabled(bool debugEnabled = true)
        {
            _debugEnabled = debugEnabled;
            if (!_debugEnabled)
                _debugData = null;
        }


        /// <summary>
        /// Get the debug data gathered so far.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetDebugData()
        {
            return _debugData;
        }

        /// <summary>
        /// Perform a REST call to the Postcode.nl API
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected string DoRestCall(string url)
        {
            var req = WebRequest.Create(url);
            // How do we authenticate ourselves? Using HTTP BASIC authentication (http://en.wikipedia.org/wiki/Basic_access_authentication)
            req.Credentials = new NetworkCredential(_appKey, _appSecret);
            req.Method = "GET";
            string responseHeaders = string.Empty;
            string responseValue = string.Empty;
            try
            {
                using (var response = (HttpWebResponse)req.GetResponse())
                {
                    responseHeaders = response.Headers.ToString();
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            using (var reader = new StreamReader(responseStream))
                            {
                                responseValue = reader.ReadToEnd();
                                reader.Close();
                            }
                            responseStream.Close();
                        }
                    }
                    response.Close();
                }
            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;
                responseHeaders = response.Headers.ToString();
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                            reader.Close();
                        }
                        responseStream.Close();
                    }
                }
                response.Close();
                var exception = JsonHelper.Deserialize<PostcodeNlException>(responseValue);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.NotFound: //404
                        // Could not find an address for the input values given
                        if (exception.ExceptionId == "PostcodeNl_Service_PostcodeAddress_AddressNotFoundException")
                            throw new PostcodeNlApiRestClientAddressNotFoundException(exception.Exception);
                        break;
                    case HttpStatusCode.Unauthorized: //401
                    case HttpStatusCode.Forbidden: //403
                        // Could not authenticate, probably invalid or no key/secret configured
                        throw new PostcodeNlApiRestClientAuthenticationException(exception.Exception);
                    case HttpStatusCode.BadRequest: //400
                        throw new PostcodeNlApiRestClientInputInvalidException(exception.Exception);
                    case HttpStatusCode.InternalServerError: //500
                        throw new PostcodeNlApiRestClientServiceException(exception.Exception);
                    default:
                        // Other error
                        throw new PostcodeNlApiRestClientServiceException(exception.Exception);
                }
            }
            finally
            {
                if (_debugEnabled)
                {
                    _debugData = new Dictionary<string, string>
                    {
                        {"request", req.Headers.ToString()},
                        {"response", responseHeaders + responseValue}
                    };
                }
            }

            _lastResponseData = responseValue;
            return responseValue;
        }

        /// <summary>
        /// Look up an address by postcode and house number.
        /// </summary>
        /// <param name="postcode">Dutch postcode in the '1234AB' format</param>
        /// <param name="houseNumber">House number (may contain house number addition, will be separated automatically)</param>
        /// <param name="houseNumberAddition">House number addition</param>
        /// <param name="validateHouseNumberAddition">Enable to validate the addition</param>
        /// <returns>PostcodeNlAddress object</returns>
        public PostcodeNlAddress LookupAddress(string postcode, string houseNumber, string houseNumberAddition = "", bool validateHouseNumberAddition = false)
        {
            // Remove spaces in postcode ('1234 AB' should be '1234AB')
            postcode = postcode != null ? postcode.Trim().Replace(" ", "") : string.Empty;
            houseNumber = houseNumber != null ? houseNumber.Trim() : string.Empty;
            houseNumberAddition = houseNumberAddition != null ? houseNumberAddition.Trim() : string.Empty;

            if (string.IsNullOrEmpty(houseNumberAddition))
            {
                // If people put the housenumber addition in the housenumber field - split this.
                var list = SplitHouseNumber(houseNumber);
                houseNumber = list[0];
                houseNumberAddition = list[1];
            }

            // Test postcode format
            if (!IsValidPostcodeFormat(postcode))
                throw new PostcodeNlApiRestClientInputInvalidException("Postcode '" + postcode + "' needs to be in the 1234AB format.");
            // Test housenumber format
            if (!houseNumber.All(char.IsDigit))
                throw new PostcodeNlApiRestClientInputInvalidException("House number '" + houseNumber + "' must contain digits only.");

            // Create the REST url we want to retrieve. (making sure we escape any user input)
            var url = _restApiUrl + "/addresses/postcode/" + HttpUtility.UrlEncode(postcode) + "/" + HttpUtility.UrlEncode(houseNumber) + "/" + HttpUtility.UrlEncode(houseNumberAddition);
            string response = DoRestCall(url);

            PostcodeNlAddress address;
            try
            {
                address = JsonHelper.Deserialize<PostcodeNlAddress>(response);
            }
            catch (Exception)
            {
                // We received a response, but we did not understand it...
                throw new PostcodeNlApiRestClientClientException("Did not understand Postcode.nl API response: '" + response + "'.");
            }

            if (validateHouseNumberAddition)
            {
                if (address.HouseNumberAddition == null)
                    throw new PostcodeNlApiRestClientInputInvalidException("Housenumber addition '" + houseNumberAddition + "' is not known for this address, valid additions are: '" + string.Join(", ", address.HouseNumberAdditions) + "'.");
            }

            return address;
        }

        /// <summary>
        /// Validate if string has a correct Dutch postcode format.
        /// Syntax: 1234AB, or 1234ab - no space in between.First digit cannot be a zero.
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        public bool IsValidPostcodeFormat(string postcode)
        {
            return Regex.IsMatch(postcode, "[1-9][0-9]{3}[a-zA-Z]{2}");
        }

        /// <summary>
        /// Split a housenumber addition from a housenumber.
        /// Examples: "123 2", "123 rood", "123a", "123a4", "123-a", "123 II"
        /// (the official notation is to separate the housenumber and addition with a single space)
        /// </summary>
        /// <param name="houseNumber"></param>
        /// <returns>Array with houseNumber and houseNumberAddition values</returns>
        public string[] SplitHouseNumber(string houseNumber)
        {
            string houseNumberAddition = string.Empty;
            Match match = Regex.Match(houseNumber, "(?<number>[0-9]+)(?:[^0-9a-zA-Z]+(?<addition1>[0-9a-zA-Z ]+)|(?<addition2>[a-zA-Z](?:[0-9a-zA-Z ]*)))?");
            if (match.Success)
            {
                houseNumber = match.Groups["number"].Value;
                houseNumberAddition = !string.IsNullOrEmpty(match.Groups["addition2"].Value)
                    ? match.Groups["addition2"].Value
                    : !string.IsNullOrEmpty(match.Groups["addition1"].Value)
                        ? match.Groups["addition1"].Value
                        : string.Empty;
            }
            return new[] { houseNumber, houseNumberAddition };
        }

        /// <summary>
        /// Return the last decoded JSON response received, can be used to get more information from exceptions, or debugging.
        /// </summary>
        /// <returns></returns>
        public string GetLastResponseData()
        {
            return _lastResponseData;
        }
    }
}