namespace E7.EasyResult.Errors;

/// <summary>
/// Enumerates the standard categories of application errors used in conjunction with the result pattern.
/// These categories help classify failures for clearer communication, diagnostics, and HTTP response mapping.
/// </summary>
/// <remarks>
/// Each error type is assigned a specific code for traceability and consistency across the system,
/// particularly useful in result-based flows and domain-to-HTTP translation layers.
/// </remarks>
/// <author>Silvio Luiz Bassi</author>
/// <company>Enfatiza7 Consultoria em Tecnologia LTDA</company>
public enum HttpErrorType
{
    /// <summary>
    /// Represents an error caused by an invalid or malformed request.
    /// Typically corresponds to HTTP 400 (Bad Request).
    /// </summary>
    BadRequest = 400,

    /// <summary>
    /// Represents an error where the user is not authenticated.
    /// Corresponds to HTTP 401 (Unauthorized).
    /// </summary>
    Unauthorized = 401,

    /// <summary>
    /// Represents an error where payment is required to proceed.
    /// Corresponds to HTTP 402 (Payment Required).
    /// </summary>
    PaymentRequired = 402,

    /// <summary>
    /// Represents an error where the user is authenticated but does not have the necessary permissions to perform the action.
    /// Corresponds to HTTP 403 (Forbidden).
    /// </summary>
    Forbidden = 403,

    /// <summary>
    /// Represents an error where a requested resource could not be found.
    /// Corresponds to HTTP 404 (Not Found).
    /// </summary>
    NotFound = 404,

    /// <summary>
    /// Represents an error where the HTTP method used is not supported for the requested resource.
    /// Corresponds to HTTP 405 (Method Not Allowed).
    /// </summary>
    MethodNotAllowed = 405,

    /// <summary>
    /// Represents an error where the requested resource is capable of generating only content not acceptable according to the Accept headers.
    /// Corresponds to HTTP 406 (Not Acceptable).
    /// </summary>
    NotAcceptable = 406,

    /// <summary>
    /// Represents an error where client authentication must be done by a proxy.
    /// Corresponds to HTTP 407 (Proxy Authentication Required).
    /// </summary>
    ProxyAuthenticationRequired = 407,

    /// <summary>
    /// Represents an error where the server timed out waiting for the request.
    /// Corresponds to HTTP 408 (Request Timeout).
    /// </summary>
    RequestTimeout = 408,

    /// <summary>
    /// Represents a conflict in the application's state, usually due to concurrency or data integrity violations.
    /// Typically maps to HTTP 409 (Conflict).
    /// </summary>
    Conflict = 409,

    /// <summary>
    /// Represents an error where the requested resource is no longer available and will not be available again.
    /// Corresponds to HTTP 410 (Gone).
    /// </summary>
    Gone = 410,

    /// <summary>
    /// Represents an error where the request did not specify the length of its content, which is required.
    /// Corresponds to HTTP 411 (Length Required).
    /// </summary>
    LengthRequired = 411,

    /// <summary>
    /// Represents an error where the server does not meet one of the preconditions specified in the request headers.
    /// Corresponds to HTTP 412 (Precondition Failed).
    /// </summary>
    PreconditionFailed = 412,

    /// <summary>
    /// Represents an error where the request payload is larger than the server is willing or able to process.
    /// Corresponds to HTTP 413 (Payload Too Large).
    /// </summary>
    PayloadTooLarge = 413,

    /// <summary>
    /// Represents an error where the URI provided was too long for the server to process.
    /// Corresponds to HTTP 414 (URI Too Long).
    /// </summary>
    UriTooLong = 414,

    /// <summary>
    /// Represents an error where the request entity has a media type which the server or resource does not support.
    /// Corresponds to HTTP 415 (Unsupported Media Type).
    /// </summary>
    UnsupportedMediaType = 415,

    /// <summary>
    /// Represents an error where the client has asked for a portion of the file, but the server cannot supply that portion.
    /// Corresponds to HTTP 416 (Range Not Satisfiable).
    /// </summary>
    RangeNotSatisfiable = 416,

    /// <summary>
    /// Represents an error where the server cannot meet the requirements of the Expect request-header field.
    /// Corresponds to HTTP 417 (Expectation Failed).
    /// </summary>
    ExpectationFailed = 417,

    /// <summary>
    /// Represents an error where the server understands the content type of the request entity, but was unable to process the contained instructions.
    /// Corresponds to HTTP 422 (Unprocessable Entity).
    /// </summary>
    UnprocessableEntity = 422,

