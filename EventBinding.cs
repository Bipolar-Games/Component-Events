using System.Reflection;
using UnityEngine;

namespace Bipolar.ComponentEvents
{
	public class EventBinding
	{
		public EventInfo EventInfo { get; }
		public System.Delegate InvokeDelegate { get; }

		private EventBinding(Component target, EventData eventData)
		{

		}

		public static void Create(Component target, EventData eventData) 
			=> new EventBinding(target, eventData);


		public void Enable(Component targetComponent)
		{
			EventInfo.AddEventHandler(targetComponent, InvokeDelegate);
		}

		public void Disable(Component targetComponent)
		{
			EventInfo.RemoveEventHandler(targetComponent, InvokeDelegate);
		}
	}
#endif
}


