// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceTask.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Hosting.Services.MarathonClient.Models
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    ///     The service task.
    /// </summary>
    public class ServiceTask
    {
        /// <summary>
        ///     Gets or sets the app id.
        /// </summary>
        [JsonProperty("appId")]
        public string AppId { get; set; }

        /// <summary>
        ///     Gets or sets the host.
        /// </summary>
        [JsonProperty("host")]
        public string Host { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        ///     Gets or sets the ip addresses.
        /// </summary>
        [JsonProperty("ipAddresses")]
        public List<IpAddress> IpAddresses { get; set; }

        /// <summary>
        ///     Gets or sets the ports.
        /// </summary>
        [JsonProperty("ports")]
        public List<int> Ports { get; set; }

        /// <summary>
        ///     Gets or sets the slave id.
        /// </summary>
        [JsonProperty("slaveId")]
        public string SlaveId { get; set; }

        /// <summary>
        ///     Gets or sets the started at.
        /// </summary>
        [JsonProperty("stagedAt")]
        public DateTime StagedAt { get; set; }

        /// <summary>
        ///     Gets or sets the started at.
        /// </summary>
        [JsonProperty("startedAt")]
        public DateTime StartedAt { get; set; }

        /// <summary>
        ///     Gets or sets the state.
        /// </summary>
        [JsonProperty("state")]
        public string State { get; set; }

        /// <summary>
        ///     Gets or sets the version.
        /// </summary>
        [JsonProperty("version")]
        public DateTime Version { get; set; }
    }
}