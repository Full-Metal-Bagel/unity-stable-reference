using System;
using System.Diagnostics.CodeAnalysis;

namespace UnityStableReference;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly)]
public sealed class StableWrapperCodeGenAttribute : Attribute
{
}

[SuppressMessage("Design", "CA1040:Avoid empty interfaces")]
public interface IStableWrapper { }

public interface IStableWrapper<out T> : IStableWrapper
{
    T Value { get; }
}

[Serializable]
public class StableWrapper<T> : IStableWrapper<T>
{
#if UNITY_5_6_OR_NEWER
    [field: UnityEngine.SerializeField]
#endif
    public T Value { get; private set; } = default!;
}

[SuppressMessage("Design", "CA1000:Do not declare static members on generic types")]
[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates")]
[Serializable]
public record struct StableReference<T>() where T : notnull
{
    public static string WrapperPropertyName => nameof(_wrapper);
#if UNITY_5_6_OR_NEWER
    [UnityEngine.SerializeReference]
#endif
    private IStableWrapper _wrapper = default!;
    public T Value => ((IStableWrapper<T>)_wrapper).Value;
    public static implicit operator T(StableReference<T> self) => self.Value;
}
