using System;
using Newtonsoft.Json;

namespace Boondocks.Services.Base
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Performs a deep clone via serialization / deserialization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Clone<T>(this T source)
        {
            if (source == null)
                return default(T);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.Auto
            };

            string json = JsonConvert.SerializeObject(source, settings);

            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}
