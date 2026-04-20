using UnityEngine;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler 
{
    [SerializeField] private DragController dragController;
    private Vector3 startPosition;
    private bool isDragging;
    private Vector3 startSize;
    public bool CameFromInventory { get; private set; }

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
        CameFromInventory = transform.parent == dragController.InventoryContainer;
        transform.SetParent(dragController.Canvas, true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        dragController.DropItem(this);
        GetComponent<RectTransform>().sizeDelta = startSize;
    }
    public void ResetPosition()
    {
        transform.position = startPosition;
        transform.SetParent(dragController.Canvas, true); // keep it on canvas so it's clickable
    }
}
