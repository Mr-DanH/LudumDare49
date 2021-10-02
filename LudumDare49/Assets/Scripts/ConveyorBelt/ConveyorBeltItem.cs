using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
    Features:
    Each item will have 1 type of animal with a random amount of them.
*/
public class ConveyorBeltItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] Text type;
    [SerializeField] Text countdown;
    [SerializeField] Text spawnNum;
    [SerializeField] RectTransform dragVisuals;

    [SerializeField] RectTransform island;
    [SerializeField] RectTransform target;

    public float TimeOut { get; private set; }
    public int ItemType { get; private set; }

    public void Init(float timeOut, int itemType, int numToSpawn)
    {
        TimeOut = timeOut;
        ItemType = itemType;
        type.text = itemType.ToString();
        spawnNum.text = numToSpawn.ToString();
        gameObject.SetActive(true);
    }

    public void Refresh(float timer)
    {
        float duration = TimeOut - timer;
        countdown.text = $"{duration.ToString("0.0")}s";
    }

    bool isDragging;

    void IBeginDragHandler.OnBeginDrag(PointerEventData data)
    {
        isDragging = true;
        target.gameObject.SetActive(true);
    }

    void IDragHandler.OnDrag(PointerEventData data)
    {
        if (isDragging)
        {
            // are we over the island? If so turn off these visuals
            dragVisuals.transform.position = data.position;
            Vector2 screenPos = data.position;
            Vector2 targetPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(island, screenPos, Camera.main, out targetPos);
            target.transform.localPosition = targetPos;
        }
    }

    void IEndDragHandler.OnEndDrag(PointerEventData data)
    {
        // have we let go over the island? If so destroy this object. If not bring it back to the conveyor belt.
        dragVisuals.anchoredPosition = Vector3.zero;
        isDragging = false;
        target.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        target.gameObject.SetActive(false);
    }
}
