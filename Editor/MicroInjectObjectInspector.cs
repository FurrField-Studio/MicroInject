using FurrFieldStudio.MicroInject.Components;
using UnityEditor;
using UnityEngine;

namespace FurrFieldStudio.MicroInject.Editor
{
    [CustomEditor(typeof(MicroInjectObject))]
    [CanEditMultipleObjects]
    public class MicroInjectObjectInspector : UnityEditor.Editor
    {
        private MicroInjectObject _microInjectObject;
        
        private SerializedProperty _checkChildren;
        
        private SerializedProperty _registerGlobally;
        
        private SerializedProperty _objectName;
        
        private SerializedProperty _enableAutogenerateContainer;
        
        private SerializedProperty _generatedFolder;
        private SerializedProperty _generatedNamespace;
        private SerializedProperty _generatedClassName;
        
        private SerializedProperty _dependenciesContainer;

        private SerializedProperty _soContainer;
        
        private SerializedProperty _blackboardDependencies;
        
        private SerializedProperty _generatedContainer;

        void OnEnable()
        {
            _checkChildren = serializedObject.FindProperty("checkChildren");
            _registerGlobally = serializedObject.FindProperty("registerGlobally");
            _objectName = serializedObject.FindProperty("objectName");
            _enableAutogenerateContainer = serializedObject.FindProperty("enableAutogenerateContainer");
            
            _generatedFolder = serializedObject.FindProperty("generatedFolder");
            _generatedNamespace = serializedObject.FindProperty("generatedNamespace");
            _generatedClassName = serializedObject.FindProperty("generatedClassName");
            
            _dependenciesContainer = serializedObject.FindProperty("dependenciesContainer");
            
            _soContainer = serializedObject.FindProperty("soContainer");
            
            _blackboardDependencies = serializedObject.FindProperty("blackboardDependencies");
                        
            _generatedContainer = serializedObject.FindProperty("generatedContainer");

            _microInjectObject = (MicroInjectObject)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(_checkChildren);
            EditorGUILayout.PropertyField(_registerGlobally);
            EditorGUILayout.PropertyField(_objectName);
            EditorGUILayout.PropertyField(_enableAutogenerateContainer);

            if (_enableAutogenerateContainer.boolValue)
            {
                EditorGUILayout.BeginVertical("BOX");
                
                GUILayout.Label("Generation settings");
                
                EditorGUILayout.PropertyField(_generatedFolder);
                EditorGUILayout.PropertyField(_generatedNamespace);
                EditorGUILayout.PropertyField(_generatedClassName);
                
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.PropertyField(_dependenciesContainer);

            EditorGUILayout.PropertyField(_soContainer);
            
            EditorGUILayout.PropertyField(_blackboardDependencies);

            GUI.enabled = false;
            EditorGUILayout.PropertyField(_generatedContainer);
            GUI.enabled = true;

            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Generate concrete bucket")) _microInjectObject.GenerateConcreteBucket();
            if (GUILayout.Button("Add concrete bucket component")) _microInjectObject.AddConreteBucketComponent();
            if (GUILayout.Button("Inject dependencies")) _microInjectObject.InjectDependencies();
        }
    }
}