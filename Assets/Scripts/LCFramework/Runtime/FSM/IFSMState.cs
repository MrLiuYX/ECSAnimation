namespace Native.FSM
{
    public interface IFSMState
    {
        public void OnInit(IFSM fsm);

        public void OnEnter(IFSM fsm);

        public void OnExit(IFSM fsm);

        public void OnUpdate(IFSM fsm, float elpaseSecond);

        public void OnFixedUpdate(IFSM fsm, float elpaseSecond);

        public void OnLateUpdate(IFSM fsm, float elpaseSecond);
    }
}
