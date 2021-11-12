using System;
using System.Collections;
using FurrFieldStudio.MicroInject;
using UnityEngine;

namespace Samples.Usage_Sample.Scripts
{
    public class DynamicDependency : MonoBehaviour
    {
        public DynamicDependencyName DynamicDependencyName = new DynamicDependencyName();

        private void Awake()
        {
            DynamicDependencyName.Component = this;

            StartCoroutine(DynamicDependencyCoroutine());
        }
        
        private IEnumerator DynamicDependencyCoroutine()
        {
            yield return new WaitForSeconds(1.9f);
            DynamicDependencyName.Name = "DynamicTest1";
        }
    }
}