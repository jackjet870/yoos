// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : Valerio Fornito
// Created          : 11-05-2013
//
// Last Modified By : Valerio Fornito
// Last Modified On : 11-15-2013
// ***********************************************************************
// <copyright file="Authorization.cs" company="Autodistribution Italia Spa">
//     Copyright (c) Autodistribution Italia Spa. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace AditOAUTH.Server
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using AditOAUTH.Server.Exception;
    using AditOAUTH.Server.Grant;
    using AditOAUTH.Server.HTTPError;
    using AditOAUTH.Server.Storage;
    using AditOAUTH.Server.Storage.PDO;
    using AditOAUTH.Server.Util;

    /// <summary> Definition for Authorization </summary>
    public class Authorization
    {
        /// <summary> The registered grant types </summary>
        private readonly Dictionary<string, GrantType> grantTypes = new Dictionary<string, GrantType>();

        /// <summary> Gets the Client PDO Class </summary>
        /// <value>The client.</value>
        internal IClient Client { get; private set; }

        /// <summary> Gets the Session PDO Class </summary>
        /// <value>The session.</value>
        internal ISession Session { get; private set; }

        /// <summary> Gets the Scope PDO Class </summary>
        /// <value>The scope.</value>
        internal IScope Scope { get; private set; }

        /// <summary> The TTL (time to live) of an access token in seconds (default: 3600) </summary>
        private int accessTokenTTL = 3600;

        /// <summary> Default scope(s) to be used if none is provided </summary>
        private string defaultScope;

        /// <summary> The request object </summary>
        private IRequest request;

        /// <summary>
        /// The delimeter between scopes specified in the scope query string parameter
        /// The OAuth 2 specification states it should be a space but most use a comma
        /// </summary>
        private char scopeDelimeter = ' ';

        /// <summary> Initializes a new instance of the <see cref="Authorization" /> class </summary>
        /// <param name="client"> A class which inherits from Storage/IClient</param>
        /// <param name="session"> A class which inherits from Storage/ISession</param>
        /// <param name="scope"> A class which inherits from Storage/IScope</param>
        /// <param name="connectionString"> Database connection string </param>
        public Authorization(IClient client, ISession session, IScope scope, string connectionString)
        {
            this.Client = client;
            this.Session = session;
            this.Scope = scope;
            Db.ConnectionString = connectionString;
        }

        /// <summary> Gets or sets the scope delimeter </summary>
        /// <value>The scope delimeter</value>
        public char ScopeDelimeter
        {
            get { return this.scopeDelimeter; }
            set { this.scopeDelimeter = char.IsSeparator(value) ? ' ' : value; }
        }

        /// <summary> Gets or sets the access token TTL </summary>
        /// <value>The access token TTL</value>
        public int AccessTokenTTL
        {
            get { return this.accessTokenTTL; }
            set { this.accessTokenTTL = value; }
        }

        /// <summary> Gets the response types </summary>
        /// <value>The response types</value>
        public string ResponseTypes { get; private set; }

        /// <summary> Gets or sets a value indicating whether the "scope" parameter to be in checkAuthoriseParams() </summary>
        public bool RequireScopeParam { get; set; }

        /// <summary> Gets or sets the default scope </summary>
        /// <value>The default scope</value>
        public string DefaultScope
        {
            get { return this.defaultScope; }
            set { this.defaultScope = string.IsNullOrEmpty(value) ? null : value; }
        }

        /// <summary> Gets or sets a value indicating whether the require "state" parameter to be in checkAuthoriseParams() </summary>
        /// <value><c>true</c> if [require state parameter]; otherwise, <c>false</c></value>
        public bool RequireStateParam { get; set; }

        /// <summary> Gets or sets the request </summary>
        /// <value>The request</value>
        public IRequest Request
        {
            get { return this.request ?? (this.request = Util.Request.BuildFromGlobals()); }
            set { this.request = value; }
        }

        /// <summary>Gets all headers that have to be send with the error response </summary>
        /// <param name="error">The error message key</param>
        /// <returns>List{System.String} with header values</returns>
        public static List<string> GetExceptionHttpHeaders(string error)
        {
            var headers = new List<string>();
            switch (HTTPErrorCollection.Instance[error].HTTPStatusCode)
            {
                case 401:
                    headers.Add("HTTP/1.1 401 Unauthorized");
                    break;
                case 500:
                    headers.Add("HTTP/1.1 500 Internal Server Error");
                    break;
                case 501:
                    headers.Add("HTTP/1.1 501 Not Implemented");
                    break;
                default:
                    headers.Add("HTTP/1.1 400 Bad Request");
                    break;
            }

            // Add "WWW-Authenticate" header
            //
            // RFC 6749, section 5.2.:
            // "If the client attempted to authenticate via the 'Authorization'
            // request header field, the authorization server MUST
            // respond with an HTTP 401 (Unauthorized) status code and
            // include the "WWW-Authenticate" response header field
            // matching the authentication scheme used by the client.
            if (error == "invalid_client")
            {
                string authScheme = null;
                var request = new Request();
                if (request.Server("AUTH_USER") != null)
                    authScheme = "Basic";
                else
                {
                    var authHeader = request.Header("Authorization");
                    if (authHeader != null)
                    {
                        if (authHeader.IndexOf("Bearer", StringComparison.Ordinal) == 0)
                            authScheme = "Bearer";
                        else if (authHeader.IndexOf("Basic", StringComparison.Ordinal) == 0)
                            authScheme = "Basic";
                    }
                }

                if (authScheme != null)
                    headers.Add(string.Format("WWW-Authenticate: {0} realm=\"\"", authScheme));
            }

            return headers;
        }

        /// <summary> Enable support for a grant </summary>
        /// <param name="grantType">A grant class which conforms to Grant/IGrantType</param>
        /// <param name="identifier">An identifier for the grant (autodetected if not passed)</param>
        public void AddGrantType(GrantType grantType, string identifier = null)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                identifier = grantType.Identifier;
            }

            // Inject server into grant
            grantType.AuthServer = this;

            this.grantTypes[identifier] = grantType;

            if (!string.IsNullOrEmpty(grantType.ResponseType))
            {
                this.ResponseTypes = grantType.ResponseType;
            }
        }

        /// <summary> Check if a grant type has been enabled </summary>
        /// <param name="identifier">The grant type identifier</param>
        /// <returns>Returns "true" if enabled, "false" if not.</returns>
        public bool HasGrantType(string identifier)
        {
            return this.grantTypes.ContainsKey(identifier);
        }

        /// <summary> Issues the access token </summary>
        /// <param name="inputParams">The input parameters</param>
        /// <returns>FlowResult with the access token</returns>
        /// <exception cref="ClientException"> Various exceptions </exception>
        public FlowResult IssueAccessToken(Parameters inputParams = null)
        {
            var grantType = this.GetParam("grant_type", HTTPMethod.Post, inputParams).ToString();

            if (string.IsNullOrEmpty(grantType))
                throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_request"].Message, "grant_type"));

            // Ensure grant type is one that is recognised and is enabled
            if (!this.grantTypes.ContainsKey(grantType))
                throw new ClientException(string.Format(HTTPErrorCollection.Instance["unsupported_grant_type"].Message, grantType));

            // Complete the flow
            return this.GetGrantType(grantType).CompleteFlow(inputParams);
        }

        /// <summary> Return a grant type class </summary>
        /// <param name="grantType">The grant type identifer</param>
        /// <returns>Grant\AuthCode or Grant\ClientCredentials or Grant\Implict or Grant\Password or Grant\RefreshToken</returns>
        /// <exception cref="InvalidGrantTypeException">Thrown if grant type is invalid</exception>
        public GrantType GetGrantType(string grantType)
        {
            if (this.grantTypes[grantType] != null)
            {
                return this.grantTypes[grantType];
            }

            throw new InvalidGrantTypeException(string.Format(HTTPErrorCollection.Instance["unsupported_grant_type"].Message, grantType));
        }

        /// <summary> Get a parameter from passed input parameters </summary>
        /// <param name="parameter">Required parameter</param>
        /// <param name="method">Get / put / post / delete</param>
        /// <param name="inputParams">Passed input parameters</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>System.String. "Null" if parameter is missing</returns>
        public object GetParam(string parameter, HTTPMethod method = HTTPMethod.Get, Parameters inputParams = null, object defaultValue = null)
        {
            if (inputParams != null && inputParams.GetType().GetProperty(parameter) != null)
            {
                return inputParams.GetType().GetProperty(parameter).GetValue(parameter);
            }

            if (parameter == "client_id" && this.Request != null && !string.IsNullOrEmpty(this.Request.Server("AUTH_USER")))
            {
                return this.Request.Server("AUTH_USER");
            }

            if (parameter == "client_secret" && this.Request != null && !string.IsNullOrEmpty(this.Request.Server("AUTH_PASSWORD")))
            {
                return this.Request.Server("AUTH_PASSWORD");
            }

            if (this.Request == null) return null;

            switch (method)
            {
                case HTTPMethod.Get:
                    return this.Request.Get(parameter, defaultValue != null ? defaultValue.ToString() : null);
                case HTTPMethod.Post:
                    return this.Request.Post(parameter, defaultValue != null ? defaultValue.ToString() : null);
                case HTTPMethod.Cookie:
                    return this.Request.Cookie(parameter, (HttpCookie)defaultValue);
                case HTTPMethod.File:
                    return this.Request.File(parameter, (HttpPostedFile)defaultValue);
                case HTTPMethod.Server:
                    return this.Request.Server(parameter, defaultValue != null ? defaultValue.ToString() : null);
                case HTTPMethod.Header:
                    return this.Request.Header(parameter, defaultValue != null ? defaultValue.ToString() : null);
                default:
                    return null;
            }
        }

        /// <summary> Get a parameter from passed input parameters </summary>
        /// <param name="method"> Get / put / post / delete </param>
        /// <param name="inputParams"> Passed input parameters </param>
        /// <param name="defaultValue"> The default value </param>
        /// <param name="parameter"> Required parameters </param>
        /// <returns> NameValueCollection "Null" if parameter is missing </returns>
        public dynamic GetParam(HTTPMethod method = HTTPMethod.Get, Parameters inputParams = null, string defaultValue = null, params string[] parameter)
        {
            var response = new Parameters();
            foreach (var p in parameter)
            {
                response.GetType().GetProperty(p).SetValue(parameter, this.GetParam(p, method, inputParams));
            }

            return response;
        }
    }
}