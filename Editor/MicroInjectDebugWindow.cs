using FurrFieldStudio.MicroInject.Components;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace FurrFieldStudio.MicroInject.Editor
{
    public class MicroInjectDebugWindow : EditorWindow
    {
        private static bool GeneratedScripts
        {
            get => SessionState.GetBool(MI_CONCRETE_COMPONENTS_ADD_TEMPVAL, false);

            set => SessionState.SetBool(MI_CONCRETE_COMPONENTS_ADD_TEMPVAL, value);
        }

        private const string MI_CONCRETE_COMPONENTS_ADD_TEMPVAL = "MicroInject_AddConcreteComponents";
        
        [MenuItem("Tools/Micro Inject/Debug window")]
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            MicroInjectDebugWindow debugWindow = (MicroInjectDebugWindow) GetWindow(typeof(MicroInjectDebugWindow), false, "Micro Inject");
            debugWindow.Show();
        }

        private void OnGUI()
        {
            DebugDependencies();
        }

        [MenuItem("Tools/Micro Inject/Regenerate containers")]
        private static void InjectDependencies()
        {
            foreach (var mio in FindObjectsOfType<MicroInjectObject>())
            {
                mio.GenerateConcreteBucket();
            }

            GeneratedScripts = true;
        }
        
        [DidReloadScripts]
        private static void CreateAssetWhenReady()
        {
            if(!GeneratedScripts) return;

            if(EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += CreateAssetWhenReady;
                return;
            }
     
            EditorApplication.delayCall += AddConcreteComponents;
        }


        private static void AddConcreteComponents()
        {
            foreach (var mio in FindObjectsOfType<MicroInjectObject>())
            {
                mio.AddConreteBucketComponent();
            }

            EditorApplication.delayCall -= CreateAssetWhenReady;
            EditorApplication.delayCall -= AddConcreteComponents;
            
            GeneratedScripts = false;
        }

        #region DebugViews

        private static void DebugDependencies()
        {
            GUILayout.BeginVertical("BOX");
            GUILayout.Label("Dependencies");
            
            foreach (var kvp in MicroInject.GetGlobalBuckets())
            {
                GUILayout.BeginVertical("BOX");
                
                EditorGUILayout.LabelField("Type:", kvp.Key.ToString());

                if (kvp.Value is Component comp)
                {
                    EditorGUILayout.ObjectField("Component:", comp, typeof(Component), true);
                }

                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }

        #endregion
    }
}