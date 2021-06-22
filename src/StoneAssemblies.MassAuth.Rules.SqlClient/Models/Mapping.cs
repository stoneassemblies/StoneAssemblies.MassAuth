// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Mapping.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Rules.SqlClient.Models
{
    /// <summary>
    ///     The mapping.
    /// </summary>
    public class Mapping
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Mapping" /> class.
        /// </summary>
        /// <param name="ruleName">
        ///     The name.
        /// </param>
        /// <param name="messageTypeName">
        ///     The message type.
        /// </param>
        /// <param name="storedProcedure">
        ///     The stored procedure.
        /// </param>
        public Mapping(string ruleName, string messageTypeName, string storedProcedure, int priority)
        {
            this.RuleName = ruleName;
            this.MessageTypeName = messageTypeName;
            this.StoredProcedure = storedProcedure;
            this.Priority = priority;
        }

        /// <summary>
        ///     Gets the message type.
        /// </summary>
        public string MessageTypeName { get; }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        public string RuleName { get; }

        /// <summary>
        ///     Gets the stored procedure.
        /// </summary>
        public string StoredProcedure { get; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        public int Priority { get; set; }
    }
}