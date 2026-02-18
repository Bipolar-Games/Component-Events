using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Bipolar.ComponentEvents
{
	[System.Serializable]
	public abstract class BaseEventData
	{
		public string eventName;
		public abstract UnityEventBase UnityEvent { get; }
		public EventInfo EventInfo { get; set; }
		public System.Delegate InvokeDelegate { get; set; }

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


