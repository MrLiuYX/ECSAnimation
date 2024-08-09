using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class UIEventTrigger : MonoBehaviour, IPointerUpHandler, IPointerMoveHandler, IPointerExitHandler, IPointerEnterHandler, IPointerDownHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Action<PointerEventData> onPointerClick;
    public Action<PointerEventData> onPointerDown;
    public Action<PointerEventData> onPointerEnter;
    public Action<PointerEventData> onPointerExit;
    public Action<PointerEventData> onPointerMove;
    public Action<PointerEventData> onPointerUp;
    public Action<PointerEventData> onBeginDrag;
    public Action<PointerEventData> onDrag;
    public Action<PointerEventData> onEndDrag;

    public static UIEventTrigger Get(GameObject go)
    {
        UIEventTrigger temp = null;
        if ((temp = go.GetComponent<UIEventTrigger>()) == null)
            temp = go.AddComponent<UIEventTrigger>();
        return temp;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDrag?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        onDrag?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onEndDrag?.Invoke(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onPointerClick?.Invoke(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown?.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter?.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit?.Invoke(eventData);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        onPointerMove?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUp?.Invoke(eventData);
    }
}
