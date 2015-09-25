using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using PostcodeNlApi.Address;
using PostcodeNlApi.Exceptions;
using PostcodeNlApi.Helpers;
using PostcodeNlApi.Signal;

namespace PostcodeNlApi
{ /** Common superclass for Exceptions raised by this class. */
    public class PostcodeNlApiRestClientException : Exception
    {
        public PostcodeNlApiRestClientException(string message) : base(message) { }
    }

    /** Exception raised when user input is invalid. */
    public class PostcodeNlApiRestClientInputInvalidException : PostcodeNlApiRestClientException
    {
        public PostcodeNlApiRestClientInputInvalidException(string message) : base(message) { }
    }

    /** Exception raised when user input is valid, but no address could be found. */
    public class PostcodeNlApiRestClientAddressNotFoundException : PostcodeNlApiRestClientException
    {
        public PostcodeNlApiRestClientAddressNotFoundException(string message) : base(message) { }
    }

    /** Exception raised when an unexpected error occurred in this client. */
    public class PostcodeNlApiRestClientClientException : PostcodeNlApiRestClientException
    {
        public PostcodeNlApiRestClientClientException(string message) : base(message) { }
    }

    /** Exception raised when an unexpected error occurred on the remote service. */
    public class PostcodeNlApiRestClientServiceException : PostcodeNlApiRestClientException
    {
        public PostcodeNlApiRestClientServiceException(string message) : base(message) { }
    }

    /** Exception raised when there is a authentication problem.
        In a production environment, you probably always want to catch, log and hide these exceptions.
*/
    public class PostcodeNlApiRestClientAuthenticationException : PostcodeNlApiRestClientException
    {
        public PostcodeNlApiRestClientAuthenticationException(string message) : base(message) { }
    }

    /**
        Class to connect to the Postcode.nl API web service.


        References:
            <https://api.postcode.nl/>
*/
    public class PostcodeNlApiRestClient
    {
        ///** (string) Version of the client */
        //private const string VERSION = "1.0.0.0";
        ///** (int) Maximum number of miliseconds allowed to set up the connection. */
        //private const int CONNECTTIMEOUT = 3000;
        ///** (int) Maximum number of seconds allowed to receive the response. */
        //private const int TIMEOUT = 10;
        /** (string) URL where the REST web service is located */
        private readonly string _restApiUrl = "https://api.postcode.nl/rest";
        /** (string) Internal storage of the application key of the authentication. */
        private readonly string _appKey;
        /** (string) Internal storage of the application secret of the authentication. */
        private readonly string _appSecret;
        /** (boolean) If debug data is stored. */
        private bool _debugEnabled;
        /** (mixed) Debug data storage. */
        private Dictionary<string, string> _debugData;

        public PostcodeNlApiRestClient(string appKey, string appSecret, string restApiUrl = null)
        {
            _appKey = appKey;
            _appSecret = appSecret;
            if (!string.IsNullOrEmpty(restApiUrl))
                _restApiUrl = restApiUrl;

            if (string.IsNullOrEmpty(_appKey) || string.IsNullOrEmpty(_appSecret))
                throw new PostcodeNlApiRestClientException("No application key / secret configured, you can obtain these at https://api.postcode.nl.");
        }

        /**
    Toggle debug option.

    Parameters:
    debugEnabled - (boolean) what to set the option to
    */
        public void SetDebugEnabled(bool debugEnabled = true)
        {
            _debugEnabled = debugEnabled;
            if (!_debugEnabled)
                _debugData = null;
        }


        /**
    Get the debug data gathered so far.

    Returns:
    (mixed) Debug data
    */
        public Dictionary<string, string> GetDebugData()
        {
            return _debugData;
        }

