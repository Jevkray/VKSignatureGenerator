Embedded signature generator for VKontakte messenger. 
This code is written in C# and consists of two classes: ApiRequestParams and BaseRequest. The ApiRequestParams class extends SortedDictionary<string, string> and is used to store query parameters. It has a constructor that takes a string value and splits it into key-value pairs to populate the dictionary. It also has a GetRequestLength method that returns the total length of the request parameters. The Add method is redefined to calculate the length of the query and add a key-value pair to the dictionary.

The BaseRequest class is an abstract class that provides basic functionality for executing API requests. The static defaultEncoding field is set to UTF-8 encoding and a static ApplicationKey field containing a specific application key. It also has a method field representing the API method being called.

The BaseRequest class has a BuildRequestUrl method that creates a request URL based on the API method, host, path, and request parameters. It uses the GetMethodParams method to get specific method parameters, and then iterates over them to add them to the URL. It also calculates the signature of the request using the GetSignature method.

The GetSignature method takes an ApiRequestParams object and generates a signature based on the method, sorted request parameters, and the application key. It uses a SortedSet<string> to sort the parameters and a StringBuilder to combine them. Then it calculates the MD5 hash of the combined string and encodes it using URL encoding.

The BaseRequest class also has abstract methods GetMethodParams, GetApiHost and GetApiPath, which must be implemented by subclasses. These methods define specific method parameters, API node, and API path for each request.

The code also includes a subclass, PushRequest, which extends BaseRequest and presents a specific API request for push notifications. It overrides the GetMethodParams, GetApiHost, and GetApiPath methods to provide specific parameters, host, and path for the push request.
