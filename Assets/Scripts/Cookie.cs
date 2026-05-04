using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Lives on the cookie prefab. Holds all data and visuals for a single cookie.
/// Configured by CookieBuilder once the minigames are complete.
/// </summary>
public class Cookie : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Image cookieDisplay;
    [SerializeField] private Image toppingDisplay;

    // ─── Cookie Data ──────────────────────────────────────────────
    public int Dough { get; private set; } = -1;
    public List<string> Toppings { get; private set; } = new List<string>();
    public float Quality { get; private set; } = 1f;

    /// <summary>
    /// Called by CookieBuilder to configure this cookie's data and visuals.
    /// </summary>
    public void Configure(int dough, List<string> toppings, float quality,
                          Sprite cookieSprite, Sprite toppingSprite)
    {
        Dough = dough;
        Toppings = new List<string>(toppings);
        Quality = quality;

        if (cookieDisplay != null)
        {
            cookieDisplay.sprite = cookieSprite;
            cookieDisplay.gameObject.SetActive(true);
        }

        if (toppingDisplay != null)
        {
            toppingDisplay.sprite = toppingSprite;
            toppingDisplay.gameObject.SetActive(toppingSprite != null);
        }
    }
}