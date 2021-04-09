namespace StoneAssemblies.MassAuth.Services
{
    using System;
    using System.Reflection;

    using StoneAssemblies.MassAuth.Messages;

    public class AuthorizationRequestMessageFactory
    {
        public static AuthorizationRequestMessage From(MessageBase payload)
        {
            var makeGenericType = typeof(AuthorizationRequestMessage<>).MakeGenericType(payload.GetType());
            var authorizationMessage = Activator.CreateInstance(makeGenericType);
            var propertyInfo = makeGenericType.GetProperty("Payload", BindingFlags.Instance | BindingFlags.Public);
            propertyInfo?.SetValue(authorizationMessage, payload);
            return (AuthorizationRequestMessage) authorizationMessage;
        }
    }
}