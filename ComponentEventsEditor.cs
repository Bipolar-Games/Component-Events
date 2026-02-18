#if UNITY_EDITOR
using System.Reflection;
using System;
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

#if !UNITY_2021_1_OR_NEWER
            if (PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup) == ScriptingImplementation.IL2CPP)
            {
                EditorGUILayout.HelpBox("Component Events is not supported in IL2CPP scripting backend", MessageType.Error);
                //return;
            }
#endif

            // DRAWING SCRITPT FIELD
            var componentProperty = serializedObject.FindProperty(nameof(ComponentEvents.targetComponent));
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(componentProperty);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            // CREATING/UPDATING EVENTS
            bool somethingChanged = false;
            var eventsDataProperty = serializedObject.FindProperty(nameof(ComponentEvents.eventsData));
            if (false)
            {
                var targetComponent = componentProperty?.objectReferenceValue;
                if (targetComponent == null)
                    return;

                var componentType = targetComponent.GetType();
                var events = componentType.GetEvents();

                eventsDataProperty.arraySize = Mathf.Max(eventsDataProperty.arraySize, events.Length);
                for (int i = 0; i < events.Length; i++)
                {
                    var componentEvent = events[i];
                    int serializedEventIndex = FindIndex(eventsDataProperty, CompareNames);
                    bool CompareNames(SerializedProperty property) =>
                        GetEventDataName(property) == componentEvent.Name;

                    if (serializedEventIndex < 0)
                    {
                        var newProperty = InsertArrayElementAtIndex(eventsDataProperty, i);
                        CreateNewEventDataInProperty(newProperty, componentType, componentEvent);

                        //var unityEventProperty = newProperty.FindPropertyRelative("unityEvent");
                        //unityEventProperty.managedReferenceValue = CreateUnityEvent(componentEvent, componentType);

                        somethingChanged = true;
                    }
                    else
                    {
                        var singleEventProperty = eventsDataProperty.GetArrayElementAtIndex(i);
                        var correctEventType = ComponentEventsUtility.GetEventDataType(componentEvent.EventHandlerType);
                        if (CheckType(singleEventProperty, correctEventType) == false)
                        {
                            CreateNewEventDataInProperty(singleEventProperty, componentType, componentEvent);
                            somethingChanged = true;
                        }

                        if (serializedEventIndex != i)
                        {
                            eventsDataProperty.MoveArrayElement(serializedEventIndex, i);
                            somethingChanged = true;
                        }
                    }
                }
                eventsDataProperty.arraySize = events.Length;
    
            }

            // DRAWING EVENTS
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

			somethingChanged |= EditorGUI.EndChangeCheck();
            if (somethingChanged)
                serializedObject.ApplyModifiedProperties();
        }

        private static void CreateNewEventDataInProperty(SerializedProperty property, Type componentType, EventInfo componentEvent)
        {
            property.managedReferenceValue = ComponentEventsUtility.CreateEventData(componentEvent);
            property.FindPropertyRelative(nameof(BaseEventData.eventName)).stringValue = componentEvent.Name;
        }

        private static bool CheckType(SerializedProperty eventDataProperty, Type correctEventType)
        {
            string eventTypeName = eventDataProperty.type;
            int realTypeNameStart = eventTypeName.IndexOf('<') + 1;
            int realTypeNameLength = eventTypeName.IndexOf('>') - realTypeNameStart;
            eventTypeName = eventTypeName.Substring(realTypeNameStart, realTypeNameLength);
            bool isCorrect = eventTypeName == correctEventType.Name;
            return isCorrect;
        }

        private static SerializedProperty InsertArrayElementAtIndex(SerializedProperty arrayProperty, int i)
        {
            arrayProperty.InsertArrayElementAtIndex(i);
            var addedElement = arrayProperty.GetArrayElementAtIndex(i);
            return addedElement;
        }

        private static string GetEventDataName(SerializedProperty property)
        {
            return property?.FindPropertyRelative(nameof(EventData.eventName))?.stringValue;
        }


        public static int FindIndex(SerializedProperty arrayProperty, Predicate<SerializedProperty> predicate)
        {
            if (arrayProperty != null)
            {
                for (int i = 0; i < arrayProperty.arraySize; i++)
                {
                    var element = arrayProperty.GetArrayElementAtIndex(i);
                    if (predicate(element))
                        return i;
                }
            }

            return -1;
        }
    }

}
#endif
