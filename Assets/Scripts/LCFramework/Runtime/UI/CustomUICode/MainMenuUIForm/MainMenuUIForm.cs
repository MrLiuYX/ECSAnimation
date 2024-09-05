using UnityEngine;
using UnityEngine.UI;
using Native.Component;
using Native.Event;
using Native.Procedure;

namespace Native.UI
{
	public partial class MainMenuUIForm : UIFormLogic
	{

		public override void OnInit(object userData)
		{
			base.OnInit(userData);
			BindComponents(gameObject);
		}

		public override void OnOpen(object userData)
		{
			base.OnOpen(userData);
			RegisterEvent();
		}

		public override void OnClose(object userData)
		{
			base.OnClose(userData);
			RemoveEvent();
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void RegisterEvent()
		{
			_btnEnterGame.onClick.AddListener(OnBtnbtnEnterGameClicked);
		}
		private void RemoveEvent()
		{
			_btnEnterGame.onClick.RemoveListener(OnBtnbtnEnterGameClicked);
		}

		private void OnBtnbtnEnterGameClicked()
		{
			(LaunchComponent.Procedure.GetCurrentProcedure() as ProcedureMain).GoToGame();
		}
	}
}
