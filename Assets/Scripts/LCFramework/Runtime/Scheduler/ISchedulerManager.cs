using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Scheduler
{
    public interface ISchedulerManager
    {
        /// <summary>
        /// 启动一个延迟多少帧运行的方法
        /// </summary>
        /// <param name="func">方法函数</param>
        /// <param name="delay">延迟多少帧开始开始第一次运行</param>
        /// <param name="interval">每次运行间隔多少帧</param>
        /// <param name="repeatTime">重复执行几次</param>
        /// <returns></returns>
        public int Scheduler_Frame(Action func, int delay, int interval = 0, int repeatTime = 1);

        /// <summary>
        /// 启动一个延迟多少毫秒运行的方法
        /// </summary>
        /// <param name="func">方法函数</param>
        /// <param name="delay">延迟多少毫秒</param>
        /// <param name="interval">间隔多少毫秒</param>
        /// <param name="repeatTime">重复执行几次</param>
        /// <returns></returns>
        public int Scheduler_Time(Action func, int delay, int interval = 0, int repeatTime = 1);

        /// <summary>
        /// 是否已经删除了此schedule
        /// </summary>
        /// <param name="id">scheduleId</param>
        /// <param name="clear">是否清理</param>
        /// <returns></returns>
        public bool HasCancelSchedule(int id, bool clear = false);

        /// <summary>
        /// 取消注册
        /// </summary>
        /// <param name="id"></param>
        public void UnRegisterScheuler(int id);
    }
}
