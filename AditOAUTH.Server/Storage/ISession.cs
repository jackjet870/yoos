// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : Valerio Fornito
// Created          : 11-05-2013
//
// Last Modified By : vfornito
// Last Modified On : 11-06-2013
// ***********************************************************************
// <copyright file="ISession.cs" company="Autodistribution Italia Spa">
//     Copyright (c) Autodistribution Italia Spa. All rights reserved.
// </copyright>
// <summary>The Session Interface</summary>
// ***********************************************************************

namespace AditOAUTH.Server.Storage
{
    using System;
    using System.Collections.Generic;

    /// <summary> Interface for session </summary>
    public interface ISession
    {
        /// <summary>
        ///     Create a new session
        ///     <para />
        ///     Example SQL query:
        ///     <code>
        /// INSERT INTO oauth_sessions (client_id, owner_type,  owner_id)
        /// VALUE (:clientId, :ownerType, :ownerId)
        /// </code>
        ///     <para />
        /// </summary>
        /// <param name="clientId">The client ID</param>
        /// <param name="ownerType">The type of the session owner (e.g. "user")</param>
        /// <param name="ownerId">The ID of the session owner (e.g. "123")</param>
        /// <returns>System.Int32. The session ID</returns>
        int CreateSession(string clientId, string ownerType, string ownerId);

        /// <summary>
        ///     Delete a session
        ///     <para />
        ///     Example SQL query:
        ///     <code>
        /// DELETE FROM oauth_sessions WHERE client_id = :clientId AND owner_type = :type AND owner_id = :typeId
        /// </code>
        /// </summary>
        /// <param name="clientId">The client ID</param>
        /// <param name="ownerType">The type of the session owner (e.g. "user")</param>
        /// <param name="ownerId">The ID of the session owner (e.g. "123")</param>
        void DeleteSession(string clientId, string ownerType, string ownerId);

        /// <summary>
        ///     Associate a redirect URI with a session
        ///     <para />
        ///     Example SQL query:
        ///     <code>
        /// INSERT INTO oauth_session_redirects (session_id, redirect_uri) VALUE (:sessionId, :redirectUri)
        /// </code>
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <param name="redirectUri">The redirect URI</param>
        void AssociateRedirectUri(int sessionId, string redirectUri);

        /// <summary>
        ///     Associate an access token with a session
        ///     <para />
        ///     Example SQL query:
        ///     <code>
        /// INSERT INTO oauth_session_access_tokens (session_id, access_token, access_token_expires)
        /// VALUE (:sessionId, :accessToken, :accessTokenExpire)
        /// </code>
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <param name="accessToken">The access token</param>
        /// <param name="expireTime">Timestamp of the access token expiry time</param>
        /// <returns>The Access Token ID</returns>
        int AssociateAccessToken(int sessionId, string accessToken, DateTime expireTime);

        /// <summary>
        ///     Associate a refresh token with a session
        ///     <para />
        ///     Example SQL query:
        ///     <code>
        /// INSERT INTO oauth_session_refresh_tokens (session_access_token_id, refresh_token, refresh_token_expires,
        /// client_id) VALUE (:accessTokenId, :refreshToken, :expireTime, :clientId)
        /// </code>
        /// </summary>
        /// <param name="accessTokenId">The access token ID</param>
        /// <param name="refreshToken">The refresh token</param>
        /// <param name="expireTime">Timestamp of the refresh token expiry time</param>
        /// <param name="clientId">The client ID</param>
        void AssociateRefreshToken(int accessTokenId, string refreshToken, DateTime expireTime, string clientId);

        /// <summary>
        ///     Associate an authorization code with a session
        ///     <para />
        ///     Example SQL query:
        ///     <code>
        /// INSERT INTO oauth_session_authcodes (session_id, auth_code, auth_code_expires)
        ///  VALUE (:sessionId, :authCode, :authCodeExpires)
        /// </code>
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <param name="authCode">The authorization code</param>
        /// <param name="expireTime">Timestamp of the access token expiry time</param>
        /// <returns>System.Int32. The auth code ID</returns>
        int AssociateAuthCode(int sessionId, string authCode, DateTime expireTime);

        /// <summary>
        ///     Remove an associated authorization token from a session
        ///     <para />
        ///     Example SQL query:
        ///     <code>
        /// DELETE FROM oauth_session_authcodes WHERE session_id = :sessionId
        /// </code>
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        void RemoveAuthCode(int sessionId);

