using System;
using System.Collections.Generic;
using System.Text;

namespace KrisMecn.Helpers.Extensions
{
    public static class UriExtensions
    {
        public static bool IsHttp(this Uri uri) => uri.Scheme == "http" || uri.Scheme == "https";
    }
}