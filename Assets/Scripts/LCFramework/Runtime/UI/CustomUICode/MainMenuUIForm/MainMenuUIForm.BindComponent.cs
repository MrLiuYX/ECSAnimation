using UnityEngine;
using UnityEngine.UI;

namespace Native.UI
{
	public partial class MainMenuUIForm
	{
		private Image _imgBg;
		private Text _txtEnterGame;
		private Button _btnEnterGame;
		private GridLayoutGroup _gridChooseLevel;
		private Image _imgChooseLevelBg;
		private RectTransform _rectChooseLevel;

		public virtual void BindComponents(GameObject go)
		{
			var autoBindComponent = go.GetComponent<UIAutoBind>();
			_imgBg = autoBindComponent.GetAutoBindComponent<Image>(0);
			_txtEnterGame = autoBindComponent.GetAutoBindComponent<Text>(1);
			_btnEnterGame = autoBindComponent.GetAutoBindComponent<Button>(2);
			_gridChooseLevel = autoBindComponent.GetAutoBindComponent<GridLayoutGroup>(3);
			_imgChooseLevelBg = autoBindComponent.GetAutoBindComponent<Image>(4);
			_rectChooseLevel = autoBindComponent.GetAutoBindComponent<RectTransform>(5);
		}
	}
}
