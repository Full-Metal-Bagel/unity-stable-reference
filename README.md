[![openupm](https://img.shields.io/npm/v/com.fullmetalbagel.unity-stable-reference?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.fullmetalbagel.unity-stable-reference/)

# Unity Stable Reference

A Unity package that provides stable `[SerializeReference]` support through source generation, ensuring your polymorphic serialized references survive refactoring, namespace changes, and type modifications.

<img src="https://github.com/user-attachments/assets/46634cf0-07e3-41ec-b8c8-85a62672a686" width="600" alt="Unity Stable Reference Inspector" />

## Table of Contents

- [Overview](#overview)
- [Requirements](#requirements)
- [Installation](#installation)
- [Features](#features)
- [How It Works](#how-it-works)
- [Usage](#usage)
- [Advanced Examples](#advanced-examples)
- [Comparison with Unity's MoveFromAttribute](#comparison-with-unitys-movefromattribute)
- [Troubleshooting](#troubleshooting)
- [API Reference](#api-reference)
- [Contributing](#contributing)
- [License](#license)

## Overview

Unity's `[SerializeReference]` attribute enables polymorphic serialization, but it has a critical limitation: serialized references break when you refactor your code. Moving a class to a different namespace, renaming it, or restructuring your type hierarchy can cause Unity to lose track of serialized data, resulting in null references and data loss.

**Unity Stable Reference** solves this problem by using GUIDs to create permanent, refactoring-safe references. Your serialized data remains intact regardless of how you reorganize your code.

### The Problem

```csharp
// Original code
namespace MyGame.Characters {
    public class Warrior : ICharacter { }
}

// After refactoring - Unity loses the reference!
namespace MyGame.Entities.Characters {
    public class WarriorClass : ICharacter { }
}
```

### The Solution

With Unity Stable Reference, your references survive any refactoring:

```csharp
[Guid("8F9CC34B-A30B-48AB-967C-5B3F0CE0793A"), StableWrapperCodeGen]
public class WarriorClass : ICharacter { }
// GUID ensures the reference is maintained
```

## Requirements

- Unity 2022.3 or newer
- .NET Standard 2.0 compatibility
- Roslyn compiler support (included in Unity 2022.3+)

## Installation

### Option 1: Install via OpenUPM (Recommended)

The package is available on the [OpenUPM registry](https://openupm.com/packages/com.fullmetalbagel.unity-stable-reference/). You can install it via openupm-cli:

```bash
openupm add com.fullmetalbagel.unity-stable-reference
```

### Option 2: Install via Git URL

1. Open the Package Manager in Unity (Window → Package Manager)
2. Click the "+" button in the top-left corner
3. Select "Add package from git URL..."
4. Enter the following URL:
```
https://github.com/quabug/unity-stable-reference.git?path=Packages/com.fullmetalbagel.unity-stable-reference
```

### Option 3: Manual Installation

1. Clone or download this repository
2. Copy the `Packages/com.fullmetalbagel.unity-stable-reference` folder into your project's `Packages` directory

## Features

- **GUID-based Stability**: Uses .NET's `[Guid]` attribute to maintain references across refactoring
- **Source Generation**: Zero runtime reflection, all wrapper code is generated at compile-time
- **Type Safety**: Full IntelliSense support and compile-time type checking
- **Inspector Support**: Custom property drawers for seamless Unity Editor integration
- **Minimal Overhead**: Thin wrapper pattern with negligible performance impact
- **Interface Support**: Works with interfaces, abstract classes, and concrete types
- **No Dependencies**: Self-contained package with no external dependencies
- **Roslyn Analyzer**: Provides helpful diagnostics and code fixes for missing GUIDs

## How It Works

Unity Stable Reference uses Roslyn source generators to create stable wrapper classes for your types. When you mark a type with the `[StableWrapperCodeGen]` attribute and a `[Guid]` attribute, the system generates specialized wrapper classes that can be reliably serialized by Unity.

## Usage

### Quick Start

1. **Import the Generated Stable Wrappers sample**:
   - Open the Package Manager (Window → Package Manager)
   - Find "Unity Stable Reference" in your project packages
   - In the package details, expand "Samples"
   - Click "Import" next to "Generated Stable Wrappers"

2. **Configure your assembly**:
   - Navigate to `Assets/Samples/Unity Stable Reference/{version}/__StableWrapper__/`
   - Select the `__StableWrapper__.asmdef` file
   - In the Inspector, add your project's assembly as a reference
   - Click "Apply"

3. **Mark your types with attributes**:

```csharp
using System;
using System.Runtime.InteropServices;
using UnityStableReference;

// The Guid ensures your reference survives refactoring
[Guid("8F9CC34B-A30B-48AB-967C-5B3F0CE0793A"), StableWrapperCodeGen]
public class PlayerData : IGameData 
{
    public string Name { get; set; }
    public int Level { get; set; }
}
```

4. **Use StableReference in your MonoBehaviours**:

```csharp
using System;
using UnityEngine;
using UnityStableReference;

public class GameManager : MonoBehaviour
{
    [SerializeField] 
    private StableReference<IGameData> _playerData;
    
    void Start()
    {
        // Create new instance
        _playerData = new PlayerData { Name = "Hero", Level = 1 };
        
        // Access the value
        if (_playerData.Value != null)
        {
            Debug.Log($"Player: {_playerData.Value}");
        }
    }
}
```

### Alternative Setup Methods

<details>
<summary>Method 1: Assembly-level attribute</summary>

Add to any C# file in your assembly (outside of any namespace):

```csharp
[assembly: UnityStableReference.StableWrapperCodeGen]
```

Then ensure "Auto Referenced" is enabled on your assembly definition.
</details>

<details>
<summary>Method 2: Manual assembly setup</summary>

1. Create your own wrapper assembly
2. Reference both your game assembly and StableReference.Runtime
3. Add the assembly-level attribute as shown above
</details>

## Advanced Examples

### Working with Interfaces

```csharp
// Define your interface
public interface IWeapon 
{
    float Damage { get; }
    void Attack();
}

// Implement concrete types with GUIDs
[Guid("A1B2C3D4-E5F6-7890-ABCD-EF1234567890"), StableWrapperCodeGen]
public class Sword : IWeapon 
{
    public float Damage => 10f;
    public void Attack() => Debug.Log("Swing!");
}

[Guid("B2C3D4E5-F6A7-8901-BCDE-F12345678901"), StableWrapperCodeGen]
public class Bow : IWeapon 
{
    public float Damage => 8f;
    public void Attack() => Debug.Log("Shoot!");
}

// Use in MonoBehaviour
public class Player : MonoBehaviour 
{
    [SerializeField] private StableReference<IWeapon> _primaryWeapon;
    [SerializeField] private StableReference<IWeapon> _secondaryWeapon;
    
    void Start() 
    {
        _primaryWeapon?.Value?.Attack();
        _secondaryWeapon?.Value?.Attack();
    }
}
```

### Collections and Arrays

```csharp
public class Inventory : MonoBehaviour 
{
    // Arrays of stable references
    [SerializeField] private StableReference<IItem>[] _items;
    
    // Lists work too
    [SerializeField] private List<StableReference<IItem>> _equipment;
    
    // Nested in other serializable classes
    [Serializable]
    public class ItemSlot 
    {
        public StableReference<IItem> item;
        public int quantity;
    }
    
    [SerializeField] private ItemSlot[] _slots;
}
```

### Inheritance Hierarchies

```csharp
// Base class
[Guid("C3D4E5F6-A7B8-9012-CDEF-234567890123"), StableWrapperCodeGen]
public abstract class Enemy : IEntity 
{
    public abstract void Attack();
}

// Derived classes with their own GUIDs
[Guid("D4E5F6A7-B890-1234-DEFA-345678901234"), StableWrapperCodeGen]
public class Goblin : Enemy 
{
    public override void Attack() => Debug.Log("Goblin attacks!");
}

[Guid("E5F6A7B8-9012-3456-EFAB-456789012345"), StableWrapperCodeGen]
public class Dragon : Enemy 
{
    public override void Attack() => Debug.Log("Dragon breathes fire!");
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

## Troubleshooting

### Common Issues

#### "Missing Guid attribute" warning

**Problem**: You see warnings about missing Guid attributes on types marked with `[StableWrapperCodeGen]`.

**Solution**: The package includes a Roslyn analyzer that detects this issue. In Visual Studio or Rider:
1. Click on the warning
2. Use the quick fix action (lightbulb icon)
3. Select "Add Guid attribute" to automatically generate a unique GUID

Alternatively, manually add a Guid:
```csharp
[Guid("YOUR-UNIQUE-GUID-HERE"), StableWrapperCodeGen]
public class YourClass { }
```

#### References become null after code changes

**Problem**: Your serialized references are lost after refactoring.

**Possible causes**:
1. **Missing or changed GUID**: Ensure the Guid attribute is present and hasn't been modified
2. **Assembly not referenced**: Check that `__StableWrapper__.asmdef` references your assembly
3. **Generated code not updated**: Try reimporting the package or recompiling

#### Type dropdown shows "None" or is empty

**Problem**: The type selection dropdown in the Inspector doesn't show your types.

**Solutions**:
1. Ensure your types have both `[Guid]` and `[StableWrapperCodeGen]` attributes
2. Check that the assembly containing your types is referenced in `__StableWrapper__.asmdef`
3. Verify that Unity has compiled successfully (check Console for errors)
4. Try right-clicking the `__StableWrapper__` folder and selecting "Reimport"

#### "Could not find wrapper type" errors

**Problem**: Runtime errors about missing wrapper types.

**Solution**: 
1. Ensure the `__StableWrapper__` assembly is included in your build
2. Check that "Auto Referenced" is enabled on the assembly definition
3. Verify the assembly is not being stripped in build settings

### Performance Considerations

- **First-time generation**: Initial code generation may take a few seconds for large codebases
- **Runtime overhead**: Minimal - only a thin wrapper indirection
- **Memory usage**: Each wrapped instance has a small memory overhead (typically 16-24 bytes)

### Best Practices

1. **Generate GUIDs once**: Never change a GUID after serialized data exists
2. **Version control**: Always commit GUID attributes with your types
3. **Assembly organization**: Keep wrapped types in dedicated assemblies for faster compilation
4. **Testing**: Test serialization after major refactoring to ensure references are maintained

## API Reference

### Attributes

#### `[StableWrapperCodeGen]`
Marks a type for stable wrapper generation. Must be used with `[Guid]`.

```csharp
[Guid("..."), StableWrapperCodeGen]
public class MyClass { }
```

#### `[assembly: StableWrapperCodeGen]`
Enables wrapper generation for all marked types in an assembly.

```csharp
[assembly: UnityStableReference.StableWrapperCodeGen]
```

### Types

#### `StableReference<T>`
The main wrapper type for creating stable references.

**Properties**:
- `Value`: Gets or sets the wrapped object
- `IsValid`: Returns true if the reference contains a non-null value

**Methods**:
- Implicit conversion to/from `T`
- Supports `==` and `!=` operators

**Example**:
```csharp
StableReference<IMyInterface> stableRef = new MyImplementation();
IMyInterface value = stableRef; // Implicit conversion
```

## Contributing

We welcome contributions! Please follow these guidelines:

1. **Fork the repository** and create a feature branch
2. **Write tests** for any new functionality
3. **Follow the existing code style** (use the .editorconfig file)
4. **Update documentation** as needed
5. **Submit a pull request** with a clear description

### Development Setup

1. Clone the repository
2. Open the Unity project for testing Unity integration
3. Use the `dotnet/UnityStableReference.sln` solution for analyzer development

### Running Tests

```bash
cd dotnet
dotnet test
```

### Building the Analyzer

```bash
cd dotnet
dotnet build UnityStableReference/UnityStableReference.csproj
dotnet publish UnityStableReference/UnityStableReference.csproj -c Release
```

The published DLL will be automatically copied to the Unity package.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
