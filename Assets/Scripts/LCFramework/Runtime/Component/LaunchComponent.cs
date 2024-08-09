using Native.Resource;
using Native.FSM;
using Native.Event;
using System.Collections.Generic;
using UnityEngine;
using Native.Construct;
using System.Reflection;
using System;
using Unity.Mathematics;

namespace Native.Component
{
    public partial class LaunchComponent : MonoBehaviour
    {
        private List<ComponentBase> _components;
        private static List<Action> _staticClear;
        private bool _needOrderManager;
        public static LaunchComponent Instance;
        public void RegisterComponent(ComponentBase componentBase)
        {
            _components.Add(componentBase);
        }

        private void Awake()
        {
#if UNITY_EDITOR
            //Application.targetFrameRate = 60;
#else
            Application.targetFrameRate = 60;
#endif
            Instance = this;
            _needOrderManager = false;
            _components = new List<ComponentBase>();
            var components = GameObject.FindObjectsOfType<ComponentBase>();
            for (int i = 0; i < components.Length; i++)
            {
                components[i].RegisterComponent();
            }

            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].ManagerSet();
            }

            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].ComponentSet();
            }

            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].Manager.Init();
            }

            Event.Subscribe(ManagerOrderChange.EventId, OnManagerOrderChanged);
        }

        private void OnEnable()
        {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].Manager.OnEnable();
            }
        }

        private void Start()
        {
            _components.Sort((x, y) =>
            {
                if (x.Manager.Order > y.Manager.Order)
                    return 1;
                return 0;
            });

            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].GameStartExcute();
            }
        }

        private void Update()
        {
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i].Manager.EnableUpdate)
                    _components[i].Manager.OnUpdate(Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i].Manager.EnableFixedUpdate)
                    _components[i].Manager.OnFixedUpdate(Time.fixedDeltaTime);
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i].Manager.EnableLateUpdate)
                    _components[i].Manager.OnLateUpdate(Time.deltaTime);
            }

            if (_needOrderManager)
            {
                _components.Sort((x, y) =>
                {
                    if (x.Manager.Order > y.Manager.Order)
                        return 1;
                    return 0;
                });
                _needOrderManager = false;
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].Manager.OnDisable();
            }
        }

        private void OnDestroy()
        {
            Event.UnSubscribe(ManagerOrderChange.EventId, OnManagerOrderChanged);

            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].Manager.ShutDown();
            }
        }

        private void OnManagerOrderChanged(object sender, EventArgsBase e)
        {
            _needOrderManager = true;
        }

        public static void StaticClearRegister(Action call)
        {
            _staticClear.Add(call);
        }

        private void OnApplicationQuit()
        {
            for (int i = 0; i < _staticClear.Count; i++)
            {
                _staticClear[i].Invoke();
            }
            _staticClear.Clear();
        }
    }
}
