using System;
using System.Linq;

namespace Boondocks.Auth.Domain.Entities
{
    /// <summary>
    /// Represents a Docker resource and the allowed actions
    /// that can be preformed by an authenticated client.
    /// </summary>
    public class ResourcePermission
    {
        /// <summary>
        /// The type of resource. 
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// The name of the resource. 
        /// </summary>
        public string Name { get; }

        public string Class { get; set;} = "";

        /// <summary>
        /// Let of actions specific to the resources allowed by the
        /// authenticated user.
        /// </summary>
        public string[] Actions { get; }

        public ResourcePermission(
            string type,
            string name,
            string[] actions) {

            Type = type ?? throw new ArgumentNullException(nameof(type));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Actions = actions ?? throw new ArgumentNullException(nameof(actions));
        }      

        /// <summary>
        /// Returns a new resource entity containing only the allowed actions.
        /// </summary>
        /// <param name="actions">The subset of allowed actions.</param>
        /// <returns>New resource access entity with restricted set of actions.</returns>
        public ResourcePermission SetAllowedActions(string[] actions)
        {
            var allowedActions = actions.Where(a => Actions.Contains(a));

            return new ResourcePermission(Type, Name, allowedActions.ToArray());
        }      
    }
}