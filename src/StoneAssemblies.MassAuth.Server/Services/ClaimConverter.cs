namespace StoneAssemblies.MassAuth.Engine.Services
{
    using System;
    using System.Security.Claims;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal class ClaimConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Claim);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var type = (string)jsonObject["type"];
            var value = (string)jsonObject["value"];
            var valueType = (string)jsonObject["valueType"];
            var issuer = (string)jsonObject["issuer"];
            var originalIssuer = (string)jsonObject["originalIssuer"];
            return new Claim(type, value, valueType, issuer, originalIssuer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}