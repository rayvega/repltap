using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ReplTap.Core
{
    public static class ReplEngine
    {
        private static ScriptState<object> _state;

        public static async Task<string> Execute(string code)
        {
            _state = _state == null
                ? await CSharpScript.RunAsync(code)
                : await _state.ContinueWithAsync(code);

            var output = _state.ReturnValue?.ToString();

            return output;
        }
    }
}
