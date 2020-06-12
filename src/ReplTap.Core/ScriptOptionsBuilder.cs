using Microsoft.CodeAnalysis.Scripting;

namespace ReplTap.Core
{
    public interface IScriptOptionsBuilder
    {
        ScriptOptions Build();
    }

    public class ScriptOptionsBuilder : IScriptOptionsBuilder
    {
        public ScriptOptions Build()
        {
            var imports = new[]
            {
                "System",
                "System.IO",
                "System.Collections.Generic",
                "System.Console",
                "System.Diagnostics",
                "System.Text",
                "System.Threading.Tasks",
            };

            return ScriptOptions.Default.WithImports(imports);
        }
    }
}