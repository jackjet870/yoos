// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : Valerio Fornito
// Created          : 11-05-2013
//
// Last Modified By : Valerio Fornito
// Last Modified On : 11-05-2013
// ***********************************************************************
// <copyright file="IScope.cs" company="Autodistribution Italia Spa">
//     Copyright (c) Autodistribution Italia Spa. All rights reserved.
// </copyright>
// <summary>Scope Interface</summary>
// ***********************************************************************

namespace AditOAUTH.Server.Storage
{
    /// <summary> Interface IScope </summary>
    public interface IScope
    {
        /// <summary>
        ///     Return information about a scope
        ///     <para />
        ///     Example SQL query:
        ///     <code>
        /// SELECT * FROM oauth_scopes WHERE scope = :scope
        /// </code>
        ///     <para />
        ///     Response:
        ///     <code>
        /// ScopeResponse (
        ///     [id] => (int) The scope's ID
        ///     [scope] => (string) The scope itself
        ///     [name] => (string) The scope's name
        ///     [description] => (string) The scope's description
        /// )
        /// </code>
        /// </summary>
        /// <param name="scope">The scope</param>
        /// <param name="clientId">The client ID (default = null)</param>
        /// <param name="grantType">The grant type used in the request (default = null)</param>
        /// <returns>If the scope doesn't exist return null</returns>
        ScopeResponse GetScope(string scope, string clientId = null, string grantType = null);
    }

    /// <summary> Defines a Client Response </summary>
    public class ScopeResponse
    {
        /// <summary> Gets or sets the scope's ID </summary>
        public int ID { get; set; }
        /// <summary> Gets or sets the scope itself </summary>
        public string Scope { get; set; }
        /// <summary> Gets or sets the scope's name </summary>
        public string Name { get; set; }
        /// <summary> Gets or sets the scope's description </summary>
        public string Description { get; set; }
    }
}