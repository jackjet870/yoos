// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : vfornito
// Created          : 12-06-2013
//
// Last Modified By : vfornito
// Last Modified On : 12-06-2013
// ***********************************************************************
// <copyright file="GrantType.cs" company="Autodistribution Italia Spa">
//     Copyright (c) Autodistribution Italia Spa. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace AditOAUTH.Server.Grant
{
    using System;

    /// <summary> Grant Typ Identifier Enum </summary>
    public enum GrantTypIdentifier
    {
        /// <summary> Authorization code grant type </summary>
        authorization_code,
        /// <summary> Client credentials grant type </summary>
        client_credentials,
        /// <summary> Implicit grant type </summary>
        @implicit,
        /// <summary> Password grant type </summary>
        password,
        /// <summary> Refresh_token grant type </summary>
        refresh_token
    }

    /// <summary> Response Type Identifier Enum </summary>
    public enum ResponseTypeIdentifier
    {
        /// <summary> Code response type </summary>
        Code,
        /// <summary>Token response type </summary>
        Token,
    }

    /// <summary> Class GrantType. </summary>
    public abstract class GrantType
    {
        /// <summary>
        /// Gets or sets the grant identifier (used to validate grant_type in AditOAUTH.Server\Authorization::IssueAccessToken())
        /// </summary>
        /// <value>The identifier.</value>
        public GrantTypIdentifier Identifier { get; set; }
        /// <summary>
        /// Gets the response type (used to validate response_type in AditOAUTH.Server\Grant\AuthCode::CheckAuthoriseParams())
        /// </summary>
        /// <value>The type of the response.</value>
        public ResponseTypeIdentifier? ResponseType { get; internal set; }

        /// <summary>
        /// Gets or sets the AuthServer instance
        /// </summary>
        /// <value>The authentication server.</value>
        public Authorization AuthServer { internal get; set; }

        /// <summary>
        /// Complete the grant flow
        /// </summary>
        /// <param name="inputParams">Null unless the input parameters have been manually set</param>
        /// <returns>FlowResult with the result</returns>
        public abstract FlowResult CompleteFlow(dynamic inputParams = null);
    }

    /// <summary>
    /// Defines a Flow Result
    /// </summary>
    public class FlowResult
    {
        /// <summary> Gets the access token. </summary>
        /// <value>The access token.</value>
        public string AccessToken { get; internal set; }
        /// <summary> Gets the type of the token. </summary>
        /// <value>The type of the token.</value>
        public string TokenType { get; internal set; }
        /// <summary> Gets the access token expires. </summary>
        /// <value>The access token expires.</value>
        public DateTime AccessTokenExpires { get; internal set; }
        /// <summary> Gets the expires in. </summary>
        /// <value>The expires in.</value>
        public int ExpiresIn { get; internal set; }
        /// <summary> Gets the refresh token. </summary>
        /// <value>The refresh token.</value>
        public string RefreshToken { get; internal set; }
    }
}
