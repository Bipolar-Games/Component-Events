using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Bipolar.ComponentEvents
{
	public class EventBinding
	{
		public EventInfo EventInfo { get; }
		public System.Delegate InvokeDelegate { get; }

		public void Enable(Component targetComponent)
		{
			EventInfo.AddEventHandler(targetComponent, InvokeDelegate);
		}

		public void Disable(Component targetComponent)
		{
			EventInfo.RemoveEventHandler(targetComponent, InvokeDelegate);
		}
	}


	[System.Serializable]
	public abstract class BaseEventData
	{
		public string eventName;
		public abstract UnityEventBase UnityEvent { get; }


		public EventInfo EventInfo { get; set; }
		public System.Delegate InvokeDelegate { get; set; }

		public void Initialize(Component targetComponent)
		{
			var componentType = targetComponent.GetType();
			EventInfo = componentType.GetEvent(eventName);

			var eventParameters = ComponentEventsUtility.GetEventParameterExpressions(EventInfo);
			Expression instanceExpression = Expression.Constant(UnityEvent);
			var invokeUnityEventInfo = UnityEvent.GetType().GetMethod(nameof(UnityEngine.Events.UnityEvent.Invoke));
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

		internal void Clear()
		{
			eventName = null;
			InvokeDelegate = null;
			EventInfo = null;
			UnityEvent.RemoveAllListeners();
		}

		public void Enable(Component targetComponent)
		{
			EventInfo.AddEventHandler(targetComponent, InvokeDelegate);
		}

		public void Disable(Component targetComponent)
		{
			EventInfo.RemoveEventHandler(targetComponent, InvokeDelegate);
		}
	}

	[System.Serializable]
	internal partial class EventData : BaseEventData
	{
		[SerializeField]
		internal UnityEvent unityEvent;
		public override UnityEventBase UnityEvent => unityEvent;
	}

	[System.Serializable]
	internal partial class EventDataInt : BaseEventData
	{
		[SerializeField]
		internal UnityEvent<int> unityEvent;
		public override UnityEventBase UnityEvent => unityEvent;
	}

	[System.Serializable]
	internal partial class EventDataFloat : BaseEventData
	{
		[SerializeField]
		internal UnityEvent<float> unityEvent;
		public override UnityEventBase UnityEvent => unityEvent;
	}

	[System.Serializable]
	internal partial class EventDataString : BaseEventData
	{
		[SerializeField]
		internal UnityEvent<string> unityEvent;
		public override UnityEventBase UnityEvent => unityEvent;
	}

	[System.Serializable]
	internal partial class EventDataBool : BaseEventData
	{
		[SerializeField]
		internal UnityEvent<bool> unityEvent;
		public override UnityEventBase UnityEvent => unityEvent;
	}

	[System.Serializable]
	internal partial class EventDataDouble : BaseEventData
	{
		[SerializeField]
		internal UnityEvent<double> unityEvent;
		public override UnityEventBase UnityEvent => unityEvent;
	}

	[System.Serializable]
	internal partial class EventDataChar : BaseEventData
	{
		[SerializeField]
		internal UnityEvent<char> unityEvent;
		public override UnityEventBase UnityEvent => unityEvent;
	}

	[System.Serializable]
	internal partial class EventDataObject : BaseEventData
	{
		[SerializeField]
		internal UnityEvent<Object> unityEvent;
		public override UnityEventBase UnityEvent => unityEvent;
	}

	[System.Serializable]
	internal partial class EventDataGameObject : BaseEventData
	{
		[SerializeField]
		internal UnityEvent<GameObject> unityEvent;
		public override UnityEventBase UnityEvent => unityEvent;
	}

#if UNITY_EDITOR

	internal class ComponentEventsBuildPreprocessor : UnityEditor.Build.IPreprocessBuildWithReport
	{
		public int callbackOrder => 0;

		public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
		{
			Debug.Log("Preprocess build");
			//PlayerSettings.SetAdditionalIl2CppArgs("--compilation-defines=HEJKA");
		}
	}
#endif
}


