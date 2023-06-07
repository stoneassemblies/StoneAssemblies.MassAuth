// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BalanceController.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Bank.Balance.Services.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using StoneAssemblies.MassAuth.Bank.Balance.Services.Models;
    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Services.Attributes;

    /// <summary>
    ///     The balance controller.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BalanceController : ControllerBase
    {
        /// <summary>
        ///     Request the account balance for an account.
        /// </summary>
        /// <param name="accountBalanceRequestMessage">
        ///     The account balance request message.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [AuthorizeByRule]
        [Authorize]
        [HttpGet("[action]")]
        public Task<AccountBalance> Get([FromQuery] AccountBalanceRequestMessage accountBalanceRequestMessage)
        {
            var random = new Random();

            var accountBalance = new AccountBalance
            {
                PrimaryAccountNumber = accountBalanceRequestMessage.PrimaryAccountNumber,
                Balance = random.NextDouble() * random.Next(0, 1000),
                DateTime = DateTime.Now,
            };

            return Task.FromResult(accountBalance);
        }

        /// <summary>
        /// Gets a balance.
        /// </summary>
        /// <param name="primaryAccountNumber">
        /// The primary account number.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [AuthorizeByRule(typeof(AccountBalanceRequestMessage))]
        [HttpGet("[action]")]
        public Task<AccountBalance> Get1(string primaryAccountNumber)
        {
            var random = new Random();

            var accountBalance = new AccountBalance
            {
                PrimaryAccountNumber = primaryAccountNumber,
                Balance = random.NextDouble() * random.Next(0, 1000),
                DateTime = DateTime.Now,
            };

            return Task.FromResult(accountBalance);
        }

        /// <summary>
        /// Gets a balance.
        /// </summary>
        /// <param name="primaryAccountNumber">
        /// The primary account number.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [AuthorizeByRule(typeof(AccountBalanceRequestMessage))]
        [HttpGet("[action]/{primaryAccountNumber}")]
        public Task<AccountBalance> Get(string primaryAccountNumber)
        {
            var random = new Random();

            var accountBalance = new AccountBalance
            {
                PrimaryAccountNumber = primaryAccountNumber,
                Balance = random.NextDouble() * random.Next(0, 1000),
                DateTime = DateTime.Now,
            };

            return Task.FromResult(accountBalance);
        }
    }
}