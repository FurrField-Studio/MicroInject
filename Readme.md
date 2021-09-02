# Micro Inject

MicroInject is simple Dependency Injection framework made for Unity Engine that allows you to easily inject components.
<br>
<br>
Features:
<br>

- Runtime injection
- In-editor injection - injecting dependencies without any runtime performance impact
- Named dependencies - injecting dependencies that can be named (Useful in multiplayer projects when we want to inject Player1 components first and Player2 components when Player1 dies, to see Player2 stats)
- (<b>Preview</b> not finished) Dynamically named dependencies - the same as above but names can change during runtime (Useful when we want to inject specific player data based on a name during runtime)

<br>
<br>

## Basic usage

### Marking as dependency
To use component as dependency you need to mark its class using ``[Dependency]`` attribute.
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

### Marking field for injecting
To inject dependency into a field you need to mark it using ``[Inject]`` attribute, and call ``{{project.codename}}.InjectDependencies(this)`` in ``void Start()``
<br>
Example:

```csharp
public class InjectTest : MonoBehaviour
{
    [Inject]
    public ClassA ClassA;
    
    void Start()
    {
        {{project.codename}}.InjectDependencies(this);
    }
}
```