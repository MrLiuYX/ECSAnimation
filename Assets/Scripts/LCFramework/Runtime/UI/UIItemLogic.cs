using Native.Component;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Native.UI
{
    public class UIItemLogic : MonoBehaviour
    {
        //测试用
        protected string spineName = "caocao_gou,daochan_huli,dianweigou,dongzhuo_qx,guojiagou,huanggai,huangzhong,huatuo_yang,liubei,lusu,machao";
        protected UIFormLogic UIFormLogic;
        protected RectTransform SelfRect;

        public virtual void OnInitBind()
        {
            SelfRect = GetComponent<RectTransform>();
        }

        public virtual void OnSpawn(UIFormLogic logic, object userData)
        {
            UIFormLogic = logic;
            var allTexts = gameObject.GetComponentsInChildren<Text>(true);
            for (int i = 0; i < allTexts.Length; i++)
            {
                ConfigManager.Instance.ReigsterLanguage(allTexts[i]);
            }

            var buttons = gameObject.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].onClick.AddListener(PlayClickedAudio);
            }
        }

        public virtual void OnUnsapwn()
        {
            var allTexts = gameObject.GetComponentsInChildren<Text>(true);
            for (int i = 0; i < allTexts.Length; i++)
            {
                ConfigManager.Instance.RemoveLanguage(allTexts[i]);
            }

            var buttons = gameObject.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].onClick.RemoveListener(PlayClickedAudio);
            }
        }

        public virtual void OnUpdate(float elpaseSeconds)
        {

        }

        public virtual void OnFixedUpdate(float elpaseSeconds)
        {

        }
        public virtual void OnLateUpdate(float elpaseSeconds)
        {

        }

        public virtual void OnRefresh(object userData)
        {

        }

        private void PlayClickedAudio()
        {
            //LaunchComponent.Audio.PlayUIClickAudio();
        }
    }
}
