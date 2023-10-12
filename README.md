# Postcode.nl API REST Client

A ASP.Net 4.5.1+ class, which offers methods to directly talk with the [Postcode.nl API](https://developer.postcode.eu/documentation) through the REST endpoint offered.
You will need to create an account with the [Postcode.nl API](https://www.postcode.nl/services/adresdata/api) service.


## License

All source code is licensed under the [GNU Lesser General Public License](http://www.gnu.org/licenses/lgpl.html)

## Installation

The easiest way to get started with Postcode.nl API REST Client is to use the NuGet package

	Install-Package PostcodeNlApi

Or download the source from my GitHub page: https://github.com/janssenr/PostcodeNlApiRestClient

## Usage Address API

Include the class in your ASP.Net project, instantiate the ASP.Net class with your authentication details and call the 'LookupAddress' method.
You can handle errors by catching the defined Exception classes.

* See our [Address API description](https://www.postcode.nl/services/adresdata/api) for more information
* See our [Address API method documentation](https://developer.postcode.eu/documentation/nl/overview) for the possible fields

```
var api = new PostcodeNlApiRestClient(apiKey: "<your key>", apiSecret: "<your secret>");
var result = api.LookupAddress("2012ES", "30");
```
