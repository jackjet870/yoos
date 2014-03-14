// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : vfornito
// Created          : 11-15-2013
//
// Last Modified By : vfornito
// Last Modified On : 11-27-2013
// ***********************************************************************
// <copyright file="ClientCredentials.cs" company="Autodistribution Italia Spa">
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

    /// <summary> Class ClientCredentials </summary>
    public class ClientCredentials : GrantType
    {
        /// <summary> Initializes a new instance of the <see cref="ClientCredentials"/> class </summary>
        public ClientCredentials()
        {
            this.Identifier = GrantTypIdentifier.client_credentials;
        }

        /// <summary> Gets or sets override the default access token expire time </summary>
        public int AccessTokenTTL { internal get; set; }

        /// <summary> Complete the client credentials grant </summary>
        /// <param name="inputParams">The input parameters</param>
        /// <returns>FlowResult with flow details </returns>
        /// <exception cref="ClientException"> If the flow fails </exception>
        public override FlowResult CompleteFlow(dynamic inputParams = null)
        {
            // Get the required params
            var authParams = this.AuthServer.GetParam(new[] { "client_id", "client_secret" }, HTTPMethod.Post, inputParams);

            if (string.IsNullOrEmpty(authParams.client_id)) throw new ClientException(HTTPErrorType.invalid_request, "client_id");
            if (string.IsNullOrEmpty(authParams.client_secret)) throw new ClientException(HTTPErrorType.invalid_request, "client_secret");

            // Validate client ID and client secret
            var clientDetails = this.AuthServer.Client.GetClient(authParams.client_id, authParams.client_secret, null);
            if (clientDetails == null) throw new ClientException(HTTPErrorType.invalid_client);

            authParams.client_details = clientDetails;

            // Validate any scopes that are in the request
            string scope = this.AuthServer.GetParam("scope", HTTPMethod.Post, inputParams).ToString();
            var scopes = scope.Split(this.AuthServer.ScopeDelimeter);

            scopes = scopes.Where(s => !string.IsNullOrEmpty(s)).Select(s => s.Trim()).ToArray();

            if (this.AuthServer.RequireScopeParam && string.IsNullOrEmpty(this.AuthServer.DefaultScope) && scopes.Length == 0)
                throw new ClientException(HTTPErrorType.invalid_request, "scope");

            if (scopes.Length == 0 && !string.IsNullOrEmpty(this.AuthServer.DefaultScope))
                scopes = this.AuthServer.DefaultScope.Split(this.AuthServer.ScopeDelimeter);

            var sr = new List<ScopeResponse>();
            foreach (var s in scopes)
            {
                var scopeDetails = this.AuthServer.Scope.GetScope(Identifier, s, authParams.client_id);

                if (scopeDetails == null) throw new ClientException(HTTPErrorType.invalid_scope, scope);

                sr.Add(scopeDetails);
            }

            // Generate an access token
            var accessToken = SecureKey.Make();
            var accessTokenExpiresIn = (this.AccessTokenTTL > 0) ? this.AccessTokenTTL : this.AuthServer.AccessTokenTTL;
            var accessTokenExpires = DateTime.Now.AddSeconds(accessTokenExpiresIn);

            // Create a new session
            var sessionId = this.AuthServer.Session.CreateSession(authParams.client_id, OwnerType.Client, authParams.client_id);

            // Add the access token
            var accessTokenId = this.AuthServer.Session.AssociateAccessToken(sessionId, accessToken, accessTokenExpires);

            // Associate scopes with the new session
            foreach (var s in sr)
            {
                this.AuthServer.Session.AssociateScope(accessTokenId, s.ID);
            }

            return new FlowResult
            {
                access_token = accessToken,
                token_type = "Bearer",
                access_token_expires = accessTokenExpires,
                expires_in = accessTokenExpiresIn
            };
        }
    }
}
