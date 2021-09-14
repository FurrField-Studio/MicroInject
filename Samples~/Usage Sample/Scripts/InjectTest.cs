using System.Collections;
using System.Collections.Generic;
using FurrFieldStudio.MicroInject;
using Samples.Usage_Sample.Scripts;
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

    public DynamicInject<DynamicDependency> DynamicInject;

    void Start()
    {
        MicroInject.InjectDependencies(this);
        StartCoroutine(DynamicInjectCoroutine());
    }

    void InjectNamedDependency()
    {
        MicroInject.InjectNamedDependency(nameof(ClassNamedDependency), this);
    }

    private IEnumerator DynamicInjectCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        DynamicInject.Name = "DynamicTest";
        Debug.Log(DynamicInject.Value);

        yield return new WaitForSeconds(1.5f);
        Debug.Log(DynamicInject.Value);
        
        yield return new WaitForSeconds(2f);
        DynamicInject.Name = "DynamicTest1";
        Debug.Log(DynamicInject.Value);
    }
}
