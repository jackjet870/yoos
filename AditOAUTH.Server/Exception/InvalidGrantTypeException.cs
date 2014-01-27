﻿// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : vfornito
// Created          : 11-05-2013
//
// Last Modified By : vfornito
// Last Modified On : 11-27-2013
// ***********************************************************************
// <copyright file="InvalidGrantTypeException.cs" company="Autodistribution Italia Spa">
//     Copyright (c) Autodistribution Italia Spa. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace AditOAUTH.Server.Exception
{
    /// <summary> Class InvalidGrantTypeException </summary>
    public class InvalidGrantTypeException : OAuth2Exception
    {
        /// <summary> Initializes a new instance of the <see cref="InvalidGrantTypeException"/> class </summary>
        /// <param name="message">The message to visualize</param>
        public InvalidGrantTypeException(string message) : base(message) { }
    }
}
