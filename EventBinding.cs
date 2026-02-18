using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Bipolar.ComponentEvents
{
	public class EventBinding
	{
		private EventInfo EventInfo;
		private System.Delegate InvokeDelegate; 
		private Component TargetComponent;

		private EventBinding(Component targetComponent, EventData eventData)
		{
			TargetComponent = targetComponent;

			var componentType = targetComponent.GetType();
			EventInfo = componentType.GetEvent(eventData.eventName);

			var eventParameters = ComponentEventsUtility.GetEventParameterExpressions(EventInfo);
			Expression instanceExpression = Expression.Constant(eventData.UnityEvent);

			var invokeUnityEventInfo = eventData.UnityEvent.GetType().GetMethod(nameof(UnityEvent.Invoke));
			var unityEventParameters = invokeUnityEventInfo.GetParameters();

			Expression expression = CreateUnityEventInvokeExpression();
			LambdaExpression lambda = Expression.Lambda(EventInfo.EventHandlerType, expression, eventParameters);

			var compiledDelegate = lambda.Compile();
			InvokeDelegate = compiledDelegate;

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

		public static EventBinding Create(Component target, EventData eventData) => 
			new EventBinding(target, eventData);

		public void Enable()
		{
			EventInfo.AddEventHandler(TargetComponent, InvokeDelegate);
		}

		public void Disable()
		{
			EventInfo.RemoveEventHandler(TargetComponent, InvokeDelegate);
		}
	}
}


