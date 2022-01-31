// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluationResult.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Rules.Interfaces
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The evaluation result.
    /// </summary>
    public class EvaluationResult
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EvaluationResult" /> class.
        /// </summary>
        /// <param name="succeeded">
        ///     The succeeded.
        /// </param>
        /// <param name="data">
        ///     The data.
        /// </param>
        private EvaluationResult(bool succeeded, Dictionary<string, object> data)
        {
            this.Succeeded = succeeded;
            this.Data = data;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        public Dictionary<string, object> Data { get; }

        /// <summary>
        ///     Gets a value indicating whether succeeded.
        /// </summary>
        public bool Succeeded { get; }

        /// <summary>
        ///     The error.
        /// </summary>
        /// <param name="data">
        ///     The data.
        /// </param>
        /// <returns>
        ///     The <see cref="EvaluationResult" />.
        /// </returns>
        public static EvaluationResult Error(Dictionary<string, object> data = null)
        {
            return new EvaluationResult(false, data);
        }

        /// <summary>
        ///     The success.
        /// </summary>
        /// <returns>
        ///     The <see cref="EvaluationResult" />.
        /// </returns>
        public static EvaluationResult Success()
        {
            return new EvaluationResult(true, null);
        }
    }
}