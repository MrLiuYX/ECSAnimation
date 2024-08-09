using Native.Construct;
using System.Collections.Generic;
using Native.FSM;

namespace Native.Procedure
{
    public class ProcedureManager : ManagerBase<ProcedureManager>, IProcedureManager
    {
        private ProcedureBase[] _procedures;
        private IFSM _procedureFSM;

        public ProcedureBase[] GetAllProcedure()
        {
            return _procedures;
        }

        public ProcedureBase GetCurrentProcedure()
        {
            return (ProcedureBase)_procedureFSM.GetCurrentState();
        }

        public void StartProcedure<T>() where T : ProcedureBase
        {
            InternalCheckCreateProcedureFSM();
            _procedureFSM.StartState<T>();
        }

        public void StartProcedure<T>(T t) where T : ProcedureBase
        {
            InternalCheckCreateProcedureFSM();
            _procedureFSM.StartState(t);
        }

        public void SetProcedureBase(params ProcedureBase[] procedureBases)
        {
            _procedures = procedureBases;
        }

        private void InternalCheckCreateProcedureFSM()
        {
            _procedureFSM =
                _procedureFSM == null
                ? Component.LaunchComponent.FSM.CreateFSM(this, "MainProcedure", _procedures)
                : _procedureFSM;
        }

        public override void OnUpdate(float elapseSecond)
        {
            base.OnUpdate(elapseSecond);
            _procedureFSM.OnUpdate(_procedureFSM, elapseSecond);
        }

        public override void OnFixedUpdate(float elapseSecond)
        {
            base.OnUpdate(elapseSecond);
            _procedureFSM.OnFixedUpdate(_procedureFSM, elapseSecond);
        }

        public override void OnLateUpdate(float elapseSecond)
        {
            base.OnUpdate(elapseSecond);
            _procedureFSM.OnLateUpdate(_procedureFSM, elapseSecond);
        }
    }
}
