namespace StoneAssemblies.MassAuth.Extensions
{
    using System.Linq;
    using System.Security.Claims;

    public static class ClaimsPrincipalExtension
    {
        // TODO: Review this later also into keycloak configuration.
        public static string GetUserId(this ClaimsPrincipal @this)
        {
            var preferredUserNameClaim = @this.Claims.FirstOrDefault(claim => claim.Type.Equals("preferred_username"));
            // var emailClaim = @this.Claims.FirstOrDefault(claim => claim.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"));

            var userId = preferredUserNameClaim?.Value ?? @this.Identity.Name;
            return userId;
        }
    }
}