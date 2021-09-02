using System.Collections;
using System.Collections.Generic;
using FurrFieldStudio.MicroInject;
using UnityEngine;

[AutoInjectInEditor]
public class InjectTest : MonoBehaviour
{
    [Inject]
    public ClassA ClassA;
    
    [Inject(true)]
    public ClassB ClassB;
    
    [Inject("Test")]
    public ClassNamedDependency ClassNamedDependency;
    
    [Inject("Test1",true)]
    public ClassNamedDependency ClassNamedDependencyInEditor;
    
    void Start()
    {
        MicroInject.InjectDependencies(this);
    }

    void InjectNamedDependency()
    {
        MicroInject.InjectNamedDependency(nameof(ClassNamedDependency), this);
    }
}
