namespace Boondocks.Cli.ExtensionMethods
{
    using System.Linq;
    using Services.Contracts.Interfaces;
    using System;
    using System.Collections.Generic;

    internal static class NamedEntityExtensions
    {
        /// <summary>
        /// Finds a name given part of its id or all of its name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public static T FindEntity<T>(this IEnumerable<T> entities, string search) where T : INamedEntity
        {
            return entities
                .FirstOrDefault(e => String.Equals(e.Name, search, StringComparison.CurrentCultureIgnoreCase) || e.Id.ToString("N").StartsWith(search));
        }
    }
}
