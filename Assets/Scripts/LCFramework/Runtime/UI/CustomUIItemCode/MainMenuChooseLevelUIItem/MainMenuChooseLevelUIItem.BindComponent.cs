using UnityEngine;
using UnityEngine.UI;

namespace Native.UI
{
	public partial class MainMenuChooseLevelUIItem
	{
		private Text _txtLevelName;

		public virtual void BindComponents(GameObject go)
		{
			var autoBindComponent = go.GetComponent<UIAutoBind>();
			_txtLevelName = autoBindComponent.GetAutoBindComponent<Text>(0);
		}
	}
}
