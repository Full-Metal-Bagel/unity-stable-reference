using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnityStableReference;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[SuppressMessage("MicrosoftCodeAnalysisReleaseTracking", "RS2008:Enable analyzer release tracking")]
public class StableWrapperDiagnosticAnalyzer : DiagnosticAnalyzer
{
    // Define diagnostic descriptor
    public static readonly DiagnosticDescriptor NoGuid = new(
        id: "SW001",
        title: "Type must have `GuidAttribute`",
        messageFormat: "Type {0} must have `GuidAttribute` for generating stable wrapper",
        category: "StableWrapper",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
        ImmutableArray.Create(NoGuid);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    private void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;
        
        // Check if type has StableWrapperCodeGenAttribute but no GuidAttribute
        if (namedTypeSymbol.GetAttributes().Any(a => a.AttributeClass?.Name == "StableWrapperCodeGenAttribute") &&
            !namedTypeSymbol.GetAttributes().Any(a => a.AttributeClass?.Name == "GuidAttribute"))
        {
            var diagnostic = Diagnostic.Create(NoGuid, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}