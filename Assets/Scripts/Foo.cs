using System;
using System.Collections.Generic;
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

[Serializable, Guid("3DDC5AB5-736A-4691-9208-B5641C9E7DE5"), StableWrapperCodeGen]
public class FooImpl2 : IFoo
{
    public float Float;
    
    public void Print()
    {
        Debug.Log("FooImpl2 " + Float);
    }
}

[Serializable, Guid("DB4EAC44-D2E6-4667-8650-6AFAFC5439E6"), StableWrapperCodeGen]
public class FooImpl3 : IFoo
{
    public void Print()
    {
        Debug.Log("FooImpl3");
    }
}

public class Foo : MonoBehaviour
{
    public StableReference<IFoo> Reference;
    public List<StableReference<IFoo>> ListReference;

    private void Start()
    {
        Reference.Value.Print();
    }
}
