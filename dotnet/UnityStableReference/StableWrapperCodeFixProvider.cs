using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace UnityStableReference;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StableWrapperCodeFixProvider)), Shared]
[SuppressMessage("MicrosoftCodeAnalysisReleaseTracking", "RS2008:Enable analyzer release tracking")]
public class StableWrapperCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(StableWrapperDiagnosticAnalyzer.NoGuid.Id);

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        
        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;
        
        // Find the type declaration identified by the diagnostic
        var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf()
            .OfType<TypeDeclarationSyntax>().First();
        
        // Register a code action that will add the GuidAttribute
        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Add GuidAttribute",
                createChangedDocument: c => AddGuidAttributeAsync(context.Document, declaration, c),
                equivalenceKey: "AddGuidAttribute"),
            diagnostic);
    }

    private async Task<Document> AddGuidAttributeAsync(Document document, TypeDeclarationSyntax typeDecl, CancellationToken cancellationToken)
    {
        // Generate a new GUID
        var guid = System.Guid.NewGuid().ToString("D").ToUpper();
        
        // Create a new attribute syntax
        var attributeSyntax = SyntaxFactory.Attribute(
            SyntaxFactory.ParseName("Guid"),
            SyntaxFactory.AttributeArgumentList(
                SyntaxFactory.SeparatedList(
                    new[] { SyntaxFactory.AttributeArgument(
                        SyntaxFactory.LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            SyntaxFactory.Literal(guid))) })));
        
        // Get the existing attribute list or create a new one
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var newTypeDecl = typeDecl;
        
        if (typeDecl.AttributeLists.Count > 0)
        {
            // Add to existing attribute list
            var firstAttributeList = typeDecl.AttributeLists[0];
            var newAttributeList = firstAttributeList.AddAttributes(attributeSyntax);
            newTypeDecl = typeDecl.ReplaceNode(firstAttributeList, newAttributeList);
        }
        else
        {
            // Create a new attribute list
            var attributeList = SyntaxFactory.AttributeList(
                SyntaxFactory.SingletonSeparatedList(attributeSyntax));
            newTypeDecl = typeDecl.AddAttributeLists(attributeList);
        }
        
        var newRoot = root.ReplaceNode(typeDecl, newTypeDecl);
        
        // Check if we need to add the using directive
        var usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Runtime.InteropServices"));
        var compilationUnit = (CompilationUnitSyntax)newRoot;
        
        if (!compilationUnit.Usings.Any(u => u.Name.ToString() == "System.Runtime.InteropServices"))
        {
            newRoot = compilationUnit.AddUsings(usingDirective);
        }
        
        return document.WithSyntaxRoot(newRoot.WithAdditionalAnnotations(Formatter.Annotation));
    }
}