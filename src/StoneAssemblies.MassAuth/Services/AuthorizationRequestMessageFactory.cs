namespace StoneAssemblies.MassAuth.Services
{
    using System;
    using System.Reflection;

    using StoneAssemblies.MassAuth.Messages;

    /// <summary>
    /// The authorization request message factory.
    /// </summary>
    public static class AuthorizationRequestMessageFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="AuthorizationRequestMessage"/>.
        /// </summary>
        /// <param name="payload">
        /// The payload.
        /// </param>
        /// <returns>
        /// The <see cref="AuthorizationRequestMessage"/>.
        /// </returns>
        public static AuthorizationRequestMessage From(MessageBase payload)
        {
            var makeGenericType = typeof(AuthorizationRequestMessage<>).MakeGenericType(payload.GetType());
            var authorizationMessage = Activator.CreateInstance(makeGenericType);
            var propertyInfo = makeGenericType.GetProperty("Payload", BindingFlags.Instance | BindingFlags.Public);
            propertyInfo?.SetValue(authorizationMessage, payload);
            return (AuthorizationRequestMessage)authorizationMessage;
        }
    }
}