namespace StoneAssemblies.MassAuth.Bank.Balance.Services.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using StoneAssemblies.MassAuth.Bank.Balance.Services.Models;
    using StoneAssemblies.MassAuth.Services.Attributes;

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
        [HttpGet]
        public Task<AccountBalance> AccountBalanceRequest(
            [FromQuery] AccountBalanceRequestMessage accountBalanceRequestMessage)
        {
            var random = new Random();

            var accountBalance = new AccountBalance
                                     {
                                         PrimaryAccountNumber = accountBalanceRequestMessage.PrimaryAccountNumber,
                                         Balance = random.NextDouble() * random.Next(0, 1000),
                                         DateTime = DateTime.Now
                                     };

            return Task.FromResult(accountBalance);
        }
    }
}