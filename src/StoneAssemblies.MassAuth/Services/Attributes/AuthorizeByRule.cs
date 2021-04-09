namespace StoneAssemblies.MassAuth.Services.Attributes
{
    using Microsoft.AspNetCore.Mvc;

    public class AuthorizeByRule : ServiceFilterAttribute
    {
        public AuthorizeByRule()
            : base(typeof(AuthorizeByRuleFilter))
        {
        }
    }
}