using System;
using System.Text;

namespace ReplTap.ConsoleHost.Extensions
{
    public static class StringBuilderExtensions
    {
        public static void ReplaceWith(this StringBuilder builder, string text)
        {
            builder
                .Clear()
                .Append(text);
        }

        public static string Slice(this StringBuilder builder, Range range)
        {
            return builder.ToString()[range];
        }
    }
}
