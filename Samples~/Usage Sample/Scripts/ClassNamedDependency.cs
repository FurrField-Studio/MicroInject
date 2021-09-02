using System.Collections;
using System.Collections.Generic;
using FurrFieldStudio.MicroInject;
using UnityEngine;

[Dependency]
public class ClassNamedDependency : MonoBehaviour
{

    [NamedDependencyField]
    public string DependencyName;
    
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
