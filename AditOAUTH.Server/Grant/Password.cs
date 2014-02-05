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
        /// <returns>FlowResult the completed flow</returns>
        public override FlowResult CompleteFlow(dynamic inputParams = null)
        {
            // Get the required params
            var authParams = this.AuthServer.GetParam(HTTPMethod.Post, inputParams, "client_id", "client_secret", "username", "password");

            if (string.IsNullOrEmpty(authParams.client_id)) throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_request"].Message, "client_id"));
            if (string.IsNullOrEmpty(authParams.client_secret)) throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_request"].Message, "client_secret"));

            // Validate client credentials
            var clientDetails = this.AuthServer.Client.GetClient(authParams.client_id, authParams.client_secret, null, Identifier);
            if (clientDetails == null) throw new ClientException(HTTPErrorCollection.Instance["invalid_client"].Message);

            authParams.client_details = clientDetails;

            if (string.IsNullOrEmpty(authParams.username)) throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_request"].Message, "username"));
            if (string.IsNullOrEmpty(authParams.password)) throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_request"].Message, "password"));

            // Check if user"s username and password are correct
            if (this.VerifyCredentialsCallback == null) throw new InvalidGrantTypeException("Null or non-callable callback set");
            var userId = this.VerifyCredentialsCallback(authParams.username, authParams.password);

            if (string.IsNullOrEmpty(userId)) throw new ClientException(HTTPErrorCollection.Instance["invalid_credentials"].Message);

            // Validate any scopes that are in the request
            string scope = this.AuthServer.GetParam("scope", HTTPMethod.Post, inputParams, string.Empty).ToString();
            var scopes = scope.Split(this.AuthServer.ScopeDelimeter);
            scopes = scopes.Where(s => !string.IsNullOrEmpty(s)).Select(s => s.Trim()).ToArray();

            if (this.AuthServer.RequireScopeParam && string.IsNullOrEmpty(this.AuthServer.DefaultScope) && scopes.Length == 0)
                throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_request"].Message, "scope"));

            if (scopes.Length == 0 && this.AuthServer.DefaultScope != null)
            {
                scopes = this.AuthServer.DefaultScope.Split(this.AuthServer.ScopeDelimeter);
            }

            var sr = new List<ScopeResponse>();

            foreach (var scopeDetails in scopes.Select(s => this.AuthServer.Scope.GetScope(s, authParams.client_id, this.Identifier)))
            {
                if (scopeDetails == null) throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_scope"].Message, scope));
                sr.Add(scopeDetails);
            }

            authParams.scopes = sr;

            // Generate an access token
            var accessToken = SecureKey.Make();
            var accessTokenExpiresIn = (this.AccessTokenTTL != 0) ? this.AccessTokenTTL : this.AuthServer.AccessTokenTTL;
            var accessTokenExpires = DateTime.Now.AddSeconds(accessTokenExpiresIn);

            // Create a new session
            var sessionId = this.AuthServer.Session.CreateSession(authParams.client_id, OwnerType.User, userId);

            // Associate an access token with the session
            var accessTokenId = this.AuthServer.Session.AssociateAccessToken(sessionId, accessToken, accessTokenExpires);

            // Associate scopes with the access token
            foreach (var s in sr)
            {
                this.AuthServer.Session.AssociateScope(accessTokenId, s.ID);
            }

            var response = new FlowResult
            {
                AccessToken = accessToken,
                TokenType = "Bearer",
                AccessTokenExpires = accessTokenExpires,
                ExpiresIn = accessTokenExpiresIn
            };

            // Associate a refresh token if set
            if (this.AuthServer.HasGrantType("refresh_token"))
            {
                var refreshToken = SecureKey.Make();
                var refreshTokenTTL = DateTime.Now.AddSeconds(((RefreshToken)this.AuthServer.GetGrantType("refresh_token")).RefreshTokenTTL);
                this.AuthServer.Session.AssociateRefreshToken(accessTokenId, refreshToken, refreshTokenTTL, authParams.client_id);
                response.RefreshToken = refreshToken;
            }

            return response;
        }
    }
}