using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Item : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private DragController dragController;
    private Vector3 startPosition;
    private bool isDragging;
    private Vector3 startSize;
    public bool CameFromInventory { get; private set; }
    private Customer hoveredCustomer;

    private void Start()
    {
        startPosition = transform.position;
        startSize = GetComponent<RectTransform>().sizeDelta;
        hoveredCustomer = null;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        CheckCustomerHover(eventData);
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

        if (hoveredCustomer != null)
        {
            // serve the customer
            dragController.ServeCustomer(this, hoveredCustomer);
            hoveredCustomer.SetHighlight(false);
            hoveredCustomer = null;
        }
        else
        {
            dragController.DropItem(this);
        }

        GetComponent<RectTransform>().sizeDelta = startSize;
    }

    private void CheckCustomerHover(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        Customer found = null;
        foreach (var result in results)
        {

            Customer c = result.gameObject.GetComponentInParent<Customer>();
            Debug.Log($"Hit: {result.gameObject.name} | Customer found: {(c != null ? c.gameObject.name : "NULL")} | Customer script on: {(c != null ? c.gameObject.transform.parent?.name : "N/A")}");

            if (c != null)
            {
                found = c;
                break;
            }
        }

        if (hoveredCustomer != null && !hoveredCustomer) hoveredCustomer = null;

        if (found != hoveredCustomer)
        {
            Debug.Log($"Hover changed: {hoveredCustomer?.name ?? "null"} → {found?.name ?? "null"}");
            if (hoveredCustomer != null) hoveredCustomer.SetHighlight(false);
            hoveredCustomer = found;
            if (hoveredCustomer != null) hoveredCustomer.SetHighlight(true);
        }
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
        transform.SetParent(dragController.Canvas, true);
    }
}
