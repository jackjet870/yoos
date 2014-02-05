// ***********************************************************************
// Assembly         : AditOAUTH.Server
// Author           : vfornito
// Created          : 02-04-2014
//
// Last Modified By : vfornito
// Last Modified On : 02-05-2014
// ***********************************************************************
// <copyright file="Parameters.cs" company="Autodistribution Italia Spa">
//     Copyright (c) Autodistribution Italia Spa. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace AditOAUTH.Server
{
    using System.Collections.Generic;
    using System.Dynamic;

    /// <summary> Defines the Parameters Class widely used across the program. </summary>
    public class Parameters : DynamicObject
    {
        /// <summary> The values </summary>
        private readonly IDictionary<string, object> values;

        /// <summary> Initializes a new instance of the <see cref="Parameters" /> class. </summary>
        public Parameters()
        {
            this.values = new Dictionary<string, object>
                              {
                                  { "client_id", null },
                                  { "client_secret", null },
                                  { "username", null },
                                  { "password", null },
                                  { "scope", null },
                                  { "redirect_uri", null },
                                  { "response_type", null },
                                  { "state", null },
                                  { "code", null },
                                  { "refresh_token", null },
                                  { "client_details", null },
                                  { "user_id", null },
                                  { "scopes", null }
                              };
        }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
        /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (this.values.ContainsKey(binder.Name))
            {
                result = this.values[binder.Name];
                return true;
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Provides the implementation for operations that set member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as setting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, the <paramref name="value" /> is "Test".</param>
        /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)</returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (!this.values.ContainsKey(binder.Name)) return false;
            this.values[binder.Name] = value.ToString().ToUpper();
            return true;
        }
    }
}
