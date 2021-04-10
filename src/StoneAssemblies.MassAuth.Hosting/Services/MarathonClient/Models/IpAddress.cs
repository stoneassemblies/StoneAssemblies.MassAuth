// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpAddress.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Hosting.Services.MarathonClient.Models
{
    using Newtonsoft.Json;

    /// <summary>
    ///     The ip address.
    /// </summary>
    public class IpAddress
    {
        /// <summary>
        ///     Gets or sets the ip address.
        /// </summary>
        [JsonProperty("ipAddress")]
        public string Ip { get; set; }

        /// <summary>
        ///     Gets or sets the protocol.
        /// </summary>
        [JsonProperty("protocol")]
        public string Protocol { get; set; }
    }
}