// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : vfornito
// Created          : 11-05-2013
//
// Last Modified By : vfornito
// Last Modified On : 11-27-2013
// ***********************************************************************
// <copyright file="OAuth2Exception.cs" company="Autodistribution Italia Spa">
//     Copyright (c) Autodistribution Italia Spa. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace AditOAUTH.Server.Exception
{
    /// <summary> Class OAuth2Exception </summary>
    public class OAuth2Exception : System.Exception
    {
        /// <summary> Initializes a new instance of the <see cref="OAuth2Exception"/> class </summary>
        public OAuth2Exception() : base("Generic Oauth2Exception: you mustn't see this...") { }
        /// <summary> Initializes a new instance of the <see cref="OAuth2Exception" /> class with a specified error message </summary>
        /// <param name="message">The message that describes the error.</param>
        public OAuth2Exception(string message) : base(message) { }
    }
}
