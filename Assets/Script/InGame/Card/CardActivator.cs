using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardActivator : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 defaultpos;
    private GraphicRaycaster graycaster;
    private PointerEventData pointerEventData;
    private HorizontalLayoutGroup myhands_HLayout;
    private bool isdrag = false;
    public int siblingindex;

    // Start is called before the first frame update
    void Start()
    {
        defaultpos = transform.position;
        graycaster = transform.parent.parent.parent.GetComponent<GraphicRaycaster>();
        if (graycaster == null)
            graycaster = transform.parent.parent.parent.parent.GetComponent<GraphicRaycaster>();
        siblingindex = transform.GetSiblingIndex();
        isdrag = false;
        myhands_HLayout = transform.parent.GetComponent<HorizontalLayoutGroup>();
    }

    #region 드래그 핸들러
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (AuctionManager.IsHandCard)
        {
            defaultpos = transform.position;
            isdrag = true;
            //transform.localScale = Vector3.one * 0.7f;
        } 
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (AuctionManager.IsHandCard&&isdrag)
        {
            transform.position = Input.mousePosition;
            //transform.localScale = Vector3.one * 0.7f;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (AuctionManager.IsHandCard&&isdrag)
        {
            transform.position = defaultpos;
            transform.SetSiblingIndex(siblingindex);
            List<RaycastResult> results = new List<RaycastResult>();
            pointerEventData = new PointerEventData(GetComponent<EventSystem>());
            pointerEventData.position = Input.mousePosition;
            //transform.localScale = Vector3.one * 0.7f;
            graycaster.Raycast(eventData, results);
            if (results[0].gameObject != null && results[0].gameObject.transform.parent.tag == "Player")
                results[0].gameObject.transform.parent.GetComponent<CardEffectGetter>().dropcard(gameObject, siblingindex);
            isdrag = false;
        }
    }
    #endregion

    #region 포인터 핸들러

    public void OnPointerEnter(PointerEventData eventData)
    {
        myhands_HLayout.enabled = false;
        transform.localScale = Vector3.one;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 40f, transform.localPosition.z);
        siblingindex = transform.GetSiblingIndex();
        transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!isdrag)
        {
            transform.SetSiblingIndex(siblingindex);
            transform.localScale = Vector3.one * 0.7f;
            myhands_HLayout.enabled = true;
        }
    }

    #endregion
}
