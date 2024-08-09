using UnityEngine;
using System.Collections.Generic;
using Native.Component;
using Native.Event;

namespace Native.UI
{
    public class UIGroup : IUIGroup, IReference
    {
        public string GroupName { get; private set; }

        public int GroupDeepLayer { get; private set; }

        public IUIForm CurrentUIForm { get; private set; }
        public Transform GroupRoot { get; private set; }

        private LinkedList<IUIForm> _uiforms;

        public static UIGroup Create(string groupName, int groupDeepLayer, Transform groupRoot)
        {
            var data = ReferencePool.Acquire<UIGroup>();
            data.GroupName = groupName;
            data.GroupDeepLayer = groupDeepLayer;
            data._uiforms = data._uiforms ?? new LinkedList<IUIForm>();
            data.GroupRoot = groupRoot;
            return data;
        }

        public void Clear()
        {
            GroupName = default(string);
            GroupDeepLayer = default(int);
            CurrentUIForm = null;
            GroupRoot = null;
            _uiforms.Clear();
        }

        public void OpenUIForm(IUIForm uiform, object userData)
        {
            if (!_uiforms.Contains(uiform))
            {
                _uiforms.AddLast(uiform);
                uiform.UIFormLogic.transform.SetParent(GroupRoot);
                var rect = uiform.UIFormLogic.AddOrGetComponent<RectTransform>();
                uiform.UIFormLogic.AddOrGetComponent<Canvas>();
                uiform.UIFormLogic.AddOrGetComponent<UnityEngine.UI.GraphicRaycaster>();

                rect.localPosition = Vector3.zero;
                rect.localRotation = Quaternion.identity;
                rect.localScale = Vector3.one;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.pivot = Vector2.one * 0.5f;
                rect.sizeDelta = Vector2.zero;
            }

            uiform.UIFormLogic.transform.SetAsLastSibling();
            uiform.OnOpen(userData);

            var node = _uiforms.Find(uiform);
            _uiforms.Remove(node);
            _uiforms.AddLast(node);

            node = _uiforms.Find(uiform).Previous;
            while (node != null)
            {
                if (node.Value.UIState == UIState.Open)
                {
                    node.Value.OnCover();
                    break;
                }

                node = node.Previous;
            }

            LaunchComponent.Event.Fire(Native.Event.OnOpenUISuccess.EventId, OnOpenUISuccess.Create(uiform));
        }

        public void CloseUIForm(IUIForm uiform, object userData)
        {
            uiform.OnClose(userData);

            var node = _uiforms.Find(uiform).Previous;
            while (node != null)
            {
                if (node.Value.UIState == UIState.Open)
                {
                    node.Value.OnReveal();
                    break;
                }

                node = node.Previous;
            }

            LaunchComponent.Event.Fire(Native.Event.OnCloseUISuccess.EventId, OnCloseUISuccess.Create(uiform));
        }

        public void DestroyUIForm(IUIForm uiform)
        {
            _uiforms.Remove(uiform);
        }
    }
}
