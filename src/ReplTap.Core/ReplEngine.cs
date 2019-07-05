using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ReplTap.Core
{
    public interface IReplEngine
    {
        Task<CodeResult> Execute(string code);
    }

    public class ReplEngine : IReplEngine
    {
        private static ScriptState<object> _state;

        public async Task<CodeResult> Execute(string code)
        {
            var result = new CodeResult();
            
            try
            {
                _state = _state == null
                    ? await CSharpScript.RunAsync(code)
                    : await _state.ContinueWithAsync(code);

                result.Output = _state.ReturnValue?.ToString();
                result.State = OutputState.Valid;
            }
            catch (Exception)
            {
                result.State = OutputState.Continue;
            }

            return result;
        }
    }
}
