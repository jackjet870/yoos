// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : Valerio Fornito
// Created          : 11-15-2013
//
// Last Modified By : Valerio Fornito
// Last Modified On : 11-15-2013
// ***********************************************************************
// <copyright file="Request.cs" company="Autodistribution Italia Spa">
//     Copyright (c) Autodistribution Italia Spa. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace AditOAUTH.Server.Util
{
    using System.Collections.Specialized;
    using System.Text.RegularExpressions;
    using System.Web;

    /// <summary> Defines a Request object </summary>
    public class Request : IRequest
    {
        /// <summary> The _cookies </summary>
        private readonly HttpCookieCollection cookies;

        /// <summary> The _files </summary>
        private readonly HttpFileCollection files;

        /// <summary> The _get </summary>
        private readonly NameValueCollection get;

        /// <summary> The _headers </summary>
        private readonly NameValueCollection headers;

        /// <summary> The _post </summary>
        private readonly NameValueCollection post;

        /// <summary> The _server </summary>
        private readonly NameValueCollection server;

        /// <summary> Initializes a new instance of the <see cref="Request" /> class. </summary>
        /// <param name="get">The get</param>
        /// <param name="post">The post</param>
        /// <param name="cookies">The cookies</param>
        /// <param name="files">The files</param>
        /// <param name="server">The server</param>
        /// <param name="headers">The headers</param>
        public Request(NameValueCollection get = null, NameValueCollection post = null, HttpCookieCollection cookies = null, HttpFileCollection files = null, NameValueCollection server = null, NameValueCollection headers = null)
        {
            this.get = get ?? new NameValueCollection();
            this.post = post ?? new NameValueCollection();
            this.cookies = cookies ?? new HttpCookieCollection();
            this.files = files;
            this.server = server ?? new NameValueCollection();

            if (headers == null) this.headers = this.ReadHeaders();
        }

        /// <summary> Gets the specified index from get server variable. </summary>
        /// <param name="index">The index to search</param>
        /// <param name="defaultValue">The default value if the index does not exists</param>
        /// <returns>String representing the value from the server variable at index</returns>
        public string Get(string index, string defaultValue = null)
        {
            return this.get[index] ?? defaultValue;
        }

        /// <summary> Gets the specified index from post server variable </summary>
        /// <param name="index">The index to search</param>
        /// <param name="defaultValue">The default value if the index does not exists</param>
        /// <returns>String representing the value from the server variable at index</returns>
        public string Post(string index, string defaultValue = null)
        {
            return this.post[index] ?? defaultValue;
        }

        /// <summary> Gets the specified index from cookies server variable </summary>
        /// <param name="index">The index to search</param>
        /// <param name="defaultValue">The default value if the index does not exists</param>
        /// <returns>HttpCookie representing the value from the server variable at index</returns>
        public HttpCookie Cookie(string index, HttpCookie defaultValue = null)
        {
            return this.cookies[index] ?? defaultValue;
        }

        /// <summary> Gets the specified index from files server variable. </summary>
        /// <param name="index">The index to search</param>
        /// <param name="defaultValue">The default value if the index does not exists</param>
        /// <returns>System.Object representing the value from the server variable at index</returns>
        public HttpPostedFile File(string index, HttpPostedFile defaultValue = null)
        {
            return this.files[index] ?? defaultValue;
        }

        /// <summary> Get the specified index from server server variable </summary>
        /// <param name="index">The index to search</param>
        /// <param name="defaultValue">The default value if the index does not exists</param>
        /// <returns>System.Object representing the value from the server variable at index</returns>
        public string Server(string index, string defaultValue = null)
        {
            return this.server[index] ?? defaultValue;
        }

        /// <summary> Headers the specified index from header server variable </summary>
        /// <param name="index">The index to search</param>
        /// <param name="defaultValue">The default value if the index does not exists</param>
        /// <returns>System.Object representing the value from the server variable at index</returns>
        public string Header(string index, string defaultValue = null)
        {
            return this.headers[index] ?? defaultValue;
        }

        /// <summary> Builds from globals server variables </summary>
        /// <returns>A new <see cref="Request" /></returns>
        public static Request BuildFromGlobals()
        {
            // define a context request
            if (HttpContext.Current == null) return null;
            var cr = HttpContext.Current.Request;
            return new Request(cr.QueryString, cr.Form, cr.Cookies, cr.Files, cr.ServerVariables, cr.Headers);
        }

        /// <summary> Reads the headers </summary>
        /// <returns>NameValueCollection of the headers</returns>
        private NameValueCollection ReadHeaders()
        {
            var rh = new NameValueCollection();
            foreach (string kv in this.server)
            {
                if (kv.Substring(0, 5).ToLower() != "http_") continue;
                var name = kv.Substring(5);
                name = name.Replace(' ', '-');
                name = name.Replace('_', ' ');
                name = name.ToLower();
                Regex.Replace(name, "(?:^|\\s)\\w", m => m.Value.ToUpper());
                rh[name] = this.server[kv];
            }

            return rh;
        }
    }
}