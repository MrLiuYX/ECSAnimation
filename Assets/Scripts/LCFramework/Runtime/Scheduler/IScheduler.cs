using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Native.Scheduler.Scheduler;

namespace Native.Scheduler
{
    public interface IScheduler : IReference
    {
        /// <summary>
        /// 上一级
        /// </summary>
        public IScheduler Parent { get; }
        
        /// <summary>
        /// 下一级
        /// </summary>
        public IScheduler Child { get; }

        /// <summary>
        /// 最低级
        /// </summary>
        public IScheduler SchedulerLoweast { get; }

        /// <summary>
        /// Tick方法
        /// </summary>
        public ITicker Ticker { get; }
        
        /// <summary>
        /// 总刻度
        /// </summary>
        public int MaxSize { get; }

        /// <summary>
        /// 每个刻度的单位值
        /// </summary>
        public int SingleUnit { get; }

        /// <summary>
        /// 创建父亲
        /// </summary>
        public IScheduler CreateParent(int parentMaxSize);

        /// <summary>
        /// 注册定时器
        /// </summary>
        /// <param name="pack"></param>
        public void RegisterScheduler(FuncPack pack);

        /// <summary>
        /// 移动到下一个刻度
        /// </summary>
        public void MoveOne();

        /// <summary>
        /// 运行
        /// </summary>
        public void Invoke();

        public void GetAllFuncPackAndClear(List<FuncPack> tempList);

        public int GetOffset(int Interval);
    }

    public interface ITicker
    {
        /// <summary>
        /// 运行Index个数
        /// </summary>
        public int Tick(float elapseSeconds);
        public ITicker Clone();
    }
}