    /// <summary>
    /// Represents an error where the resource that is being accessed is locked.
    /// Corresponds to HTTP 423 (Locked).
    /// </summary>
    Locked = 423,

    /// <summary>
    /// Represents an error where the request failed due to failure of a previous request.
    /// Corresponds to HTTP 424 (Failed Dependency).
    /// </summary>
    FailedDependency = 424,

    /// <summary>
    /// Represents an error where the client should switch to a different protocol.
    /// Corresponds to HTTP 426 (Upgrade Required).
    /// </summary>
    UpgradeRequired = 426,

    /// <summary>
    /// Represents an error where the origin server requires the request to be conditional.
    /// Corresponds to HTTP 428 (Precondition Required).
    /// </summary>
    PreconditionRequired = 428,

    /// <summary>
    /// Represents an error where the user has sent too many requests in a given amount of time.
    /// Corresponds to HTTP 429 (Too Many Requests).
    /// </summary>
    TooManyRequests = 429,

    /// <summary>
    /// Represents an error where the server is unwilling to process the request because its header fields are too large.
    /// Corresponds to HTTP 431 (Request Header Fields Too Large).
    /// </summary>
    RequestHeaderFieldsTooLarge = 431,

    /// <summary>
    /// Represents an error where a server operator has received a legal demand to deny access to a resource.
    /// Corresponds to HTTP 451 (Unavailable For Legal Reasons).
    /// </summary>
    UnavailableForLegalReasons = 451,

    /// <summary>
    /// Represents a generic error on the server side that prevented the request from being fulfilled.
    /// Corresponds to HTTP 500 (Internal Server Error).
    /// </summary>
    InternalServerError = 500,

    /// <summary>
    /// Represents an error where the server either does not recognize the request method, or it lacks the ability to fulfill the request.
    /// Corresponds to HTTP 501 (Not Implemented).
    /// </summary>
    NotImplemented = 501,

    /// <summary>
    /// Represents an error where the server was acting as a gateway or proxy and received an invalid response from the upstream server.
    /// Corresponds to HTTP 502 (Bad Gateway).
    /// </summary>
    BadGateway = 502,

    /// <summary>
    /// Represents an error where the server is currently unavailable (because it is overloaded or down for maintenance).
    /// Corresponds to HTTP 503 (Service Unavailable).
    /// </summary>
    ServiceUnavailable = 503,

    /// <summary>
    /// Represents an error where the server was acting as a gateway or proxy and did not receive a timely response from the upstream server.
    /// Corresponds to HTTP 504 (Gateway Timeout).
    /// </summary>
    GatewayTimeout = 504,

    /// <summary>
    /// Represents an error where the server does not support the HTTP protocol version used in the request.
    /// Corresponds to HTTP 505 (HTTP Version Not Supported).
    /// </summary>
    HttpVersionNotSupported = 505,

    /// <summary>
    /// Represents an error where the server has an internal configuration error resulting in transparent content negotiation failure.
    /// Corresponds to HTTP 506 (Variant Also Negotiates).
    /// </summary>
    VariantAlsoNegotiates = 506,

    /// <summary>
    /// Represents an error where the method could not be performed on the resource because the server is unable to store the representation needed to successfully complete the request.
    /// Corresponds to HTTP 507 (Insufficient Storage).
    /// </summary>
    InsufficientStorage = 507,

    /// <summary>
    /// Represents an error where the server detected an infinite loop while processing the request.
    /// Corresponds to HTTP 508 (Loop Detected).
    /// </summary>
    LoopDetected = 508,

    /// <summary>
    /// Represents an error where further extensions to the request are required for the server to fulfill it.
    /// Corresponds to HTTP 510 (Not Extended).
    /// </summary>
    NotExtended = 510,

    /// <summary>
    /// Represents an error where the client needs to authenticate to gain network access.
    /// Corresponds to HTTP 511 (Network Authentication Required).
    /// </summary>
    NetworkAuthenticationRequired = 511,

    /// <summary>
    /// Represents a violation of business rules or domain logic.
    /// Used to indicate that an operation, although syntactically correct, is not allowed.
    /// </summary>
    Business = 1000,

    /// <summary>
    /// Represents a data validation failure, such as format errors or missing required values.
    /// Typically used for client-side input validation.
    /// </summary>
    Validation = 1001,

    /// <summary>
    /// Represents a failure during the creation of a new object or resource.
    /// Useful for capturing construction or instantiation issues at runtime.
    /// </summary>
    FailToCreateObject = 1002
}