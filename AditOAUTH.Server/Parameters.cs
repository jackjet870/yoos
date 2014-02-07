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
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;

    using AditOAUTH.Server.Grant;

    /// <summary> Defines the Parameters Class widely used across the program. </summary>
    public class Parameters : DynamicObject
    {
        /// <summary> The values </summary>
        private readonly IDictionary<string, object> values;

        /// <summary> Initializes a new instance of the <see cref="Parameters"/> class.  </summary>
        /// <param name="properties"> The properties for the dynamic class </param>
        public Parameters(IEnumerable<string> properties)
        {
            var enumerable = properties as string[] ?? properties.ToArray();
            if (!enumerable.Any()) throw new System.Exception("Initializing a dynamic class without properties is forbidden");

            this.values = new Dictionary<string, object>();

            foreach (var property in enumerable)
            {
                this.values.Add(property, null);
            }
        }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
        /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (!this.values.ContainsKey(binder.Name))
                this.values.Add(binder.Name, null);
            result = binder.Name.Equals("response_type") ? (ResponseTypeIdentifier)this.values[binder.Name] : this.values[binder.Name];
            return true;
        }

        /// <summary>
        /// Provides the implementation for operations that set member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as setting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, the <paramref name="value" /> is "Test".</param>
        /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)</returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (!this.values.ContainsKey(binder.Name))
                this.values.Add(binder.Name, null);
            this.values[binder.Name] = binder.Name.Equals("response_type") ? Enum.Parse(typeof(ResponseTypeIdentifier), value.ToString(), true) : value;
            return true;
        }
    }
}
