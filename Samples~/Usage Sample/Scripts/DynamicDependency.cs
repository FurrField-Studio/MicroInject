using System;
using System.Collections;
using FurrFieldStudio.MicroInject;
using UnityEngine;

namespace Samples.Usage_Sample.Scripts
{
    public class DynamicDependency : MonoBehaviour
    {
        public DynamicDependencyName DynamicDependencyName;

        private void Awake()
        {
            DynamicDependencyName = new DynamicDependencyName(this);
            DynamicDependencyName.Name = "DynamicTest";

            StartCoroutine(DynamicDependencyCoroutine());
        }
        
        private IEnumerator DynamicDependencyCoroutine()
        {
            yield return new WaitForSeconds(1.9f);
            DynamicDependencyName.Name = "DynamicTest1";
        }
    }
}