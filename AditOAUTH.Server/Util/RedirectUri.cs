// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : Valerio Fornito
// Created          : 11-06-2013
//
// Last Modified By : Valerio Fornito
// Last Modified On : 11-07-2013
// ***********************************************************************
// <copyright file="RedirectUri.cs" company="Autodistribution Italia Spa">
//     Copyright (c) Autodistribution Italia Spa. All rights reserved.
// </copyright>
// <summary>Build a Correct URI</summary>
// ***********************************************************************

namespace AditOAUTH.Server.Util
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    /// <summary> Class RedirectUri </summary>
    public class RedirectUri
    {
        /// <summary> Makes the specified URI. </summary>
        /// <param name="uri">The URI</param>
        /// <param name="parameters">The parameters</param>
        /// <param name="queryDelimiter">The query delimiter</param>
        /// <returns>System.String. Composite url</returns>
        public static string Make(string uri, Dictionary<string, string> parameters, string queryDelimiter = "?")
        {
            return parameters.Aggregate(uri, (current, p) => current + ((current.Contains(queryDelimiter) ? "&" : queryDelimiter) + WebUtility.UrlEncode(p.Key) + "=" + WebUtility.UrlEncode(p.Value)));
        }
    }
}