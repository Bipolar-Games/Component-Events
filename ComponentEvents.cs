using UnityEngine;

namespace Bipolar.ComponentEvents
{
	public sealed partial class ComponentEvents : MonoBehaviour
	{
		[SerializeField]
		internal Component targetComponent;

		[SerializeReference]
		internal EventData[] eventsData;

		private EventBinding[] runtimeBindigns;

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

			runtimeBindigns = new EventBinding[count];
			for (int i = 0; i < count; i++)
				runtimeBindigns[i] = new EventBinding(targetComponent, eventsData[i]);
		}

		private void OnEnable()
		{
			foreach (var binding in runtimeBindigns)
				binding.Enable();
		}

		private void OnDisable()
		{
			foreach (var binding in runtimeBindigns)
				binding.Disable();
		}

		private void OnDestroy()
		{
			for (int i = 0; i < runtimeBindigns.Length; i++)
				runtimeBindigns[i] = null;

			runtimeBindigns = null;
		}

		private void OnValidate()
		{
			RefreshEvents();
		}
	}
}
