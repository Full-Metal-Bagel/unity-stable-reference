[![openupm](https://img.shields.io/npm/v/com.fullmetalbagel.unity-stable-reference?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.fullmetalbagel.unity-stable-reference/)

# Unity Stable Reference

A Unity package that provides stable `[SerializeReference]` to objects through source generation, allowing for reliable serialization even after type changes.

## Requirements

- Unity 2022.3 or newer

## Overview

Unity Stable Reference solves the problem of maintaining references to objects in Unity when types change or evolve. By generating wrapper classes with stable GUIDs, it ensures that serialized references remain valid across code changes and refactoring.

## Features

- Creates stable serialized references with GUIDs
- Works with any C# type including interfaces
- Uses source generation for compile-time type safety
- Minimal runtime overhead
- No dependencies on third-party packages

## How It Works

Unity Stable Reference uses Roslyn source generators to create stable wrapper classes for your types. When you mark a type with the `[StableWrapperCodeGen]` attribute and a `[Guid]` attribute, the system generates specialized wrapper classes that can be reliably serialized by Unity.

## Usage

1. Import the sample "Generated Stable Wrappers":
   - Open the Package Manager in Unity (Window > Package Manager)
   - Select the "Unity Stable Reference" package
   - Click on "Samples" and import "Generated Stable Wrappers"

2. Reference your assembly in the sample's assembly definition:
   - Navigate to the imported sample folder (usually in Assets/Samples/Unity Stable Reference/1.0.0/__StableWrapper__)
   - Open the assembly definition file (asmdef)
   - Add your project's assembly as a reference in the Inspector

3. Add necessary attribute to your type:

```csharp
using System.Runtime.InteropServices;
using UnityStableReference;

[Guid("8F9CC34B-A30B-48AB-967C-5B3F0CE0793A"), StableWrapperCodeGen]
public class Foo : IFoo { }
```

4. Use the `StableReference<T>` type in your MonoBehaviours:

```csharp
[Serializable]
public class MyComponent : MonoBehaviour
{
    [SerializeField] private StableReference<IFoo> _foo;
    
    public void DoSomething()
    {
        // Access the wrapped object
        IFoo foo = _foo.Value;
        // or use implicit conversion
        IFoo foo2 = _foo;
    }
}
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
