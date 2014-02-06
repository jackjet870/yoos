// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : vfornito
// Created          : 11-05-2013
//
// Last Modified By : vfornito
// Last Modified On : 11-27-2013
// ***********************************************************************
// <copyright file="ClientException.cs" company="Autodistribution Italia Spa">
//     Copyright (c) Autodistribution Italia Spa. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace AditOAUTH.Server.Exception
{
    using AditOAUTH.Server.HTTPError;

    /// <summary> Class ClientException </summary>
    public class ClientException : OAuth2Exception
    {
        /// <summary> Gets the error type. </summary>
        /// <value>The error type.</value>
        public HTTPErrorType Type { get; internal set; }
        /// <summary> Gets a message that describes the current exception. </summary>
        /// <value>The message.</value>
        /// <returns>The error message that explains the reason for the exception, or an empty string("").</returns>
        public new string Message { get; internal set; }
        /// <summary> Gets the HTTP error code. </summary>
        /// <value>The code.</value>
        public int Code { get; internal set; }

        /// <summary> The error collection  </summary>
        private static HTTPErrorCollection.HTTPError err;

        /// <summary> Initializes a new instance of the <see cref="ClientException" /> class </summary>
        /// <param name="type">Http Error type.</param>
        /// <param name="property">The property which caused the exception.</param>
        public ClientException(HTTPErrorType type, string property = null)
            : base(FormatMessage(property))
        {
            err = HTTPErrorCollection.Instance[type];
            this.Type = err.Type;
            this.Message = FormatMessage(property);
            this.Code = err.HTTPStatusCode;
        }

        /// <summary> Formats the message for error output
        /// </summary>
        /// <param name="property">The property which caused the exception.</param>
        /// <returns>System.String with the formatted message</returns>
        private static string FormatMessage(string property)
        {
            return string.Format(err.Message, property);
        }
    }
}
