// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : Valerio Fornito
// Created          : 11-05-2013
//
// Last Modified By : Valerio Fornito
// Last Modified On : 11-05-2013
// ***********************************************************************
// <copyright file="Scope.cs" company="Autodistribution Italia Spa">
//     Copyright (c) Autodistribution Italia Spa. All rights reserved.
// </copyright>
// <summary>Scope Class</summary>
// ***********************************************************************

namespace AditOAUTH.Server.Storage.PDO
{
    using System.Linq;

    using AditOAUTH.Server.Grant;

    using _Data;

    /// <summary> Class Scope. </summary>
    public class Scope : IScope
    {
        /// <summary> Return information about a scope </summary>
        /// <param name="scope">The scope</param>
        /// <param name="clientId">The client ID (default = null)</param>
        /// <param name="grantType">The grant type used in the request (default = null)</param>
        /// <returns>If the scope doesn't exist return null</returns>
        public ScopeResponse GetScope( GrantTypIdentifier grantType, string scope, string clientId = null)
        {
            ScopeResponse sr = null;
            using (var adc = new AditOAUTHDataContext(Db.ConnectionString))
            {
                // SELECT * 
                // FROM oauth_scopes 
                // WHERE oauth_scopes.scope = :scope
                var sc = adc.oauth_scopes.SingleOrDefault(o => o.scope == scope);
                if (sc != null)
                    sr = new ScopeResponse { ID = sc.id, Scope = sc.scope, Name = sc.name, Description = sc.description };
            }

            return sr;
        }
    }
}