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
    [SerializeField] Text spawnNum;
    [SerializeField] GameObject notValid;

    [SerializeField] RectTransform island;


    [Header("Belt")]
    [SerializeField] AnimationCurve beltMovement;
    [SerializeField] AnimationCurve fallingMovement;
    [SerializeField] Vector2 startingPos = new Vector2(720, 35);
    [SerializeField] Vector2 endingPos = new Vector2(118, -122);
    float timer = 0;

    public bool Removable { get; private set; }
    public AnimalController.AnimalDef ItemType { get; private set; }

    int spawnCount;
    bool isDragging;
    bool isOnIsland = false;

    public void Init(int numToSpawn, AnimalController.AnimalDef itemType)
    {
        ItemType = itemType;
        type.sprite = ItemType.m_visual.m_sprite;
        spawnCount = numToSpawn;
        spawnNum.text = $"x{numToSpawn.ToString()}";

        transform.localPosition = startingPos;

        gameObject.SetActive(true);
    }

    public void MoveItem(float deltaTime, float duration)
    {
        if (!isDragging)
        {
            timer += deltaTime;
            float t = timer / duration;
            float moment = beltMovement.Evaluate(t);
            float yMovement = fallingMovement.Evaluate(t);
            Vector2 pos = new Vector2();
            pos.x = Mathf.Lerp(startingPos.x, endingPos.x, moment);
            pos.y = Mathf.Lerp(startingPos.y, endingPos.y, yMovement);
            transform.localPosition = pos;

            if (moment >= 1)
            {
                FallOff();
            }
        }
    }

    public void FallOff()
    {
        Removable = true;
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData data)
    {
        isDragging = true;
    }

    void IDragHandler.OnDrag(PointerEventData data)
    {
        if (isDragging)
        {
            Vector2 screenPos = data.position;
            transform.position = data.position;
            
            Vector2 targetIslandPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(island, screenPos, Camera.main, out targetIslandPos);

            isOnIsland = targetIslandPos.magnitude <= Island.Instance.Radius;

            notValid.SetActive(!isOnIsland);
        }
    }

    void IEndDragHandler.OnEndDrag(PointerEventData data)
    {
        if (isOnIsland)
        {
            Vector3 targetIslandPos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(island, data.position, Camera.main, out targetIslandPos);
            Island.Instance.SpawnAnimalsFromCrate(ItemType, spawnCount, targetIslandPos);
        }
        else
        {
            ConveyorBelt.Instance.CreateSpecificItem(ItemType, spawnCount);
        }
    
        isDragging = false;
        Removable = true;
    }
}
