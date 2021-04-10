namespace StoneAssemblies.MassAuth.Bank
{
    using StoneAssemblies.MassAuth.Messages;

    /// <summary>
    /// The account balance request message.
    /// </summary>
    public class AccountBalanceRequestMessage : MessageBase
    {
        /// <summary>
        /// Gets or sets the primary account number.
        /// </summary>
        public string PrimaryAccountNumber { get; set; }
    }
}