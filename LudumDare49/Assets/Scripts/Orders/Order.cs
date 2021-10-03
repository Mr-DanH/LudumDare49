using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Order : MonoBehaviour
{
    enum eOrderState
    {
        Fufilling,
        Processing,
        Collected,
    }

    [SerializeField] Image animal;
    [SerializeField] Image progressBar;
    [SerializeField] Image fulfillmentAmount;
    [SerializeField] Button CollectButton;
    [SerializeField] CrateScriptableObject crateScriptableObject;

    public AnimalController.AnimalDef AnimalDef { get; private set; }
    public bool Collected { get { return state == eOrderState.Collected; } }

    eOrderState state;
    float fulfillmentNum;

    public void Init(AnimalController.AnimalDef def, int fulfillment)
    {
        AnimalDef = def;
        animal.sprite = AnimalDef.m_visual.m_sprite;
        fulfillmentNum = fulfillment;
        fulfillmentAmount.sprite = crateScriptableObject.GetCrateNum((int)fulfillmentNum);
    }

    public void Refresh(int numOnIsland)
    {
        float prop = numOnIsland / fulfillmentNum;
        progressBar.fillAmount = prop;

        bool isCollectable = prop >= 1 && state == eOrderState.Fufilling;
        CollectButton.interactable = isCollectable;
    }

    public void OnCollect()
    {
        state = eOrderState.Processing;
        StartCoroutine(CollectOrder());
    }

    IEnumerator<YieldInstruction> CollectOrder()
    {
        yield return StartCoroutine(AnimalController.Instance.CollectOrder(transform.parent.localPosition, AnimalDef, (int)fulfillmentNum));
        
        state = eOrderState.Collected;
    }
}
