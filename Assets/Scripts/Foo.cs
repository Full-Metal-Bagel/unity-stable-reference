using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityStableReference;

public interface IFoo
{
    void Print();
}

[Serializable, Guid("A267E890-16F1-406E-8E00-8FB9588F3135"), StableWrapperCodeGen]
public class FooImpl1 : IFoo
{
    public int Int;
    
    public void Print()
    {
        Debug.Log("FooImpl1 " + Int);
    }
}

[Serializable, Guid("35FE2D63-412D-41AA-9F0D-BB3416F11235"), StableWrapperCodeGen]
public class FooImpl3 : IFoo
{
    public void Print()
    {
        Debug.Log("FooImpl3");
    }
}

[Serializable, Guid("3DDC5AB5-736A-4691-9208-B5641C9E7DE5"), StableWrapperCodeGen]
public class FooImpl2 : IFoo
{
    public float Float;
    
    public void Print()
    {
        Debug.Log("FooImpl2 " + Float);
    }
}

public class Foo : MonoBehaviour
{
    public StableReference<IFoo> Reference;

    private void Start()
    {
        Reference.Value.Print();
    }
}
