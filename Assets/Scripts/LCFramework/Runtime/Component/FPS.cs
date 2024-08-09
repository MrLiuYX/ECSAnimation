using UnityEngine;

/// <summary>
/// ��ӡFPS
/// </summary>
public class FPS : MonoBehaviour
{
    float _updateInterval = 1f;
    float _accum = .0f;
    int _frames = 0;
    float _timeLeft;
    string fpsFormat;

    void Start()
    {
        _timeLeft = _updateInterval;
    }

    void OnGUI()
    {
        GUIStyle labelFont = new GUIStyle();

        labelFont.normal.textColor = new Color(1, 0, 0);

        labelFont.fontSize = 50;
        GUI.Label(new Rect(100, 100, 200, 200), fpsFormat, labelFont);

    }

    void Update()
    {
        _timeLeft -= Time.deltaTime;
        _accum += Time.timeScale / Time.deltaTime;
        ++_frames;

        if (_timeLeft <= 0)
        {
            float fps = _accum / _frames;
            fpsFormat = System.String.Format("{0:F2}FPS", fps);

            _timeLeft = _updateInterval;
            _accum = .0f;
            _frames = 0;
        }
    }
}
