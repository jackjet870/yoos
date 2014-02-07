// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : vfornito
// Created          : 11-27-2013
//
// Last Modified By : vfornito
// Last Modified On : 11-27-2013
// ***********************************************************************
// <copyright file="RefreshToken.cs" company="Autodistribution Italia Spa">
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

    /// <summary> Class RefreshToken </summary>
    internal class RefreshToken : GrantType
    {
        /// <summary> Initializes a new instance of the <see cref="RefreshToken"/> class. </summary>
        public RefreshToken()
        {
            this.Identifier = GrantTypIdentifier.refresh_token;
        }

        /// <summary> Gets or sets override the default access token expire time </summary>
        public int AccessTokenTTL { internal get; set; }

        /// <summary> Gets the refresh token TTL </summary>
        public int RefreshTokenTTL { get { return 604800; } }

        /// <summary> Gets or sets a value indicating whether [rotate refresh tokens] </summary>
        /// <value><c>true</c> if [rotate refresh tokens]; otherwise, <c>false</c>.</value>
        public bool RotateRefreshTokens { get; set; }

        /// <summary> Complete the refresh token grant </summary>
        /// <param name="inputParams">Null unless the input parameters have been manually set</param>
        /// <returns>FlowResult with the result</returns>
        /// <exception cref="ClientException"> Thrown if there is something wrong </exception>
        public override FlowResult CompleteFlow(dynamic inputParams = null)
        {
            // Get the required params
            var authParams = this.AuthServer.GetParam(new[] { "client_id", "client_secret", "refresh_token", "scope" }, HTTPMethod.Post, inputParams);

            if (string.IsNullOrEmpty(authParams.client_id)) throw new ClientException(HTTPErrorType.invalid_request, "client_id");
            if (string.IsNullOrEmpty(authParams.client_secret)) throw new ClientException(HTTPErrorType.invalid_request, "client_secret");

            // Validate client ID and client secret
            var clientDetails = this.AuthServer.Client.GetClient(Identifier, authParams.client_id, authParams.client_secret);

            if (clientDetails == null) throw new ClientException(HTTPErrorType.invalid_client);

            authParams.client_details = clientDetails;

            if (string.IsNullOrEmpty(authParams.refresh_token)) throw new ClientException(HTTPErrorType.invalid_request, "refresh_token");

            // Validate refresh token
            var accessTokenId = this.AuthServer.Session.ValidateRefreshToken(authParams.refresh_token, authParams.client_id);
            if (accessTokenId == 0) throw new ClientException(HTTPErrorType.invalid_refresh);

            // Get the existing access token
            var accessTokenDetails = this.AuthServer.Session.GetAccessToken(accessTokenId);

            // Get the scopes for the existing access token
            List<ScopesResponse> scopes = this.AuthServer.Session.GetScopes(accessTokenDetails.AccessToken);

            // Generate new tokens and associate them to the session
            var accessToken = SecureKey.Make();
            var accessTokenExpiresIn = (this.AccessTokenTTL != 0) ? this.AccessTokenTTL : this.AuthServer.AccessTokenTTL;
            var accessTokenExpires = DateTime.Now.AddSeconds(accessTokenExpiresIn);

            // Associate the new access token with the session
            var newAccessTokenId = this.AuthServer.Session.AssociateAccessToken(accessTokenDetails.SessionID, accessToken, accessTokenExpires);

            var refreshToken = string.Empty;
            if (this.RotateRefreshTokens)
            {
                // Generate a new refresh token
                refreshToken = SecureKey.Make();
                var refreshTokenExpires = DateTime.Now.AddSeconds(this.RefreshTokenTTL);

                // Revoke the old refresh token
                this.AuthServer.Session.RemoveRefreshToken(authParams.refresh_token);

                // Associate the new refresh token with the new access token
                this.AuthServer.Session.AssociateRefreshToken(newAccessTokenId, refreshToken, refreshTokenExpires, authParams.client_id.ToString());
            }

            // There isn't a request for reduced scopes so assign the original ones (or we're not rotating scopes)
            if (string.IsNullOrEmpty(authParams.scope))
            {
                foreach (var scope in scopes)
                {
                    this.AuthServer.Session.AssociateScope(newAccessTokenId, scope.ID);
                }
            }
            else if (!string.IsNullOrEmpty(authParams.scope) && this.RotateRefreshTokens)
            {
                // The request is asking for reduced scopes and rotate tokens is enabled
                string[] reqestedScopes = authParams.scope.Split(this.AuthServer.ScopeDelimeter);
                reqestedScopes = reqestedScopes.Where(r => !string.IsNullOrEmpty(r)).Select(r => r.Trim()).ToArray();

                // Check that there aren't any new scopes being included
                var existingScopes = scopes.Select(s => s.Scope).ToList();

                foreach (var reqScope in reqestedScopes)
                {
                    if (!existingScopes.Contains(reqScope)) throw new ClientException(HTTPErrorType.invalid_request, "scope");

                    // Associate with the new access token
                    var scopeDetails = this.AuthServer.Scope.GetScope(Identifier, reqScope, authParams.client_id);
                    this.AuthServer.Session.AssociateScope(newAccessTokenId, scopeDetails.ID);
                }
            }

            var response = new FlowResult
            {
                AccessToken = accessToken,
                TokenType = "Bearer",
                AccessTokenExpires = accessTokenExpires,
                ExpiresIn = accessTokenExpiresIn
            };

            if (this.RotateRefreshTokens) response.RefreshToken = refreshToken;

            return response;
        }
    }
}