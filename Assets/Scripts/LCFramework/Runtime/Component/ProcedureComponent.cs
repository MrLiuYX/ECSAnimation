using UnityEngine;
using Native.Procedure;
using Native.Construct;

namespace Native.Component
{
    public class ProcedureComponent : ComponentBase
    {
        [SerializeField]
        private string[] _procedureTypes;
        [SerializeField]
        private string _startProcedure;
        private int _startIndex;

        private Procedure.ProcedureBase[] _procedureBases;

        public override void ManagerSet()
        {
            Manager = (IManager)LaunchComponent.Procedure;

        }

        public override void ComponentSet()
        {
            base.ComponentSet();
            _startIndex = -1;
            if (_procedureTypes.Length == 0)
                return;
            _procedureBases = new ProcedureBase[_procedureTypes.Length];
            for (int i = 0; i < _procedureBases.Length; i++)
            {
                _procedureBases[i] = (ProcedureBase)System.Activator.CreateInstance(System.Type.GetType($"{_procedureTypes[i]}"));
                if (_procedureTypes[i] == _startProcedure)
                {
                    _startIndex = i;
                }
            }
            LaunchComponent.Procedure.SetProcedureBase(_procedureBases);
        }

        public override void GameStartExcute()
        {
            base.GameStartExcute();
            if (_startIndex != -1)
                LaunchComponent.Procedure.StartProcedure(_procedureBases[_startIndex]);
            else
                UnityEngine.Debug.LogError($"Can't launch procedure, because you not set or create procedure for this project");
        }
    }
}
