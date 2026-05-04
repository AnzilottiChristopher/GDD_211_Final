using UnityEngine;
using UnityEngine.UI;

public class DragController : MonoBehaviour
{
    public Transform Canvas => canvasTransform;
    public Transform InventoryContainer => inventory1_container;

    [SerializeField] private Image inventory1_image;
    [SerializeField] private Transform inventory1_container;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private Image trashZone;

    // ─── Drop: trash, inventory, or reset ─────────────────────────
    public void DropItem(Item item)
    {
        // Trash zone — just destroy the cookie
        if (RectOverlap(item.GetComponent<RectTransform>(), trashZone.GetComponent<RectTransform>()))
        {
            Destroy(item.gameObject);
            return;
        }

        // Inventory
        if (RectOverlap(item.GetComponent<RectTransform>(), inventory1_image.GetComponent<RectTransform>()))
        {
            int limit = item.CameFromInventory ? 3 : 3;
            if (inventory1_container.childCount < limit)
            {
                item.transform.SetParent(inventory1_container, false);
            }
            else
            {
                Debug.Log("Inventory Full");
                item.ResetPosition();
            }
        }
        else
        {
            Debug.Log("Missed inventory");
            item.ResetPosition();
        }
    }

    // ─── Serve cookie to customer ──────────────────────────────────
    public void ServeCustomer(Item item, Customer customer)
    {
        // All cookie data lives on the Cookie component — no builder needed
        Cookie cookie = item.GetComponent<Cookie>();
        if (cookie == null)
        {
            Debug.LogWarning("[DragController] Item has no Cookie component!");
            return;
        }

        bool correct = customer.CheckOrder(cookie.Dough, cookie.Toppings);
        customer.Serve(correct, cookie.Quality);
        Destroy(item.gameObject);
    }

    // ─── Geometry helpers ─────────────────────────────────────────
    private bool RectOverlap(RectTransform firstRect, RectTransform secondRect)
    {
        Rect a = GetWorldRect(firstRect);
        Rect b = GetWorldRect(secondRect);
        return a.Overlaps(b);
    }

    private Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        return new Rect(corners[0].x, corners[0].y,
                        corners[2].x - corners[0].x,
                        corners[2].y - corners[0].y);
    }
}