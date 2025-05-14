using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = UnityStableReference.Tests.CSharpAnalyzerVerifier<
    UnityStableReference.StableWrapperDiagnosticAnalyzer>;

namespace UnityStableReference.Tests;

[TestClass]
public class StableWrapperDiagnosticAnalyzerTests
{
    [TestMethod]
    public async Task NoDiagnosticForClassWithGuidAttribute()
    {
        var test = """
                   using System.Runtime.InteropServices;
                   using UnityStableReference;

                   namespace TestNamespace
                   {
                       [StableWrapperCodeGen, Guid("12345678-1234-1234-1234-123456789012")]
                       public class TestClass
                       {
                       }
                   }
                   """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task DiagnosticForClassWithoutGuidAttribute()
    {
        var test = """
                   using UnityStableReference;

                   namespace TestNamespace
                   {
                       [StableWrapperCodeGen]
                       public class TestClass
                       {
                       }
                   }
                   """;

        var expected = VerifyCS.Diagnostic(StableWrapperDiagnosticAnalyzer.NoGuid.Id)
            .WithLocation(6, 18)
            .WithArguments("TestClass");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }
}