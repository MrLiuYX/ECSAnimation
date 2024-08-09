using System;
using UnityEngine;

namespace Native.UI
{
    public interface IUIGroup
    {
        public string GroupName { get; }
        public int GroupDeepLayer { get; }
        public IUIForm CurrentUIForm { get; }
        public Transform GroupRoot { get; }
        public void OpenUIForm(IUIForm uiform, object userData);
        public void CloseUIForm(IUIForm uiform, object userData);
        public void DestroyUIForm(IUIForm uiform);
    }
}
