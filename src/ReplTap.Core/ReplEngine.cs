using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Threading.Tasks;

namespace ReplTap
{
    public class ReplEngine
    {
        private static ScriptState<object> state;

        public static async Task<string> Execute(string code)
        {
            state = state == null
                ? await CSharpScript.RunAsync(code)
                : await state.ContinueWithAsync(code);

            var output = state.ReturnValue?.ToString();

            return output;
        }
    }
}
