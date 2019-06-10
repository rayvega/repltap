using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;

namespace ReplTap.Core.Completions
{
    public interface IRoslynCompletionsProvider
    {
        Task<CompletionList> GetCompletions(string code);
    }

    public class RoslynCompletionsProvider : IRoslynCompletionsProvider
    {
        public async Task<CompletionList> GetCompletions(string code)
        {
            // based on: https://www.strathweb.com/2018/12/using-roslyn-c-completion-service-programmatically/
            
            var host = MefHostServices.Create(MefHostServices.DefaultAssemblies);
            var workspace = new AdhocWorkspace(host);

            var compilationOptions = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                usings: new[] {"System"});

            var scriptProjectInfo = ProjectInfo.Create(
                    ProjectId.CreateNewId(),
                    VersionStamp.Create(),
                    "Script",
                    "Script",
                    LanguageNames.CSharp,
                    isSubmission: true)
                .WithMetadataReferences(new[]
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
                })
                .WithCompilationOptions(compilationOptions);
            var scriptProject = workspace.AddProject(scriptProjectInfo);
            var scriptDocumentInfo = DocumentInfo.Create(
                DocumentId.CreateNewId(scriptProject.Id), "Script",
                sourceCodeKind: SourceCodeKind.Script,
                loader: TextLoader.From(TextAndVersion.Create(SourceText.From(code), VersionStamp.Create())));
            var scriptDocument = workspace.AddDocument(scriptDocumentInfo);

            var position = code.Length - 1;
            var completionService = CompletionService.GetService(scriptDocument);
            
            var results = await completionService.GetCompletionsAsync(scriptDocument, position);
            
            return results;
        }
    }
}