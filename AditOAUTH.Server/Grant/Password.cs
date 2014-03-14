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

    /// <summary> Class Password </summary>
    internal class Password : GrantType
    {
        /// <summary> Initializes a new instance of the <see cref="Password"/> class </summary>
        public Password()
        {
            this.Identifier = GrantTypIdentifier.password;
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
            var authParams = this.AuthServer.GetParam(new[] { "client_id", "client_secret", "username", "password" }, HTTPMethod.Post, inputParams);

            if (string.IsNullOrEmpty(authParams.client_id)) throw new ClientException(HTTPErrorType.invalid_request, "client_id");
            if (string.IsNullOrEmpty(authParams.client_secret)) throw new ClientException(HTTPErrorType.invalid_request, "client_secret");

            // Validate client credentials
            var clientDetails = this.AuthServer.Client.GetClient(Identifier, authParams.client_id, authParams.client_secret);
            if (clientDetails == null) throw new ClientException(HTTPErrorType.invalid_client);

            authParams.client_details = clientDetails;

            if (string.IsNullOrEmpty(authParams.username)) throw new ClientException(HTTPErrorType.invalid_request, "username");
            if (string.IsNullOrEmpty(authParams.password)) throw new ClientException(HTTPErrorType.invalid_request, "password");

            // Check if user"s username and password are correct
            if (this.VerifyCredentialsCallback == null) throw new InvalidGrantTypeException(HTTPErrorType.invalid_grant_callback);
            var userId = this.VerifyCredentialsCallback(authParams.username, authParams.password);

            if (string.IsNullOrEmpty(userId)) throw new ClientException(HTTPErrorType.invalid_credentials);

            // Validate any scopes that are in the request
            string scope = this.AuthServer.GetParam("scope", HTTPMethod.Post, inputParams).ToString();
            var scopes = scope.Split(this.AuthServer.ScopeDelimeter);
            scopes = scopes.Where(s => !string.IsNullOrEmpty(s)).Select(s => s.Trim()).ToArray();

            if (this.AuthServer.RequireScopeParam && string.IsNullOrEmpty(this.AuthServer.DefaultScope) && scopes.Length == 0)
                throw new ClientException(HTTPErrorType.invalid_request, "scope");

            if (scopes.Length == 0 && this.AuthServer.DefaultScope != null)
            {
                scopes = this.AuthServer.DefaultScope.Split(this.AuthServer.ScopeDelimeter);
            }

            var sr = new List<ScopeResponse>();

            foreach (var scopeDetails in scopes.Select(s => this.AuthServer.Scope.GetScope(this.Identifier, s, authParams.client_id)))
            {
                if (scopeDetails == null) throw new ClientException(HTTPErrorType.invalid_scope, scope);
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
                access_token = accessToken,
                token_type = "Bearer",
                access_token_expires = accessTokenExpires,
                expires_in = accessTokenExpiresIn
            };

            // Associate a refresh token if set
            if (this.AuthServer.HasGrantType(GrantTypIdentifier.refresh_token))
            {
                var refreshToken = SecureKey.Make();
                var refreshTokenTTL = DateTime.Now.AddSeconds(((RefreshToken)this.AuthServer.GetGrantType(GrantTypIdentifier.refresh_token)).RefreshTokenTTL);
                this.AuthServer.Session.AssociateRefreshToken(accessTokenId, refreshToken, refreshTokenTTL, authParams.client_id);
                response.refresh_token = refreshToken;
            }

            return response;
        }
    }
}