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

	public abstract class EventData<TEvent> : EventData
		where TEvent : UnityEventBase
	{
		[SerializeField]
		internal TEvent unityEvent;
		public override UnityEventBase UnityEvent => unityEvent;
	}

	[System.Serializable]
	internal class EventDataVoid : EventData<UnityEvent>
	{ }

	public abstract class EventDataWithArgs<TArg> : EventData<UnityEvent<TArg>>
	{ }

	[System.Serializable]
	internal class EventDataInt : EventDataWithArgs<int>
	{ }

	[System.Serializable]
	internal class EventDataFloat : EventDataWithArgs<float>
	{ }

	[System.Serializable]
	internal class EventDataString : EventDataWithArgs<string>
	{ }

	[System.Serializable]
	internal class EventDataBool : EventDataWithArgs<bool>
	{ }

	[System.Serializable]
	internal class EventDataDouble : EventDataWithArgs<double>
	{ }

	[System.Serializable]
	internal class EventDataChar : EventDataWithArgs<char>
	{ }

	[System.Serializable]
	internal class EventDataObject : EventDataWithArgs<Object>
	{ }

	[System.Serializable]
	internal class EventDataGameObject : EventDataWithArgs<GameObject>
	{ }

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


