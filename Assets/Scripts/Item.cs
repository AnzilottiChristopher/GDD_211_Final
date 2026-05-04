using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Item : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private DragController dragController;

    private Vector3 startPosition;
    private Vector3 startSize;
    private Customer hoveredCustomer;
    private Transform originalParent; // add this field

    public bool CameFromInventory { get; private set; }

    // ─── Called by CookieBuilder after instantiating the prefab ──
    // Use this if DragController isn't already assigned on the prefab itself
    public void Init(DragController controller)
    {
        if (dragController == null)
            dragController = controller;
    }

    private void Start()
    {
        originalParent = transform.parent;
        startPosition = transform.position;
        startSize = GetComponent<RectTransform>().sizeDelta;
        hoveredCustomer = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CameFromInventory = transform.parent == dragController.InventoryContainer;
        transform.SetParent(dragController.Canvas, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        CheckCustomerHover(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (hoveredCustomer != null)
        {
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
            Debug.Log($"Hit: {result.gameObject.name} | Customer found: {(c != null ? c.gameObject.name : "NULL")}");
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
        transform.SetParent(originalParent, true);
    }
}