using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FurrFieldStudio.MicroInject.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace FurrFieldStudio.MicroInject.Editor
{
    public class MicroInjectWindow : EditorWindow
    {

        private bool m_IsDebugFoldoutOpen;
        
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
            
            if (GUILayout.Button("Clear MicroInject lists"))
            {
                FurrFieldStudio.MicroInject.MicroInject.ClearMicroInjectLists();
            }

            m_IsDebugFoldoutOpen = EditorGUILayout.Foldout(m_IsDebugFoldoutOpen, "Debug");
            if (m_IsDebugFoldoutOpen)
            {
                DebugDepencencies();
                DebugNamedDependencies();
                DebugDynamicInjectFields();
            }
        }

        private static void InjectDependencies()
        {
            MicroInjectLoadDependencies microInjectLoadDependencies = FindObjectsOfType<MicroInjectLoadDependencies>()[0];
            
            Dictionary<string, Component> namedDependencies = new Dictionary<string, Component>();
            Dictionary<Type, Component> dependencies = new Dictionary<Type, Component>();
            
            foreach (var component in microInjectLoadDependencies.DependenciesContainer)
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
            MicroInjectLoadDependencies[] loadDependenciesArray = FindObjectsOfType<MicroInjectLoadDependencies>();

            if (loadDependenciesArray.Length == 0) return;

            MicroInjectLoadDependencies microInjectLoadDependencies = loadDependenciesArray[0];
            
            microInjectLoadDependencies.DependenciesContainer.Clear();
            
            int countLoaded = SceneManager.sceneCount;
            Scene[] loadedScenes = new Scene[countLoaded];

            GameObject[] rootObjs;
            
            for (int i = 0; i < countLoaded; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;
                
                rootObjs = SceneManager.GetSceneAt(i).GetRootGameObjects();
                foreach (var root in rootObjs)
                {
                    RegisterAsDependencies[] registerAsDependenciesArray = root.GetComponentsInChildren<RegisterAsDependencies>(true);
                    if (registerAsDependenciesArray.Length <= 0) continue;
                    
                    Type registerAsDependenciesType = registerAsDependenciesArray[0].GetType();
                    Type componentType;
                    foreach (var registerAsDependency in registerAsDependenciesArray)
                    {
                        if (!registerAsDependency.RegisterInEditor) continue;
                        
                        Component[] components = registerAsDependency.GetComponents(typeof(Component));
                        foreach (var com in components)
                        {
                            componentType = com.GetType();
                            if (componentType != registerAsDependenciesType && com is MonoBehaviour && componentType.GetCustomAttribute<Dependency>() != null)
                            {
                                microInjectLoadDependencies.DependenciesContainer.Add(com);
                            }
                        }
                    }
                }
            }
        }

        #region DebugViews

        private static void DebugDepencencies()
        {
            GUILayout.BeginVertical("BOX");
            GUILayout.Label("Dependencies");
            
            foreach (var kvp in MicroInject.GetDependenciesList())
            {
                GUILayout.BeginVertical("BOX");
                
                EditorGUILayout.LabelField("Type:", kvp.Key.ToString());
                EditorGUILayout.ObjectField("Component:", kvp.Value, typeof(Component), true);
                
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }

        private static void DebugNamedDependencies()
        {
            GUILayout.BeginVertical("BOX");
            GUILayout.Label("Named Dependencies");
            
            foreach (var kvp in MicroInject.GetNamedDependencies())
            {
                GUILayout.BeginVertical("BOX");
                
                EditorGUILayout.LabelField("Name:", kvp.Key);
                EditorGUILayout.ObjectField("Component:", kvp.Value, typeof(Component), true);
                
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }

        private static void DebugDynamicInjectFields()
        {
            GUILayout.BeginVertical("BOX");
            GUILayout.Label("Dynamic Inject Fields");

            Component comp = null;
            
            foreach (var kvp in MicroInject.GetDynamicInjectFields())
            {
                GUILayout.BeginVertical("BOX");
                
                EditorGUILayout.LabelField("Name:", kvp.Key);

                if (kvp.Value.Count > 0) comp = kvp.Value[0].ObjectValue;
                else comp = null;
                
                EditorGUILayout.ObjectField("Component:", comp, typeof(Component), true);

                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }

        #endregion
    }
}