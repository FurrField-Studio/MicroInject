using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FurrFieldStudio.MicroInject
{
    public class RebuildDependencyListOnSceneChange : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
            SceneManager.sceneUnloaded += scene =>
            {
                MicroInject.RebuildDependencyList();
            };
        }
    }
}