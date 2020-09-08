using System.Text;

namespace ReplTap.ConsoleHost.Extensions
{
    public static class StringBuilderExtensions
    {
        public static void ReplaceWith(this StringBuilder builder, string? text)
        {
            builder
                .Clear()
                .Append(text);
        }
    }
}