using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Bipolar.ComponentEvents
{
	public class EventBinding
	{
		private readonly EventInfo eventInfo;
		private readonly System.Delegate invokeDelegate; 
		private readonly Component targetComponent;

		private EventBinding(Component targetComponent, EventData eventData)
		{
			this.targetComponent = targetComponent;

			var componentType = targetComponent.GetType();
			eventInfo = componentType.GetEvent(eventData.eventName);

			var eventParameters = ComponentEventsUtility.GetEventParameterExpressions(eventInfo);
			Expression instanceExpression = Expression.Constant(eventData.UnityEvent);

			var invokeUnityEventInfo = eventData.UnityEvent.GetType().GetMethod(nameof(UnityEvent.Invoke));
			var unityEventParameters = invokeUnityEventInfo.GetParameters();

			Expression expression = CreateUnityEventInvokeExpression();
			LambdaExpression lambda = Expression.Lambda(eventInfo.EventHandlerType, expression, eventParameters);

			var compiledDelegate = lambda.Compile();
			invokeDelegate = compiledDelegate;

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
			eventInfo.AddEventHandler(targetComponent, invokeDelegate);
		}

		public void Disable()
		{
			eventInfo.RemoveEventHandler(targetComponent, invokeDelegate);
		}
	}
}


