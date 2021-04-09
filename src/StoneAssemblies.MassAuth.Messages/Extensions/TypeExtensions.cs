namespace StoneAssemblies.MassAuth.Messages.Extensions
{
    using System;

    public static class TypeExtensions
    {
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