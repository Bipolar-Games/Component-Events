using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.ComponentEvents
{
	using Object = UnityEngine.Object;

	public static class EventTypeMappings
	{
		private static readonly Dictionary<Type, Type> eventDataTypesByArgumentType = new Dictionary<Type, Type>
		{
			[typeof(int)] = typeof(EventDataInt),
			[typeof(bool)] = typeof(EventDataBool),
			[typeof(float)] = typeof(EventDataFloat),
			[typeof(string)] = typeof(EventDataString),
			[typeof(char)] = typeof(EventDataChar),
			[typeof(double)] = typeof(EventDataDouble),
			[typeof(GameObject)] = typeof(EventDataGameObject),
			[typeof(Object)] = typeof(EventDataObject)
		};
		
		public static bool TryGetEventDataType(Type argumentType, out Type eventDataType) =>
			eventDataTypesByArgumentType.TryGetValue(argumentType, out eventDataType);

		public static void Add<TEvent, TEventData>() =>
			Add(typeof(TEvent), typeof(TEventData));

		public static void Add(Type argumentType, Type eventDataType)
		{
			eventDataTypesByArgumentType[argumentType] = eventDataType;
		}

		public static void Remove<TEvent>() =>
			Remove(typeof(TEvent));

		public static void Remove(Type argumentType)
		{
			eventDataTypesByArgumentType.Remove(argumentType);
		}
	}
}
