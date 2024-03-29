﻿using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FurrFieldStudio.MicroInject.Components
{
    public class MicroInjectManager : MonoBehaviour
    {
        public bool RebuildDependencyListOnSceneChange;

        private void Start()
        {
            DontDestroyOnLoad(this);

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += change =>
            {
                if (change == PlayModeStateChange.ExitingPlayMode)
                {
                    MicroInject.Instance.ClearMicroInjectLists();
                }
            };
#endif

            if (RebuildDependencyListOnSceneChange)
            {
                SceneManager.sceneUnloaded += scene => MicroInject.Instance.RebuildDependencyList();
            }
        }
    }
}