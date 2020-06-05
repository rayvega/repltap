using System.Collections.Generic;

namespace ReplTap.Core
{
    public class CodeResult
    {
        public string? Output { get; set; }
        public OutputState State { get; set; }
        public List<string>? Variables { get; set; }
    }
}