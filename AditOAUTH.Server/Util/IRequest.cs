// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : vfornito
// Created          : 11-06-2013
//
// Last Modified By : vfornito
// Last Modified On : 11-27-2013
// ***********************************************************************
// <copyright file="IRequest.cs" company="Autodistribution Italia Spa">
//     Copyright (c) Autodistribution Italia Spa. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace AditOAUTH.Server.Util
{
    using System.Web;

    /// <summary> Interface IRequest </summary>
    public interface IRequest
    {
        /// <summary> Gets the specified index. </summary>
        /// <param name="index">The index</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>System.String the value of get, or defaultvalue</returns>
        string Get(string index, string defaultValue = null);

        /// <summary> Posts the specified index. </summary>
        /// <param name="index">The index</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>System.String the value of post, or defaultvalue</returns>
        string Post(string index, string defaultValue = null);

        /// <summary> Cookies the specified index. </summary>
        /// <param name="index">The index</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>HttpCookie the value of cookie, or defaultvalue</returns>
        HttpCookie Cookie(string index, HttpCookie defaultValue = null);

        /// <summary> Files the specified index. </summary>
        /// <param name="index">The index</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>HttpPostedFile the value of file, or defaultvalue</returns>
        HttpPostedFile File(string index, HttpPostedFile defaultValue = null);

        /// <summary> Servers the specified index. </summary>
        /// <param name="index">The index</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>System.String the value of server, or defaultvalue</returns>
        string Server(string index, string defaultValue = null);

        /// <summary> Headers the specified index. </summary>
        /// <param name="index">The index</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>System.String the value of header, or defaultvalue</returns>
        string Header(string index, string defaultValue = null);
    }
}