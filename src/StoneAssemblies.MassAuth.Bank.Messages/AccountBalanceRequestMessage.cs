// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountBalanceRequestMessage.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Bank.Messages
{
    using StoneAssemblies.MassAuth.Messages;

    /// <summary>
    ///     The account balance request message.
    /// </summary>
    public class AccountBalanceRequestMessage : MessageBase
    {
        /// <summary>
        ///     Gets or sets the primary account number.
        /// </summary>
        public string PrimaryAccountNumber { get; set; }
    }
}