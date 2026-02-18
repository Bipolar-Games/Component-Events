#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Bipolar.ComponentEvents.Editor
{
    [CustomEditor(typeof(ComponentEvents))]
    public class ComponentEventsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
		{
			using (new EditorGUI.DisabledScope(true))
				EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);

#if !UNITY_2022_1_OR_NEWER
            if (PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup) == ScriptingImplementation.IL2CPP)
            {
                EditorGUILayout.HelpBox("Component Events is not supported in IL2CPP scripting backend", MessageType.Error);
                //return;
            }
#endif
			var componentProperty = serializedObject.FindProperty(nameof(ComponentEvents.targetComponent));
			var eventsDataProperty = serializedObject.FindProperty(nameof(ComponentEvents.eventsData));

			DrawComponentProperty();

			var targetComponent = componentProperty?.objectReferenceValue;
			if (targetComponent)
				DrawEventProperties();

			void DrawComponentProperty()
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(componentProperty);
				if (EditorGUI.EndChangeCheck())
					serializedObject.ApplyModifiedProperties();
			}

			void DrawEventProperties()
			{
				EditorGUILayout.Space();
				EditorGUI.BeginChangeCheck();
				for (int i = 0; i < eventsDataProperty.arraySize; i++)
				{
					var eventProperty = eventsDataProperty.GetArrayElementAtIndex(i);
					var unityEventProperty = eventProperty?.FindPropertyRelative(nameof(EventData.unityEvent));
					if (unityEventProperty != null)
					{
						var label = new GUIContent(ObjectNames.NicifyVariableName(GetEventDataName(eventProperty)));
						EditorGUILayout.PropertyField(unityEventProperty, label);
					}
				}

				bool somethingChanged = EditorGUI.EndChangeCheck();
				if (somethingChanged)
					serializedObject.ApplyModifiedProperties();
			}
		}

        private static string GetEventDataName(SerializedProperty property)
        {
            return property?.FindPropertyRelative(nameof(EventData.eventName))?.stringValue;
        }
    }
}
#endif
