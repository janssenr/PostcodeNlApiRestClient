Postcode.nl API REST Client
=============

A ASP.NET 4.5.1+ class, which offers methods to directly talk with the [Postcode.nl API](https://api.postcode.nl/documentation) through the REST endpoint offered.
You will need to create an account with the [Postcode.nl API](https://api.postcode.nl) service.

Implements both the [Address service](https://api.postcode.nl/documentation/address-api-description) and the [Signal service](https://api.postcode.nl/documentation/signal-api-description).

License
=============

All source code is licensed under the [GNU Lesser General Public License](http://www.gnu.org/licenses/lgpl.html)

Installation
=============

The best way to install is by using [PHP Composer](https://getcomposer.org/), get package [`postcode-nl/api-restclient`](https://packagist.org/packages/postcode-nl/api-restclient) and stay up to date easily.

Or download the source from my GitHub page: https://github.com/janssenr/PostcodeNlApiRestClient

Usage Address API
=============

Include the class in your ASP.Net project, instantiate the ASP.Net class with your authentication details and call the 'lookupAddress' method.
You can handle errors by catching the defined Exception classes.

* See [Address API description](https://api.postcode.nl/documentation/address-api-description) for more information
* See [Address API method documentation](https://api.postcode.nl/documentation/rest-json-endpoint#address-api) for the possible fields

Usage Signal API
=============

Include the class in your ASP.Net project, instantiate the ASP.Net class with your authentication details and call the 'doSignalCheck' method.
You can handle errors by catching the defined Exception classes.

* See [Signal API description](https://api.postcode.nl/documentation/signal-api-description) for more information
* See [Signal API check method documentation](https://api.postcode.nl/documentation/rest-json-endpoint#signal-api) for the possible fields to pass.
* See [basic example](https://api.postcode.nl/documentation/signal-api-example) for a practical example
