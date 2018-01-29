using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Boondocks.Services.WebApiClient
{
    internal static class ObjectToDictionaryHelper
    {
        internal static IDictionary<string, string> ToDictionary(this object source)
        {
            //Check to see if this is already a dictionary
            if (source is IDictionary<string, string> stringDictionary)
            {
                return stringDictionary;
            }

            //Check to see if this is an object dictionary (e.g. ExpandoObject)
            if (source is IDictionary<string, object> objectDictionary)
            {
                return objectDictionary.ToDictionary(p => p.Key, p => p.Value?.ToString());
            }

            var dictionary = new Dictionary<string, string>();

            if (source != null)
            {
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
                {
                    object value = property.GetValue(source);

                    if (value != null)
                    {
                        dictionary.Add(property.Name, value.ToString());
                    }
                }
            }

            return dictionary;
        }
    }
}