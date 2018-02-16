namespace Boondocks.Base
{
    using Newtonsoft.Json;

    public static class ObjectExtensions
    {
        /// <summary>
        ///     Performs a deep clone via serialization / deserialization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Clone<T>(this T source)
        {
            if (source == null)
                return default(T);

            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.Auto
            };

            var json = JsonConvert.SerializeObject(source, settings);

            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}