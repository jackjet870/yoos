// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : vfornito
// Created          : 02-04-2014
//
// Last Modified By : vfornito
// Last Modified On : 02-04-2014
// ***********************************************************************
// <copyright file="HTTPMethod.cs" company="Autodistribution Italia Spa">
//     Copyright (c) Autodistribution Italia Spa. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace AditOAUTH.Server.Util
{
    /// <summary>
    /// Enum for HTTP Methods
    /// </summary>
    public enum HTTPMethod
    {
        /// <summary> The get HTTP Method </summary>
        Get,
        /// <summary> The post HTTP Method </summary>
        Post,
        /// <summary> The cookie HTTP Method </summary>
        Cookie,
        /// <summary> The file HTTP Method </summary>
        File,
        /// <summary> The server HTTP Method </summary>
        Server,
        /// <summary> The header HTTP Method </summary>
        Header
    }
}
