// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : Valerio Fornito
// Created          : 11-05-2013
//
// Last Modified By : Valerio Fornito
// Last Modified On : 11-05-2013
// ***********************************************************************
// <copyright file="IClient.cs" company="Autodistribution Italia Spa">
//     Copyright (c) Autodistribution Italia Spa. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace AditOAUTH.Server.Storage
{
    using AditOAUTH.Server.Grant;

    /// <summary> Interface IClient </summary>
    public interface IClient
    {
        /// <summary>
        ///     Validate a client
        ///     <para />
        ///     Example SQL query:
        ///     <code>
        /// # Client ID + redirect URI
        /// SELECT oauth_clients.id, oauth_clients.secret, oauth_client_endpoints.redirect_uri, oauth_clients.name
        ///  FROM oauth_clients LEFT JOIN oauth_client_endpoints ON oauth_client_endpoints.client_id = oauth_clients.id
        ///  WHERE oauth_clients.id = :clientId AND oauth_client_endpoints.redirect_uri = :redirectUri
        /// <para />
        /// # Client ID + client secret
        /// SELECT oauth_clients.id, oauth_clients.secret, oauth_clients.name FROM oauth_clients WHERE
        ///  oauth_clients.id = :clientId AND oauth_clients.secret = :clientSecret
        /// <para />
        /// # Client ID + client secret + redirect URI
        /// SELECT oauth_clients.id, oauth_clients.secret, oauth_client_endpoints.redirect_uri, oauth_clients.name FROM
        ///  oauth_clients LEFT JOIN oauth_client_endpoints ON oauth_client_endpoints.client_id = oauth_clients.id
        ///  WHERE oauth_clients.id = :clientId AND oauth_clients.secret = :clientSecret AND
        ///  oauth_client_endpoints.redirect_uri = :redirectUri
        /// </code>
        ///     <para />
        ///     Response:
        ///     <code>
        /// ClientResponse (
        ///     [client_id] => (string) The client ID
        ///     [client secret] => (string) The client secret
        ///     [redirect_uri] => (string) The redirect URI used in this request
        ///     [name] => (string) The name of the client
        /// )
        /// </code>
        /// </summary>
        /// <param name="grantType">The grant type used in the request (default = null)</param>
        /// <param name="clientId">The client's ID</param>
        /// <param name="clientSecret">The client's secret (default = null)</param>
        /// <param name="redirectUri">The client's redirect URI (default = null)</param>
        /// <returns>Returns null if the validation fails, ClientResponse on success</returns>
        ClientResponse GetClient(GrantTypIdentifier grantType, string clientId, string clientSecret = null, string redirectUri = null);
    }

    /// <summary> Defines a Client Response </summary>
    public class ClientResponse
    {
        /// <summary> Gets or sets the client ID </summary>
        public string ClientID { get; set; }
        /// <summary> Gets or sets the client secret </summary>
        public string ClientSecret { get; set; }
        /// <summary> Gets or sets the redirect URI used in this request </summary>
        public string RedirectUri { get; set; }
        /// <summary> Gets or sets the name of the client </summary>
        public string Name { get; set; }

        /// <summary> Gets or sets a value indicating whether [automatic approve] </summary>
        /// <value><c>true</c> if [automatic approve]; otherwise, <c>false</c></value>
        public bool AutoApprove { get; set; }
    }
}