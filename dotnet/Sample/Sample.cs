using System.Runtime.InteropServices;
using UnityStableReference;

[assembly: StableWrapperCodeGen]

namespace UnityStableReference.Sample;

public interface IFoo { }

[Guid("8F9CC34B-A30B-48AB-967C-5B3F0CE0793A"), StableWrapperCodeGen]
public class Foo : IFoo { }