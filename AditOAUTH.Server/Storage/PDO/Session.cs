// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : Valerio Fornito
// Created          : 11-05-2013
//
// Last Modified By : Valerio Fornito
// Last Modified On : 11-06-2013
// ***********************************************************************
// <copyright file="Session.cs" company="Autodistribution Italia Spa">
//     Copyright (c) Autodistribution Italia Spa. All rights reserved.
// </copyright>
// <summary>The Session Class</summary>
// ***********************************************************************

namespace AditOAUTH.Server.Storage.PDO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using _Data;

    /// <summary> Class Session. </summary>
    public class Session : ISession
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
        /// <exception cref="System.ArgumentException">
        ///     clientId
        ///     or
        ///     ownerType
        ///     or
        ///     ownerId
        /// </exception>
        public int CreateSession(string clientId, OwnerType ownerType, string ownerId)
        {
            int session;
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentException("clientId");
            if (string.IsNullOrEmpty(ownerId)) throw new ArgumentException("ownerId");
            // INSERT INTO oauth_sessions (client_id, owner_type,  owner_id) VALUE (:clientId, :ownerType, :ownerId)
            using (var adc = new AditOAUTHDataContext(Db.ConnectionString))
            {
                var os = new oauth_session { client_id = clientId, owner_type = ownerType.ToString(), owner_id = ownerId };
                adc.oauth_sessions.InsertOnSubmit(os);
                adc.SubmitChanges();
                session = os.id;
            }

            return session;
        }

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
        /// <exception cref="System.ArgumentException">
        ///     clientId
        ///     or
        ///     ownerType
        ///     or
        ///     ownerId
        /// </exception>
        public void DeleteSession(string clientId, OwnerType ownerType, string ownerId)
        {
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentException("clientId");
            if (string.IsNullOrEmpty(ownerId)) throw new ArgumentException("ownerId");
            // DELETE FROM oauth_sessions WHERE client_id = :clientId AND owner_type = :type AND owner_id = :typeId
            using (var adc = new AditOAUTHDataContext(Db.ConnectionString))
            {
                var os = adc.oauth_sessions.SingleOrDefault(o => o.client_id == clientId && o.owner_type == ownerType.ToString() && o.owner_id == ownerId);
                if (os == null) return;
                adc.oauth_sessions.DeleteOnSubmit(os);
                adc.SubmitChanges();
            }
        }

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
        /// <exception cref="System.ArgumentException">
        ///     sessionId
        ///     or
        ///     redirectUri
        /// </exception>
        public void AssociateRedirectUri(int sessionId, string redirectUri)
        {
            if (sessionId <= 0) throw new ArgumentException("sessionId");
            if (string.IsNullOrEmpty(redirectUri)) throw new ArgumentException("redirectUri");
            // INSERT INTO oauth_session_redirects (session_id, redirect_uri) VALUE (:sessionId, :redirectUri)
            using (var adc = new AditOAUTHDataContext(Db.ConnectionString))
            {
                adc.oauth_session_redirects.InsertOnSubmit(new oauth_session_redirect
                {
                    session_id = sessionId,
                    redirect_uri = redirectUri
                });
                adc.SubmitChanges();
            }
        }

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
        /// <exception cref="System.ArgumentException">
        ///     sessionId
        ///     or
        ///     accessToken
        /// </exception>
        public int AssociateAccessToken(int sessionId, string accessToken, DateTime expireTime)
        {
            int ret;
            if (sessionId <= 0) throw new ArgumentException("sessionId");
            if (string.IsNullOrEmpty(accessToken)) throw new ArgumentException("accessToken");
            // INSERT INTO oauth_session_access_tokens (session_id, access_token, access_token_expires) VALUE (:sessionId, :accessToken, :accessTokenExpire)
            using (var adc = new AditOAUTHDataContext(Db.ConnectionString))
            {
                var osat = new oauth_session_access_token
                {
                    session_id = sessionId,
                    access_token = accessToken,
                    access_token_expires = expireTime
                };
                adc.oauth_session_access_tokens.InsertOnSubmit(osat);
                adc.SubmitChanges();
                ret = osat.id;
            }

            return ret;
        }

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
        /// <exception cref="System.ArgumentException">
        ///     accessTokenId
        ///     or
        ///     refreshToken
        ///     or
        ///     clientId
        /// </exception>
        public void AssociateRefreshToken(int accessTokenId, string refreshToken, DateTime expireTime, string clientId)
        {
            if (accessTokenId <= 0) throw new ArgumentException("accessTokenId");
            if (string.IsNullOrEmpty(refreshToken)) throw new ArgumentException("refreshToken");
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentException("clientId");
            // INSERT INTO oauth_session_refresh_tokens (session_access_token_id, refresh_token, refresh_token_expires, client_id) VALUE (:accessTokenId, :refreshToken, :expireTime, :clientId)
            using (var adc = new AditOAUTHDataContext(Db.ConnectionString))
            {
                adc.oauth_session_refresh_tokens.InsertOnSubmit(new oauth_session_refresh_token
                {
                    session_access_token_id = accessTokenId,
                    refresh_token = refreshToken,
                    refresh_token_expires = expireTime,
                    client_id = clientId
                });
                adc.SubmitChanges();
            }
        }

        /// <summary>
        ///     Associate an authorization code with a session
        ///     <para />
        ///     Example SQL query:
        ///     <code>
        /// INSERT INTO oauth_session_authcodes (session_id, auth_code, auth_code_expires)
        /// VALUE (:sessionId, :authCode, :authCodeExpires)
        /// </code>
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <param name="authCode">The authorization code</param>
        /// <param name="expireTime">Timestamp of the access token expiry time</param>
        /// <returns>System.Int32. The auth code ID</returns>
        /// <exception cref="System.ArgumentException">
        ///     sessionId
        ///     or
        ///     authCode
        /// </exception>
        public int AssociateAuthCode(int sessionId, string authCode, DateTime expireTime)
        {
            int ac;
            if (sessionId <= 0) throw new ArgumentException("sessionId");
            if (string.IsNullOrEmpty(authCode)) throw new ArgumentException("authCode");
            // INSERT INTO oauth_session_authcodes (session_id, auth_code, auth_code_expires) VALUE (:sessionId, :authCode, :authCodeExpires)
            using (var adc = new AditOAUTHDataContext(Db.ConnectionString))
            {
                var osa = new oauth_session_authcode
                {
                    session_id = sessionId,
                    auth_code = authCode,
                    auth_code_expires = expireTime
                };
                adc.oauth_session_authcodes.InsertOnSubmit(osa);
                adc.SubmitChanges();

                ac = osa.id;
            }

            return ac;
        }

        /// <summary>
        ///     Remove an associated authorization token from a session
        ///     <para />
        ///     Example SQL query:
        ///     <code>
        /// DELETE FROM oauth_session_authcodes WHERE session_id = :sessionId
        /// </code>
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <exception cref="System.ArgumentException">Raised when sessionId is 0 or less</exception>
        public void RemoveAuthCode(int sessionId)
        {
            if (sessionId <= 0) throw new ArgumentException("sessionId");
            // DELETE FROM oauth_session_authcodes WHERE session_id = :sessionId
            using (var adc = new AditOAUTHDataContext(Db.ConnectionString))
            {
                var os = adc.oauth_session_authcodes.SingleOrDefault(o => o.session_id == sessionId);
                if (os == null) return;
                adc.oauth_session_authcodes.DeleteOnSubmit(os);
                adc.SubmitChanges();
            }
        }

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
        /// <exception cref="System.ArgumentException">
        ///     clientId
        ///     or
        ///     redirectUri
        ///     or
        ///     authCode
        /// </exception>
        /// <exception cref="System.Exception">Too many result in Session[ValidateAuthCode]</exception>
        public ValidateAuthCodeResponse ValidateAuthCode(string clientId, string redirectUri, string authCode)
        {
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentException("clientId");
            if (string.IsNullOrEmpty(redirectUri)) throw new ArgumentException("redirectUri");
            if (string.IsNullOrEmpty(authCode)) throw new ArgumentException("authCode");

            ValidateAuthCodeResponse vacr = null;

            // SELECT oauth_sessions.id AS session_id, oauth_session_authcodes.id AS authcode_id
            // FROM oauth_sessions 
            // JOIN oauth_session_authcodes ON oauth_session_authcodes.session_id= oauth_sessions.id 
            // JOIN oauth_session_redirects ON oauth_session_redirects.session_id= oauth_sessions.id 
            // WHERE oauth_sessions.client_id = :clientId 
            // AND oauth_session_authcodes.auth_code= :authCode 
            // AND  oauth_session_authcodes.auth_code_expires >= :time 
            // AND oauth_session_redirects.redirect_uri = :redirectUri
            using (var adc = new AditOAUTHDataContext(Db.ConnectionString))
            {
                var validate = from os in adc.oauth_sessions
                               join osa in adc.oauth_session_authcodes on os.id equals osa.session_id
                               join osr in adc.oauth_session_redirects on os.id equals osr.session_id
                               where os.client_id == clientId
                                     && osa.auth_code == authCode
                                     && osa.auth_code_expires >= DateTime.Now
                                     && osr.redirect_uri == redirectUri
                               select new ValidateAuthCodeResponse
                               {
                                   SessionID = os.id,
                                   AuthcodeID = osa.id,
                               };

                if (validate.Count() > 1) throw new Exception("Too many result in Session[ValidateAuthCode]");
                if (validate.Any())
                    vacr = validate.SingleOrDefault();
            }

            return vacr;
        }

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
        /// <exception cref="System.ArgumentException">Raised when accessToken is empty </exception>
        /// <exception cref="System.Exception">Too many result in Session[ValidateAccessToken]</exception>
        public ValidateAccessTokenResponse ValidateAccessToken(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken)) throw new ArgumentException("accessToken");

            ValidateAccessTokenResponse vatr = null;

            // SELECT session_id, oauth_sessions.client_id, oauth_sessions.owner_id, oauth_sessions.owner_type 
            // FROM oauth_session_access_tokens 
            // JOIN oauth_sessions ON oauth_sessions.id = session_id 
            // WHERE access_token = :accessToken 
            // AND access_token_expires >= :time
            using (var adc = new AditOAUTHDataContext(Db.ConnectionString))
            {
                var validate = from osat in adc.oauth_session_access_tokens
                               join os in adc.oauth_sessions on osat.session_id equals os.id
                               where osat.access_token == accessToken
                                     && osat.access_token_expires >= DateTime.Now
                               select new ValidateAccessTokenResponse
                               {
                                   SessionID = osat.session_id,
                                   ClientID = os.client_id,
                                   OwnerID = os.owner_id,
                                   OwnerType = (OwnerType)Enum.Parse(typeof(OwnerType), os.owner_type)
                               };

                if (validate.Count() > 1) throw new Exception("Too many result in Session[ValidateAccessToken]");
                if (validate.Any())
                    vatr = validate.SingleOrDefault();
            }

            return vatr;
        }

        /// <summary>
        ///     Removes a refresh token
        ///     <para />
        ///     Example SQL query:
        ///     <code>
        /// DELETE FROM oauth_session_refresh_tokens WHERE refresh_token = :refreshToken
        /// </code>
        /// </summary>
        /// <param name="refreshToken">The refresh token to be removed</param>
        /// <exception cref="System.ArgumentException">Raised when refreshToken is empty</exception>
        public void RemoveRefreshToken(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken)) throw new ArgumentException("refreshToken");
            // DELETE FROM oauth_session_refresh_tokens WHERE refresh_token = :refreshToken
            using (var adc = new AditOAUTHDataContext(Db.ConnectionString))
            {
                var os = adc.oauth_session_refresh_tokens.SingleOrDefault(o => o.refresh_token == refreshToken);
                if (os == null) return;
                adc.oauth_session_refresh_tokens.DeleteOnSubmit(os);
                adc.SubmitChanges();
            }
        }

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
        /// <exception cref="System.ArgumentException">
        ///     refreshToken
        ///     or
        ///     clientId
        /// </exception>
        public int ValidateRefreshToken(string refreshToken, string clientId)
        {
            if (string.IsNullOrEmpty(refreshToken)) throw new ArgumentException("refreshToken");
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentException("clientId");

            var vrt = 0;

            // SELECT session_access_token_id 
            // FROM oauth_session_refresh_tokens 
            // WHERE refresh_token = :refreshToken 
            // AND client_id = :clientId 
            // AND refresh_token_expires >= :time
            using (var adc = new AditOAUTHDataContext(Db.ConnectionString))
            {
                var validate = adc.oauth_session_refresh_tokens.SingleOrDefault(o => o.refresh_token == refreshToken && o.client_id == clientId && o.refresh_token_expires >= DateTime.Now);
                if (validate != null)
                    vrt = validate.session_access_token_id;
            }

            return vrt;
        }

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
        /// <exception cref="System.ArgumentException">Raised when accessTokenId is 0 or less </exception>
        /// <exception cref="System.Exception">Too many result in Session[ValidateAccessToken]</exception>
        public GetAccessTokenResponse GetAccessToken(int accessTokenId)
        {
            if (accessTokenId <= 0) throw new ArgumentException("accessTokenId");

            GetAccessTokenResponse gat = null;

            // SELECT * FROM oauth_session_access_tokens WHERE id = :accessTokenId
            using (var adc = new AditOAUTHDataContext(Db.ConnectionString))
            {
                var get = adc.oauth_session_access_tokens.Where(o => o.id == accessTokenId).Select(o => new GetAccessTokenResponse
                    {
                        ID = o.id,
                        SessionID = o.session_id,
                        AccessToken = o.access_token,
                        AccessTokenExpires = o.access_token_expires
                    });

                if (get.Count() > 1) throw new Exception("Too many result in Session[ValidateAccessToken]");
                if (get.Any())
                    gat = get.SingleOrDefault();
            }

            return gat;
        }

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
        /// <exception cref="System.ArgumentException">
        ///     authCodeId
        ///     or
        ///     scopeId
        /// </exception>
        public void AssociateAuthCodeScope(int authCodeId, int scopeId)
        {
            if (authCodeId <= 0) throw new ArgumentException("authCodeId");
            if (scopeId <= 0) throw new ArgumentException("scopeId");

            // INSERT INTO oauth_session_authcode_scopes (oauth_session_authcode_id, scope_id) VALUES (:authCodeId, :scopeId)
            using (var adc = new AditOAUTHDataContext(Db.ConnectionString))
            {
                adc.oauth_session_authcode_scopes.InsertOnSubmit(new oauth_session_authcode_scope
                {
                    oauth_session_authcode_id = authCodeId,
                    scope_id = scopeId
                });
                adc.SubmitChanges();
            }
        }

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
        /// <exception cref="System.ArgumentException">Raised when oauthSessionAuthCodeId is 0 or less</exception>
        public List<int> GetAuthCodeScopes(int oauthSessionAuthCodeId)
        {
            if (oauthSessionAuthCodeId <= 0) throw new ArgumentException("oauthSessionAuthCodeId");

            List<int> rt;
            // SELECT scope_id FROM oauth_session_authcode_scopes WHERE oauth_session_authcode_id = :authCodeId
            using (var adc = new AditOAUTHDataContext(Db.ConnectionString))
            {
                rt = adc.oauth_session_authcode_scopes.Where(o => o.oauth_session_authcode_id == oauthSessionAuthCodeId).Select(o => o.scope_id).ToList();
            }

            return rt;
        }

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
        /// <exception cref="System.ArgumentException">
        ///     accessTokenId
        ///     or
        ///     scopeId
        /// </exception>
        public void AssociateScope(int accessTokenId, int scopeId)
        {
            if (accessTokenId <= 0) throw new ArgumentException("accessTokenId");
            if (scopeId <= 0) throw new ArgumentException("scopeId");

            // INSERT INTO oauth_session_token_scopes (session_access_token_id, scope_id) VALUE (:accessTokenId, :scopeId)
            using (var adc = new AditOAUTHDataContext(Db.ConnectionString))
            {
                adc.oauth_session_token_scopes.InsertOnSubmit(new oauth_session_token_scope
                {
                    session_access_token_id = accessTokenId,
                    scope_id = scopeId
                });
                adc.SubmitChanges();
            }
        }

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
        /// <returns>Null or list of ScopesResponse</returns>
        /// <exception cref="System.ArgumentException">Raised when accessToken is empty</exception>
        public List<ScopesResponse> GetScopes(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken)) throw new ArgumentException("accessToken");

            List<ScopesResponse> gsr = null;

            // SELECT oauth_scopes.*
            // FROM oauth_session_token_scopes 
            // JOIN oauth_session_access_tokens ON oauth_session_access_tokens.id = oauth_session_token_scopes.session_access_token_id 
            // JOIN oauth_scopes ON oauth_scopes.id = oauth_session_token_scopes.scope_id 
            // WHERE access_token = :accessToken
            using (var adc = new AditOAUTHDataContext(Db.ConnectionString))
            {
                var get = from osts in adc.oauth_session_token_scopes
                          join osat in adc.oauth_session_access_tokens on osts.session_access_token_id equals osat.id
                          join os in adc.oauth_scopes on osts.scope_id equals os.id
                          where osat.access_token == accessToken
                          select new ScopesResponse
                          {
                              ID = os.id,
                              Scope = os.scope,
                              Name = os.name,
                              Description = os.description
                          };
                if (get.Any())
                    gsr = get.ToList();
            }

            return gsr;
        }
    }
}