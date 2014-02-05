// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : vfornito
// Created          : 11-27-2013
//
// Last Modified By : vfornito
// Last Modified On : 11-27-2013
// ***********************************************************************
// <copyright file="Implicit.cs" company="Autodistribution Italia Spa">
//     Copyright (c) Autodistribution Italia Spa. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace AditOAUTH.Server.Grant
{
    using System;

    using AditOAUTH.Server.Storage;
    using AditOAUTH.Server.Util;

    /// <summary> Class Implicit. </summary>
    public class Implicit : GrantType
    {
        /// <summary> Initializes a new instance of the <see cref="Implicit"/> class. </summary>
        public Implicit()
        {
            this.Identifier = "implicit";
            this.ResponseType = "token";
        }

        /// <summary>
        /// Complete the client credentials grant
        /// </summary>
        /// <param name="authParams">The authentication parameters.</param>
        /// <returns>FlowResult information about the completed flow</returns>
        public override FlowResult CompleteFlow(dynamic authParams = null)
        {
            if (authParams == null) return null;
            // Remove any old sessions the user might have
            this.AuthServer.Session.DeleteSession(authParams.client_id, OwnerType.User, authParams.user_id);

            // Generate a new access token
            var accessToken = SecureKey.Make();

            // Compute expiry time
            var accessTokenExpires = DateTime.Now.AddSeconds(this.AuthServer.AccessTokenTTL);

            // Create a new session
            var sessionId = this.AuthServer.Session.CreateSession(authParams.client_id, OwnerType.User, authParams.user_id);

            // Create an access token
            var accessTokenId = this.AuthServer.Session.AssociateAccessToken(sessionId, accessToken, accessTokenExpires);

            // Associate scopes with the access token
            foreach (var scope in authParams.scopes)
            {
                this.AuthServer.Session.AssociateScope(accessTokenId, scope.ID);
            }

            return new FlowResult { AccessToken = accessToken };
        }
    }
}
