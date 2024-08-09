using Native.Component;
using System;

namespace Native.Event
{
	public class OnProcedureChangedEvent : EventArgsBase
	{
		private static string _eventName = "OnProcedureChangedEvent";
		private static long _eventId = 0;
		public static long EventId
		{
			get
			{
				_eventId = _eventId == 0 ? Native.Component.LaunchComponent.Event.GetEventId(_eventName) : _eventId;
				return _eventId;
			}
		}
		public override void Clear()
		{
			base.Clear();
		}

		public Type LastProcedure
		{
			private set;
			get;
		}

		public static OnProcedureChangedEvent Create(Type lastProcedure)
		{
			var data = ReferencePool.Acquire<OnProcedureChangedEvent>();
            data.LastProcedure = lastProcedure;
			return data;
		}
	}
}
