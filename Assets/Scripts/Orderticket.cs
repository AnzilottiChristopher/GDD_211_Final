using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Represents a single order ticket hanging on the clothesline.
/// Configured by Clothesline when a customer spawns.
/// </summary>
public class OrderTicket : MonoBehaviour
{
    [SerializeField] private Image doughIcon;
    [SerializeField] private Image toppingIcon;
    [SerializeField] private TextMeshProUGUI doughLabel;
    [SerializeField] private TextMeshProUGUI toppingLabel;

    private int slotIndex;

    /// <summary>
    /// Called by Clothesline to set up this ticket's visuals.
    /// slotIndex links this ticket to its customer so it can be removed on serve.
    /// </summary>
    public void Setup(int customerSlotIndex, Sprite doughSprite, Sprite toppingSprite,
                      string doughName, string toppingName)
    {
        slotIndex = customerSlotIndex;

        if (doughIcon != null)    doughIcon.sprite = doughSprite;
        if (toppingIcon != null)  toppingIcon.sprite = toppingSprite;
        if (doughLabel != null)   doughLabel.text = doughName;
        if (toppingLabel != null) toppingLabel.text = toppingName;
    }

    public int SlotIndex => slotIndex;
}