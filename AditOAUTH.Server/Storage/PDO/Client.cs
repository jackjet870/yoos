// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : Valerio Fornito
// Created          : 11-05-2013
//
// Last Modified By : Valerio Fornito
// Last Modified On : 11-05-2013
// ***********************************************************************
// <copyright file="Client.cs" company="Autodistribution Italia Spa">
//     Copyright (c) Autodistribution Italia Spa. All rights reserved.
// </copyright>
// <summary>Client Interface</summary>
// ***********************************************************************

namespace AditOAUTH.Server.Storage.PDO
{
    using System;
    using System.Linq;

    using _Data;
    using AditOAUTH.Server.Grant;

    /// <summary> Defines a Client Object </summary>
    public class Client : IClient
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
        public ClientResponse GetClient(GrantTypIdentifier grantType, string clientId, string clientSecret = null, string redirectUri = null)
        {
            Uri uri = null;
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentException("clientId");
            if (!string.IsNullOrEmpty(redirectUri))
                uri = new Uri(redirectUri);

            ClientResponse response = null;
            using (var adc = new AditOAUTHDataContext(Constants.DBConnectionString))
            {
                if (!string.IsNullOrEmpty(redirectUri) && string.IsNullOrEmpty(clientSecret))
                {
                    if (uri == null) throw new ArgumentException("redirectUri");

                    // SELECT oauth_clients.id, oauth_clients.secret, oauth_client_endpoints.redirect_uri, oauth_clients.name 
                    // FROM oauth_clients 
                    // LEFT JOIN oauth_client_endpoints ON oauth_client_endpoints.client_id = oauth_clients.id 
                    // WHERE oauth_clients.id = :clientId 
                    // AND oauth_client_endpoints.redirect_uri = :redirectUri
                    var client = from oc in adc.oauth_clients
                                 join oce in adc.oauth_client_endpoints on oc.id equals oce.client_id into ce
                                 from suboc in ce.DefaultIfEmpty()
                                 where oc.id == clientId
                                       && suboc.uri_protocol == uri.Scheme
                                       && suboc.uri_domain == uri.Host
                                       && suboc.uri_port == uri.Port
                                 select
                                     new
                                     {
                                         oc.id,
                                         oc.secret,
                                         oc.name,
                                         redirect_uri = suboc == null ? string.Empty : suboc.uri_protocol + suboc.uri_domain + (suboc.uri_port.HasValue ? ":" + suboc.uri_port : string.Empty) + suboc.uri_path,
                                         oc.auto_approve
                                     };

                    var c = client.SingleOrDefault();
                    if (c != null)
                        response = new ClientResponse
                        {
                            ClientID = c.id,
                            ClientSecret = c.secret,
                            RedirectUri = c.redirect_uri,
                            Name = c.name,
                            AutoApprove = c.auto_approve
                        };
                }
                else if (!string.IsNullOrEmpty(clientSecret) && string.IsNullOrEmpty(redirectUri))
                {
                    // SELECT oauth_clients.id, oauth_clients.secret, oauth_clients.name
                    // FROM oauth_clients 
                    // WHERE oauth_clients.id = :clientId 
                    // AND oauth_clients.secret = :clientSecret
                    var client =
                        adc.oauth_clients.Where(o => o.id == clientId && o.secret == clientSecret)
                            .Select(o => new { o.id, secrect = o.secret, o.name })
                            .SingleOrDefault();
                    if (client != null)
                        response = new ClientResponse
                        {
                            ClientID = client.id,
                            ClientSecret = client.secrect,
                            Name = client.name
                        };
                }
                else if (!string.IsNullOrEmpty(clientSecret) && !string.IsNullOrEmpty(redirectUri))
                {
                    // SELECT oauth_clients.id, oauth_clients.secret, oauth_client_endpoints.redirect_uri, oauth_clients.name 
                    // FROM oauth_clients 
                    // LEFT JOIN oauth_client_endpoints ON oauth_client_endpoints.client_id = oauth_clients.id 
                    // WHERE oauth_clients.id = :clientId 
                    // AND oauth_clients.secret = :clientSecret 
                    // AND oauth_client_endpoints.redirect_uri = :redirectUri
                    var client = from oc in adc.oauth_clients
                                 join oce in adc.oauth_client_endpoints on oc.id equals oce.client_id into ce
                                 from suboc in ce.DefaultIfEmpty()
                                 where oc.id == clientId
                                       && oc.secret == clientSecret
                                       && suboc.uri_protocol == uri.Scheme
                                       && suboc.uri_domain == uri.Host
                                       && suboc.uri_port == uri.Port
                                 select
                                     new
                                     {
                                         oc.id,
                                         oc.secret,
                                         oc.name,
                                         redirect_uri = suboc == null ? string.Empty : suboc.uri_protocol + suboc.uri_domain + (suboc.uri_port.HasValue ? ":" + suboc.uri_port : string.Empty) + suboc.uri_path,
                                     };
                    var c = client.SingleOrDefault();
                    if (c != null)
                        response = new ClientResponse
                        {
                            ClientID = c.id,
                            ClientSecret = c.secret,
                            RedirectUri = c.redirect_uri,
                            Name = c.name
                        };
                }
            }

            return response;
        }
    }
}