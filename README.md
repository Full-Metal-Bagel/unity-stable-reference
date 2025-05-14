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

<details>
<summary>Alternatively, you can also:</summary>

1. Add the assembly-level attribute to your project:
   - Create or open any C# file in your _Assembly-CSharp_ assembly
   - Add the following line at the top of the file (outside any namespace):
     ```csharp
     [assembly: UnityStableReference.StableWrapperCodeGen]
     ```
   - Make sure to add the appropriate using statement if needed

2. Enable Auto Referenced on your assemblies:
   - For each assembly that uses `[StableWrapperCodeGen]`, locate its assembly definition file (.asmdef)
   - Select the .asmdef file in the Inspector
   - Check the "Auto Referenced" checkbox to ensure the generated code is properly referenced
   - Save the changes
</details>
    
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

## Comparison with Unity's MoveFromAttribute

Unity's built-in `[MoveFrom]` attribute provides an alternative approach to handle serialization issues. Understanding its advantages and disadvantages can help determine when to use this package instead.

### MoveFromAttribute Advantages
- Simple to implement with minimal setup (just add the attribute with a path string)
- Built into Unity with no additional dependencies
- No runtime overhead after deserialization is complete
- Works well for straightforward field renames and moves between parent/child classes
- Handles one-time migrations where the serialized path is known
- Native integration with Unity's serialization system

### MoveFromAttribute Disadvantages
- Limited to handling field renames and simple relocations
- Cannot maintain references when changing between interface implementations
- Doesn't work well with polymorphic types or significant class restructuring
- Path strings can be error-prone and hard to maintain
- Not designed for ongoing type evolution scenarios

`UnityStableReference` is ideal when you need more robust reference stability, especially when significant refactoring is anticipated.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
