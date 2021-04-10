namespace StoneAssemblies.MassAuth.Rules.Extensions
{
    using System.Collections.Concurrent;
    using System.Reflection;

    /// <summary>
    ///     The object extensions.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        ///     The property info cache.
        /// </summary>
        private static readonly ConcurrentDictionary<string, PropertyInfo> PropertyInfoCache = new ConcurrentDictionary<string, PropertyInfo>();

        /// <summary>
        ///     Gets a property value.
        /// </summary>
        /// <param name="this">
        ///     The this.
        /// </param>
        /// <param name="propertyName">
        ///     The property name.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public static object GetPropertyValue(this object @this, string propertyName)
        {
            var type = @this.GetType();
            var key = $"TypeName={type.FullName};PropertyName={propertyName}";
            var propertyInfo = PropertyInfoCache.GetOrAdd(
                key,
                k => type.GetProperty(
                    propertyName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy));

            return propertyInfo?.GetValue(@this);
        }
    }
}