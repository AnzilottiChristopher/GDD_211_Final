using UnityEngine;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private DragController dragController;
    private Vector3 startPosition;
    private bool isDragging;
    private Vector3 startSize;

    private void Start()
    {
        startPosition = transform.position;
        startSize = GetComponent<RectTransform>().sizeDelta;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        transform.SetParent(dragController.Canvas, true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        dragController.DropItem(this);
        GetComponent<RectTransform>().sizeDelta = startSize;
    }
}
