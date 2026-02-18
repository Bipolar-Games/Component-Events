using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Bipolar.ComponentEvents
{
	public static class ComponentEventsUtility
	{
		public static BaseEventData CreateEventData(EventInfo componentEvent)
		{
			var eventHandlerType = componentEvent.EventHandlerType;
			Type eventDataType = GetEventDataType(eventHandlerType);

			var unityEventInstance = (BaseEventData)Activator.CreateInstance(eventDataType);
			unityEventInstance.eventName = componentEvent.Name;
			return unityEventInstance;
		}

		public static Type GetEventDataType(Type eventHandlerType)
		{
			var methodInfo = eventHandlerType.GetMethod(nameof(Action.Invoke));
			var eventParameters = methodInfo.GetParameters();

			int possibleParametersCount = Mathf.Min(2, eventParameters.Length);
			for (int i = 0; i < possibleParametersCount; i++)
			{
				var argumentType = eventParameters[i].ParameterType;
				if (EventTypeMappings.TryGetEventDataType(argumentType, out var eventDataType))
					return eventDataType;
			}

			return typeof(EventData);
		}
	}

	public partial class ComponentEvents
	{
		private void RefreshEvents()
		{
#if UNITY_EDITOR
			if (targetComponent == null)
				return;

			var componentType = targetComponent.GetType();
			var events = componentType.GetEvents();
			var eventsDataToSerialize = new List<BaseEventData>();

			for (int i = 0; i < events.Length; i++)
			{
				var eventData = GetEventData(events[i]);
				eventsDataToSerialize.Add(eventData);
			}

			eventsData = eventsDataToSerialize.ToArray();
#endif
		}

		private BaseEventData GetEventData(EventInfo componentEvent)
		{
			int serializedEventIndex = eventsData == null
				? -1
				: Array.FindIndex(eventsData, CompareNames);
			bool CompareNames(BaseEventData eventDatum) =>
				eventDatum.eventName == componentEvent.Name;

			if (serializedEventIndex >= 0)
			{
				var eventData = eventsData[serializedEventIndex];
				var correctEventDataType = ComponentEventsUtility.GetEventDataType(componentEvent.EventHandlerType);
				if (eventData.GetType() == correctEventDataType)
					return eventData;
			}

			return CreateEventData(componentEvent);
		}

		private BaseEventData CreateEventData(EventInfo componentEvent)
		{
			return ComponentEventsUtility.CreateEventData(componentEvent);
		}
	}
}
