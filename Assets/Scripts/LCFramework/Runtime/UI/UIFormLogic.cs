using Native.Component;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Native.UI
{
    public class UIFormLogic : MonoBehaviour
    {
        public IUIForm UIForm;
        protected Animation SelfAnimation;

        public virtual void OnInit(object userData)
        {
            var allTexts = gameObject.GetComponentsInChildren<Text>(true);
            for (int i = 0; i < allTexts.Length; i++)
            {
                ConfigManager.Instance.ReigsterLanguage(allTexts[i]);
            }
            SelfAnimation = gameObject.GetComponent<Animation>();

            var buttons = gameObject.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].onClick.AddListener(PlayClickedAudio);
            }
        }
        public virtual void OnOpen(object userData)
        {
            gameObject.SetActive(true);
        }
        public virtual void OnCover()
        {

        }
        public virtual void OnReveal()
        {

        }
        public virtual void OnUpdate(float elapseSeconds)
        {
            UIItemPool.OnUpdate(this, elapseSeconds);
        }
        public virtual void OnFixedUpdate(float elapseSeconds)
        {
            UIItemPool.OnFixedUpdate(this, elapseSeconds);
        }
        public virtual void OnLateUpdate(float elapseSeconds)
        {
            UIItemPool.OnLateUpdate(this, elapseSeconds);
        }
        public virtual void OnClose(object userData)
        {
            gameObject.SetActive(false);
            UnspawnAllUIItem();
        }

        public virtual void Destroy()
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
            UIItemPool.DestroyAllPool(this);
        }

        public void CloseSelf()
        {
            Native.Component.LaunchComponent.UI.CloseUIForm(UIForm.SerializeId, null);
        }

        public void SpawnUIItem<T>(Resource.LoadAssetCallback loadAssetCallback, object userData = null) where T : UIItemLogic
        {
            UIItemPool.AcquirePool<T>(this).Spawn(this, loadAssetCallback, userData);
        }

        public void UnspawnAllUIItemByPool<T>() where T : UIItemLogic
        {
            UIItemPool.ReleasePool<T>(this);
        }

        public void UnspawnAllUIItem()
        {
            UIItemPool.ReleaseAllPool(this);
        }        

        public void UnSignleSpawnUIItem(UIItemLogic logic)
        {
            UIItemPool.ReleaseSignle(this, logic);
        }

        private void PlayClickedAudio()
        {
            //LaunchComponent.Audio.PlayUIClickAudio();
        }
    }
}
