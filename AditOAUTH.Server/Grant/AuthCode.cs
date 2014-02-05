// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : vfornito
// Created          : 11-05-2013
//
// Last Modified By : vfornito
// Last Modified On : 11-26-2013
// ***********************************************************************
// <copyright file="AuthCode.cs" company="Autodistribution Italia Spa">
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

    /// <summary> OAuth 2.0 Auth code grant </summary>
    public class AuthCode : GrantType
    {
        /// <summary> Initializes a new instance of the <see cref="AuthCode"/> class. </summary>
        public AuthCode()
        {
            this.Identifier = "authorization_code";
            this.ResponseType = "code";
        }

        /// <summary> Gets or sets override the default access token expire time </summary>
        public int AccessTokenTTL { internal get; set; }

        /// <summary> The TTL of the auth token </summary>
        private int authTokenTTL = 600;

        /// <summary> Gets or sets override the default access token expire time </summary>
        /// <value>The authentication token TTL.</value>
        public int AuthTokenTTL
        {
            internal get { return this.authTokenTTL; }
            set { this.authTokenTTL = value; }
        }

        /// <summary>
        /// Checks the authorise parameters.
        /// </summary>
        /// <param name="inputParams">Optional array of parsed _GET keys</param>
        /// <returns>Dictionary{System.StringSystem.Object} Authorise request parameters</returns>
        /// <exception cref="ClientException"> Client Exception </exception>
        public Parameters CheckAuthoriseParams(dynamic inputParams = null)
        {
            // Auth params
            var authParams = this.AuthServer.GetParam(HTTPMethod.Get, inputParams, "client_id", "redirect_uri", "response_type", "scope", "state");

            if (string.IsNullOrEmpty(authParams.client_id)) throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_request"].Message, "client_id"));
            if (string.IsNullOrEmpty(authParams.redirect_uri)) throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_request"].Message, "redirect_uri"));
            if (this.AuthServer.RequireStateParam && string.IsNullOrEmpty(authParams.state)) throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_request"].Message, "state"));

            // Validate client ID and redirect URI
            var clientDetails = this.AuthServer.Client.GetClient(authParams.client_id, null, authParams.redirect_uri, Identifier);
            if (clientDetails == null) throw new ClientException(HTTPErrorCollection.Instance["invalid_client"].Message);

            authParams.client_details = clientDetails;

            if (string.IsNullOrEmpty(authParams.response_type)) throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_request"].Message, "response_type"));

            // Ensure response type is one that is recognised
            if (!this.AuthServer.ResponseTypes.Contains(authParams.response_type)) throw new ClientException(HTTPErrorCollection.Instance["unsupported_response_type"].Message);

            // Validate scopes
            string[] scopes = authParams.scope.Split(this.AuthServer.ScopeDelimeter);
            scopes = scopes.Where(s => !string.IsNullOrEmpty(s)).Select(s => s.Trim()).ToArray();

            if (this.AuthServer.RequireScopeParam && this.AuthServer.DefaultScope == null && scopes.Length == 0)
                throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_request"].Message, "scope"));

            if (scopes.Length == 0 && this.AuthServer.DefaultScope != null)
                scopes = this.AuthServer.DefaultScope.Split(this.AuthServer.ScopeDelimeter);

            var sr = new List<ScopeResponse>();
            foreach (var s in scopes)
            {
                var scopeDetails = this.AuthServer.Scope.GetScope(s, authParams.client_id, Identifier);
                if (scopeDetails == null) throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_scope"].Message, s));
                sr.Add(scopeDetails);
            }

            authParams.scopes = sr;
            return authParams;
        }

        /// <summary>
        /// Parse a new authorise request
        /// </summary>
        /// <param name="type"> The session owner's type</param>
        /// <param name="typeId">The type identifier.</param>
        /// <param name="authParams">The session owner"s ID</param>
        /// <returns>System.String An authorisation code</returns>
        public string NewAuthoriseRequest(OwnerType type, string typeId, dynamic authParams)
        {
            // Generate an auth code
            var authCode = SecureKey.Make();

            // Remove any old sessions the user might have
            this.AuthServer.Session.DeleteSession(authParams.client_id, type, typeId);

            // Create a new session
            var sessionId = this.AuthServer.Session.CreateSession(authParams.client_id, type, typeId);

            // Associate a redirect URI
            this.AuthServer.Session.AssociateRedirectUri(sessionId, authParams.redirect_uri);

            // Associate the auth code
            var authCodeId = this.AuthServer.Session.AssociateAuthCode(sessionId, authCode, DateTime.Now.AddSeconds(this.authTokenTTL));

            // Associate the scopes to the auth code
            foreach (var scope in authParams.scopes)
            {
                this.AuthServer.Session.AssociateAuthCodeScope(authCodeId, scope.ID);
            }

            return authCode;
        }

        /// <summary> Complete the auth code grant </summary>
        /// <param name="inputParams">The input parameters.</param>
        /// <returns>FlowResult with the value of the flow</returns>
        /// <exception cref="ClientException"> Various client exceptions </exception>
        public override FlowResult CompleteFlow(dynamic inputParams = null)
        {
            // Get the required params
            var authParams = this.AuthServer.GetParam(HTTPMethod.Post, inputParams, "client_id", "client_secret", "redirect_uri", "code");

            if (string.IsNullOrEmpty(authParams.client_id)) throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_request"].Message, "client_id"));
            if (string.IsNullOrEmpty(authParams.client_secret)) throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_request"].Message, "client_secret"));
            if (string.IsNullOrEmpty(authParams.redirect_uri)) throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_request"].Message, "redirect_uri"));

            // Validate client ID and redirect URI
            var clientDetails = this.AuthServer.Client.GetClient(authParams.client_id, authParams.client_secret, authParams.redirect_uri, Identifier);
            if (clientDetails == null) throw new ClientException(HTTPErrorCollection.Instance["invalid_client"].Message);

            authParams.client_details = clientDetails;

            // Validate the authorization code
            if (string.IsNullOrEmpty(authParams.code)) throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_request"].Message, "code"));

            // Verify the authorization code matches the client_id and the request_uri
            var authCodeDetails = this.AuthServer.Session.ValidateAuthCode(authParams.client_id, authParams.redirect_uri, authParams.code);
            if (authCodeDetails == null) throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_grant"].Message, "code"));

            // Get any associated scopes
            var scopes = this.AuthServer.Session.GetAuthCodeScopes(authCodeDetails.AuthcodeID);

            // A session ID was returned so update it with an access token and remove the authorisation code
            var accessToken = SecureKey.Make();
            var accessTokenExpiresIn = (this.AccessTokenTTL != 0) ? this.AccessTokenTTL : this.AuthServer.AccessTokenTTL;
            var accessTokenExpires = DateTime.Now.AddSeconds(accessTokenExpiresIn);

            // Remove the auth code
            this.AuthServer.Session.RemoveAuthCode(authCodeDetails.SessionID);

            // Create an access token
            var accessTokenId = this.AuthServer.Session.AssociateAccessToken(authCodeDetails.SessionID, accessToken, accessTokenExpires);

            // Associate scopes with the access token
            if (scopes.Count > 0)
            {
                foreach (var scope in scopes)
                {
                    this.AuthServer.Session.AssociateScope(accessTokenId, scope);
                }
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
