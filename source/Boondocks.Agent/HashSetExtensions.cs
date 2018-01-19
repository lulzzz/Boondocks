using System.Collections.Generic;
using Boondocks.Services.Device.Contracts;

namespace Boondocks.Agent
{
    internal static class HashSetExtensions
    {
        public static bool TryAdd<T>(this HashSet<T> hashSet, T value)
        {
            if (hashSet.Contains(value))
                return false;

            hashSet.Add(value);

            return true;
        }

        public static bool TryAdd(this HashSet<string> hashSet, VersionReference version)
        {
            if (version == null)
                return false;

            return hashSet.TryAdd(version.ImageId);
        }
    }
}