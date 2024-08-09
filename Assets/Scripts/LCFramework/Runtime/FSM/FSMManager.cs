using UnityEngine;
using Native.Construct;
using System.Collections.Generic;

namespace Native.FSM
{
    public class FSMManager : ManagerBase<FSMManager>, IFSMManager
    {
        private Dictionary<string, IFSM> _currentFSM;
        private int _serilizedId;

        public override void Init()
        {
            base.Init();
            _currentFSM = new Dictionary<string, IFSM>();
        }

        public IFSM CreateFSM<T>(T Owner, string fsmName, params IFSMState[] fsmStates)
        {
            if (_currentFSM.ContainsKey(fsmName))
            {
                UnityEngine.Debug.LogError($"Already exist FSM {fsmName}");
                return null;
            }

            var fsm = new FSM(Owner, fsmName, fsmStates);
            _currentFSM.Add(fsmName, fsm);
            return fsm;
        }

        public IFSM GetFSM(string fsmName)
        {
            if (!_currentFSM.ContainsKey(fsmName))
            {
                UnityEngine.Debug.LogError($"FSM {fsmName} not exist");
                return null;
            }

            _currentFSM.TryGetValue(fsmName, out IFSM fsm);
            return fsm;
        }

        public void DeleteFSM(string fsmName)
        {
            if (!_currentFSM.ContainsKey(fsmName))
            {
                Debug.LogWarning($"FSM {fsmName} not exist!");
            }

            var fsm = _currentFSM[fsmName];
            _currentFSM.Remove(fsmName);
        }

        public override void SubInspectorDraw()
        {
            base.SubInspectorDraw();
            GUILayout.BeginVertical();
            {
                GUILayout.Label("FSM Content:");
                GUILayout.BeginVertical("Box");
                foreach (var fsm in _currentFSM)
                {
                    GUILayout.Label($"FSM \"{fsm.Key}\"");
                    if (fsm.Value.GetCurrentState() != null)
                        GUILayout.Label($"Current State {fsm.Value.GetCurrentState().GetType().FullName}");
                    var allStates = fsm.Value.GetAllState();
                    for (int i = 0; i < allStates.Length; i++)
                    {
                        GUILayout.Label($"Has state {allStates[i].GetType().FullName}");
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }
    }
}
