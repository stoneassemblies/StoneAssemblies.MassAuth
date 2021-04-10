// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Messages.Extensions
{
    using System;

    /// <summary>
    ///     The type extensions.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        ///     Gets a type flat name.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <returns>
        ///     The flat name.
        /// </returns>
        public static string GetFlatName(this Type type)
        {
            // TODO: Improve this later.
            var flatName = type.Name.Split('`')[0];
            foreach (var genericArgument in type.GetGenericArguments())
            {
                flatName += "-" + genericArgument.GetFlatName();
            }

            return flatName;
        }
    }
}