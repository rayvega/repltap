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
            var scriptOptions = ScriptOptions.Default;

            // add imports

            var imports = new[]
            {
                "System",
                "System.IO",
                "System.Collections.Generic",
                "System.Console",
                "System.Diagnostics",
                "System.Linq",
                "System.Text",
                "System.Threading.Tasks",
            };

            scriptOptions = scriptOptions.AddImports(imports);

            // add references

            var systemLinq = typeof(System.Linq.Enumerable).Assembly;

            scriptOptions = scriptOptions.AddReferences(systemLinq);

            return scriptOptions;
        }
    }
}