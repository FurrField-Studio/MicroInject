# Micro Inject

MicroInject is simple Dependency Injection framework made for Unity Engine that allows you to easily inject components.
<br>

<b>Full documentation is available here:</b> [Documentation](https://furrfield-studio.github.io/MicroInject-Docs/)

<br>
Features:
<br>

- Runtime injection
- In-editor injection - injecting dependencies without any runtime performance impact
- Named dependencies - injecting dependencies that can be named (Useful when we have couple dependencies that have the same type)
- Dynamically named dependencies (or for short Dynamic dependencies) - the same as above but name can be changed during runtime (Useful when we want to inject specific player data based on a name that can change during runtime)

<br>

# Installation

<details>
    <summary>Using OpenUPM (Recommended) (Needs to be published to OpenUPM)</summary>

<br>
Add the OpenUPM registry with the ``com.furrfield`` scope to your project
<br>

- Open ``Edit/Project Settings/Package Manager``
- Add a new Scoped Registry:
```
Name: OpenUPM
URL:  https://package.openupm.com/
Scope(s): com.furrfield
```
- Click save
<br>

Add this package:

- Open ``Window/Package Manager``
- Click ``+``
- Click ``Add package from git URL`` or ``Add package by name``
- Paste com.furrfield.micro-inject
- Click ``Add``
</details>

<details>
    <summary>Using package (Recommended)</summary>

- Open ``Window/Package Manager``
- Click ``+``
- Click ``Add package from git URL`` or ``Add package by name``
- Add ``https://github.com/FurrField-Studio/MicroInject.git`` in Package Manager

</details>

<details>
    <summary>Using AssetStore (No Samples available) (Needs to be published to AssetStore)</summary>
    
</details>

<details>
    <summary>Using .unitypackage (No Samples available)</summary>
<br>

- Go to ``https://github.com/FurrField-Studio/MicroInject/releases`` and download latest ``MicroInject.unitypackage``
- Import it to your project
    
</details>

<br>

## Basic usage

First add ``Micro Inject Manager`` somewhere in your gameobjects, so system can clear internal data and manage scene switching.

### Marking component as dependency
To use component as a dependency you need to mark its class using ``[Dependency]`` attribute.
<br>
Example:

```csharp
[Dependency]
public class ClassB : MonoBehaviour
{
    //variables

    void Start()
    {
        //code
    }
    
    //code
}
```

then add ``RegisterAsDependencies`` component to gameobject that have components with ``[Dependency]`` attribute.

### Marking field for injection
To inject dependency into a field you need to mark it using ``[Inject]`` attribute, and call ``MicroInject.InjectDependencies(this)`` in ``void Start()``
<br>
Example:

```csharp
public class InjectTest : MonoBehaviour
{
    [Inject]
    public ClassA ClassA;
    
    void Start()
    {
        MicroInject.InjectDependencies(this);
    }
}
```