// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : vfornito
// Created          : 11-05-2013
//
// Last Modified By : vfornito
// Last Modified On : 11-27-2013
// ***********************************************************************
// <copyright file="ClientException.cs" company="Autodistribution Italia Spa">
//     Copyright (c) Autodistribution Italia Spa. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace AditOAUTH.Server.Exception
{
    /// <summary> Class ClientException </summary>
    public class ClientException : OAuth2Exception
    {
        /// <summary> Initializes a new instance of the <see cref="ClientException"/> class </summary>
        /// <param name="message">The message to visualize</param>
        public ClientException(string message) : base(message) { }
    }
}
