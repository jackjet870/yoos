// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : vfornito
// Created          : 12-06-2013
//
// Last Modified By : vfornito
// Last Modified On : 12-06-2013
// ***********************************************************************
// <copyright file="Db.cs" company="Autodistribution Italia Spa">
//     Copyright (c) Autodistribution Italia Spa. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace AditOAUTH.Server.Storage.PDO
{
    /// <summary> Class Db. </summary>
    public static class Db
    {
        /// <summary> The private connectionstring var </summary>
        private static string _connectionstring;

        /// <summary> Gets or sets the connection string. </summary>
        /// <value>The connection string</value>
        /// <exception cref="System.Exception">Connectionstring is not present</exception>
        internal static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionstring))
                    throw new System.Exception("Connectionstring is not present");
                return _connectionstring;

            }
            set { _connectionstring = value; }
        }
    }
}
