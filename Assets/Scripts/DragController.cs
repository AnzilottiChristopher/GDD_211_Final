using UnityEngine;
using UnityEngine.UI;

public class DragController : MonoBehaviour
{
    public Transform Canvas => canvasTransform;

    [SerializeField] private Image inventory1_image;
    [SerializeField] private Transform inventory1_container;
    [SerializeField] private Transform canvasTransform;
    public Transform InventoryContainer => inventory1_container;
    public void DropItem(Item item)
    {
        if (RectOverlap(item.GetComponent<RectTransform>(),inventory1_image.GetComponent<RectTransform>()))
        {
            int limit = item.CameFromInventory ? 5 : 4;
            if (inventory1_container.childCount < limit)
            {
                item.transform.SetParent(inventory1_container, false); //Add to inventory
            }
            else
            {
                Debug.Log("Inventory Full");
                item.ResetPosition();
            }
        }
        else
        {
            Debug.Log("TEST");
             //Remove from inventory
        }
    }

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
    // corners[0] = bottom-left, corners[2] = top-right
    return new Rect(corners[0].x, corners[0].y,
                    corners[2].x - corners[0].x,
                    corners[2].y - corners[0].y);
}
}