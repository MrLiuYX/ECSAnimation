using Native.Event;

namespace Native.Construct
{
    public class ManagerBaseSingle<T> : Single<T> where T : class, new()
    {
        public virtual void OnInit()
        {

        }

        public virtual void OnDisable()
        {

        }

        public virtual void OnEnable()
        {

        }

        public virtual void OnUpdate(float elapseSecond)
        {

        }

        public virtual void OnFixedUpdate(float elapseSecond)
        {

        }

        public virtual void OnLateUpdate(float elapseSecond)
        {

        }

        public virtual void OnShutDown()
        {

        }

        public virtual void OnReset()
        {

        }
    }
}
