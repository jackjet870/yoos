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
    using System.Collections.Generic;

    /// <summary> Class GrantType. </summary>
    public abstract class GrantType
    {
        /// <summary>
        /// Gets the grant identifier (used to validate grant_type in AditOAUTH.Server\Authorization::IssueAccessToken())
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier { get; set; }
        /// <summary>
        /// Gets the response type (used to validate response_type in AditOAUTH.Server\Grant\AuthCode::CheckAuthoriseParams())
        /// </summary>
        /// <value>The type of the response.</value>
        public string ResponseType { get; internal set; }

        /// <summary>
        /// Gets or sets the AuthServer instance
        /// </summary>
        /// <value>The authentication server.</value>
        public Authorization AuthServer { internal get; set; }

        /// <summary>
        /// Complete the grant flow
        /// </summary>
        /// <param name="inputParams">Null unless the input parameters have been manually set</param>
        /// <returns>Dictionary{System.StringSystem.Object} with the result</returns>
        public abstract Dictionary<string, object> CompleteFlow(Dictionary<string, object> inputParams = null);
    }
}
