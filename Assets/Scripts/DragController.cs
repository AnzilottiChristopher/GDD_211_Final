using UnityEngine;
using UnityEngine.UI;

public class DragController : MonoBehaviour
{
    public Transform Canvas => canvasTransform;

    [SerializeField] private Image inventory1_image;
    [SerializeField] private Transform invetory1_container;
    [SerializeField] private Transform canvasTransform;

    public void DropItem(Item item)
    {
        if (RectOverlap(item.GetComponent<RectTransform>(),inventory1_image.GetComponent<RectTransform>()))
        {
            item.transform.SetParent(invetory1_container, false); //Add to inventory
        }
        else
        {
            Debug.Log("TEST");
             //Remove from inventory
        }
    }

    private bool RectOverlap(RectTransform firstRect, RectTransform secondRect)
    {
        if (firstRect.position.x + firstRect.rect.width * 0.5f < secondRect.position.x - secondRect.rect.width * 0.5f)
        {
            return false;
        }
        if (secondRect.position.x + secondRect.rect.width * 0.5f < firstRect.position.x - firstRect.rect.width * 0.5f)
        {
            return false;
        }
        if (firstRect.position.y + firstRect.rect.height * 0.5f < secondRect.position.y - secondRect.rect.height * 0.5f)
        {
            return false;
        }
        if (secondRect.position.y + secondRect.rect.height * 0.5f < firstRect.position.y - firstRect.rect.height * 0.5f)
        {
            return false;
        }
        return true;
    }
}