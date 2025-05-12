using System.Runtime.InteropServices;
using UnityEngine;
using UnityStableReference;

[Guid("14BD11C0-2CD0-4BA5-9756-D9C64DCF4105")]
public interface IFoo {}

[Guid("A267E890-16F1-406E-8E00-8FB9588F3135")]
public class FooImpl1 : IFoo {}

[Guid("3DDC5AB5-736A-4691-9208-B5641C9E7DE5")]
public class FooImpl2 : IFoo {}

public class Foo : MonoBehaviour
{
    public StableReference<IFoo> Reference;
}
