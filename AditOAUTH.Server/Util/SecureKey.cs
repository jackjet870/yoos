// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : vfornito
// Created          : 11-07-2013
//
// Last Modified By : vfornito
// Last Modified On : 11-27-2013
// ***********************************************************************
// <copyright file="SecureKey.cs" company="Autodistribution Italia Spa">
//     Copyright (c) Autodistribution Italia Spa. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace AditOAUTH.Server.Util
{
    using System;
    using System.Security.Cryptography;

    /// <summary> Class SecureKey </summary>
    public class SecureKey
    {
        /// <summary> Create a new Key </summary>
        /// <returns>System.String the key</returns>
        public static string Make()
        {
            return Convert.ToBase64String(new HMACSHA256().Key).Replace("/", string.Empty).Replace("+", string.Empty).Replace("=", string.Empty);
        }
    }
}