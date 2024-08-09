using Native.FSM;
using Native.Component;
using Native.Event;
using UnityEngine;

namespace Native.Procedure
{
	public class ProcedureInitManager : ProcedureBase
	{
		public override void OnInit(IFSM fsm)
		{
			base.OnInit(fsm);
			ECSBridgeManager.Instance.InitManager();
		}

		public override void OnEnter(IFSM fsm)
		{
			base.OnEnter(fsm);
		}

		public override void OnUpdate(IFSM fsm, float elpaseSecond)
		{
			base.OnUpdate(fsm, elpaseSecond);
			if (!ECSBridgeManager.ECSInitDone) return;
			ECSBridgeManager.Instance.GetSystem();
			fsm.ChangeState<ProcedureGame>();
		}

		public override void OnExit(IFSM fsm)
		{
			base.OnExit(fsm);
		}
	}
}
