using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PieceHandler : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 startingPosition;

    public bool droppedOnSlot;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startingPosition = rectTransform.localPosition;

        canvasGroup.alpha = .7f;
        canvasGroup.blocksRaycasts = false;
        droppedOnSlot = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        StartCoroutine("CheckIfDroppedOnSlot");
       

    }

    public void OnPointerDown(PointerEventData eventData)
    {
   
    }

    public void OnDrop(PointerEventData eventData)
    {
        
    }

    IEnumerator CheckIfDroppedOnSlot()
    {
        yield return new WaitForEndOfFrame();
        if (droppedOnSlot == false)
        {
           // rectTransform.anchoredPosition
            rectTransform.localPosition = startingPosition;
        }
    }
}
