﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace FurrFieldStudio.MicroInject.Editor
{
    public class MicroInjectWindow : EditorWindow
    {
        [MenuItem("FurrField Studio/Micro Inject")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            MicroInjectWindow window = (MicroInjectWindow)EditorWindow.GetWindow(typeof(MicroInjectWindow), false, "Micro Inject");
            window.Show();
        }

        void OnGUI()
        {
            if (GUILayout.Button("Pregenerate dependency list"))
            {
                PreregisterDependencies();
            }
            
            if (GUILayout.Button("Inject dependencies"))
            {
                InjectDependencies();
            }
        }

        private static void InjectDependencies()
        {
            LoadDependencies loadDependencies = FindObjectsOfType<LoadDependencies>()[0];
            
            Dictionary<string, Component> namedDependencies = new Dictionary<string, Component>();
            Dictionary<Type, Component> dependencies = new Dictionary<Type, Component>();
            
            foreach (var component in loadDependencies.DependenciesContainer)
            {
                (bool, string) isNamedDependency = IsNamedDependency(component);
                if (isNamedDependency.Item1)
                {
                    namedDependencies.Add(isNamedDependency.Item2, component);
                }
                else
                {
                    dependencies.Add(component.GetType(), component);
                }
            }

            Dictionary<Type, bool> markedAsAutoInject = new Dictionary<Type, bool>();

            Type componentType;
            foreach (var component in FindObjectsOfType<Component>())
            {
                componentType = component.GetType();
                if (markedAsAutoInject.ContainsKey(componentType))
                {
                    if (markedAsAutoInject[componentType])
                    {
                        MicroInject.InjectDependenciesInEditor(component, dependencies, namedDependencies);
                    }
                }
                else
                {
                    if (Attribute.GetCustomAttribute(componentType, typeof(AutoInjectInEditor)) != null)
                    {
                        markedAsAutoInject.Add(componentType, true);
                        MicroInject.InjectDependenciesInEditor(component, dependencies, namedDependencies);
                    }
                    else
                    {
                        markedAsAutoInject.Add(componentType, false);
                    }   
                }
            }
        }

        private static (bool, string) IsNamedDependency(Component component)
        {
            foreach (var fi in component.GetType().GetFields())
            {
                if (fi.GetCustomAttribute<NamedDependencyField>() != null)
                {
                    return (true, (string) fi.GetValue(component));
                }
            }
            
            return (false, null);
        }

        private static void PreregisterDependencies()
        {
            LoadDependencies loadDependencies = Object.FindObjectsOfType<LoadDependencies>()[0];

            loadDependencies.DependenciesContainer.Clear();

            var rootObjs = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in rootObjs)
            {
                RegisterAsDependencies[] registerAsDependenciesArray = root.GetComponentsInChildren<RegisterAsDependencies>(true);
                if (registerAsDependenciesArray.Length > 0)
                {
                    Type registerAsDependenciesType = registerAsDependenciesArray[0].GetType();
                    Type componentType;
                    foreach (var registerAsDependency in registerAsDependenciesArray)
                    {
                        if (registerAsDependency.RegisterInEditor)
                        {
                            Component[] components = registerAsDependency.GetComponents(typeof(Component));
                            foreach (var com in components)
                            {
                                componentType = com.GetType();
                                if (componentType != registerAsDependenciesType && com is MonoBehaviour && componentType.GetCustomAttribute<Dependency>() != null)
                                {
                                    loadDependencies.DependenciesContainer.Add(com);
                                }
                            }
                        }
                    }   
                }
            }
        }
    }
}