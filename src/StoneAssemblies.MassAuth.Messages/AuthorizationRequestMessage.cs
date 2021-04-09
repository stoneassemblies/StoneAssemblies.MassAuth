namespace StoneAssemblies.MassAuth.Messages
{
    using System.Collections.Generic;
    using System.Security.Claims;

    public interface IAuthorizationRequestMessage
    {
        IEnumerable<Claim> Claims { get; set; }

        string Username { get; }
    }

    public class AuthorizationRequestMessage : IAuthorizationRequestMessage
    {
        public IEnumerable<Claim> Claims { get; set; }

        public string Username { get; set; }
    }

    public class AuthorizationRequestMessage<TPayload> : AuthorizationRequestMessage,
                                                         IAuthorizationRequestMessage<TPayload>
        where TPayload : MessageBase
    {
        public TPayload Payload { get; set; }

        public TPayload GetPayload()
        {
            return this.Payload;
        }
    }

    public interface IAuthorizationRequestMessage<out TPayload> : IAuthorizationRequestMessage
        where TPayload : MessageBase
    {
        TPayload Payload { get; }
    }
}