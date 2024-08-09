using System;
using System.Collections.Generic;
using Native.Construct;
using UnityEngine;

namespace Native.Event
{
    public class EventManager : ManagerBase<EventManager>, IEventManager
    {
        private sealed class InternalData : IReference
        {
            public long EventID;
            public EventArgsBase Args;

            public static InternalData Create(long eventID, EventArgsBase args)
            {
                var data = ReferencePool.Acquire<InternalData>();
                data.EventID = eventID;
                data.Args = args;
                return data;
            }

            public void Clear()
            {
                EventID = default(long);
                ReferencePool.Release(Args);
            }
        }

        private Dictionary<long, EventHandler<EventArgsBase>> _eventPool;
        private long _serializeEventId;
        private Dictionary<long, string> _eventDict;

        private Queue<InternalData> _waitFireEvent;

        public bool InitDone { get; private set; }

        public override void Init()
        {
            base.Init();
            _eventPool = new Dictionary<long, EventHandler<EventArgsBase>>();
            _eventDict = new Dictionary<long, string>();
            _waitFireEvent = new Queue<InternalData>();
            _serializeEventId = 0;
            InitDone = true;
        }

        public bool CheckHasEvent(long eventId, EventHandler<EventArgsBase> @eventCall)
        {
            if (!_eventPool.ContainsKey(eventId))
            {
                return false;
            }

            var lists = _eventPool[eventId].GetInvocationList();
            for (int i = 0; i < lists.Length; i++)
            {
                if (((EventHandler<EventArgsBase>)lists[i]) == @eventCall)
                {
                    return true;
                }
            }

            return false;
        }

        public void Fire(long eventId, EventArgsBase @eventArgs)
        {
            if (!_eventPool.ContainsKey(eventId))
            {
                UnityEngine.Debug.LogWarning($"lyx [EventManager.Fire] Can't find eventId:{eventId}, eventName:{_eventDict[eventId]}");
                ReferencePool.Release(@eventArgs);
                return;
            }            

            _waitFireEvent.Enqueue(InternalData.Create(eventId, @eventArgs));
        }

        public void FireNow(long eventId, EventArgsBase @eventArgs)
        {
            if (!_eventPool.ContainsKey(eventId))
            {
                UnityEngine.Debug.LogWarning($"lyx [EventManager.Fire] Can't find eventId:{eventId}, eventName:{_eventDict[eventId]}");
                ReferencePool.Release(@eventArgs);
                return;
            }

            _eventPool[eventId].Invoke(this, @eventArgs);
            ReferencePool.Release(@eventArgs);
        }

        public void Subscribe(long eventId, EventHandler<EventArgsBase> @eventCall)
        {
            if (!_eventPool.ContainsKey(eventId))
            {
                _eventPool.Add(eventId, @eventCall);
            }
            else
            {
                if (CheckHasEvent(eventId, @eventCall))
                {
                    UnityEngine.Debug.LogError($"lyx [EventManager.Subscribe] already subscribe eventFunction \"{eventCall.Method.Name}\"");
                    return;
                }
                _eventPool[eventId] += @eventCall;
            }
        }

        public void UnSubscribe(long eventId, EventHandler<EventArgsBase> @eventCall)
        {
            if (!_eventPool.ContainsKey(eventId))
            {
                UnityEngine.Debug.LogError($"lyx [EventManager.UnSubscribe] Can't find eventId:{eventId}, eventName:{_eventDict[eventId]}");
                return;
            }

            if (!CheckHasEvent(eventId, @eventCall))
            {
                UnityEngine.Debug.LogError($"lyx [EventManager.UnSubscribe] Can't find eventFunction \"{@eventCall.Method.Name}\"");
                return;
            }

            _eventPool[eventId] -= @eventCall;

            if (_eventPool[eventId] == null)
            {
                _eventPool.Remove(eventId);
            }
        }

        public override void OnUpdate(float elapseSecond)
        {
            base.OnUpdate(elapseSecond);

            while (_waitFireEvent.Count != 0)
            {
                var data = _waitFireEvent.Dequeue();
                if(_eventPool.ContainsKey(data.EventID))
                    _eventPool[data.EventID].Invoke(this, data.Args);
                ReferencePool.Release(data);
            }
        }

        public long GetEventId(string eventName)
        {
            _eventDict.Add(++_serializeEventId, eventName);
            return _serializeEventId;
        }

        public override void SubInspectorDraw()
        {
            base.SubInspectorDraw();
            GUILayout.BeginVertical();
            {
                GUILayout.Label("自身属性");
                foreach (var eventId in _eventPool.Keys)
                {
                    GUILayout.Label($"=======eventName:{_eventDict[eventId]} eventId:{eventId}=======");
                    if (_eventPool[eventId] == null)
                        continue;
                    var lists = _eventPool[eventId].GetInvocationList();
                    for (int i = 0; i < lists.Length; i++)
                    {
                        GUILayout.Label($"Already register function {(((EventHandler<EventArgsBase>)lists[i]).Method.Name)} from Type {((EventHandler<EventArgsBase>)lists[i]).Method.DeclaringType.Name}");
                    }
                }
            }
            GUILayout.EndVertical();
        }
    }
}
