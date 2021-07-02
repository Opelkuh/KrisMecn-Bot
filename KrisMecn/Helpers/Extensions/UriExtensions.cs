using System;

namespace KrisMecn.Helpers.Extensions
{
    public static class UriExtensions
    {
        public static bool IsHttp(this Uri uri) => uri.Scheme == "http" || uri.Scheme == "https";
    }
}