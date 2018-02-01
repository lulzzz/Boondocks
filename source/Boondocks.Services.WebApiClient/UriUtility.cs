//using System;

//namespace Boondocks.Services.WebApiClient
//{
//    public static class UriUtility
//    {
//        public static Uri BuildUri(Uri baseUri, string path, object routeValues)
//        {
//            if (baseUri == null)
//            {
//                throw new ArgumentNullException(nameof(baseUri));
//            }

//            var builder = new UriBuilder(baseUri);

//            if (!string.IsNullOrEmpty(path))
//            {
//                builder.Path += path;
//            }

//            if (routeValues != null)
//            {
//                builder.Query = routeValues.GetQueryString();
//            }

//            return builder.Uri;
//        }
//    }
//}