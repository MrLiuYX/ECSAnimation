using System.Collections.Generic;
namespace Native.FSM
{
    public class FSM : IFSM
    {
        public object Owner { get; private set; }
        public string FSMName { get; private set; }

        private Dictionary<string, object> _stringData;
        private Dictionary<int, object> _intData;
        private IFSMState[] _fsmStates;
        private IFSMState _currentFSMState;
        private HashSet<IFSMState> _initFSMState;

        public FSM(object owner, string fsmName, params IFSMState[] fsmStates)
        {
            Owner = owner;
            FSMName = fsmName;
            _stringData = new Dictionary<string, object>();
            _intData = new Dictionary<int, object>();
            _initFSMState = new HashSet<IFSMState>();
            _fsmStates = fsmStates;
        }

        public IFSMState GetCurrentState()
        {
            if (_currentFSMState == null)
            {
                UnityEngine.Debug.LogError($"FSM \"{FSMName}\" isn't start yet");
            }
            return _currentFSMState;
        }

        public IFSMState[] GetAllState()
        {
            return _fsmStates;
        }

        public void ChangeState<T>() where T : IFSMState
        {
            var type = typeof(T);
            IFSMState target = null;

            for (int i = 0; i < _fsmStates.Length; i++)
            {
                if (_fsmStates[i].GetType() == type)
                {
                    target = _fsmStates[i];
                    break;
                }
            }

            if (target == null)
            {
                UnityEngine.Debug.LogError($"Can't find state \"{type.FullName}\" in fsm \"{FSMName}\"");
                return;
            }

            var last = _currentFSMState;
            if (_currentFSMState != null)
            {
                _currentFSMState.OnExit(this);
            }

            _currentFSMState = target;

            if (!_initFSMState.Contains(_currentFSMState))
            {
                _currentFSMState.OnInit(this);
                _initFSMState.Add(_currentFSMState);
            }

            _currentFSMState.OnEnter(this);

            if(Owner == Native.Component.LaunchComponent.Procedure)
            {
                Native.Component.LaunchComponent.Event.FireNow(Native.Event.OnProcedureChangedEvent.EventId, Native.Event.OnProcedureChangedEvent.Create(last.GetType()));
            }
        }

        public void ChangeState<T>(T t) where T : IFSMState
        {
            for (int i = 0; i < _fsmStates.Length; i++)
            {
                if (_fsmStates[i].GetType() == t.GetType())
                {
                    if (_currentFSMState != null)
                    {
                        _currentFSMState.OnExit(this);
                    }

                    _currentFSMState = _fsmStates[i];

                    if (!_initFSMState.Contains(_currentFSMState))
                    {
                        _currentFSMState.OnInit(this);
                        _initFSMState.Add(_currentFSMState);
                    }

                    _currentFSMState.OnEnter(this);
                }
            }
        }

        public void StartState<T>() where T : IFSMState
        {
            var type = typeof(T);
            IFSMState target = null;

            for (int i = 0; i < _fsmStates.Length; i++)
            {
                if (_fsmStates[i].GetType() == type)
                {
                    target = _fsmStates[i];
                    break;
                }
            }

            if (target == null)
            {
                UnityEngine.Debug.LogError($"Can't find state \"{type.FullName}\" in fsm \"{FSMName}\"");
                return;
            }

            if (_currentFSMState != null)
            {
                _currentFSMState.OnExit(this);
            }

            _currentFSMState = target;

            if (!_initFSMState.Contains(_currentFSMState))
            {
                _currentFSMState.OnInit(this);
                _initFSMState.Add(_currentFSMState);
            }

            _currentFSMState.OnEnter(this);
        }

        public void StartState<T>(T t) where T : IFSMState
        {
            for (int i = 0; i < _fsmStates.Length; i++)
            {
                if (_fsmStates[i].GetType() == t.GetType())
                {
                    if (_currentFSMState != null)
                    {
                        _currentFSMState.OnExit(this);
                    }

                    _currentFSMState = _fsmStates[i];

                    if (!_initFSMState.Contains(_currentFSMState))
                    {
                        _currentFSMState.OnInit(this);
                        _initFSMState.Add(_currentFSMState);
                    }

                    _currentFSMState.OnEnter(this);
                }
            }
        }

        public void SetData(string key, object value)
        {
            if (!_stringData.ContainsKey(key))
                _stringData.Add(key, value);
            else
                _stringData[key] = value;
        }

        public void SetData(int key, object value)
        {
            if (!_intData.ContainsKey(key))
                _intData.Add(key, value);
            else
                _intData[key] = value;
        }

        public object GetData(string key)
        {
            if (_stringData.ContainsKey(key))
                return _stringData[key];
            UnityEngine.Debug.LogWarning($"FSM \"{FSMName}\" not has the key \"{key}\"");
            return null;
        }

        public object GetData(int key)
        {
            if (_intData.ContainsKey(key))
                return _intData[key];
            UnityEngine.Debug.LogWarning($"FSM \"{FSMName}\" not has the key \"{key}\"");
            return null;
        }

        public void OnUpdate(IFSM fsm, float elpaseSecond)
        {
            _currentFSMState.OnUpdate(this, elpaseSecond);
        }

        public void OnFixedUpdate(IFSM fsm, float elpaseSecond)
        {
            _currentFSMState.OnFixedUpdate(this, elpaseSecond);
        }

        public void OnLateUpdate(IFSM fsm, float elpaseSecond)
        {
            _currentFSMState.OnLateUpdate(this, elpaseSecond);
        }
    }
}
