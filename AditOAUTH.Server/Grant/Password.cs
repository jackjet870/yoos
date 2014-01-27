// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : vfornito
// Created          : 11-27-2013
//
// Last Modified By : vfornito
// Last Modified On : 11-27-2013
// ***********************************************************************
// <copyright file="Password.cs" company="Autodistribution Italia Spa">
//     Copyright (c) Autodistribution Italia Spa. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace AditOAUTH.Server.Grant
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exception;
    using HTTPError;
    using Storage;
    using Util;

    /// <summary> Class Password. </summary>
    internal class Password : GrantType
    {
        /// <summary> Initializes a new instance of the <see cref="Password"/> class. </summary>
        public Password()
        {
            this.Identifier = "password";
        }

        /// <summary> Gets or sets override the default access token expire time </summary>
        public int AccessTokenTTL { internal get; set; }

        /// <summary> Gets or sets the callback to authenticate a user's username and password </summary>
        public Func<string, string, string> VerifyCredentialsCallback { internal get; set; }

        /// <summary>
        ///     Complete the password grant
        /// </summary>
        /// <param name="inputParams">The input parameters</param>
        /// <returns>System.Collections.Generic.Dictionary{System.String,System.Object} the completed flow</returns>
        public override Dictionary<string, object> CompleteFlow(Dictionary<string, object> inputParams = null)
        {
            // Get the required params
            var authParams = this.AuthServer.GetParam(new List<string> { "client_id", "client_secret", "username", "password" }, "post", inputParams);

            if (authParams["client_id"] == null) throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_request"].Message, "client_id"));
            if (authParams["client_secret"] == null) throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_request"].Message, "client_secret"));

            // Validate client credentials
            var clientDetails = this.AuthServer.Client.GetClient(authParams["client_id"].ToString(), authParams["client_secret"].ToString(), null, Identifier);
            if (clientDetails == null) throw new ClientException(HTTPErrorCollection.Instance["invalid_client"].Message);

            authParams["client_details"] = clientDetails;

            if (authParams["username"] == null) throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_request"].Message, "username"));
            if (authParams["password"] == null) throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_request"].Message, "password"));

            // Check if user"s username and password are correct
            if (this.VerifyCredentialsCallback == null) throw new InvalidGrantTypeException("Null or non-callable callback set");
            var userId = this.VerifyCredentialsCallback(authParams["username"].ToString(), authParams["password"].ToString());

            if (string.IsNullOrEmpty(userId)) throw new ClientException(HTTPErrorCollection.Instance["invalid_credentials"].Message);

            // Validate any scopes that are in the request
            var scope = this.AuthServer.GetParam("scope", "post", inputParams, string.Empty).ToString();
            var scopes = scope.Split(this.AuthServer.ScopeDelimeter);
            scopes = scopes.Where(s => !string.IsNullOrEmpty(s)).Select(s => s.Trim()).ToArray();

            if (this.AuthServer.RequireScopeParam && string.IsNullOrEmpty(this.AuthServer.DefaultScope) && scopes.Length == 0)
                throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_request"].Message, "scope"));

            if (scopes.Length == 0 && this.AuthServer.DefaultScope != null)
            {
                scopes = this.AuthServer.DefaultScope.Split(this.AuthServer.ScopeDelimeter);
            }

            var sr = new List<ScopeResponse>();

            foreach (var s in scopes)
            {
                var scopeDetails = this.AuthServer.Scope.GetScope(s, authParams["client_id"].ToString(), Identifier);

                if (scopeDetails == null) throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_scope"].Message, scope));
                sr.Add(scopeDetails);
            }

            authParams["scopes"] = sr;

            // Generate an access token
            var accessToken = SecureKey.Make();
            var accessTokenExpiresIn = (this.AccessTokenTTL != 0) ? this.AccessTokenTTL : this.AuthServer.AccessTokenTTL;
            var accessTokenExpires = DateTime.Now.AddSeconds(accessTokenExpiresIn);

            // Create a new session
            var sessionId = this.AuthServer.Session.CreateSession(authParams["client_id"].ToString(), "user", userId);

            // Associate an access token with the session
            var accessTokenId = this.AuthServer.Session.AssociateAccessToken(sessionId, accessToken, accessTokenExpires);

            // Associate scopes with the access token
            foreach (var s in sr)
            {
                this.AuthServer.Session.AssociateScope(accessTokenId, s.ID);
            }

            var response = new Dictionary<string, object>
            {
                { "access_token", accessToken },
                { "token_type", "Bearer" },
                { "expires", accessTokenExpires },
                { "expires_in", accessTokenExpiresIn }
            };

            // Associate a refresh token if set
            if (this.AuthServer.HasGrantType("refresh_token"))
            {
                var refreshToken = SecureKey.Make();
                var refreshTokenTTL = DateTime.Now.AddSeconds(((RefreshToken)this.AuthServer.GetGrantType("refresh_token")).RefreshTokenTTL);
                this.AuthServer.Session.AssociateRefreshToken(accessTokenId, refreshToken, refreshTokenTTL, authParams["client_id"].ToString());
                response.Add("refresh_token", refreshToken);
            }

            return response;
        }
    }
}