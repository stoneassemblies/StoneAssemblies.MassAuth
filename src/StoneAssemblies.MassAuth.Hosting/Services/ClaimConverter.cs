// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClaimConverter.cs" company="Stone Assemblies">
// Copyright Â© 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Hosting.Services
{
    using System;
    using System.Security.Claims;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    ///     The claim converter.
    /// </summary>
    public class ClaimConverter : JsonConverter
    {
        /// <summary>
        ///     Gets a value indicating whether the converter can write.
        /// </summary>
        public override bool CanWrite => false;

        /// <summary>
        ///     Determines if can convert to <paramref name="objectType" />.
        /// </summary>
        /// <param name="objectType">
        ///     The object type.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Claim);
        }

        /// <summary>
        ///     Read from json.
        /// </summary>
        /// <param name="reader">
        ///     The reader.
        /// </param>
        /// <param name="objectType">
        ///     The object type.
        /// </param>
        /// <param name="existingValue">
        ///     The existing value.
        /// </param>
        /// <param name="serializer">
        ///     The serializer.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
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

        /// <summary>
        ///     Write a json.
        /// </summary>
        /// <param name="writer">
        ///     The writer.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="serializer">
        ///     The serializer.
        /// </param>
        /// <exception cref="NotImplementedException">
        ///     This method is not implemented.
        /// </exception>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}