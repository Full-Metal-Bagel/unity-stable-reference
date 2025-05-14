using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = UnityStableReference.Tests.CSharpCodeFixVerifier<
    UnityStableReference.StableWrapperDiagnosticAnalyzer,
    UnityStableReference.StableWrapperCodeFixProvider>;

namespace UnityStableReference.Tests;

[TestClass]
public class StableWrapperCodeFixProviderTests
{
    [TestMethod]
    public async Task CodeFixAddsGuidAttribute()
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

        var fixtest = """
                      using UnityStableReference;
                      using System.Runtime.InteropServices;

                      namespace TestNamespace
                      {
                          [StableWrapperCodeGen, Guid("59029207-F6B6-4477-978A-62CE931D6619")]
                          public class TestClass
                          {
                          }
                      }
                      """;

        var expected = VerifyCS.Diagnostic(StableWrapperDiagnosticAnalyzer.NoGuid.Id)
            .WithLocation(6, 18)
            .WithArguments("TestClass");

        // Enable the testing mode to use a fixed GUID
        UnityStableReference.StableWrapperCodeFixProvider.UseTestingGuid = true;
        try
        {
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
        finally
        {
            // Reset to default behavior
            UnityStableReference.StableWrapperCodeFixProvider.UseTestingGuid = false;
        }
    }
}