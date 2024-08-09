
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class DemoTestButton : MonoBehaviour
{
    private bool _hide;
    private const int _totalCount = 20;
    private float FPS;

    private int _currentCount;
    private float _currentValue;
    private void Update()
    {
        _currentValue += Time.deltaTime;

        if(_currentCount == _totalCount)
        {
            FPS = 1 / (_currentValue / _currentCount);
            _currentCount = 0;
            _currentValue = 0;
        }
        _currentCount++;
    }

    private void OnGUI()
    {
        if (!ECSBridgeManager.ECSInitDone) return;
        GUIStyle fontStyle = new GUIStyle();
        fontStyle.normal.background = null;    //设置背景填充
        fontStyle.normal.textColor = new Color(1, 0, 0);   //设置字体颜色
        fontStyle.fontSize = 40;

        var str = _hide ? "显示" : "隐藏";
        GUILayout.Label(Size("", 30));
        GUILayout.Label(Size("方阵1的顶点有3405个, 方阵2 3405个, 方阵3 300个, 方阵4 7210个", 30));
        GUILayout.Label(Size(string.Format("FPS:{0:f2}", FPS), 30));
        if (GUILayout.Button(Size(str, 30), GUILayout.Width(100), GUILayout.Height(100)))
        {
            _hide = !_hide;
        }

        if (_hide)
        {
            return;
        }

        for (int i = 0; i < 4; i++)
        {
            if(GUILayout.Button(Size($"方阵{i + 1}", 30), GUILayout.Width(100), GUILayout.Height(100)))
            {
                ECSBridgeManager.Instance.PlayerInputSystem.Create(i, 1000);
            }
        }

        if(GUILayout.Button(Size($"Clear", 30), GUILayout.Width(100), GUILayout.Height(100)))
        {
            ECSBridgeManager.Instance.PlayerInputSystem.Clear();
        }

        if (ECSBridgeManager.Instance.PlayerInputSystem == null) return;
        GUILayout.Label(Size($"实体数量{ECSBridgeManager.Instance.PlayerInputSystem.Count}", 40));
    }

    private string Size(string str, int size)
    {
        return $"<size={size}>{str}</size>";
    }
}