        /**
		Perform a REST call to the Postcode.nl API
	*/
        protected string DoRestCall(string method, string url, string data = null)
        {
            var req = WebRequest.Create(url);
            // How do we authenticate ourselves? Using HTTP BASIC authentication (http://en.wikipedia.org/wiki/Basic_access_authentication)
            req.Credentials = new NetworkCredential(_appKey, _appSecret);
            req.Method = method;
            if (method.ToUpper() != "GET" && data != null)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(data);

                req.ContentType = "application/json";
                req.ContentLength = buffer.Length;

                Stream stream = req.GetRequestStream();
                stream.Write(buffer, 0, buffer.Length);
                stream.Close();
            }

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
                    _debugData = new Dictionary<string, string> { { "request", req.Headers.ToString() } };
                    if (method.ToUpper() != "GET" && data != null)
                    {
                        _debugData["request"] = data;
                    }
                    _debugData.Add("response", responseHeaders + responseValue);
                }
            }
            return responseValue;
        }

        /**
    Look up an address by postcode and house number.

    Parameters:
    postcode - (string) Dutch postcode in the '1234AB' format
    houseNumber - (string) House number (may contain house number addition, will be separated automatically)
    houseNumberAddition - (string) House number addition
    validateHouseNumberAddition - (boolean) Strictly validate the addition

    Returns:
    (array) The address found
    street - (string) Official name of the street.
    houseNumber - (int) House number
    houseNumberAddition - (string|null) House number addition if given and validated, null if addition is not valid / not found
    postcode - (string) Postcode
    city - (string) Official city name
    municipality - (string) Official municipality name
    province - (string) Official province name
    rdX - (int) X coordinate of the Dutch Rijksdriehoeksmeting
    rdY - (int) Y coordinate of the Dutch Rijksdriehoeksmeting
    latitude - (float) Latitude of the address (front door of the premise)
    longitude - (float) Longitude of the address
    bagNumberDesignationId - (string) Official Dutch BAG id
    bagAddressableObjectId - (string) Official Dutch BAG Address Object id
    addressType - (string) Type of address, see reference link
    purposes - (array) Array of strings, each indicating an official Dutch 'usage' category, see reference link
    surfaceArea - (int) Surface area of object in square meters (all floors)
    houseNumberAdditions - (array) All housenumber additions which are known for the housenumber given.

    Reference:
    <https://api.postcode.nl/Documentation/api>
    */
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
            //if (!IsValidPostcodeFormat(postcode))
            //    throw new PostcodeNlApiRestClientInputInvalidException("Postcode '" + postcode + "' needs to be in the 1234AB format.");
            //// Test housenumber format
            //if (!houseNumber.All(char.IsDigit))
            //    throw new PostcodeNlApiRestClientInputInvalidException("House number '" + houseNumber + "' must contain digits only.");

            // Create the REST url we want to retrieve. (making sure we escape any user input)
            var url = _restApiUrl + "/addresses/" + HttpUtility.UrlEncode(postcode) + "/" + HttpUtility.UrlEncode(houseNumber) + "/" + HttpUtility.UrlEncode(houseNumberAddition);
            string response = DoRestCall("GET", url);

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

        /**
		Perform a Postcode.nl Signal check on the given transaction and/or customer information.

		Parameters:
			customer - (array) Data concerning a customer
			access - (array) Data concerning how a customer is accessing a service
			transaction - (array) Data concerning a transaction of a customer
			config - (array) Configuration for the signal check

		Returns:
			(array) signals
				checkId - (string) Identifier of the check (22 characters)
				signals - (array) All signals, each signal is an array, containing:
					concerning - (string) Field this signal is about
					type - (string) Name of signal type, including vendor, service name and response type
					warning - (boolean) If this signal is significant.
					message - (string) Human readable explanation for the signal
					data - (array|null) Optional data of the signal. See documentation on website for data definitions of the specific signal types.
				warningCount - (int) Number of warnings found in signals
				reportPdfUrl - (string) URL to a report of the signal check

		Reference:
			<https://api.postcode.nl/documentation>
	*/

        public PostcodeNlSignalResponse DoSignalCheck(PostcodeNlSignalRequest signalRequest)
        {
            string url = _restApiUrl + "/signal/check";
            string data = JsonHelper.Serialize(signalRequest);
            string response = DoRestCall("POST", url, data);
            PostcodeNlSignalResponse signalResponse;
            try
            {
                signalResponse = JsonHelper.Deserialize<PostcodeNlSignalResponse>(response);
            }
            catch (Exception)
            {
                // We received a response, but we did not understand it...
                throw new PostcodeNlApiRestClientClientException("Did not understand Postcode.nl API response: '" + response + "'.");
            }
            return signalResponse;
        }

        /**
    Validate a postcode string for correct format.
    (is 1234AB, or 1234ab - no space in between!)

    Parameters:
    postcode - (string) Postcode input

    Returns
    (boolean) if the postcode format is correct
    */
        public bool IsValidPostcodeFormat(string postcode)
        {
            return Regex.IsMatch(postcode, "[0-9]{4}[a-zA-Z]{2}");
        }

        /**
    Split a housenumber addition from a housenumber.
    Examples: "123 2", "123 rood", "123a", "123a4", "123-a", "123 II"
    (the official notation is to separate the housenumber and addition with a single space)

    Parameters:
    houseNumber - (string) Housenumber input

    Returns
    (array) split 'houseNumber' and 'houseNumberAddition'
    */
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
    }
}