// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : vfornito
// Created          : 11-27-2013
//
// Last Modified By : vfornito
// Last Modified On : 11-27-2013
// ***********************************************************************
// <copyright file="Resource.cs" company="Autodistribution Italia Spa">
//     Copyright (c) Autodistribution Italia Spa. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace AditOAUTH.Server
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Exception;
    using Storage;
    using Util;

    /// <summary> Class Resource. </summary>
    public class Resource
    {
        /// <summary> Gets the access token. </summary>
        /// <value>The access token</value>
        public string AccessToken { get; private set; }
        /// <summary> Gets the type of the owner of the access token </summary>
        /// <value>The type of the owner</value>
        public string OwnerType { get; private set; }
        /// <summary> Gets the ID of the owner of the access token </summary>
        /// <value>The owner identifier</value>
        public string OwnerId { get; private set; }
        /// <summary>
        /// The scopes associated with the access token
        /// </summary>
        private readonly List<string> _sessionScopes = new List<string>();
        /// <summary>
        /// The session storage class
        /// </summary>
        private readonly ISession _storage;
        /// <summary> Gets or sets The Request object </summary>
        /// <value>The request</value>
        public IRequest Request { get; set; }
        /// <summary> The query string key which is used by clients to present the access token (default: access_token) </summary>
        private string _tokenKey = "access_token";
        /// <summary> Gets or sets the token key </summary>
        /// <value>The token key</value>
        public string TokenKey { get { return this._tokenKey; } set { this._tokenKey = value; } }
        /// <summary> Gets the client ID </summary>
        /// <value>The client identifier</value>
        public string ClientId { get; private set; }
        /// <summary> The session ID </summary>
        private int _sessionId;

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public Resource(ISession session)
        {
            this._storage = session;
        }

        /// <summary> Determines whether the specified access token is valid. </summary>
        /// <param name="headersOnly">Limit Access Token to Authorization header only</param>
        /// <returns><c>true</c> if the specified headers only is valid; otherwise, <c>false</c>.</returns>
        /// <exception cref="InvalidAccessTokenException">Access token is not valid</exception>
        public bool IsValid(bool headersOnly = false)
        {
            this.AccessToken = this.DetermineAccessToken(headersOnly);

            var result = this._storage.ValidateAccessToken(this.AccessToken);

            if (result == null) throw new InvalidAccessTokenException("Access token is not valid");

            this.AccessToken = this.AccessToken;
            this._sessionId = result.SessionID;
            this.ClientId = result.ClientID;
            this.OwnerType = result.OwnerType;
            this.OwnerId = result.OwnerID;

            var sss = this._storage.GetScopes(this.AccessToken);
            foreach (var scope in sss)
            {
                this._sessionScopes.Add(scope.Scope);
            }

            return true;
        }

        /// <summary> Gets the session scopes </summary>
        /// <returns>List{System.String} the scopes</returns>
        public List<string> GetScopes()
        {
            return this._sessionScopes;
        }

        /**
         * Checks if the presented access token has the given scope(s).
         *
         * @param array|string  An array of scopes or a single scope as a string
         * @return bool         Returns bool if all scopes are found, false if any fail
         */

        /// <summary> Checks if the presented access token has the given scopes </summary>
        /// <param name="scopes">An array of scopes</param>
        /// <returns>Returns true if all scopes are found, false if any fail</returns>
        public bool HasScope(List<string> scopes)
        {
            return scopes.All(scope => this._sessionScopes.Contains(scope));
        }

        /// <summary> Checks if the presented access token has the given scope </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>Returns true if the scope is found, false if fail</returns>
        public bool HasScope(string scope)
        {
            return this._sessionScopes.Contains(scope);
        }

        /// <summary> Reads in the access token from the headers </summary>
        /// <param name="headersOnly">Limit Access Token to Authorization header only</param>
        /// <returns>System.String the access token</returns>
        /// <exception cref="InvalidAccessTokenException">Thrown if there is no access token presented</exception>
        private string DetermineAccessToken(bool headersOnly = false)
        {
            var header = this.Request.Header("Authorization");
            if (header != null)
            {
                // Check for special case, because cURL sometimes does an
                // internal second Request and doubles the authorization header,
                // which always resulted in an error.
                //
                // 1st Request: Authorization: Bearer XXX
                // 2nd Request: Authorization: Bearer XXX, Bearer XXX
                if (header.IndexOf(',') != -1)
                {
                    var headerPart = header.Split(',');
                    this.AccessToken = Regex.Replace(headerPart[0], @"/^(?:\s+)?Bearer\s/", string.Empty).Trim();
                }
                else
                {
                    this.AccessToken = Regex.Replace(header, @"/^(?:\s+)?Bearer\s/", string.Empty);
                }

                this.AccessToken = (this.AccessToken == "Bearer") ? string.Empty : this.AccessToken;
            }
            else if (headersOnly == false)
            {
                var method = this.Request.Server("REQUEST_METHOD");
                switch (method.ToLower())
                {
                    case "get":
                        this.AccessToken = this.Request.Get(this.TokenKey);
                        break;
                    case "post":
                        this.AccessToken = this.Request.Post(this.TokenKey);
                        break;
                    case "server":
                        this.AccessToken = this.Request.Server(this.TokenKey);
                        break;
                    case "header":
                        this.AccessToken = this.Request.Header(this.TokenKey);
                        break;
                    default:
                        this.AccessToken = string.Empty;
                        break;
                }
            }

            if (string.IsNullOrEmpty(this.AccessToken))
                throw new InvalidAccessTokenException("Access token is missing");

            return this.AccessToken;
        }
    }
}
