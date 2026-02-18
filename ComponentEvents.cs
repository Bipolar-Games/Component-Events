using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Bipolar.ComponentEvents
{
	public sealed partial class ComponentEvents : MonoBehaviour
	{
		[SerializeField]
		internal Component targetComponent;

		[SerializeReference]
		internal BaseEventData[] eventsData;

		private void Awake()
		{
			if (targetComponent == null)
			{
				enabled = false;
				Destroy(this);
				return;
			}

			CreateEventBindings();
		}

		private void CreateEventBindings()
		{
			var componentType = targetComponent.GetType();
			var events = componentType.GetEvents();
			int count = Mathf.Min(events.Length, eventsData.Length);
			for (int i = 0; i < count; i++)
				BindEvent(eventsData[i]);

			void BindEvent(BaseEventData unityEventData)
			{
				var eventInfo = componentType.GetEvent(unityEventData.eventName);
				unityEventData.EventInfo = eventInfo;

				var eventParameters = GetEventParameterExpressions();

				Expression instanceExpression = Expression.Constant(unityEventData.UnityEvent);
				var invokeUnityEventInfo = unityEventData.UnityEvent.GetType().GetMethod(nameof(UnityEvent.Invoke));
				var unityEventParameters = invokeUnityEventInfo.GetParameters();

				Expression expression = CreateUnityEventInvokeExpression();

				LambdaExpression lambda = Expression.Lambda(eventInfo.EventHandlerType, expression, eventParameters);

				Delegate compiledDelegate = lambda.Compile();
				unityEventData.InvokeDelegate = compiledDelegate;

				ParameterExpression[] GetEventParameterExpressions()
				{
					Type eventHandlerType = eventInfo.EventHandlerType;
					MethodInfo invokeMethodInfo = eventHandlerType.GetMethod(nameof(Action.Invoke));
					ParameterInfo[] parameterInfos = invokeMethodInfo.GetParameters();
					ParameterExpression[] eventParameters = parameterInfos.Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();
					return eventParameters;
				}

				Expression CreateUnityEventInvokeExpression()
				{
					int possibleParametersCount = Mathf.Min(2, eventParameters.Length);
					if (unityEventParameters.Length > 0)
					{
						for (int i = 0; i < possibleParametersCount; i++)
						{
							var argumentType = eventParameters[i].Type;
							if (EventTypeMappings.TryGetEventDataType(argumentType, out _))
							{
								Expression passedParameter = eventParameters[i];
								return Expression.Call(instanceExpression, invokeUnityEventInfo, passedParameter);
							}
						}
					}
					return Expression.Call(instanceExpression, invokeUnityEventInfo);
				}
			}
		}



		private void OnEnable()
		{
			foreach (var eventDatum in eventsData)
			{
				EventInfo eventInfo = eventDatum.EventInfo;
				Debug.Log(eventDatum.UnityEvent.GetPersistentEventCount() + " persistent actions");
				eventInfo.AddEventHandler(targetComponent, eventDatum.InvokeDelegate);
			}
		}

		private void OnDisable()
		{
			foreach (var eventDatum in eventsData)
				eventDatum?.EventInfo?.RemoveEventHandler(targetComponent, eventDatum.InvokeDelegate);
		}

		private void OnDestroy()
		{
			for (int i = 0; i < eventsData.Length; i++)
			{
				eventsData[i].Clear();
				eventsData[i] = null;
			}
		}


		private void OnValidate()
		{
			RefreshEvents();
		}
	}
}
