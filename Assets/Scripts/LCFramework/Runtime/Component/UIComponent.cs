using Native.Component;
using Native.Construct;
using Native.UI;
using UnityEngine;

public class UIComponent : ComponentBase
{
    [SerializeField]
    private string[] _groupNames;
    [SerializeField]
    private int[] _groupDeepLayers;
    [SerializeField]
    private float _uiFormDestroyTime;

    public override void ManagerSet()
    {
        Manager = (IManager)LaunchComponent.UI;
    }

    public override void ComponentSet()
    {
        base.ComponentSet();
        LaunchComponent.UI.SetGroupData(_groupNames, _groupDeepLayers);
        ((UIManager)LaunchComponent.UI).UIRoot = gameObject.GetComponent<Transform>();
        ((UIManager)LaunchComponent.UI).UICanvas = gameObject.GetComponent<Canvas>();
        ((UIManager)LaunchComponent.UI).UIFormDestroyTime = _uiFormDestroyTime;
    }
}
