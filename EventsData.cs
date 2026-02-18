using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Bipolar.ComponentEvents
{
	[System.Serializable]
	public abstract class EventData
	{
		public string eventName;
		public abstract UnityEventBase UnityEvent { get; }
	}

	[System.Serializable]
	internal class EventDataVoid : EventData
	{
		[SerializeField]
		internal UnityEvent unityEvent;
		public override UnityEventBase UnityEvent => unityEvent;
	}

	[System.Serializable]
	internal class EventDataInt : EventData
	{
		[SerializeField]
		internal UnityEvent<int> unityEvent;
		public override UnityEventBase UnityEvent => unityEvent;
	}

	[System.Serializable]
	internal class EventDataFloat : EventData
	{
		[SerializeField]
		internal UnityEvent<float> unityEvent;
		public override UnityEventBase UnityEvent => unityEvent;
	}

	[System.Serializable]
	internal class EventDataString : EventData
	{
		[SerializeField]
		internal UnityEvent<string> unityEvent;
		public override UnityEventBase UnityEvent => unityEvent;
	}

	[System.Serializable]
	internal class EventDataBool : EventData
	{
		[SerializeField]
		internal UnityEvent<bool> unityEvent;
		public override UnityEventBase UnityEvent => unityEvent;
	}

	[System.Serializable]
	internal class EventDataDouble : EventData
	{
		[SerializeField]
		internal UnityEvent<double> unityEvent;
		public override UnityEventBase UnityEvent => unityEvent;
	}

	[System.Serializable]
	internal class EventDataChar : EventData
	{
		[SerializeField]
		internal UnityEvent<char> unityEvent;
		public override UnityEventBase UnityEvent => unityEvent;
	}

	[System.Serializable]
	internal class EventDataObject : EventData
	{
		[SerializeField]
		internal UnityEvent<Object> unityEvent;
		public override UnityEventBase UnityEvent => unityEvent;
	}

	[System.Serializable]
	internal class EventDataGameObject : EventData
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


