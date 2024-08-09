using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class ResourceHelper : MonoBehaviour
{
    private class InternalData
    {
        public UnityWebRequest Request;
        public UnityWebRequestAsyncOperation Operation;
        public bool Free;
        public object UserData;
        public Action<byte[], object> byteCall;
        public Action<string, object> stringCall;
    }

    private static HashSet<InternalData> _internalDatas;

    static ResourceHelper()
    {
        _internalDatas = new HashSet<InternalData>();
    }

    public void LoadAsset(string path, Action<byte[], object> action, object userData)
    {
        var data = Acquire();
        data.Request = UnityWebRequest.Get(path);
        data.Operation = data.Request.SendWebRequest();
        data.Free = false;
        data.byteCall = action;
        data.stringCall = null;
        data.UserData = userData;
    }

    public void LoadAsset(string path, Action<string, object> action, object userData)
    {
        var data = Acquire();
        data.Request = UnityWebRequest.Get(path);
        data.Operation = data.Request.SendWebRequest();
        data.Free = false;
        data.stringCall = action;
        data.byteCall = null;
        data.UserData = userData;
    }

    private InternalData Acquire()
    {
        var allAgents = _internalDatas.Where(x => x.Free);
        var temp = allAgents.ElementAtOrDefault(0);
        temp = temp ?? new InternalData() { Free = true };
        if (!_internalDatas.Contains(temp))
            _internalDatas.Add(temp);
        return temp;
    }

    private void Update()
    {
        foreach (var item in _internalDatas)
        {
            if (item.Operation.isDone
                && !item.Free)
            {
                item.stringCall?.Invoke(item.Request.downloadHandler.text, item.UserData);
                item.byteCall?.Invoke(item.Request.downloadHandler.data, item.UserData);
                item.Free = true;
                item.Request.Dispose();
            }
        }
    }
}
