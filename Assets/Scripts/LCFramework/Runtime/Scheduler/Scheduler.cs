using Native.Component;
using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace Native.Scheduler
{
    public class Scheduler : IScheduler
    {
        public class FuncPack : IReference
        {
            public int SchedulerId;
            public Action Function;
            public int RepeatTime;
            public int Delay;
            public int Interval;
            public Dictionary<IScheduler, int> IndexOffset;

            public static FuncPack Create(int schedulerId, Action func, int delay, int interval, int RepeatTime)
            {
                var data = ReferencePool.Acquire<FuncPack>();
                data.SchedulerId = schedulerId;
                data.Function = func;
                data.RepeatTime = RepeatTime;
                data.Delay = delay;
                data.Interval = interval;
                data.IndexOffset = DictionaryPool<IScheduler, int>.Get();
                return data;
            }

            public void Clear()
            {
                Function = null;
                DictionaryPool<IScheduler, int>.Release(IndexOffset);
            }
        }

        public IScheduler Parent { get; private set; }
        public IScheduler Child { get; private set; }
        public IScheduler SchedulerLoweast { get; private set; }
        public ITicker Ticker { get; private set; }
        public int MaxSize { get; private set; }
        public int SingleUnit { get; private set; }

        private List<List<FuncPack>> _size;
        private int _currentIndex;

        public IScheduler CreateParent(int parentMaxSize)
        {
            var data = ReferencePool.Acquire<Scheduler>();
            data.Parent = null;
            data.Child = this;
            data.MaxSize = parentMaxSize;
            data.SingleUnit = data.Child.MaxSize * data.Child.SingleUnit;
            data.Ticker = data.Child.Ticker.Clone();
            data._size = new List<List<FuncPack>>(parentMaxSize);
            for (int i = 0; i < parentMaxSize; i++)
            {
                data._size.Add(null);
            }
            data.SchedulerLoweast = this.SchedulerLoweast;
            this.Parent = data;
            return data;
        }

        public void Clear()
        {
            ReferencePool.Release(this);
            if(this.Child != null)
            {
                ReferencePool.Release(this.Child);
            }
        }

        /// <summary>
        /// 帧Scheduler
        /// </summary>
        public static Scheduler CreateLowestScheduler_Farme(int maxSize, int unit)
        {
            var data = ReferencePool.Acquire<Scheduler>();
            data.Parent = null;
            data.Child = null;
            data.MaxSize = maxSize;
            data.SingleUnit = unit;
            data.Ticker = FrameTicker.Create(unit);
            data._size = new List<List<FuncPack>>();
            for (int i = 0; i < maxSize; i++)
            {
                data._size.Add(null);
            }
            data.SchedulerLoweast = data;
            return data;
        }

        /// <summary>
        /// 毫秒Scheduler
        /// </summary>
        public static Scheduler CreateLowestScheduler_MS(int maxSize, int unit)
        {
            var data = ReferencePool.Acquire<Scheduler>();
            data.Parent = null;
            data.Child = null;
            data.MaxSize = maxSize;
            data.SingleUnit = unit;
            data.Ticker = MsTicker.Create(unit);
            data._size = new List<List<FuncPack>>();
            for (int i = 0; i < maxSize; i++)
            {
                data._size.Add(null);
            }
            data.SchedulerLoweast = data;
            return data;
        }

        public void RegisterScheduler(FuncPack pack)
        {
            if(pack.Delay == 0)
            {
                Invoke(pack);
                return;
            }

            if(pack.Delay > SingleUnit)
            {
                InternalRegister(pack);
                return;
            }

            if(Child != null)
            {
                Child.RegisterScheduler(pack);
                return;
            }

            InternalRegister(pack);
        }

        private int InternalRegister(FuncPack pack, int offset = 0)
        {            
            var registerIndex = (_currentIndex + pack.Delay / SingleUnit + offset) % MaxSize;            
            if (_size[registerIndex] == null)
            {
                _size[registerIndex] = new List<FuncPack>();
            }
            _size[registerIndex].Add(pack);
            return registerIndex;
        }

        public void MoveOne()
        {
            var runPacks = ListPool<FuncPack>.Get();
            _currentIndex++;
            if (_currentIndex == MaxSize)
            {
                _currentIndex %= MaxSize;
                if (Parent != null)
                {
                    Parent.MoveOne();
                    Parent.GetAllFuncPackAndClear(runPacks);
                    for (int i = 0; i < runPacks.Count; i++)
                    {
                        var runPack = runPacks[i];
                        runPack.Delay = runPack.Delay % Parent.SingleUnit;

                        if (!runPack.IndexOffset.ContainsKey(this))
                        {
                            runPack.IndexOffset.Add(this, 0);
                        }

                        InternalRegister(runPacks[i]);
                    }
                }
            }            
            ListPool<FuncPack>.Release(runPacks);
        }

        public void Invoke()
        {
            var runPacks = ListPool<FuncPack>.Get();
            GetAllFuncPackAndClear(runPacks);
            for (int i = 0; i < runPacks.Count; i++)
            {
                Invoke(runPacks[i]);
            }
            ListPool<FuncPack>.Release(runPacks);
        }

        private void Invoke(FuncPack runPack)
        {
            if (LaunchComponent.Scheduler.HasCancelSchedule(runPack.SchedulerId, true)) return;
                
            runPack.Function?.Invoke();

            runPack.RepeatTime--;
            if(runPack.RepeatTime == 0)
            {
                ReferencePool.Release(runPack);
                return;
            }

            var parent = Parent == null ? this : Parent;
            while(parent.Parent != null)
            {
                parent = parent.Parent;
            }
            runPack.Delay = runPack.Interval + GetOffset(runPack.Interval);
            parent.RegisterScheduler(runPack);
        }

        public void GetAllFuncPackAndClear(List<FuncPack> tempList)
        {
            var index = _currentIndex % MaxSize;
            if (_size[index] == null) return;
            while (_size[index].Count != 0)
            {
                tempList.Add(_size[index][0]);
                _size[index].RemoveAt(0);
            }
        }

        public int GetOffset(int Interval)
        {
            var offset = 0;
            offset = Interval > MaxSize * SingleUnit ? _currentIndex * SingleUnit : 0;
            if (Parent != null)
                offset += Parent.GetOffset(Interval);
            return offset;
        }
    }

    /// <summary>
    /// 帧Ticker
    /// </summary>
    public class FrameTicker : ITicker, IReference
    {
        private int _currentFrame;
        private int _frameInterval;
        public static FrameTicker Create(int frameInterval)
        {
            var data = ReferencePool.Acquire<FrameTicker>();
            data._frameInterval = frameInterval;
            return data;
        }

        public void Clear()
        {

        }

        public ITicker Clone()
        {
            var data = ReferencePool.Acquire<FrameTicker>();
            data._frameInterval = _frameInterval;
            return data;
        }

        public int Tick(float elapseSeconds)
        {
            _currentFrame += 1;
            var count = _currentFrame / _frameInterval;
            _currentFrame -= count * _frameInterval;
            return count;
        }
    }

    /// <summary>
    /// 100毫秒Ticker
    /// </summary>
    public class MsTicker : ITicker, IReference
    {
        private float _currentTime;
        private int _msInterval;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msInterval">多少毫秒进位一次</param>
        /// <returns></returns>
        public static MsTicker Create(int msInterval)
        {
            var data = ReferencePool.Acquire<MsTicker>();
            data._msInterval = msInterval;
            return data;
        }

        public void Clear()
        {
            
        }

        public ITicker Clone()
        {
            var data = ReferencePool.Acquire<MsTicker>();
            data._msInterval = _msInterval;
            return data;
        }

        public int Tick(float elapseSeconds)
        {
            _currentTime += elapseSeconds * 1000;
            var count = (int)(_currentTime / _msInterval);
            _currentTime -= count * _msInterval;
            return count;
        }
    }
}
