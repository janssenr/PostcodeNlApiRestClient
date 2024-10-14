using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using PostcodeNlApi.Address;
using PostcodeNlApi.Exceptions;
using PostcodeNlApi.Helpers;
using PostcodeNlApi.Models.Validate;

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
        protected async Task<string> DoRestCallAsync(string url)
        {
            string responseHeaders = string.Empty;
            string responseValue = string.Empty;
            HttpStatusCode responseStatusCode = HttpStatusCode.OK;

            using (var client = new HttpClient())
            {
                // Set up basic authentication headers
                var byteArray = Encoding.UTF8.GetBytes($"{_appKey}:{_appSecret}");
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                try
                {
                    // Make the GET request
                    var response = await client.GetAsync(url).ConfigureAwait(false);
                    responseHeaders = response.Headers.ToString();
                    responseStatusCode = response.StatusCode;

                    responseValue = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        var exception = JsonHelper.Deserialize<PostcodeNlException>(responseValue);
                        switch (responseStatusCode)
                        {
                            case HttpStatusCode.NotFound: // 404
                                if (exception.ExceptionId == "PostcodeNl_Service_PostcodeAddress_AddressNotFoundException")
                                    throw new PostcodeNlApiRestClientAddressNotFoundException(exception.Exception);
                                break;
                            case HttpStatusCode.Unauthorized: // 401
                            case HttpStatusCode.Forbidden: // 403
                                throw new PostcodeNlApiRestClientAuthenticationException(exception.Exception);
                            case HttpStatusCode.BadRequest: // 400
                                throw new PostcodeNlApiRestClientInputInvalidException(exception.Exception);
                            case HttpStatusCode.InternalServerError: // 500
                                throw new PostcodeNlApiRestClientServiceException(exception.Exception);
                            default:
                                throw new PostcodeNlApiRestClientServiceException(exception.Exception);
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Handle request-related exceptions
                    throw new Exception($"Request error: {ex.Message}");
                }
                finally
                {
                    if (_debugEnabled)
                    {
                        _debugData = new Dictionary<string, string>
                        {
                            {"request", client.DefaultRequestHeaders.ToString()},
                            {"response", responseHeaders + responseValue}
                        };
                    }
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
        public async Task<Address.Address> LookupAddress(string postcode, string houseNumber, string houseNumberAddition = "", bool validateHouseNumberAddition = false)
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
            string response = await DoRestCallAsync(url);

            Address.Address address;
            try
            {
                address = JsonHelper.Deserialize<Address.Address>(response);
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
            return Regex.IsMatch(postcode, "^[1-9][0-9]{3}[a-zA-Z]{2}$");
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
            System.Text.RegularExpressions.Match match = Regex.Match(houseNumber, "(?<number>[0-9]+)(?:[^0-9a-zA-Z]+(?<addition1>[0-9a-zA-Z ]+)|(?<addition2>[a-zA-Z](?:[0-9a-zA-Z ]*)))?");
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

        public async Task<ValidateResponse> Validate(string country, string postcode = null, string locality = null, string street = null, string building = null, string region = null, string streetAndBuilding = null)
        {
            var parameters = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(postcode))
                parameters.Add("postcode", postcode);
            if (!string.IsNullOrWhiteSpace(locality))
                parameters.Add("locality", locality);
            if (!string.IsNullOrWhiteSpace(street))
                parameters.Add("street", street);
            if (!string.IsNullOrWhiteSpace(building))
                parameters.Add("building", building);
            if (!string.IsNullOrWhiteSpace(region))
                parameters.Add("region", region);
            if (!string.IsNullOrWhiteSpace(streetAndBuilding))
                parameters.Add("streetAndBuilding", streetAndBuilding);

            var url = GetUrl("https://api.postcode.eu/international/v1/validate/" + HttpUtility.UrlEncode(country), parameters);
            string response = await DoRestCallAsync(url.ToString());

            ValidateResponse validateReponse;
            try
            {
                validateReponse = JsonHelper.Deserialize<ValidateResponse>(response);
            }
            catch (Exception)
            {
                // We received a response, but we did not understand it...
                throw new PostcodeNlApiRestClientClientException("Did not understand Postcode.nl API response: '" + response + "'.");
            }

            return validateReponse;
        }

        private Uri GetUrl(string url, Dictionary<string, string> parameters = null)
        {
            if (parameters != null && parameters.Any())
            {
                var queryParameters = string.Join("&",
                    parameters.Select(
                        p =>
                            string.IsNullOrEmpty(p.Value)
                                ? Uri.EscapeDataString(p.Key)
                                : $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
                if (!string.IsNullOrWhiteSpace(queryParameters))
                {
                    url += "?" + queryParameters;
                }
            }
            return new Uri(url);
        }

    }
}