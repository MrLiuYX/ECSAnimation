using Native.FSM;
using Native.Component;
using Native.Event;
using UnityEngine;
using System;

namespace Native.Procedure
{
	public class ProcedureMain : ProcedureBase
	{
		private int _mainUI;
		private Action _gotoGame;

		public override void OnInit(IFSM fsm)
		{
			base.OnInit(fsm);
			_mainUI = -1;
			_gotoGame = () =>
			{
				fsm.ChangeState<ProcedureGame>();
			};
        }

		public override void OnEnter(IFSM fsm)
		{
			base.OnEnter(fsm);
            _mainUI = LaunchComponent.UI.OpenUIForm(UIFormId.MainMenuUIForm);
            ECSBridgeManager.Instance.ClearAllEntity();

			//延迟一帧让他执行清理工作 因为Ecs的Update和Mono的Update顺序无法控制
			LaunchComponent.Scheduler.Scheduler_Frame(() =>
			{
				ECSBridgeManager.Instance.OnExitGame();
			}, 1);
        }

		public override void OnUpdate(IFSM fsm, float elpaseSecond)
		{
			base.OnUpdate(fsm, elpaseSecond);
		}

		public override void OnExit(IFSM fsm)
		{
			base.OnExit(fsm);
			LaunchComponent.UI.CloseUIForm(_mainUI);
			_mainUI = -1;
            ECSBridgeManager.Instance.OnEnterGame();
        }

		public void GoToGame()
		{
			_gotoGame?.Invoke();
        }
	}
}
