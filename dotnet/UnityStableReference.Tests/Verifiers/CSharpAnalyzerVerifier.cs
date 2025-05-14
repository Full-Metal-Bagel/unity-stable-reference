using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace UnityStableReference.Tests;

public static class CSharpAnalyzerVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    public static DiagnosticResult Diagnostic()
        => CSharpAnalyzerVerifier<TAnalyzer, MSTestVerifier>.Diagnostic();

    public static DiagnosticResult Diagnostic(string diagnosticId)
        => CSharpAnalyzerVerifier<TAnalyzer, MSTestVerifier>.Diagnostic(diagnosticId);

    public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
        => CSharpAnalyzerVerifier<TAnalyzer, MSTestVerifier>.Diagnostic(descriptor);

    public static async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new Test { TestCode = source };
        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync();
    }

    public class Test : CSharpAnalyzerTest<TAnalyzer, MSTestVerifier>
    {
        public Test()
        {
            SolutionTransforms.Add((solution, projectId) =>
            {
                var compilationOptions = solution.GetProject(projectId)!.CompilationOptions!;
                compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(
                    compilationOptions.SpecificDiagnosticOptions.SetItems(GetNullableWarningsFromCompiler()));
                solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);
                
                return solution;
            });
            
            // Add reference to UnityStableReference assembly
            TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(StableWrapperCodeGenAttribute).Assembly.Location));
        }

        private static ImmutableDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
        {
            string[] args = { "/warnaserror:nullable" };
            var commandLineArguments = CSharpCommandLineParser.Default.Parse(args, baseDirectory: Environment.CurrentDirectory, sdkDirectory: Environment.CurrentDirectory);
            var nullableWarnings = commandLineArguments.CompilationOptions.SpecificDiagnosticOptions;
            return nullableWarnings;
        }
        
        protected override ImmutableArray<(Project project, Diagnostic diagnostic)> FilterDiagnostics(ImmutableArray<(Project project, Diagnostic diagnostic)> diagnostics)
        {
            // Filter out CS1705 errors (assembly reference version mismatch)
            return base.FilterDiagnostics(diagnostics)
                .Where(d => d.diagnostic.Id != "CS1705")
                .ToImmutableArray();
        }
    }
}