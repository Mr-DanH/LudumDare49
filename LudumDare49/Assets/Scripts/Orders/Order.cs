using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Order : MonoBehaviour
{
    enum eOrderState
    {
        Arriving,
        Fufilling,
        Processing,
        Collected,
    }

    [SerializeField] Image animal;
    [SerializeField] Image progressBar;
    [SerializeField] Image fulfillmentAmount;
    [SerializeField] Button CollectButton;
    [SerializeField] CrateScriptableObject crateScriptableObject;
    public Transform m_boatImage;
    public GameObject m_readyOutline;

    public AnimalController.AnimalDef AnimalDef { get; private set; }
    public Transform Port { get; private set; }
    public bool Collected { get { return state == eOrderState.Collected; } }
    public int PendingCollectionCount { get; set; }

    public Vector3 m_sailDownPos;
    Vector3 m_sailUpPos;

    eOrderState state;
    float fulfillmentNum;

    bool m_canCollect;

    void Awake()
    {
        m_sailUpPos = CollectButton.transform.localPosition;
    }

    public void Init(AnimalController.AnimalDef def, int fulfillment, Transform port)
    {
        AnimalDef = def;
        Port = port;
        animal.sprite = AnimalDef.m_visual.m_sprite;
        fulfillmentNum = fulfillment;
        fulfillmentAmount.sprite = crateScriptableObject.GetBoatNum((int)fulfillmentNum);
        m_readyOutline.gameObject.SetActive(false);
        
        StartCoroutine(Arrive());
    }

    public void Refresh(int numOnIsland)
    {
        float prop = numOnIsland / fulfillmentNum;
        progressBar.fillAmount = prop;

        m_canCollect = prop >= 1 && state == eOrderState.Fufilling;
        m_readyOutline.gameObject.SetActive(m_canCollect);
    }

    public void OnCollect()
    {
        if(!m_canCollect)
            return;

        m_readyOutline.gameObject.SetActive(false);
        progressBar.transform.parent.gameObject.SetActive(false);

        state = eOrderState.Processing;
        StartCoroutine(CollectOrder());
    }

    IEnumerator<YieldInstruction> CollectOrder()
    {
        PendingCollectionCount = (int)fulfillmentNum;

        AnimalController.Instance.CollectOrder(this, Port.localPosition, AnimalDef, (int)fulfillmentNum);

        while(PendingCollectionCount > 0)
            yield return null;

        Vector2 portPos = Port.localPosition;
        Vector2 localDir = portPos.normalized;

        yield return StartCoroutine(AnimateSail(m_sailUpPos, m_sailDownPos));
        yield return StartCoroutine(AnimatePos(portPos, portPos + (localDir * 500)));
        
        state = eOrderState.Collected;
    }

    IEnumerator<YieldInstruction> Arrive()
    {
        state = eOrderState.Arriving;

        Vector2 portPos = Port.localPosition;
        Vector2 localDir = portPos.normalized;

        CollectButton.transform.localPosition = m_sailDownPos;
        progressBar.transform.parent.gameObject.SetActive(false);

        yield return StartCoroutine(AnimatePos(portPos + (localDir * 500), portPos));
        yield return StartCoroutine(AnimateSail(m_sailDownPos, m_sailUpPos));
        
        progressBar.transform.parent.gameObject.SetActive(true);

        state = eOrderState.Fufilling;
    }

    IEnumerator<YieldInstruction> AnimateSail(Vector3 from, Vector3 to)
    {
        for(float i = 0; i < 1; i += Time.deltaTime)
        {
            CollectButton.transform.localPosition = Vector3.Lerp(from, to, i);
            yield return null;
        }
        
        CollectButton.transform.localPosition = to;
    }
    
    IEnumerator<YieldInstruction> AnimatePos(Vector3 from, Vector3 to)
    {
        Transform camera = Camera.main.transform;
        Vector3 direction = (Vector3)to - from;
        direction.z = 0;

        Vector3 worldDir = transform.parent.TransformVector(direction);
        Vector3 cameraLocalDir = camera.InverseTransformVector(worldDir);

        m_boatImage.transform.localScale = new Vector3(-Mathf.Sign(cameraLocalDir.x), 1, 1);

        for(float i = 0; i < 3; i += Time.deltaTime)
        {
            transform.localPosition = Vector3.Lerp(from, to, i / 3f);
            yield return null;
        }

        m_boatImage.localScale = new Vector3(-1, 1, 1);
        transform.localPosition = to;
    }
}
