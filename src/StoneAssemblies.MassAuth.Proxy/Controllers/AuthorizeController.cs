// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizeController.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Proxy.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using StoneAssemblies.MassAuth.Services.Attributes;

    /// <summary>
    ///     The authorize controller.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     The message type.
    /// </typeparam>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizeController<TMessage> : ControllerBase
    {
        /// <summary>
        ///     Request an authorization.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [AuthorizeByRule]
        [HttpDelete]
        public Task Delete([FromQuery] TMessage message)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Request an authorization.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [AuthorizeByRule]
        [HttpGet]
        public Task Get([FromQuery] TMessage message)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Request an authorization.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [AuthorizeByRule]
        [HttpPost]
        public Task Post([FromBody] TMessage message)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Request an authorization.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [AuthorizeByRule]
        [HttpPut]
        public Task Put([FromBody] TMessage message)
        {
            return Task.CompletedTask;
        }
    }
}