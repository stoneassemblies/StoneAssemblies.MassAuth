namespace StoneAssemblies.MassAuth.Extensions
{
    using System;
    using System.Reflection;

    /// <summary>
    /// The method info extensions.
    /// </summary>
    public static class MethodInfoExtensions
    {
        /// <summary>
        /// The try make generic method.
        /// </summary>
        /// <param name="methodInfo">
        /// The method info.
        /// </param>
        /// <param name="genericMethod">
        /// The generic method.
        /// </param>
        /// <param name="parametersTypes">
        /// The parameters types.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool TryMakeGenericMethod(this MethodInfo methodInfo, out MethodInfo genericMethod, params Type[] parametersTypes)
        {
            genericMethod = null;

            try
            {
                genericMethod = methodInfo.MakeGenericMethod(parametersTypes);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}