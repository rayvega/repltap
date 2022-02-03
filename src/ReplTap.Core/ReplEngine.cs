using System.Linq;
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
        private static ScriptState<object>? _state;
        private readonly IInputCheck _inputCheck;
        private readonly IScriptOptionsBuilder _scriptOptionsBuilder;

        public ReplEngine(IInputCheck inputCheck, IScriptOptionsBuilder scriptOptionsBuilder)
        {
            _inputCheck = inputCheck;
            _scriptOptionsBuilder = scriptOptionsBuilder;
        }

        public async Task<CodeResult> Execute(string code)
        {
            var result = new CodeResult();

            try
            {
                _state = _state == null
                    ? await CSharpScript.RunAsync(code, _scriptOptionsBuilder.Build())
                    : await _state.ContinueWithAsync(code);

                result.Output = _state.ReturnValue?.ToString() ?? string.Empty;

                result.State = OutputState.Valid;

                result.Variables = _state.Variables
                    .Select(variable => variable.Name)
                    .ToList();
            }
            catch (CompilationErrorException exception)
            {
                if (_inputCheck.IsForceExecute(code))
                {
                    result.Output = exception.Message;
                    result.State = OutputState.Error;
                }
                else
                {
                    result.State = OutputState.Continue;
                }
            }

            return result;
        }
    }
}
