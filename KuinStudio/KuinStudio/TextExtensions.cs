using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baku.KuinStudio.Text
{
    internal static class TextExtensions
    {
        public static byte[] GetBytes(this string source, Encoding encoding)
            => encoding.GetBytes(source);

        public static string GetString(this byte[] source, Encoding encoding)
            => encoding.GetString(source);
    }
}
