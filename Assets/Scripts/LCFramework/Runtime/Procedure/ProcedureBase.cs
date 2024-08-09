using Native.FSM;
namespace Native.Procedure
{
    public class ProcedureBase : IFSMState
    {
        public virtual void OnEnter(IFSM fsm)
        {
            UnityEngine.Debug.Log($"OnEnter procedure {GetType().Name}");
        }

        public virtual void OnExit(IFSM fsm)
        {
            UnityEngine.Debug.Log($"OnExit procedure {GetType().Name}");
        }

        public virtual void OnFixedUpdate(IFSM fsm, float elpaseSecond)
        {

        }

        public virtual void OnInit(IFSM fsm)
        {
            UnityEngine.Debug.Log($"OnInit procedure {GetType().Name}");
        }

        public virtual void OnLateUpdate(IFSM fsm, float elpaseSecond)
        {

        }

        public virtual void OnUpdate(IFSM fsm, float elpaseSecond)
        {
#if UNITY_WEBGL
            UnityEngine.Debug.Log($"OnUpdate procedure {GetType().Name}");
#endif
        }
    }
}
