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
    [SerializeField] Image type;
    [SerializeField] Text countdown;
    [SerializeField] Text spawnNum;
    [SerializeField] RectTransform dragVisuals;

    [SerializeField] RectTransform island;
    [SerializeField] RectTransform target;

    public float TimeOut { get; private set; }
    public bool Used { get; private set; }
    public AnimalController.AnimalDef ItemType { get; private set; }

    int spawnCount;

    public void Init(float timeOut, int numToSpawn)
    {
        TimeOut = timeOut;
        ItemType = AnimalController.Instance.GetRandomAnimalDef();
        type.sprite = ItemType.m_visual.m_sprite;
        spawnCount = numToSpawn;
        spawnNum.text = $"x{numToSpawn.ToString()}";
        gameObject.SetActive(true);
    }

    public void Refresh(float timer)
    {
        float duration = TimeOut - timer;
        countdown.text = $"{duration.ToString("0.0")}s";
    }

    bool isDragging;
    bool isOnIsland = false;

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
            Vector2 screenPos = data.position;
            dragVisuals.transform.position = data.position;
            
            Vector2 targetIslandPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(island, screenPos, Camera.main, out targetIslandPos);
            target.transform.localPosition = targetIslandPos;

            isOnIsland = targetIslandPos.magnitude <= Island.Instance.Radius;

            target.gameObject.SetActive(isOnIsland);
            dragVisuals.gameObject.SetActive(!isOnIsland);
        }
    }

    void IEndDragHandler.OnEndDrag(PointerEventData data)
    {
        if (isOnIsland)
        {
            Vector3 targetIslandPos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(island, data.position, Camera.main, out targetIslandPos);
            AnimalController.Instance.SpawnMultipleAtPosition(ItemType, targetIslandPos, spawnCount);
            Debug.Log($"Spawn positioning: [{targetIslandPos}]");
            Used = true;
        }
        dragVisuals.anchoredPosition = Vector3.zero;
        isDragging = false;
        target.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        target?.gameObject?.SetActive(false);
    }
}
