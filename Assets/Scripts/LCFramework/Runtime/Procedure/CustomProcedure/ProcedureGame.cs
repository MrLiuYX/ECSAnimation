using Native.FSM;
using Native.Component;
using Native.Event;
using UnityEngine;

namespace Native.Procedure
{
	public class ProcedureGame : ProcedureBase
	{
		public override void OnInit(IFSM fsm)
		{
			base.OnInit(fsm);
		}

		public override void OnEnter(IFSM fsm)
		{
			base.OnEnter(fsm);
			ECSBridgeManager.Instance.OnEnterGame();
		}

		public override void OnUpdate(IFSM fsm, float elpaseSecond)
		{
			base.OnUpdate(fsm, elpaseSecond);
            ECSBridgeManager.Instance.OnUpdate(elpaseSecond);
        }

		public override void OnExit(IFSM fsm)
		{
			base.OnExit(fsm);
            ECSBridgeManager.Instance.OnExitGame();
        }
	}
}
