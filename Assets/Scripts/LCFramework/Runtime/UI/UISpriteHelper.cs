using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Native.Component;
using Native.Resource;
using Native;
using System;
using UnityEngine.U2D;
using System.IO;

public static class UISpriteHelper
{
    private class InternalData : IReference
    {
        public Sprite Sprite => _sprite;
        public string SpritePath;

        private Queue<Action<Sprite>> _callback;
        private Sprite _sprite;
        private bool _loadFild;
        private SpriteAtlas _atlas;
        private string _spriteAtlasPath;


        public static InternalData Create(string path)
        {
            var data = ReferencePool.Acquire<InternalData>();
            data.SpritePath = path;
            data._callback = data._callback ?? new Queue<Action<Sprite>>();

            var directoryPath = Path.GetDirectoryName(data.SpritePath).Replace("\\", "/");
            var split = directoryPath.Split('/');
            var spriteAtlasPath = directoryPath + "/" + split[split.Length - 1] + ".spriteatlasv2";
            data._spriteAtlasPath = spriteAtlasPath;

            return data;
        }

        public void Clear()
        {
            _sprite = null;
            _callback.Clear();
        }

        public void Load(Action<Sprite> callback)
        {
            _callback.Enqueue(callback);
            InternalLoad();
        }

        private void InternalLoad()
        {
            if (_loadFild) return;

            if (_sprite != null)
            {
                while (_callback.Count != 0)
                {
                    _callback.Dequeue()?.Invoke(_sprite);
                }
                return;
            }
            
            LaunchComponent.Resource.LoadAsset(_spriteAtlasPath, new LoadAssetCallback(
                (name, asset, time, userData) =>
                {
                    var atlas = asset as SpriteAtlas;                    
                    _sprite = atlas.GetSprite(Path.GetFileNameWithoutExtension(SpritePath));
                    InternalLoad();
                }, (name, time, userData)=>
                {
                    _loadFild = true;
                }));
        }
    }


    private static Dictionary<string, InternalData> _datas = new Dictionary<string, InternalData>();

    public static void LoadIcon(this Image img, string path, bool setNativeSize = false)
    {
        if (!_datas.ContainsKey(path))
        {
            _datas.Add(path, InternalData.Create(path));
        }
        _datas[path].Load((sprite) =>
        {
            img.sprite = sprite;
            if (setNativeSize) img.SetNativeSize();
        });
    }
}