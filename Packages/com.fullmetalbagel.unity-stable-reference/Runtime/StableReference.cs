#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

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
    [field: SerializeField] public T Value { get; private set; } = default!;
}

[SuppressMessage("Design", "CA1000:Do not declare static members on generic types")]
[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates")]
[Serializable]
public record struct StableReference<T>() where T : notnull
{
    public static string WrapperPropertyName => nameof(_wrapper);
    [SerializeReference] private IStableWrapper _wrapper = default!;
    public T Value => ((IStableWrapper<T>)_wrapper).Value;
    public static implicit operator T(StableReference<T> self) => self.Value;
}
