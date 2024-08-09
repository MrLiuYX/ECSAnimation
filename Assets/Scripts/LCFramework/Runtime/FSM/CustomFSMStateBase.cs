using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Native.FSM;
using Native.Component;

namespace Native.FSM
{
    public class CustomFSMStateBase : IFSMState
    {
        public virtual void OnEnter(IFSM fsm)
        {

        }

        public virtual void OnExit(IFSM fsm)
        {

        }

        public virtual void OnFixedUpdate(IFSM fsm, float elpaseSecond)
        {

        }

        public virtual void OnInit(IFSM fsm)
        {

        }

        public virtual void OnLateUpdate(IFSM fsm, float elpaseSecond)
        {

        }

        public virtual void OnUpdate(IFSM fsm, float elpaseSecond)
        {

        }
    }
}
