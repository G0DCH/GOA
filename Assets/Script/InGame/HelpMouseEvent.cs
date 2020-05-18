using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HelpMouseEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region 포인터 핸들러
    public void OnPointerEnter(PointerEventData eventData)
    {
        CameraMovement.IsScroll = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CameraMovement.IsScroll = true;
    }
    #endregion
}
