// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluationResult.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Rules.Interfaces
{
    using System;

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
        /// <param name="description">
        ///     The description.
        /// </param>
        private EvaluationResult(bool succeeded, string description)
        {
            this.Succeeded = succeeded;
            this.Description = description;
        }

        /// <summary>
        ///     Gets the description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     Gets a value indicating whether succeeded.
        /// </summary>
        public bool Succeeded { get; }

        /// <summary>
        ///     The error.
        /// </summary>
        /// <param name="description">
        ///     The description.
        /// </param>
        /// <returns>
        ///     The <see cref="EvaluationResult" />.
        /// </returns>
        public static EvaluationResult Error(string description = "")
        {
            return new EvaluationResult(false, description);
        }

        /// <summary>
        ///     The success.
        /// </summary>
        /// <returns>
        ///     The <see cref="EvaluationResult" />.
        /// </returns>
        public static EvaluationResult Success()
        {
            return new EvaluationResult(true, string.Empty);
        }
    }
}