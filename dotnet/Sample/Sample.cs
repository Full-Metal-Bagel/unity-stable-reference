using UnityStableReference;
using System.Runtime.InteropServices;

[assembly: StableWrapperCodeGen]

namespace UnityStableReference.Sample;

public interface IFoo { }

[StableWrapperCodeGen, Guid("BADA070C-7EA8-4A91-B83A-CD3F3B2EC108")]
public class Foo : IFoo { }