        /// <summary>
        ///     Validate an authorization code
        ///     <para />
        ///     Example SQL query:
        ///     <code>
        /// SELECT oauth_sessions.id AS session_id, oauth_session_authcodes.id AS authcode_id FROM oauth_sessions
        /// JOIN oauth_session_authcodes ON oauth_session_authcodes.session_id = oauth_sessions.id
        /// JOIN oauth_session_redirects ON oauth_session_redirects.session_id = oauth_sessions.id WHERE
        /// oauth_sessions.client_id = :clientId AND oauth_session_authcodes.auth_code = :authCode
        /// AND oauth_session_authcodes.auth_code_expires >= :time AND
        /// oauth_session_redirects.redirect_uri = :redirectUri
        /// </code>
        ///     <para />
        ///     Expected response:
        ///     <code>
        /// ValidateAuthCodeResponse(
        ///     'session_id' =>  (int)
        ///     'authcode_id'  =>  (int)
        /// )
        /// </code>
        /// </summary>
        /// <param name="clientId">The client ID</param>
        /// <param name="redirectUri">The redirect URI</param>
        /// <param name="authCode">The authorization code</param>
        /// <returns>Null if invalid or ValidateAuthCodeResponse as above</returns>
        ValidateAuthCodeResponse ValidateAuthCode(string clientId, string redirectUri, string authCode);

        /// <summary>
        ///     Validate an access token
        ///     <para />
        ///     Example SQL query:
        ///     <code>
        /// SELECT session_id, oauth_sessions.client_id, oauth_sessions.owner_id, oauth_sessions.owner_type
        /// FROM oauth_session_access_tokens JOIN oauth_sessions ON oauth_sessions.id = session_id WHERE
        /// access_token = :accessToken AND access_token_expires >= UNIX_TIMESTAMP(NOW())
        /// </code>
        ///     <para />
        ///     Expected response:
        ///     <code>
        /// ValidateAccessTokenResponse(
        ///     'session_id' =>  (int),
        ///     'client_id'  =>  (string),
        ///     'owner_id'   =>  (string),
        ///     'owner_type' =>  (string)
        /// )
        /// </code>
        /// </summary>
        /// <param name="accessToken">The access token</param>
        /// <returns>Null if invalid or a ValidateAccessTokenResponse</returns>
        ValidateAccessTokenResponse ValidateAccessToken(string accessToken);

        /// <summary>
        ///     Removes a refresh token
        ///     <para />
        ///     Example SQL query:
        ///     <code>
        /// DELETE FROM oauth_session_refresh_tokens WHERE refresh_token = :refreshToken
        /// </code>
        /// </summary>
        /// <param name="refreshToken">The refresh token to be removed</param>
        void RemoveRefreshToken(string refreshToken);

        /// <summary>
        ///     Validate a refresh token
        ///     <para />
        ///     Example SQL query:
        ///     <code>
        /// SELECT session_access_token_id FROM oauth_session_refresh_tokens WHERE refresh_token = :refreshToken
        /// AND refresh_token_expires >= UNIX_TIMESTAMP(NOW()) AND client_id = :clientId
        /// </code>
        /// </summary>
        /// <param name="refreshToken">The access token</param>
        /// <param name="clientId">The client ID</param>
        /// <returns>System.Int32. The ID of the access token the refresh token is linked to (or 0 if invalid)</returns>
        int ValidateRefreshToken(string refreshToken, string clientId);

        /// <summary>
        ///     Get an access token by ID
        ///     <para />
        ///     Example SQL query:
        ///     <code>
        /// SELECT * FROM oauth_session_access_tokens WHERE id = :accessTokenId
        /// </code>
        ///     <para />
        ///     Expected response:
        ///     <code>
        /// GetAccessTokenResponse(
        ///     'id' =>  (int),
        ///     'session_id' =>  (int),
        ///     'access_token'   =>  (string),
        ///     'access_token_expires'   =>  (int)
        /// )
        /// </code>
        /// </summary>
        /// <param name="accessTokenId">The access token ID</param>
        /// <returns>Null or GetAccessTokenResponse</returns>
        GetAccessTokenResponse GetAccessToken(int accessTokenId);

