using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FurrFieldStudio.MicroInject.Components
{
    public class MicroInjectManager : MonoBehaviour
    {
        public bool RebuildDependencyListOnSceneChange;

        private void Start()
        {
            DontDestroyOnLoad(this);

            EditorApplication.playModeStateChanged += change =>
            {
                if (change == PlayModeStateChange.ExitingPlayMode)
                {
                    MicroInject.ClearMicroInjectLists();
                }
            };

            if (RebuildDependencyListOnSceneChange)
            {
                SceneManager.sceneUnloaded += scene =>
                {
                    MicroInject.RebuildDependencyList();
                };
            }
        }
    }
}