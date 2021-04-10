namespace StoneAssemblies.MassAuth.Services.Attributes
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The authorize by rule.
    /// </summary>
    public class AuthorizeByRule : ServiceFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeByRule"/> class.
        /// </summary>
        public AuthorizeByRule()
            : base(typeof(AuthorizeByRuleFilter))
        {
        }
    }
}