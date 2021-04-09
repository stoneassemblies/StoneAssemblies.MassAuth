namespace StoneAssemblies.MassAuth.Messages
{
    public class AuthorizationResponseMessage
    {
        public bool IsAuthorized { get; set; }

        // TODO: Improve this name
        public string Reason { get; set; }
    }
}