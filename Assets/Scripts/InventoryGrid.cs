using UnityEngine;
using UnityEngine.UI;

public class InventoryGrid : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup grid;
    [SerializeField] private int maxItems = 4;
    [SerializeField] private float padding = 10f;
    [SerializeField] private float topPadding = 10f;

    private void Start()
    {
        RectTransform rt = GetComponent<RectTransform>();
        float totalPadding = padding * 2; // left and right ends
        float totalCellWidth = 100f * maxItems;
        float remainingSpace = rt.rect.width - totalCellWidth - totalPadding;
        float spacing = remainingSpace / (maxItems - 1);

        grid.cellSize = new Vector2(100, 100);
        grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        grid.constraintCount = 1;
        grid.spacing = new Vector2(spacing, 0);
        grid.padding = new RectOffset((int)padding, (int)padding, (int)topPadding, 0);
    }
}