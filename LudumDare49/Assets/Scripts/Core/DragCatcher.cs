using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class DragCatcher : Singleton<DragCatcher>, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public delegate void OnDragDelta(Vector2 delta);
    public static event OnDragDelta onDragDelta;
    public bool IsDragging { get { return isDragging; } }

    bool isDragging = false;

    void IBeginDragHandler.OnBeginDrag(PointerEventData data)
    {
        isDragging = true;
    }

    void IDragHandler.OnDrag(PointerEventData data)
    {
        if (isDragging)
        {
            onDragDelta.Invoke(data.delta);
        }
    }

    void IEndDragHandler.OnEndDrag(PointerEventData data)
    {
        isDragging = false;
    }
}
