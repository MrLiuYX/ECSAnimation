using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Native.Construct;

namespace Native.Scheduler
{
    public class SchedulerManager : ManagerBase<ISchedulerManager>, ISchedulerManager
    {
        private IScheduler _frame;
        private IScheduler _time;

        private List<int> _cancelIds;
        private int _id;

        public override void Init()
        {
            base.Init();
            _frame = Scheduler.CreateLowestScheduler_Farme(100, 1)
                .CreateParent(100);
            _time = Scheduler.CreateLowestScheduler_MS(100, 10)
                .CreateParent(60)
                .CreateParent(60);
            _cancelIds = new List<int>();
        }

        public bool HasCancelSchedule(int id, bool clear = false)
        {
            if (_cancelIds.Contains(id))
            {
                if (clear) _cancelIds.Remove(id);
                return true;
            }
            return false;
        }

        public int Scheduler_Frame(Action func, int delay, int interval = 0, int repeatTime = 1)
        {
            var id = GetId();
            interval = interval < _frame.SchedulerLoweast.SingleUnit ? _frame.SchedulerLoweast.SingleUnit : interval;
            delay = delay < _frame.SchedulerLoweast.SingleUnit ? _frame.SchedulerLoweast.SingleUnit : delay + _frame.SchedulerLoweast.GetOffset(delay);            
            _frame.RegisterScheduler(Scheduler.FuncPack.Create(id, func, delay, interval, repeatTime));
            return id;
        }

        public int Scheduler_Time(Action func, int delay, int interval = 0, int repeatTime = 1)
        {
            var id = GetId();
            interval = interval < _time.SchedulerLoweast.SingleUnit ? _time.SchedulerLoweast.SingleUnit : interval;
            delay = delay < _time.SchedulerLoweast.SingleUnit ? _time.SchedulerLoweast.SingleUnit : delay + _time.SchedulerLoweast.GetOffset(delay);
            _time.RegisterScheduler(Scheduler.FuncPack.Create(id, func, delay, interval, repeatTime));
            return id;
        }

        private int GetId()
        {
            _id++;
            return _id;
        }

        public void UnRegisterScheuler(int id)
        {
            _cancelIds.Add(id);
        }

        public override void OnUpdate(float elapseSecond)
        {
            base.OnUpdate(elapseSecond);
            
            var count = _frame.Ticker.Tick(elapseSecond);
            while(count != 0)
            {
                _frame.SchedulerLoweast.MoveOne();
                _frame.SchedulerLoweast.Invoke();
                count--;
            }

            count = _time.Ticker.Tick(elapseSecond);
            while(count != 0)
            {
                _time.SchedulerLoweast.MoveOne();
                _time.SchedulerLoweast.Invoke();
                count--;
            }
        }
    }
}
