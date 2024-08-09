namespace Native.FSM
{
    public interface IFSM
    {
        public object Owner { get; }
        public string FSMName { get; }

        IFSMState GetCurrentState();
        IFSMState[] GetAllState();
        void ChangeState<T>() where T : IFSMState;
        void ChangeState<T>(T t) where T : IFSMState;
        void StartState<T>() where T : IFSMState;
        void StartState<T>(T t) where T : IFSMState;
        void SetData(string key, object value);
        void SetData(int key, object value);
        object GetData(string key);
        object GetData(int key);
        void OnUpdate(IFSM fsm, float elpaseSecond);
        void OnFixedUpdate(IFSM fsm, float elpaseSecond);
        void OnLateUpdate(IFSM fsm, float elpaseSecond);
    }
}
