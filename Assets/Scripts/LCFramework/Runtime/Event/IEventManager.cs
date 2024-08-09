using System;
using Native.Construct;

namespace Native.Event
{
    public interface IEventManager
    {
        public bool InitDone { get; }

        /// <summary>
        /// 检查是否存在此Event
        /// </summary>
        public bool CheckHasEvent(long eventId, EventHandler<EventArgsBase> @eventCall);

        /// <summary>
        /// 关注事件
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="event"></param>
        public void Subscribe(long eventId, EventHandler<EventArgsBase> @eventCall);

        /// <summary>
        /// 取消关注
        /// </summary>
        public void UnSubscribe(long eventId, EventHandler<EventArgsBase> @eventCall);

        /// <summary>
        /// 下一帧发送事件
        /// </summary>
        public void Fire(long eventId, EventArgsBase @eventArgs);

        /// <summary>
        /// 立即发送事件
        /// </summary>
        public void FireNow(long eventId, EventArgsBase @eventArgs);

        /// <summary>
        /// 获取事件id
        /// </summary>
        public long GetEventId(string eventName);
    }
}
