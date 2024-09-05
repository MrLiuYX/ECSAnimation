using UnityEngine;
using UnityEngine.UI;
using Native.Component;
using Native.Event;

namespace Native.UI
{
	public partial class MainMenuChooseLevelUIItem : UIItemLogic
	{

		public override void OnInitBind()
		{
			base.OnInitBind();
			BindComponents(gameObject);
		}

		public override void OnSpawn(UIFormLogic logic, object userData)
		{
			base.OnSpawn(logic, userData);
			RegisterEvent();
		}

		public override void OnUnsapwn()
		{
			base.OnUnsapwn();
			RemoveEvent();
		}

		private void RegisterEvent()
		{
		}
		private void RemoveEvent()
		{
		}
	}
}
