using System.Linq;
using System.Reflection;

namespace JumpsellerSync.Common.Util.Services
{
    public static class ServiceUtilities
    {
        /// <summary>
        /// Creates an array of property names that should not be included
        /// in an update operation.
        /// </summary>
        /// <typeparam name="TModel">The database model</typeparam>
        /// <param name="include">
        ///     Name of the properties that are going to be updated. 
        ///     Note that navigation properties should be present or
        ///     you'll get a runtime error thrown by EF.
        /// </param>
        /// <returns>The generated array with properties to be ignored by the update operation.</returns>
        public static string[] CreateSkipPropertiesArray<TModel>(params string[] include)
        {
            include ??= new string[0];
            return typeof(TModel)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(pi => pi.Name)
                .Where(n => !include.Contains(n))
                .ToArray();
        }
    }
}
