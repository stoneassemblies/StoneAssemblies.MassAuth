
namespace StoneAssemblies.MassAuth.Hosting.Services.MarathonClient.Models
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    ///     The service tasks.
    /// </summary>
    public class ServiceTasks
    {
        /// <summary>
        ///     Gets or sets the tasks.
        /// </summary>
        [JsonProperty("tasks")]
        public List<ServiceTask> Tasks { get; set; }
    }
}