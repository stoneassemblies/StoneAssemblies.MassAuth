namespace StoneAssemblies.MassAuth.Bank.Balance.Services.Models
{
    using System;

    public class AccountBalance
    {
        public double Balance { get; set; }

        public DateTime DateTime { get; set; }

        public string PrimaryAccountNumber { get; set; }
    }
}