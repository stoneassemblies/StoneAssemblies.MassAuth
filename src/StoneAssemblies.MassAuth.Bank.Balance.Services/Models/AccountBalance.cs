﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountBalance.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Bank.Balance.Services.Models
{
    using System;

    /// <summary>
    ///     The account balance.
    /// </summary>
    public class AccountBalance
    {
        /// <summary>
        ///     Gets or sets the balance.
        /// </summary>
        public double Balance { get; set; }

        /// <summary>
        ///     Gets or sets the date time.
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        ///     Gets or sets the primary account number.
        /// </summary>
        public string PrimaryAccountNumber { get; set; }
    }
}