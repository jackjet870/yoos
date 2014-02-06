// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : Valerio Fornito
// Created          : 11-15-2013
//
// Last Modified By : Valerio Fornito
// Last Modified On : 11-15-2013
// ***********************************************************************
// <copyright file="HTTPErrorCollection.cs" company="Autodistribution Italia Spa">
//     Copyright (c) Autodistribution Italia Spa. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace AditOAUTH.Server.HTTPError
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;


    /// <summary>
    /// Enum Type of error
    /// </summary>
    public enum HTTPErrorType
    {
        /// <summary> Invalid request type </summary>
        invalid_request,
        /// <summary> Unauthorized client type </summary>
        unauthorized_client,
        /// <summary> Access denied </summary>
        access_denied,
        /// <summary> Unsupported response type type </summary>
        unsupported_response_type,
        /// <summary> Invalid scope type </summary>
        invalid_scope,
        /// <summary> Server error type </summary>
        server_error,
        /// <summary> Temporarily unavailable type </summary>
        temporarily_unavailable,
        /// <summary> Unsupported grant_type type </summary>
        unsupported_grant_type,
        /// <summary> Invalid client type </summary>
        invalid_client,
        /// <summary> Invalid grant type </summary>
        invalid_grant,
        /// <summary> Invalid grant type </summary>
        invalid_grant_callback,
        /// <summary> Invalid credentials type </summary>
        invalid_credentials,
        /// <summary> Invalid refresh type </summary>
        invalid_refresh,
        /// <summary> Invalid access token type </summary>
        invalid_access_token,
        /// <summary> Missing access token type </summary>
        missing_access_token,
    }

    /// <summary> Defines a class of predefined HTTP errors for the application </summary>
    public class HTTPErrorCollection : CollectionBase
    {
        /// <summary> For singleton, definition of instance </summary>
        private static volatile HTTPErrorCollection instance;

        /// <summary> Object used for thread safety </summary>
        private static readonly object SyncRoot = new object();

        /// <summary> The error list </summary>
        private readonly List<HTTPError> errors;

        /// <summary> Prevents a default instance of the <see cref="HTTPErrorCollection" /> class from being created </summary>
        private HTTPErrorCollection()
        {
            // RFC 6749, section 4.1.2.1.:
            // No 503 status code for 'temporarily_unavailable', because
            // "a 503 Service Unavailable HTTP status code cannot be
            // returned to the client via an HTTP redirect"
            this.errors = new List<HTTPError>
            {
                new HTTPError
                {
                    Type = HTTPErrorType.invalid_request,
                    Message = "The request is missing a required parameter, includes an invalid parameter value, includes a parameter more than once, or is otherwise malformed. Check the \"{0}\" parameter.",
                    HTTPStatusCode = 400
                },
                new HTTPError
                {
                    Type = HTTPErrorType.unauthorized_client,
                    Message = "The client is not authorized to request an access token using this method.",
                    HTTPStatusCode = 400
                },
                new HTTPError
                {
                    Type = HTTPErrorType.access_denied,
                    Message = "The resource owner or authorization server denied the request.",
                    HTTPStatusCode = 401
                },
                new HTTPError
                {
                    Type = HTTPErrorType.unsupported_response_type,
                    Message = "The authorization server does not support obtaining an access token using this method.",
                    HTTPStatusCode = 400
                },
                new HTTPError
                {
                    Type = HTTPErrorType.invalid_scope,
                    Message = "The requested scope is invalid, unknown, or malformed. Check the \"{0}\" scope.",
                    HTTPStatusCode = 400
                },
                new HTTPError
                {
                    Type = HTTPErrorType.server_error,
                    Message = "The authorization server encountered an unexpected condition which prevented it from fulfilling the request.",
                    HTTPStatusCode = 500
                },
                new HTTPError
                {
                    Type = HTTPErrorType.temporarily_unavailable,
                    Message = "The authorization server is currently unable to handle the request due to a temporary overloading or maintenance of the server.",
                    HTTPStatusCode = 400
                },
                new HTTPError
                {
                    Type = HTTPErrorType.unsupported_grant_type,
                    Message = "The authorization grant type \"{0}\" is not supported by the authorization server",
                    HTTPStatusCode = 501
                },
                new HTTPError
                {
                    Type = HTTPErrorType.invalid_client,
                    Message = "Client authentication failed",
                    HTTPStatusCode = 400
                },
                new HTTPError
                {
                    Type = HTTPErrorType.invalid_grant,
                    Message = "The provided authorization grant is invalid, expired, revoked, does not match the redirection URI used in the authorization request, or was issued to another client. Check the \"{0}\" parameter.",
                    HTTPStatusCode = 400
                },
                new HTTPError
                {
                    Type = HTTPErrorType.invalid_grant_callback,
                    Message = "The provided authorization grant has null or non-callable callback set",
                    HTTPStatusCode = 400
                },
                new HTTPError
                {
                    Type = HTTPErrorType.invalid_credentials,
                    Message = "The user credentials were incorrect.",
                    HTTPStatusCode = 400
                },
                new HTTPError
                {
                    Type = HTTPErrorType.invalid_refresh,
                    Message = "The refresh token is invalid.",
                    HTTPStatusCode = 400
                },
                new HTTPError
                {
                    Type = HTTPErrorType.invalid_access_token,
                    Message = "The access token is invalid.",
                    HTTPStatusCode = 400
                },
                 new HTTPError
                {
                    Type = HTTPErrorType.missing_access_token,
                    Message = "The access token is missing.",
                    HTTPStatusCode = 400
                }
            };
        }

        /// <summary> Gets the instance of HTTPErrorCollection returned by the singleton </summary>
        /// <value>The instance of <see cref="HTTPErrorCollection" /></value>
        public static HTTPErrorCollection Instance
        {
            get
            {
                if (instance != null) return instance;
                lock (SyncRoot)
                {
                    if (instance == null)
                        instance = new HTTPErrorCollection();
                }

                return instance;
            }
        }

        /// <summary> Gets the <see cref="HTTPError" /> with the specified type. </summary>
        /// <param name="type">The type to search</param>
        /// <returns>HTTPError based on type </returns>
        public HTTPError this[HTTPErrorType type]
        {
            get { return this.errors.Single(e => e.Type.Equals(type)); }
        }

        /// <summary> Gets the <see cref="HTTPError" /> with the specified HTTP status code. </summary>
        /// <param name="httpStatusCode">The HTTP status code to search</param>
        /// <returns>HTTPError based on type</returns>
        public HTTPError this[int httpStatusCode]
        {
            get { return this.errors.Single(e => e.HTTPStatusCode.Equals(httpStatusCode)); }
        }

        /// <summary> Define HTTPError object </summary>
        public class HTTPError
        {
            /// <summary> Gets or sets the type. </summary>
            /// <value>The type.</value>
            public HTTPErrorType Type { get; set; }

            /// <summary> Gets or sets the message. </summary>
            /// <value>The message.</value>
            public string Message { get; set; }

            /// <summary> Gets or sets the HTTP status code. </summary>
            /// <value>The HTTP status code.</value>
            public int HTTPStatusCode { get; set; }
        }
    }
}