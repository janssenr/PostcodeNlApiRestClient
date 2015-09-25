# Postcode.nl API REST Client

A ASP.NET 4.5.1+ class, which offers methods to directly talk with the [Postcode.nl API](https://api.postcode.nl/documentation) through the REST endpoint offered.
You will need to create an account with the [Postcode.nl API](https://api.postcode.nl) service.

Implements both the [Address service](https://services.postcode.nl/adres-api/adres-validatie) and the [Signal service](https://services.postcode.nl/adres-api/signaal).

## License

All source code is licensed under the [GNU Lesser General Public License](http://www.gnu.org/licenses/lgpl.html)

## Installation

The easiest way to get started with Postcode.nl API REST Client is to use the NuGet package

	Install-Package PostcodeNlApi

Or download the source from my GitHub page: https://github.com/janssenr/PostcodeNlApiRestClient

## Usage Address API

Include the class in your ASP.Net project, instantiate the ASP.Net class with your authentication details and call the 'lookupAddress' method.
You can handle errors by catching the defined Exception classes.

* See [Address API description](https://services.postcode.nl/adres-api/adres-validatie) for more information
* See [Address API method documentation](https://api.postcode.nl/documentation/address-api) for the possible fields

```
var api = new PostcodeNlApiRestClient(apiKey: "<your key>", apiSecret: "<your secret>");
var result = api.LookupAddress("2012ES", "30");
```

## Usage Signal API

Include the class in your ASP.Net project, instantiate the ASP.Net class with your authentication details and call the 'doSignalCheck' method.
You can handle errors by catching the defined Exception classes.

* See [Signal API description](https://services.postcode.nl/adres-api/signaal) for more information
* See [Signal API check method documentation](https://api.postcode.nl/documentation/signal-api) for the possible fields to pass.
* See [basic example](https://api.postcode.nl/documentation/signal-api-example) for a practical example


```
var api = new PostcodeNlApiRestClient(apiKey: "<your key>", apiSecret: "<your secret>");
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
var result = api.DoSignalCheck(request);
```
