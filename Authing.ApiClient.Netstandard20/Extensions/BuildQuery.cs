using System;
using Flurl;

namespace Authing.ApiClient.Extensions
{
    public static class BuildQueryExtension
    {
        public static string BuildQuery(this object obj)
        {
            var url = string.Empty.SetQueryParams(obj);
            return url.Query;
        }
    }
}