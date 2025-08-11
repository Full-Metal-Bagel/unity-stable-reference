# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Unity Stable Reference is a Unity package that provides stable `[SerializeReference]` to objects through source generation. It solves the problem of maintaining references to objects in Unity when types change or evolve by generating wrapper classes with stable GUIDs.

## Architecture

The project consists of two main components:

1. **Roslyn Source Generator** (`dotnet/UnityStableReference/`)
   - `StableWrapperSourceGenerator.cs`: Main source generator that creates wrapper classes
   - `StableWrapperDiagnosticAnalyzer.cs`: Analyzer for detecting missing Guid attributes
   - `StableWrapperCodeFixProvider.cs`: Code fix provider for automatically adding missing Guid attributes
   - Uses Roslyn to generate code at compile-time for types marked with `[StableWrapperCodeGen]` and `[Guid]`

2. **Unity Package** (`Packages/com.fullmetalbagel.unity-stable-reference/`)
   - **Runtime**: Core `StableReference<T>` type and supporting code
   - **Editor**: Custom property drawers for Unity Inspector
   - **Analyzer**: Contains the compiled Roslyn analyzer DLL

## Common Commands

### Building the Roslyn Analyzer
```bash
cd dotnet
dotnet build UnityStableReference/UnityStableReference.csproj
dotnet publish UnityStableReference/UnityStableReference.csproj -c Release
```

### Running Tests
```bash
cd dotnet
dotnet test UnityStableReference.Tests/UnityStableReference.Tests.csproj
```

### Unity Development
- Open the Unity project from the root directory
- Unity version requirement: 2022.3 or newer
- The package is located at `Packages/com.fullmetalbagel.unity-stable-reference/`

## Key Concepts

1. **StableWrapperCodeGen Attribute**: Marks types that should have stable wrappers generated
2. **Guid Attribute**: Required on types with StableWrapperCodeGen to ensure stable serialization
3. **StableReference<T>**: The wrapper type used in Unity to hold references with stability
4. **Generated Code**: Source generator creates wrapper classes in the `__StableWrapper__` namespace

## Development Workflow

1. When modifying the source generator:
   - Edit files in `dotnet/UnityStableReference/`
   - Run tests in `dotnet/UnityStableReference.Tests/`
   - Build and publish to update the analyzer DLL in the Unity package

2. When modifying Unity-specific code:
   - Edit files in `Packages/com.fullmetalbagel.unity-stable-reference/`
   - Test changes in the Unity editor using the sample scene

3. The `PostPublish` target in the csproj automatically copies the built analyzer to the Unity package location