        /// <summary>
        ///     Associate scopes with an auth code (bound to the session)
        ///     <para />
        ///     Example SQL query:
        ///     <code>
        /// INSERT INTO oauth_session_authcode_scopes (oauth_session_authcode_id, scope_id) VALUES (:authCodeId, :scopeId)
        /// </code>
        /// </summary>
        /// <param name="authCodeId">The auth code ID</param>
        /// <param name="scopeId">The scope ID</param>
        void AssociateAuthCodeScope(int authCodeId, int scopeId);

        /// <summary>
        ///     Get the scopes associated with an auth code
        ///     <para />
        ///     Example SQL query:
        ///     <code>
        /// SELECT scope_id FROM oauth_session_authcode_scopes WHERE oauth_session_authcode_id = :authCodeId
        /// </code>
        ///     <para />
        ///     Expected response:
        ///     <code>
        /// List(
        ///     array(
        ///         'scope_id' => (int)
        ///     ),
        ///     array(
        ///         'scope_id' => (int)
        ///     ),
        ///     ...
        /// )
        /// </code>
        /// </summary>
        /// <param name="oauthSessionAuthCodeId">The session ID</param>
        /// <returns>List{System.Int32} Scopes</returns>
        List<int> GetAuthCodeScopes(int oauthSessionAuthCodeId);

        /// <summary>
        ///     Associate a scope with an access token
        ///     <para />
        ///     Example SQL query:
        ///     <code>
        /// INSERT INTO oauth_session_token_scopes (session_access_token_id, scope_id) VALUE (:accessTokenId, :scopeId)
        /// </code>
        /// </summary>
        /// <param name="accessTokenId">The ID of the access token</param>
        /// <param name="scopeId">The ID of the scope</param>
        void AssociateScope(int accessTokenId, int scopeId);

        /// <summary>
        ///     Get all associated access tokens for an access token
        ///     <para />
        ///     Example SQL query:
        ///     <code>
        /// SELECT oauth_scopes.* FROM oauth_session_token_scopes JOIN oauth_session_access_tokens
        /// ON oauth_session_access_tokens.id = oauth_session_token_scopes.session_access_token_id
        /// JOIN oauth_scopes ON oauth_scopes.id = oauth_session_token_scopes.scope_id
        /// WHERE access_token = :accessToken
        /// </code>
        ///     <para />
        ///     Expected response:
        ///     <code>
        /// List (
        ///     GetScopesResponse(
        ///         'key'    =>  (string),
        ///         'name'   =>  (string),
        ///         'description'    =>  (string)
        ///     ),
        ///     ...
        ///     ...
        /// )
        /// </code>
        /// </summary>
        /// <param name="accessToken">The access token</param>
        /// <returns>Null or list of GetScopesResponse</returns>
        List<ScopesResponse> GetScopes(string accessToken);
    }

    /// <summary> Response for ValidateAccessToken </summary>
    public class ValidateAccessTokenResponse
    {
        /// <summary> Gets or sets the Session ID </summary>
        public int SessionID { get; set; }
        /// <summary> Gets or sets the Client ID </summary>
        public string ClientID { get; set; }
        /// <summary> Gets or sets the Owner ID </summary>
        public string OwnerID { get; set; }
        /// <summary> Gets or sets the Owner Type </summary>
        public string OwnerType { get; set; }
    }

    /// <summary> Response For ValidateAuthCode </summary>
    public class ValidateAuthCodeResponse
    {
        /// <summary> Gets or sets the Session ID </summary>
        public int SessionID { get; set; }
        /// <summary> Gets or sets the AuthCode </summary>
        public int AuthcodeID { get; set; }
    }

    /// <summary> Response for GetAccessToken </summary>
    public class GetAccessTokenResponse
    {
        /// <summary> Gets or sets the ID </summary>
        public int ID { get; set; }
        /// <summary> Gets or sets the Session ID </summary>
        public int SessionID { get; set; }
        /// <summary> Gets or sets the Access Token </summary>
        public string AccessToken { get; set; }
        /// <summary> Gets or sets the Access token Expiration </summary>
        public DateTime AccessTokenExpires { get; set; }
    }

    /// <summary> Response for GetScopes </summary>
    public class ScopesResponse
    {
        /// <summary> Gets or sets the identifier </summary>
        /// <value>The identifier</value>
        public int ID { get; set; }
        /// <summary> Gets or sets the Scope Key </summary>
        public string Scope { get; set; }
        /// <summary> Gets or sets the Scope Name </summary>
        public string Name { get; set; }
        /// <summary> Gets or sets the Scope Description </summary>
        public string Description { get; set; }
    }
}