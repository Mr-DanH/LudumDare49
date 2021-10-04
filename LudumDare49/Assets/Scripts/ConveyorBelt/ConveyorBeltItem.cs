using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConveyorBeltItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] Crate crate;

    [SerializeField] RectTransform island;
    [SerializeField] int startingSiblingIndex = 2;


    [Header("Belt")]
    [SerializeField] AnimationCurve beltMovement;
    [SerializeField] AnimationCurve fallingMovement;
    [SerializeField] Vector2 startingPos = new Vector2(720, 35);
    [SerializeField] Vector2 endingPos = new Vector2(118, -122);

    [Header("Drag")]
    [SerializeField] Vector3 dragScale = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] GameObject target;

    float timer = 0;

    public bool Removable { get; private set; }

    bool isDragging;
    bool isOnIsland = false;

    public void Init(int numToSpawn, AnimalController.AnimalDef animalDef)
    {
        transform.localPosition = startingPos;
        RectTransform rectTransform = transform as RectTransform;
        rectTransform.SetSiblingIndex(startingSiblingIndex);
        crate.Init(numToSpawn, animalDef);
        target.SetActive(false);
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
        transform.localScale = dragScale;
        crate.MoveUpCrate();
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

            crate.SetInvalid(!isOnIsland);
            target.SetActive(isOnIsland);
        }
    }

    void IEndDragHandler.OnEndDrag(PointerEventData data)
    {
        if (isOnIsland)
        {
            Vector3 targetIslandPos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(island, data.position, Camera.main, out targetIslandPos);
            Island.Instance.SpawnAnimalsFromCrate(crate.AnimalDef, crate.NumToSpawn, targetIslandPos);
        }
    
        isDragging = false;
        Removable = true;
    }
}
