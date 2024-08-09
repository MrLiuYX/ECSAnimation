using Native.Component;
using Native.UI;

namespace Native.Event
{
	public class OnCloseUISuccess : EventArgsBase
	{
		private static string _eventName = "OnCloseUISuccess";
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

        public IUIForm UIForm
        {
            get;
            private set;
        }

        public static OnCloseUISuccess Create(IUIForm uiform)
		{
			var data = ReferencePool.Acquire<OnCloseUISuccess>();
			data.UIForm = uiform;
			return data;
		}
	}
}
