using Microsoft.Extensions.Primitives;
using System.Text;
using System.Web;

namespace ESchoolBot
{
    public static class StringBuilderExtensions
    {
        public static void AppendUrlEncoded(this StringBuilder sb, string name, string value)
        {
            if (sb.Length != 0)
            {
                sb.Append('&');
            }
            sb.Append(Uri.EscapeDataString(name));
            sb.Append('=');
            sb.Append(Uri.EscapeDataString(value));
        }
    }
}
