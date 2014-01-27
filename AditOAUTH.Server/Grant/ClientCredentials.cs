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

    /// <summary> Class ClientCredentials. </summary>
    public class ClientCredentials : GrantType
    {
        /// <summary> Initializes a new instance of the <see cref="ClientCredentials"/> class. </summary>
        public ClientCredentials()
        {
            this.Identifier = "client_credentials";
        }

        /// <summary> Gets or sets override the default access token expire time </summary>
        public int AccessTokenTTL { internal get; set; }

        /// <summary> Complete the client credentials grant </summary>
        /// <param name="inputParams">The input parameters.</param>
        /// <returns>Dictionary{System.StringSystem.Object}. with flow details </returns>
        /// <exception cref="ClientException"> If the flow fails </exception>
        public override Dictionary<string, object> CompleteFlow(Dictionary<string, object> inputParams = null)
        {
            // Get the required params
            var authParams = this.AuthServer.GetParam(new List<string> { "client_id", "client_secret" }, "post", inputParams);

            if (authParams["client_id"] == null) throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_request"].Message, "client_id"));
            if (authParams["client_secret"] == null) throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_request"].Message, "client_secret"));

            // Validate client ID and client secret
            var clientDetails = this.AuthServer.Client.GetClient(authParams["client_id"].ToString(), authParams["client_secret"].ToString(), null, Identifier);
            if (clientDetails == null) throw new ClientException(HTTPErrorCollection.Instance["invalid_client"].Message);

            authParams["client_details"] = clientDetails;

            // Validate any scopes that are in the request
            var scope = this.AuthServer.GetParam("scope", "post", inputParams, string.Empty).ToString();
            var scopes = scope.Split(this.AuthServer.ScopeDelimeter);

            scopes = scopes.Where(s => !string.IsNullOrEmpty(s)).Select(s => s.Trim()).ToArray();

            if (this.AuthServer.RequireScopeParam && string.IsNullOrEmpty(this.AuthServer.DefaultScope) && scopes.Length == 0)
                throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_request"].Message, "scope"));

            if (scopes.Length == 0 && !string.IsNullOrEmpty(this.AuthServer.DefaultScope))
                scopes = this.AuthServer.DefaultScope.Split(this.AuthServer.ScopeDelimeter);

            var sr = new List<ScopeResponse>();
            foreach (var s in scopes)
            {
                var scopeDetails = this.AuthServer.Scope.GetScope(s, authParams["client_id"].ToString(), Identifier);

                if (scopeDetails == null) throw new ClientException(string.Format(HTTPErrorCollection.Instance["invalid_scope"].Message, scope));

                sr.Add(scopeDetails);
            }

            // Generate an access token
            var accessToken = SecureKey.Make();
            var accessTokenExpiresIn = (this.AccessTokenTTL > 0) ? this.AccessTokenTTL : this.AuthServer.AccessTokenTTL;
            var accessTokenExpires = DateTime.Now.AddSeconds(accessTokenExpiresIn);

            // Create a new session
            var sessionId = this.AuthServer.Session.CreateSession(authParams["client_id"].ToString(), "client", authParams["client_id"].ToString());

            // Add the access token
            var accessTokenId = this.AuthServer.Session.AssociateAccessToken(sessionId, accessToken, accessTokenExpires);

            // Associate scopes with the new session
            foreach (var s in sr)
            {
                this.AuthServer.Session.AssociateScope(accessTokenId, s.ID);
            }

            return new Dictionary<string, object> 
            {
                { "access_token", accessToken },
                { "token_type", "Bearer" },
                { "expires", accessTokenExpires },
                { "expires_in", accessTokenExpiresIn }
            };
        }
    }
}
